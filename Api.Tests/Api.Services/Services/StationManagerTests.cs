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
    public class StationManagerTests
    {
        private List<Station> _stations = new List<Station>
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
        public async Task CreateAsync_Should_Add_New_Station()
        {
            var mockManager = new Mock<IManager<Station>>();
            var station = new Station
            {
                Name = "Ixtapa Centro",
                Email = "contact@mytransformation.com",
                StationKey = "IXT001",
                State = "Guerrero",
                Municipality = "Ixtapa Zihuatanejo"
            };
            const string OBJECT_ID = "60f0cae6b06995113987163c";

            mockManager.Setup(manager => manager.CreateAsync(It.IsAny<Station>()))
                .Callback((Station station) =>
                {
                    station.Id = OBJECT_ID;
                    _stations.Add(station);
                });

            var stationManager = new StationManager(mockManager.Object);

            await stationManager.CreateAsync(station);

            mockManager.Verify(x => x.CreateAsync(It.IsAny<Station>()), Times.Once);
            Assert.Equal(OBJECT_ID, station.Id);
        }

        [Fact]
        public async Task UpdateByIdAsync_Should_Update_Existing_Station()
        {
            var mockManager = new Mock<IManager<Station>>();
            var station = new Station
            {
                Id = "60f0cbb3117067f1b7416088",
                Name = "Acapulco Centro",
                Email = "contact@mytransformation.com",
                StationKey = "ACC001",
                State = "Guerrero",
                Municipality = "Acapulco de Juárez"
            };

            const string OBJECT_ID = "60f0cbb3117067f1b7416088";
            const string NEW_NAME = "Acapulco La Cima";

            var updatedProperties = new JsonPatchDocument<Station>();
            updatedProperties.Replace(s => s.Name, NEW_NAME);

            mockManager.Setup(manager => manager.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<Station>(), It.IsAny<JsonPatchDocument<Station>>()))
                .Callback((string id, Station station, JsonPatchDocument<Station> updatedProperties) =>
                {
                    updatedProperties.ApplyTo(station);
                    _stations[_stations.FindIndex(u => u.Id == id)] = station;
                });

            var stationManager = new StationManager(mockManager.Object);

            await stationManager.UpdateByIdAsync(OBJECT_ID, station, updatedProperties);

            mockManager.Verify(x => x.UpdateByIdAsync(It.IsAny<string>(), It.IsAny<Station>(), It.IsAny<JsonPatchDocument<Station>>()), Times.Once);
            Assert.Equal(station.Name, NEW_NAME);
        }

        [Fact]
        public async Task DeleteByIdAsync_Should_Delete_Correct_Station()
        {
            var mockManager = new Mock<IManager<Station>>();

            mockManager.Setup(manager => manager.DeleteByIdAsync(It.IsAny<string>()))
                .Callback((string id) => _stations.RemoveAt(_stations.FindIndex(s => s.Id == id)));

            const string OBJECT_ID = "60f0cbb3117067f1b7416088";

            var stationManager = new StationManager(mockManager.Object);

            await stationManager.DeleteByIdAsync(OBJECT_ID);

            mockManager.Verify(x => x.DeleteByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.Empty(_stations);
        }
    }
}
