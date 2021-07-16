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
    public class ProductManagerTests
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
            }
        };

        [Fact]
        public async Task CreateAsync_Should_Add_New_Product()
        {
            var mockManager = new Mock<IManager<Product>>();
            var product = new Product
            {
                Name = "Product 2",
                Description = "Product 2 description",
                Price = 20,
                PricePublic = 25,
                Type = "fuel"
            };
            const string OBJECT_ID = "60f0cae6b06995113987163c";

            mockManager.Setup(manager => manager.CreateAsync(It.IsAny<Product>()))
                .Callback((Product product) =>
                {
                    product.Id = OBJECT_ID;
                    _products.Add(product);
                });

            var productManager = new ProductManager(mockManager.Object);

            await productManager.CreateAsync(product);

            mockManager.Verify(x => x.CreateAsync(It.IsAny<Product>()), Times.Once);
            Assert.Equal(OBJECT_ID, product.Id);
        }

        [Fact]
        public async Task UpdateByIdAsync_Should_Update_Existing_Product()
        {
            var mockManager = new Mock<IManager<Product>>();
            var product = new Product
            {
                Id = "60f0cbb3117067f1b7416088",
                Name = "Product 1",
                Description = "Product 1 description",
                Price = 10,
                PricePublic = 18,
                Type = "fuel"
            };

            const string OBJECT_ID = "60f0cbb3117067f1b7416088";
            const string NEW_NAME = "Product 3";

            var updatedProperties = new JsonPatchDocument<Product>();
            updatedProperties.Replace(p => p.Name, NEW_NAME);

            mockManager.Setup(manager => manager.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<Product>(), It.IsAny<JsonPatchDocument<Product>>()))
                .Callback((string id, Product product, JsonPatchDocument<Product> updatedProperties) =>
                {
                    updatedProperties.ApplyTo(product);
                    _products[_products.FindIndex(u => u.Id == id)] = product;
                });

            var productManager = new ProductManager(mockManager.Object);

            await productManager.UpdateByIdAsync(OBJECT_ID, product, updatedProperties);

            mockManager.Verify(x => x.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<Product>(), It.IsAny<JsonPatchDocument<Product>>()), Times.Once);
            Assert.Equal(product.Name, NEW_NAME);
        }

        [Fact]
        public async Task DeleteByIdAsync_Should_Delete_Correct_Product()
        {
            var mockManager = new Mock<IManager<Product>>();

            mockManager.Setup(manager => manager.DeleteByIdAsync(It.IsAny<string>()))
                .Callback((string id) => _products.RemoveAt(_products.FindIndex(p => p.Id == id)));

            const string OBJECT_ID = "60f0cbb3117067f1b7416088";

            var productManager = new ProductManager(mockManager.Object);

            await productManager.DeleteByIdAsync(OBJECT_ID);

            mockManager.Verify(x => x.DeleteByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.Empty(_products);
        }
    }
}
