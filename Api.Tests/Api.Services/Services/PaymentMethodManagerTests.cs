using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Managers;
using Api.Services.Services;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using Xunit;

namespace Api.Tests.Api.Services.Services
{
    public class PaymentMethodManagerTests
    {
        private readonly List<PaymentMethod> _payments = new List<PaymentMethod>
        {
            new PaymentMethod
            {
                Id = "60f0cbb3117067f1b7416088",
                Key = "01",
                Name = "CASH",
                Description = "Cash payment",
                Status = true
            }
        };

        private readonly Mock<IPaymentMethodManager> _mockManager = new Mock<IPaymentMethodManager>();
        
        private readonly Mock<IPaymentMethodRepository> _mockRepository = new Mock<IPaymentMethodRepository>();

        [Fact]
        public async Task CreateAsync_Should_Add_New_PaymentMethod()
        {
            var mockManager = new Mock<IManager<PaymentMethod>>();
            var payment = new PaymentMethod
            {
                Key = "04",
                Name = "CARD",
                Description = "Card payment",
                Status = true
            };
            const string OBJECT_ID = "60f0cae6b06995113987163c";

            mockManager.Setup(manager => manager.CreateAsync(It.IsAny<PaymentMethod>()))
                .Callback((PaymentMethod paymentMethod) =>
                {
                    paymentMethod.Id = OBJECT_ID;
                    _payments.Add(paymentMethod);
                });

            var paymentMethodManager = new PaymentMethodManager(mockManager.Object);

            await paymentMethodManager.CreateAsync(payment);

            mockManager.Verify(x => x.CreateAsync(It.IsAny<PaymentMethod>()), Times.Once);
            Assert.Equal(OBJECT_ID, payment.Id);
        }

        [Fact]
        public async Task UpdateByIdAsync_Should_Update_Existing_PaymentMethod()
        {
            var mockManager = new Mock<IManager<PaymentMethod>>();
            var payment = new PaymentMethod
            {
                Id = "60f0cbb3117067f1b7416088",
                Key = "01",
                Name = "CASH",
                Description = "Cash payment",
                Status = true
            };

            const string OBJECT_ID = "60f0cbb3117067f1b7416088";
            const bool NEW_STATUS = false;

            var updatedProperties = new JsonPatchDocument<PaymentMethod>();
            updatedProperties.Replace(p => p.Status, NEW_STATUS);

            mockManager.Setup(manager => manager.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<PaymentMethod>(), It.IsAny<JsonPatchDocument<PaymentMethod>>()))
                .Callback((string id, PaymentMethod payment, JsonPatchDocument<PaymentMethod> updatedProperties) =>
                {
                    updatedProperties.ApplyTo(payment);
                    _payments[_payments.FindIndex(p => p.Id == id)] = payment;
                });

            var paymentMethodManager = new PaymentMethodManager(mockManager.Object);

            await paymentMethodManager.UpdateByIdAsync(OBJECT_ID, payment, updatedProperties);

            mockManager.Verify(x => x.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<PaymentMethod>(), It.IsAny<JsonPatchDocument<PaymentMethod>>()), Times.Once);
            Assert.Equal(payment.Status, NEW_STATUS);
        }

        [Fact]
        public async Task DeleteByIdAsync_Should_Delete_Correct_PaymentMethod()
        {
            var mockManager = new Mock<IManager<PaymentMethod>>();

            mockManager.Setup(manager => manager.DeleteByIdAsync(It.IsAny<string>()))
                .Callback((string id) => _payments.RemoveAt(_payments.FindIndex(p => p.Id == id)));

            const string PAYMENT_ID = "60f0cbb3117067f1b7416088";

            var paymentMethodManager = new PaymentMethodManager(mockManager.Object);

            await paymentMethodManager.DeleteByIdAsync(PAYMENT_ID);

            mockManager.Verify(x => x.DeleteByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.Empty(_payments);
        }
    }
}