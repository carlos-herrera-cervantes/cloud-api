using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Managers;
using Microsoft.AspNetCore.JsonPatch;

namespace Api.Services.Services
{
    public class StationManager : IStationManager
    {
        private readonly IManager<Station> _stationRepository;

        public StationManager(IManager<Station> stationRepostitory) => _stationRepository = stationRepostitory;

        public async Task CreateAsync(Station station) => await _stationRepository.CreateAsync(station);

        public async Task UpdateByIdAsync(string id, Station newStation, JsonPatchDocument<Station> currentStation)
            => await _stationRepository.UpdateByIdAsync(id, newStation, currentStation);

        public async Task DeleteByIdAsync(string id) => await _stationRepository.DeleteByIdAsync(id);
    }
}
