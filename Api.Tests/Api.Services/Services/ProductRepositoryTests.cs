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
    public class ProductRepositoryTests
    {
        private readonly List<Product> _products = new List<Product>
        {
            new Product
            {
                Id = "60f0cbb3117067f1b7416088",
                Name = "Product 1",
                Description = "Product 1 description",
                Price = 10,
                PricePublic = 18,
                Type = "fuel"
            },
            new Product
            {
                Id = "60f0cae6b06995113987163c",
                Name = "Product 2",
                Description = "Product 2 description",
                Price = 20,
                PricePublic = 25,
                Type = "fuel"
            }
        };

        [Fact]
        public async Task GetAllAsync_Should_Return_Products_List()
        {
            var mockRepository = new Mock<IRepository<Product>>();

            mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<ListResourceRequest>(), null))
                .ReturnsAsync(_products);

            const int TOTAL_DOCS = 2;
            const string PRODUCT_ID_1 = "60f0cbb3117067f1b7416088";
            const string PRODUCT_ID_2 = "60f0cae6b06995113987163c";

            var productRepository = new ProductRepository(mockRepository.Object);

            var result = await productRepository.GetAllAsync(new ListResourceRequest());

            mockRepository.Verify(x => x.GetAllAsync(It.IsAny<ListResourceRequest>(), null), Times.Once);
            Assert.Equal(result.Count(), TOTAL_DOCS);
            Assert.Equal(result.FirstOrDefault().Id, PRODUCT_ID_1);
            Assert.Equal(result.LastOrDefault().Id, PRODUCT_ID_2);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Product()
        {
            var mockRepository = new Mock<IRepository<Product>>();

            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _products.SingleOrDefault((p => p.Id == id)));

            const string PRODUCT_ID = "60f0cae6b06995113987163c";
            var productRepository = new ProductRepository(mockRepository.Object);

            var result = await productRepository.GetByIdAsync(PRODUCT_ID);

            mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.Equal(result.Id, PRODUCT_ID);
        }

        [Fact]
        public async Task CountAsync_Should_Return_Number_Products()
        {
            var mockRepository = new Mock<IRepository<Product>>();

            mockRepository.Setup(repo => repo.CountAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(2);

            var productRepository = new ProductRepository(mockRepository.Object);

            var result = await productRepository.CountAsync(new ListResourceRequest());

            mockRepository.Verify(x => x.CountAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            Assert.Equal(2, result);
        }
    }
}
