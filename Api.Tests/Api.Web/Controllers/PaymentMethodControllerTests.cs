using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public async Task GetAllAsync_ShouldReturn_PaymentsList()
        {
            var mockManager = new Mock<IPaymentMethodManager>();
            var mockRepository = new Mock<IPaymentMethodRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

            mockRepository.Setup(repo => repo.CountAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(1);
            mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(_paymentMethods);

            var paymentMethodController = new PaymentMethodController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await paymentMethodController.GetAllAsync(new ListResourceRequest { Page = 1, PageSize = 10 });
            var response = result as OkObjectResult;
            var responseBody = response.Value as ListPaymentMethodResponse;

            mockRepository.Verify(x => x.GetAllAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            mockRepository.Verify(x => x.CountAsync(It.IsAny<ListResourceRequest>()), Times.Once);
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
            var mockManager = new Mock<IPaymentMethodManager>();
            var mockRepository = new Mock<IPaymentMethodRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _paymentMethods.SingleOrDefault(p => p.Id == id));

            var paymentMethodController = new PaymentMethodController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await paymentMethodController.GetByIdAsync("5fd5ddc549c67b3c6527462f");
            var response = result as OkObjectResult;
            var responseBody = response.Value as SinglePaymentMethodResponse;

            mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SinglePaymentMethodResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal("5fd5ddc549c67b3c6527462f", responseBody.Data.Id);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturn_CreatedPayment()
        {
            var mockManager = new Mock<IPaymentMethodManager>();
            var mockRepository = new Mock<IPaymentMethodRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

            var payment = new PaymentMethod
            {
                Key = "04",
                Name = "Card",
                Description = "Card payment",
                Status = true
            };

            mockManager.Setup(manager => manager.CreateAsync(It.IsAny<PaymentMethod>()))
                .Callback((PaymentMethod paymentMethod) => 
                {
                    paymentMethod.Id = "60f70c1f7098da34083f12e2";
                    _paymentMethods.Add(paymentMethod);
                });

            mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var paymentMethodController = new PaymentMethodController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await paymentMethodController.CreateAsync(payment);
            var response = result as CreatedResult;
            var responseBody = response.Value as SinglePaymentMethodResponse;

            mockManager.Verify(x => x.CreateAsync(It.IsAny<PaymentMethod>()), Times.Once);
            mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            Assert.IsType<CreatedResult>(result);
            Assert.IsType<SinglePaymentMethodResponse>(response.Value);
            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
            Assert.Equal("60f70c1f7098da34083f12e2", responseBody.Data.Id);
        }

        [Fact]
        public async Task UpdateByIdAsync_ShouldReturn_UpdatedPayment()
        {
            var mockManager = new Mock<IPaymentMethodManager>();
            var mockRepository = new Mock<IPaymentMethodRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

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

            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _paymentMethods.SingleOrDefault(p => p.Id == id));

            mockManager.Setup(manager => manager.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<PaymentMethod>(), It.IsAny<JsonPatchDocument<PaymentMethod>>()))
                .Callback((string id, PaymentMethod paymentMethod, JsonPatchDocument<PaymentMethod> updatedProperties) =>
                {
                    updatedProperties.ApplyTo(paymentMethod);
                    _paymentMethods[_paymentMethods.FindIndex(p => p.Id == id)] = paymentMethod;
                });

            mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var paymentMethodController = new PaymentMethodController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await paymentMethodController.UpdateByIdAsync("5fd5ddc549c67b3c6527462f", updatedProperties);
            var response = result as OkObjectResult;
            var responseBody = response.Value as SinglePaymentMethodResponse;

            mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            mockManager.Verify(x => x.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<PaymentMethod>(), It.IsAny<JsonPatchDocument<PaymentMethod>>()), Times.Once);
            mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SinglePaymentMethodResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.False(responseBody.Data.Status);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldDelete_SpecificPayment()
        {
            var mockManager = new Mock<IPaymentMethodManager>();
            var mockRepository = new Mock<IPaymentMethodRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

            mockManager.Setup(manager => manager.DeleteByIdAsync(It.IsAny<string>()))
                .Callback((string id) => _paymentMethods.RemoveAt(_paymentMethods.FindIndex(p => p.Id == id)));

            mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var paymentMethodController = new PaymentMethodController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await paymentMethodController.DeleteByIdAsync("5fd5ddc549c67b3c6527462f");
            var response = result as NoContentResult;

            mockManager.Verify(x => x.DeleteByIdAsync(It.IsAny<string>()), Times.Once);
            mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
        }
    }
}