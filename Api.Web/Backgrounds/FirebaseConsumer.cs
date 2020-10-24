using System.Threading;
using System.Threading.Tasks;
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
            var (_, collection, _, _, model) = message;
            return collection switch
            {
                "users" => BuildQueryUsers(message),
                "products" => BuildQueryProducts(message),
                "payments" => BuildQueryPaymentMethod(message),
                "stations" => BuildQueryStation(message),
                _ => System.Console.Out.WriteLineAsync("No matches models")
            };
        }

        #endregion

        #region BuildFirebaseQueryForUsersCollection

        private Task BuildQueryUsers(CollectionEventReceived message)
            => _firebaseClient.Child($"{_path}/stations/{message.StationId}/users/tasks/{message.Id}").PutAsync(message);

        #endregion

        #region BuildFirebaseQueryForProductsCollection

        private Task BuildQueryProducts(CollectionEventReceived message)
            => _firebaseClient.Child($"{_path}/products/tasks/{message.Id}").PutAsync(message);

        #endregion

        #region BuildFirebaseQueryForPaymentMethodCollection

        private Task BuildQueryPaymentMethod(CollectionEventReceived message)
            => _firebaseClient.Child($"{_path}/payments/tasks/{message.Id}").PutAsync(message);

        #endregion

        #region BuildFirebaseQueryForStationCollection

        private Task BuildQueryStation(CollectionEventReceived message)
            => _firebaseClient.Child($"{_path}/stations/{message.Id}/tasks/node").PutAsync(message);

        #endregion
    }
}