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
            var (type, collection, id, stationId, model) = message;
            return collection switch
            {
                "users" => BuildQueryUsers(type, id, stationId, model),
                "products" => BuildQueryProducts(type, id, model),
                "payments" => BuildQueryPaymentMethod(type, id, model),
                "stations" => BuildQueryStation(type, id, model),
                _ => System.Console.Out.WriteLineAsync("No matches models")
            };
        }

        #endregion

        #region BuildFirebaseQueryForUsersCollection

        private Task BuildQueryUsers(string typeOperation, string id, string stationId, BaseEntity user) => typeOperation switch
        {
            "Create" => _firebaseClient.Child($"{_path}/stations/{stationId}/users/{user.Id}").PostAsync(user),
            "Update" => _firebaseClient.Child($"{_path}/stations/{stationId}/users/{id}").PutAsync(user),
            "Delete" => _firebaseClient.Child($"{_path}/stations/{stationId}/users/{id}").DeleteAsync(),
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

        #region BuildFirebaseQueryForPaymentMethodCollection

        private Task BuildQueryPaymentMethod(string typeOperation, string id, BaseEntity paymentMethod) => typeOperation switch
        {
            "Create" => _firebaseClient.Child($"{_path}/payments/{paymentMethod.Id}").PostAsync(paymentMethod),
            "Update" => _firebaseClient.Child($"{_path}/payments/{id}").PutAsync(paymentMethod),
            "Delete" => _firebaseClient.Child($"{_path}/payments/{id}").DeleteAsync(),
            _ => System.Console.Out.WriteLineAsync("No matches operations for payments collection")
        };

        #endregion

        #region BuildFirebaseQueryForStationCollection

        private Task BuildQueryStation(string typeOperation, string id, BaseEntity station) => typeOperation switch
        {
            "Create" => _firebaseClient.Child($"{_path}/stations/{station.Id}").PostAsync(station),
            "Update" => _firebaseClient.Child($"{_path}/stations/{id}").PutAsync(station),
            "Delete" => _firebaseClient.Child($"{_path}/stations/{id}").DeleteAsync(),
            _ => System.Console.Out.WriteLineAsync("No matches operations for stations collection")
        };

        #endregion
    }
}