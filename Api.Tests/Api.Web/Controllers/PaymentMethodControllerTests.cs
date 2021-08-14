using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Web.Controllers;
using Api.Web.Handlers;
using Api.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceStack.Redis;
using Xunit;

namespace Api.Tests.Api.Web.Controllers
{
    public class PaymentMethodControllerTests
    {
        private readonly List<PaymentMethod> _paymentMethods = new List<PaymentMethod>
        {
            new PaymentMethod
            {
                Id = "5fd5ddc549c67b3c6527462f",
                Key = "01",
                Name = "CASH",
                Description = "Cash payment",
                Status = true
            }
        };
        private readonly Mock<IPaymentMethodManager> _mockManager = new Mock<IPaymentMethodManager>();
        private readonly Mock<IPaymentMethodRepository> _mockRepository = new Mock<IPaymentMethodRepository>();
        private readonly Mock<IOperationHandler> _mockOperationHandler = new Mock<IOperationHandler>();
        private readonly Mock<IRedisClientsManagerAsync> _mockRedisManager = new Mock<IRedisClientsManagerAsync>();
        private readonly Mock<IRedisClientAsync> _mockRedisClient = new Mock<IRedisClientAsync>();
        private readonly PaymentMethodController _paymentMethodController;

        public PaymentMethodControllerTests()
        {
            _paymentMethodController = new PaymentMethodController
            (
                _mockManager.Object,
                _mockRepository.Object,
                _mockOperationHandler.Object,
                _mockRedisManager.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturn_PaymentsList()
        {
            _mockRepository.Setup(repo => repo.CountAsync(It.IsAny<ListResourceRequest>()))
                .ReturnsAsync(1);

            _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<ListResourceRequest>()))
                .ReturnsAsync(_paymentMethods);

            var result = await _paymentMethodController
                .GetAllAsync(new ListResourceRequest { Page = 1, PageSize = 10 });
            var response = result as OkObjectResult;
            var responseBody = response.Value as ListPaymentMethodResponse;

            _mockRepository.Verify(x => x.GetAllAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            _mockRepository.Verify(x => x.CountAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ListPaymentMethodResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(1, responseBody.Paginator.Page);
            Assert.Equal(10, responseBody.Paginator.PageSize);
            Assert.Equal(0, responseBody.Paginator.RemainingDocuments);
            Assert.Single(responseBody.Data);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn_SinglePayment()
        {
            _mockRedisManager.Setup(redis => redis.GetClientAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockRedisClient.Object);

            _mockRedisClient.Setup(client => client
                .GetAsync<PaymentMethod>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => null);

            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _paymentMethods.SingleOrDefault(p => p.Id == id));

            var result = await _paymentMethodController.GetByIdAsync("5fd5ddc549c67b3c6527462f");
            var response = result as OkObjectResult;
            var responseBody = response.Value as SinglePaymentMethodResponse;

            _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            _mockRedisManager.Verify(x => x.GetClientAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x =>
                x.GetAsync<PaymentMethod>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SinglePaymentMethodResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal("5fd5ddc549c67b3c6527462f", responseBody.Data.Id);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturn_CreatedPayment()
        {
            var payment = new PaymentMethod
            {
                Key = "04",
                Name = "Card",
                Description = "Card payment",
                Status = true
            };

            _mockManager.Setup(manager => manager.CreateAsync(It.IsAny<PaymentMethod>()))
                .Callback((PaymentMethod paymentMethod) => 
                {
                    paymentMethod.Id = "60f70c1f7098da34083f12e2";
                    _paymentMethods.Add(paymentMethod);
                });

            _mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var result = await _paymentMethodController.CreateAsync(payment);
            var response = result as CreatedResult;
            var responseBody = response.Value as SinglePaymentMethodResponse;

            _mockManager.Verify(x => x.CreateAsync(It.IsAny<PaymentMethod>()), Times.Once);
            _mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            Assert.IsType<CreatedResult>(result);
            Assert.IsType<SinglePaymentMethodResponse>(response.Value);
            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
            Assert.Equal("60f70c1f7098da34083f12e2", responseBody.Data.Id);
        }

        [Fact]
        public async Task UpdateByIdAsync_ShouldReturn_UpdatedPayment()
        {
            var payment = new PaymentMethod
            {
                Id = "5fd5ddc549c67b3c6527462f",
                Key = "01",
                Name = "CASH",
                Description = "Cash payment",
                Status = true
            };

            const bool NEW_STATUS = false;

            var updatedProperties = new JsonPatchDocument<PaymentMethod>();
            updatedProperties.Replace(p => p.Status, NEW_STATUS);

            _mockRedisManager.Setup(redis => redis.GetClientAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockRedisClient.Object);

            _mockRedisClient.Setup(client => client
                .GetAsync<PaymentMethod>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => null);

            _mockRedisClient.Setup(client => client
                .RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _paymentMethods.SingleOrDefault(p => p.Id == id));

            _mockManager.Setup(manager => manager
                .UpdateByIdAsync
                (
                    It.IsAny<string>(),
                    It.IsAny<PaymentMethod>(),
                    It.IsAny<JsonPatchDocument<PaymentMethod>>())
                )
                .Callback((string id, PaymentMethod paymentMethod, JsonPatchDocument<PaymentMethod> updatedProperties) =>
                {
                    updatedProperties.ApplyTo(paymentMethod);
                    _paymentMethods[_paymentMethods.FindIndex(p => p.Id == id)] = paymentMethod;
                });

            _mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var result = await _paymentMethodController.UpdateByIdAsync("5fd5ddc549c67b3c6527462f", updatedProperties);
            var response = result as OkObjectResult;
            var responseBody = response.Value as SinglePaymentMethodResponse;

            _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            _mockManager.Verify(x => x
                .UpdateByIdAsync
                (
                    It.IsAny<string>(),
                    It.IsAny<PaymentMethod>(),
                    It.IsAny<JsonPatchDocument<PaymentMethod>>()),
                    Times.Once
                );
            _mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            _mockRedisManager.Verify(x => x.GetClientAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x =>
                x.GetAsync<PaymentMethod>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x =>
                x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SinglePaymentMethodResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.False(responseBody.Data.Status);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldDelete_SpecificPayment()
        {
            _mockRedisManager.Setup(redis => redis.GetClientAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockRedisClient.Object);

            _mockRedisClient.Setup(client => client
                .RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockManager.Setup(manager => manager.DeleteByIdAsync(It.IsAny<string>()))
                .Callback((string id) => _paymentMethods.RemoveAt(_paymentMethods.FindIndex(p => p.Id == id)));

            _mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var result = await _paymentMethodController.DeleteByIdAsync("5fd5ddc549c67b3c6527462f");
            var response = result as NoContentResult;

            _mockManager.Verify(x => x.DeleteByIdAsync(It.IsAny<string>()), Times.Once);
            _mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            _mockRedisManager.Verify(x => x.GetClientAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x =>
                x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
        }
    }
}