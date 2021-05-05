using System;
using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Web.Handlers;
using Api.Web.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Hosting;
using Api.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Web.Backgrounds
{
    public class FirebaseConsumer : BackgroundService
    {
        private readonly string _path = "events/cloud-api";
        private readonly FirebaseClient _firebaseClient;
        private readonly IOperationHandler _operationHandler;
        private readonly ICustomerPurchaseManager _customerPurchaseManager;
        private readonly ICustomerPurchaseRepository _customerPurchaseRepository;

        public FirebaseConsumer(
            FirebaseClient firebaseClient,
            IOperationHandler operationHandler,
            IServiceScopeFactory factory
        )
        {
            _firebaseClient = firebaseClient;
            _operationHandler = operationHandler;

            _customerPurchaseManager = factory
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<ICustomerPurchaseManager>();

            _customerPurchaseRepository = factory
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<ICustomerPurchaseRepository>();
        }

        #region ExecuteSubscribe

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _firebaseClient
                .Child("events/local/sales/")
                .AsObservable<CustomerPurchase>()
                .Subscribe(async c =>
                {
                    if (String.IsNullOrEmpty(c.Key)) return;

                    var sale = await _customerPurchaseRepository.GetByIdAsync(c.Key);

                    if (sale is null)
                    {
                        await _customerPurchaseManager.CreateAsync(c.Object);
                        var @ref = $"events/local/sales/{c.Key}";
                        await _firebaseClient.Child(@ref).DeleteAsync();
                    }
                });

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
            var (_, collection, _, _, _) = message;
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