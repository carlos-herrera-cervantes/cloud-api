using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Repositories;
using Api.Services.Services;
using Moq;
using Xunit;

namespace Api.Tests.Api.Services.Services
{
    public class PaymentMethodRepositoryTests
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
            },
            new PaymentMethod
            {
                Id = "60f0cae6b06995113987163c",
                Key = "04",
                Name = "CARD",
                Description = "Card payment",
                Status = true
            }
        };

        [Fact]
        public async Task GetAllAsync_Should_Return_PaymentMethods_List()
        {
            var mockRepository = new Mock<IRepository<PaymentMethod>>();

            mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<ListResourceRequest>(), null))
                .ReturnsAsync(_payments);

            const int TOTAL_DOCS = 2;
            const string USER_ID_1 = "60f0cbb3117067f1b7416088";
            const string USER_ID_2 = "60f0cae6b06995113987163c";

            var paymentMethodRepository = new PaymentMethodRepository(mockRepository.Object);

            var result = await paymentMethodRepository.GetAllAsync(new ListResourceRequest());

            mockRepository.Verify(x => x.GetAllAsync(It.IsAny<ListResourceRequest>(), null), Times.Once);
            Assert.Equal(result.Count(), TOTAL_DOCS);
            Assert.Equal(result.FirstOrDefault().Id, USER_ID_1);
            Assert.Equal(result.LastOrDefault().Id, USER_ID_2);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_PaymentMethod()
        {
            var mockRepository = new Mock<IRepository<PaymentMethod>>();

            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _payments.SingleOrDefault((p => p.Id == id)));

            const string PAYMENT_ID = "60f0cae6b06995113987163c";
            var paymentMethodRepository = new PaymentMethodRepository(mockRepository.Object);

            var result = await paymentMethodRepository.GetByIdAsync(PAYMENT_ID);

            mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.Equal(result.Id, PAYMENT_ID);
        }

        [Fact]
        public async Task CountAsync_Should_Return_Number_PaymentMethods()
        {
            var mockRepository = new Mock<IRepository<PaymentMethod>>();

            mockRepository.Setup(repo => repo.CountAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(2);

            var paymentMethodRepository = new PaymentMethodRepository(mockRepository.Object);

            var result = await paymentMethodRepository.CountAsync(new ListResourceRequest());

            mockRepository.Verify(x => x.CountAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            Assert.Equal(2, result);
        }
    }
}