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
        private readonly Mock<IProductManager> _mockManager = new Mock<IProductManager>();
        private readonly Mock<IProductRepository> _mockRepository = new Mock<IProductRepository>();
        private readonly Mock<IOperationHandler> _mockOperationHandler = new Mock<IOperationHandler>();
        private readonly Mock<IRedisClientsManagerAsync> _mockRedisManager = new Mock<IRedisClientsManagerAsync>();
        private readonly Mock<IRedisClientAsync> _mockRedisClient = new Mock<IRedisClientAsync>();
        private readonly ProductController _productController;

        public ProductControllerTests()
        {
            _productController = new ProductController
            (
                _mockManager.Object,
                _mockRepository.Object,
                _mockOperationHandler.Object,
                _mockRedisManager.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturn_ProductsList()
        {
            _mockRepository.Setup(repo => repo
                .CountAsync(It.IsAny<ListResourceRequest>()))
                .ReturnsAsync(1);
            _mockRepository.Setup(repo => repo
                .GetAllAsync(It.IsAny<ListResourceRequest>()))
                .ReturnsAsync(_products);

            var result = await _productController
                .GetAllAsync(new ListResourceRequest { Page = 1, PageSize = 10 });
            var response = result as OkObjectResult;
            var responseBody = response.Value as ListProductResponse;

            _mockRepository.Verify(x => x.GetAllAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            _mockRepository.Verify(x => x.CountAsync(It.IsAny<ListResourceRequest>()), Times.Once);
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
            _mockRedisManager.Setup(redis => redis
                .GetClientAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockRedisClient.Object);

            _mockRedisClient.Setup(client => client
                .GetAsync<Product>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => null);

            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _products.SingleOrDefault(p => p.Id == id));

            var result = await _productController.GetByIdAsync("60f0cbb3117067f1b7416088");
            var response = result as OkObjectResult;
            var responseBody = response.Value as SingleProductResponse;

            _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            _mockRedisManager.Verify(x => x.GetClientAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x =>
                x.GetAsync<Product>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SingleProductResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal("60f0cbb3117067f1b7416088", responseBody.Data.Id);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturn_CreatedProduct()
        {
            var product = new Product
            {
                Name = "Product 2",
                Description = "Description 2",
                Price = 15,
                PricePublic = 25,
                Type = "fuel"
            };

            _mockManager.Setup(manager => manager.CreateAsync(It.IsAny<Product>()))
                .Callback((Product product) => 
                {
                    product.Id = "60f70c1f7098da34083f12e2";
                    _products.Add(product);
                });

            _mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var result = await _productController.CreateAsync(product);
            var response = result as CreatedResult;
            var responseBody = response.Value as SingleProductResponse;

            _mockManager.Verify(x => x.CreateAsync(It.IsAny<Product>()), Times.Once);
            _mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            Assert.IsType<CreatedResult>(result);
            Assert.IsType<SingleProductResponse>(response.Value);
            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
            Assert.Equal("60f70c1f7098da34083f12e2", responseBody.Data.Id);
        }

        [Fact]
        public async Task UpdateByIdAsync_ShouldReturn_UpdatedProduct()
        {
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

            _mockRedisManager.Setup(redis => redis
                .GetClientAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockRedisClient.Object);

            _mockRedisClient.Setup(client => client
                .GetAsync<Product>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => null);

            _mockRedisClient.Setup(client => client
                .RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _products.SingleOrDefault(p => p.Id == id));

            _mockManager.Setup(manager => manager
                .UpdateByIdAsync
                (
                    It.IsAny<string>(),
                    It.IsAny<Product>(),
                    It.IsAny<JsonPatchDocument<Product>>())
                )
                .Callback((string id, Product product, JsonPatchDocument<Product> updatedProperties) =>
                {
                    updatedProperties.ApplyTo(product);
                    _products[_products.FindIndex(p => p.Id == id)] = product;
                });

            _mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var result = await _productController.UpdateByIdAsync("60f0cbb3117067f1b7416088", updatedProperties);
            var response = result as OkObjectResult;
            var responseBody = response.Value as SingleProductResponse;

            _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            _mockManager.Verify(x => x
                .UpdateByIdAsync
                (
                    It.IsAny<string>(),
                    It.IsAny<Product>(),
                    It.IsAny<JsonPatchDocument<Product>>()),
                    Times.Once
                );
            _mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            _mockRedisManager.Verify(x => x.GetClientAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x =>
                x.GetAsync<Product>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x =>
                x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SingleProductResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(NEW_PRICE, responseBody.Data.Price);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldDelete_SpecificProduct()
        {
            _mockRedisManager.Setup(redis => redis
                .GetClientAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockRedisClient.Object);

            _mockRedisClient.Setup(client => client
                .RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockManager.Setup(manager => manager.DeleteByIdAsync(It.IsAny<string>()))
                .Callback((string id) => _products.RemoveAt(_products.FindIndex(p => p.Id == id)));

            _mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var result = await _productController.DeleteByIdAsync("60f0cbb3117067f1b7416088");
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