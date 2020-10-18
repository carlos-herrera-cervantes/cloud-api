using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Web.Handlers;
using Api.Web.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Hosting;

namespace Api.Web.Backgrounds
{
    public class FirebaseConsumer : BackgroundService
    {
        private readonly string _path = "events/cloud-api";
        private readonly FirebaseClient _firebaseClient;
        private readonly IOperationHandler _operationHandler;

        public FirebaseConsumer(FirebaseClient firebaseClient, IOperationHandler operationHandler)
        {
            _firebaseClient = firebaseClient;
            _operationHandler = operationHandler;
        }

        #region ExecuteSubscribe

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _operationHandler.Subscribe("FirebaseConsumer", async message => 
            {
                var operation = SelectQueryByModel(message);
                await operation;
            });

            return Task.CompletedTask;
        }

        #endregion

        #region ReturnsQueryByModel

        private Task SelectQueryByModel(CollectionEventReceived message)
        {
            var (type, collection, id, model) = message;
            return collection switch
            {
                "users" => BuildQueryUsers(type, id, model),
                "products" => BuildQueryProducts(type, id, model),
                _ => System.Console.Out.WriteLineAsync("No matches models")
            };
        }

        #endregion

        #region BuildFirebaseQueryForUsersCollection

        private Task BuildQueryUsers(string typeOperation, string id, BaseEntity user) => typeOperation switch
        {
            "Create" => _firebaseClient.Child($"{_path}/users/{user.Id}").PostAsync(user),
            "Update" => _firebaseClient.Child($"{_path}/users/{id}").PutAsync(user),
            "Delete" => _firebaseClient.Child($"{_path}/users/{id}").DeleteAsync(),
            _ => System.Console.Out.WriteLineAsync("No matches operations for users collection")
        };

        #endregion

        #region BuildFirebaseQueryForProductsCollection

        private Task BuildQueryProducts(string typeOperation, string id, BaseEntity product) => typeOperation switch
        {
            "Create" => _firebaseClient.Child($"{_path}/products/{product.Id}").PostAsync(product),
            "Update" => _firebaseClient.Child($"{_path}/products/{id}").PutAsync(product),
            "Delete" => _firebaseClient.Child($"{_path}/products/{id}").DeleteAsync(),
            _ => System.Console.Out.WriteLineAsync("No matches operations for products collection")
        };

        #endregion
    }
}