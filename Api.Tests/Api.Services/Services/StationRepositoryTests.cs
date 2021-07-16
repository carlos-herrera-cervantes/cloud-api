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
    public class StationRepositoryTests
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
            },
            new Station
            {
                Id = "60f0cae6b06995113987163c",
                Name = "Ixtapa Centro",
                Email = "contact@mytransformation.com",
                StationKey = "IXT001",
                State = "Guerrero",
                Municipality = "Ixtapa Zihuatanejo"
            }
        };

        [Fact]
        public async Task GetAllAsync_Should_Return_Stations_List()
        {
            var mockRepository = new Mock<IRepository<Station>>();

            mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<ListResourceRequest>(), null))
                .ReturnsAsync(_stations);

            const int TOTAL_DOCS = 2;
            const string STATION_ID_1 = "60f0cbb3117067f1b7416088";
            const string STATION_ID_2 = "60f0cae6b06995113987163c";

            var stationRepository = new StationRepository(mockRepository.Object);

            var result = await stationRepository.GetAllAsync(new ListResourceRequest());

            mockRepository.Verify(x => x.GetAllAsync(It.IsAny<ListResourceRequest>(), null), Times.Once);
            Assert.Equal(result.Count(), TOTAL_DOCS);
            Assert.Equal(result.FirstOrDefault().Id, STATION_ID_1);
            Assert.Equal(result.LastOrDefault().Id, STATION_ID_2);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Station()
        {
            var mockRepository = new Mock<IRepository<Station>>();

            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => _stations.SingleOrDefault((s => s.Id == id)));

            const string STATION_ID = "60f0cae6b06995113987163c";
            var stationRepository = new StationRepository(mockRepository.Object);

            var result = await stationRepository.GetByIdAsync(STATION_ID);

            mockRepository.Verify(x => x.GetByIdAsync(It.IsAny<string>()), Times.Once);
            Assert.Equal(result.Id, STATION_ID);
        }

        [Fact]
        public async Task CountAsync_Should_Return_Number_Stations()
        {
            var mockRepository = new Mock<IRepository<Station>>();

            mockRepository.Setup(repo => repo.CountAsync(It.IsAny<ListResourceRequest>())).ReturnsAsync(2);

            var stationRepository = new StationRepository(mockRepository.Object);

            var result = await stationRepository.CountAsync(new ListResourceRequest());

            mockRepository.Verify(x => x.CountAsync(It.IsAny<ListResourceRequest>()), Times.Once);
            Assert.Equal(2, result);
        }
    }
}
