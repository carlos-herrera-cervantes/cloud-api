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

        [Fact]
        public async Task GetAllAsync_ShouldReturn_StationsList()
        {
            var mockManager = new Mock<IStationManager>();
            var mockRepository = new Mock<IStationRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

            mockRepository.Setup(repo => repo.CountAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(1);
            mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(_stations);

            var stationController = new StationController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await stationController.GetAllAsync(new ListResourceRequest { Page = 1, PageSize = 10 });
            var response = result as OkObjectResult;
            var responseBody = response.Value as ListStationResponse;

            mockRepository.Verify(x => x.GetAllAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            mockRepository.Verify(x => x.CountAsync(It.IsAny<ListResourceRequest>()), Times.Once);
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
            var mockManager = new Mock<IStationManager>();
            var mockRepository = new Mock<IStationRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _stations.SingleOrDefault(s => s.Id == id));

            var stationController = new StationController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await stationController.GetByIdAsync("60f0cbb3117067f1b7416088");
            var response = result as OkObjectResult;
            var responseBody = response.Value as SingleStationResponse;

            mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SingleStationResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal("60f0cbb3117067f1b7416088", responseBody.Data.Id);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturn_CreatedStation()
        {
            var mockManager = new Mock<IStationManager>();
            var mockRepository = new Mock<IStationRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

            var station = new Station
            {
                Name = "Ixtapa Centro",
                Email = "contact@mytransformation.com",
                StationKey = "IXT001",
                State = "Guerrero",
                Municipality = "Ixtapa Zihuatanejo"
            };

            mockManager.Setup(manager => manager.CreateAsync(It.IsAny<Station>()))
                .Callback((Station station) => 
                {
                    station.Id = "60f70c1f7098da34083f12e2";
                    _stations.Add(station);
                });

            mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var stationController = new StationController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await stationController.CreateAsync(station);
            var response = result as CreatedResult;
            var responseBody = response.Value as SingleStationResponse;

            mockManager.Verify(x => x.CreateAsync(It.IsAny<Station>()), Times.Once);
            mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            Assert.IsType<CreatedResult>(result);
            Assert.IsType<SingleStationResponse>(response.Value);
            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
            Assert.Equal("60f70c1f7098da34083f12e2", responseBody.Data.Id);
        }

        [Fact]
        public async Task UpdateByIdAsync_ShouldReturn_UpdatedStation()
        {
            var mockManager = new Mock<IStationManager>();
            var mockRepository = new Mock<IStationRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

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

            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _stations.SingleOrDefault(s => s.Id == id));

            mockManager.Setup(manager => manager.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<Station>(), It.IsAny<JsonPatchDocument<Station>>()))
                .Callback((string id, Station station, JsonPatchDocument<Station> updatedProperties) =>
                {
                    updatedProperties.ApplyTo(station);
                    _stations[_stations.FindIndex(s => s.Id == id)] = station;
                });

            mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var stationController = new StationController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await stationController.UpdateByIdAsync("60f0cbb3117067f1b7416088", updatedProperties);
            var response = result as OkObjectResult;
            var responseBody = response.Value as SingleStationResponse;

            mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            mockManager.Verify(x => x.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<Station>(), It.IsAny<JsonPatchDocument<Station>>()), Times.Once);
            mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SingleStationResponse>(response.Value);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(NEW_NAME, responseBody.Data.Name);
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldDelete_SpecificStation()
        {
            var mockManager = new Mock<IStationManager>();
            var mockRepository = new Mock<IStationRepository>();
            var mockOperationHandler = new Mock<IOperationHandler>();

            mockManager.Setup(manager => manager.DeleteByIdAsync(It.IsAny<string>()))
                .Callback((string id) => _stations.RemoveAt(_stations.FindIndex(s => s.Id == id)));

            mockOperationHandler.Setup(operation => operation.Publish(It.IsAny<CollectionEventReceived>()));

            var stationController = new StationController
            (
                mockManager.Object,
                mockRepository.Object,
                mockOperationHandler.Object
            );

            var result = await stationController.DeleteByIdAsync("60f0cbb3117067f1b7416088");
            var response = result as NoContentResult;

            mockManager.Verify(x => x.DeleteByIdAsync(It.IsAny<string>()), Times.Once);
            mockOperationHandler.Verify(x => x.Publish(It.IsAny<CollectionEventReceived>()), Times.Once);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
        }
    }
}