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
    public class StationControllerTests
    {
        private readonly List<Station> _stations = new List<Station>
        {
            new Station
            {
                Id = "60f0cbb3117067f1b7416088",
                Name = "Acapulco Centro",
                Email = "contact@mytransformation.com",
                StationKey = "ACC001",
                State = "Guerrero",
                Municipality = "Acapulco de Juárez"
            }
        };
        private readonly Mock<IStationManager> _mockManager = new Mock<IStationManager>();
        private readonly Mock<IStationRepository> _mockRepository = new Mock<IStationRepository>();
        private readonly Mock<IOperationHandler> _mockOperationHandler = new Mock<IOperationHandler>();
        private readonly Mock<IRedisClientsManagerAsync> _mockRedisManager = new Mock<IRedisClientsManagerAsync>();
        private readonly Mock<IRedisClientAsync> _mockRedisClient = new Mock<IRedisClientAsync>();
        private readonly StationController _stationController;

        public StationControllerTests()
        {
            _stationController = new StationController
            (
                _mockManager.Object,
                _mockRepository.Object,
                _mockOperationHandler.Object,
                _mockRedisManager.Object
            );
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturn_StationsList()
        {
            _mockRepository.Setup(repo => repo.CountAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(1);
            _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(_stations);

            var result = await _stationController.GetAllAsync(new ListResourceRequest { Page = 1, PageSize = 10 });
            var response = result as OkObjectResult;
            var responseBody = response.Value as ListStationResponse;

            _mockRepository.Verify(x => x.GetAllAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            _mockRepository.Verify(x => x.CountAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<ListStationResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(1, responseBody.Paginator.Page);
            Assert.Equal(10, responseBody.Paginator.PageSize);
            Assert.Equal(0, responseBody.Paginator.RemainingDocuments);
            Assert.Single(responseBody.Data);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn_SingleStation()
        {
            _mockRedisManager.Setup(redis => redis.GetClientAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockRedisClient.Object);

            _mockRedisClient.Setup(client => client
                .GetAsync<Station>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => null);

            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _stations.SingleOrDefault(s => s.Id == id));

            var result = await _stationController.GetByIdAsync("60f0cbb3117067f1b7416088");
            var response = result as OkObjectResult;
            var responseBody = response.Value as SingleStationResponse;

            _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            _mockRedisManager.Verify(x => x.GetClientAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x =>
                x.GetAsync<Station>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SingleStationResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal("60f0cbb3117067f1b7416088", responseBody.Data.Id);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturn_CreatedStation()
        {
            var station = new Station
            {
                Name = "Ixtapa Centro",
                Email = "contact@mytransformation.com",
                StationKey = "IXT001",
                State = "Guerrero",
                Municipality = "Ixtapa Zihuatanejo"
            };

            _mockManager.Setup(manager => manager.CreateAsync(It.IsAny<Station>()))
                .Callback((Station station) => 
                {
                    station.Id = "60f70c1f7098da34083f12e2";
                    _stations.Add(station);
                });

            _mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var result = await _stationController.CreateAsync(station);
            var response = result as CreatedResult;
            var responseBody = response.Value as SingleStationResponse;

            _mockManager.Verify(x => x.CreateAsync(It.IsAny<Station>()), Times.Once);
            _mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            Assert.IsType<CreatedResult>(result);
            Assert.IsType<SingleStationResponse>(response.Value);
            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
            Assert.Equal("60f70c1f7098da34083f12e2", responseBody.Data.Id);
        }

        [Fact]
        public async Task UpdateByIdAsync_ShouldReturn_UpdatedStation()
        {
            var station = new Station
            {
                Id = "60f0cbb3117067f1b7416088",
                Name = "Acapulco Centro",
                Email = "contact@mytransformation.com",
                StationKey = "ACC001",
                State = "Guerrero",
                Municipality = "Acapulco de Juárez"
            };

            const string NEW_NAME = "Acapulco Pie de la cuesta";

            var updatedProperties = new JsonPatchDocument<Station>();
            updatedProperties.Replace(s => s.Name, NEW_NAME);

            _mockRedisManager.Setup(redis => redis.GetClientAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockRedisClient.Object);

            _mockRedisClient.Setup(client => client
                .GetAsync<Station>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => null);
            
            _mockRedisClient.Setup(client => client
                .RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _stations.SingleOrDefault(s => s.Id == id));

            _mockManager.Setup(manager => manager
                .UpdateByIdAsync(It.IsAny<string>(), It.IsAny<Station>(), It.IsAny<JsonPatchDocument<Station>>()))
                .Callback((string id, Station station, JsonPatchDocument<Station> updatedProperties) =>
                {
                    updatedProperties.ApplyTo(station);
                    _stations[_stations.FindIndex(s => s.Id == id)] = station;
                });

            _mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var result = await _stationController.UpdateByIdAsync("60f0cbb3117067f1b7416088", updatedProperties);
            var response = result as OkObjectResult;
            var responseBody = response.Value as SingleStationResponse;

            _mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            _mockManager.Verify(x =>
                x.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<Station>(), It.IsAny<JsonPatchDocument<Station>>()), Times.Once);
            _mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            _mockRedisManager.Verify(x => x.GetClientAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x => x.GetAsync<Station>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SingleStationResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(NEW_NAME, responseBody.Data.Name);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldDelete_SpecificStation()
        {
            _mockRedisManager.Setup(redis => redis.GetClientAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockRedisClient.Object);

            _mockRedisClient.Setup(client => client
                .RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockManager.Setup(manager => manager.DeleteByIdAsync(It.IsAny<string>()))
                .Callback((string id) => _stations.RemoveAt(_stations.FindIndex(s => s.Id == id)));

            _mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var result = await _stationController.DeleteByIdAsync("60f0cbb3117067f1b7416088");
            var response = result as NoContentResult;

            _mockManager.Verify(x => x.DeleteByIdAsync(It.IsAny<string>()), Times.Once);
            _mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            _mockRedisManager.Verify(x => x.GetClientAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockRedisClient.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
        }
    }
}