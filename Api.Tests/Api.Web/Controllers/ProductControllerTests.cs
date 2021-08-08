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
    public class ProductControllerTests
    {
        private readonly List<Product> _products = new List<Product>
        {
            new Product
            {
                Id = "60f0cbb3117067f1b7416088",
                Name = "Product 1",
                Description = "Description 1",
                Price = 10,
                PricePublic = 18,
                Type = "fuel"
            }
        };

        [Fact]
        public async Task GetAllAsync_ShouldReturn_ProductsList()
        {
            var mockManager = new Mock<IProductManager>();
            var mockRepository = new Mock<IProductRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

            mockRepository.Setup(repo => repo.CountAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(1);
            mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(_products);

            var productController = new ProductController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await productController.GetAllAsync(new ListResourceRequest { Page = 1, PageSize = 10 });
            var response = result as OkObjectResult;
            var responseBody = response.Value as ListProductResponse;

            mockRepository.Verify(x => x.GetAllAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            mockRepository.Verify(x => x.CountAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ListProductResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(1, responseBody.Paginator.Page);
            Assert.Equal(10, responseBody.Paginator.PageSize);
            Assert.Equal(0, responseBody.Paginator.RemainingDocuments);
            Assert.Single(responseBody.Data);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn_SingleProduct()
        {
            var mockManager = new Mock<IProductManager>();
            var mockRepository = new Mock<IProductRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _products.SingleOrDefault(p => p.Id == id));

            var productController = new ProductController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await productController.GetByIdAsync("60f0cbb3117067f1b7416088");
            var response = result as OkObjectResult;
            var responseBody = response.Value as SingleProductResponse;

            mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SingleProductResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal("60f0cbb3117067f1b7416088", responseBody.Data.Id);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturn_CreatedProduct()
        {
            var mockManager = new Mock<IProductManager>();
            var mockRepository = new Mock<IProductRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

            var product = new Product
            {
                Name = "Product 2",
                Description = "Description 2",
                Price = 15,
                PricePublic = 25,
                Type = "fuel"
            };

            mockManager.Setup(manager => manager.CreateAsync(It.IsAny<Product>()))
                .Callback((Product product) => 
                {
                    product.Id = "60f70c1f7098da34083f12e2";
                    _products.Add(product);
                });

            mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var productController = new ProductController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await productController.CreateAsync(product);
            var response = result as CreatedResult;
            var responseBody = response.Value as SingleProductResponse;

            mockManager.Verify(x => x.CreateAsync(It.IsAny<Product>()), Times.Once);
            mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            Assert.IsType<CreatedResult>(result);
            Assert.IsType<SingleProductResponse>(response.Value);
            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
            Assert.Equal("60f70c1f7098da34083f12e2", responseBody.Data.Id);
        }

        [Fact]
        public async Task UpdateByIdAsync_ShouldReturn_UpdatedProduct()
        {
            var mockManager = new Mock<IProductManager>();
            var mockRepository = new Mock<IProductRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

            var product = new Product
            {
                Id = "60f0cbb3117067f1b7416088",
                Name = "Product 1",
                Description = "Description 1",
                Price = 10,
                PricePublic = 18,
                Type = "fuel"
            };

            const decimal NEW_PRICE = 15m;

            var updatedProperties = new JsonPatchDocument<Product>();
            updatedProperties.Replace(p => p.Price, NEW_PRICE);

            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _products.SingleOrDefault(p => p.Id == id));

            mockManager.Setup(manager => manager.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<Product>(), It.IsAny<JsonPatchDocument<Product>>()))
                .Callback((string id, Product product, JsonPatchDocument<Product> updatedProperties) =>
                {
                    updatedProperties.ApplyTo(product);
                    _products[_products.FindIndex(p => p.Id == id)] = product;
                });

            mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var productController = new ProductController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await productController.UpdateByIdAsync("60f0cbb3117067f1b7416088", updatedProperties);
            var response = result as OkObjectResult;
            var responseBody = response.Value as SingleProductResponse;

            mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            mockManager.Verify(x => x.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<Product>(), It.IsAny<JsonPatchDocument<Product>>()), Times.Once);
            mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SingleProductResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(NEW_PRICE, responseBody.Data.Price);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldDelete_SpecificProduct()
        {
            var mockManager = new Mock<IProductManager>();
            var mockRepository = new Mock<IProductRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

            mockManager.Setup(manager => manager.DeleteByIdAsync(It.IsAny<string>()))
                .Callback((string id) => _products.RemoveAt(_products.FindIndex(p => p.Id == id)));

            mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var productController = new ProductController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await productController.DeleteByIdAsync("60f0cbb3117067f1b7416088");
            var response = result as NoContentResult;

            mockManager.Verify(x => x.DeleteByIdAsync(It.IsAny<string>()), Times.Once);
            mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
        }
    }
}