using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Repositories;
using MongoDB.Driver;

namespace Api.Services.Services
{
    public class StationRepository : IStationRepository
    {
        private readonly IRepository<Station> _stationRepository;

        public StationRepository(IRepository<Station> stationRepository)
            => _stationRepository = stationRepository;

        public async Task<IEnumerable<Station>> GetAllAsync(ListResourceRequest request)
            => await _stationRepository.GetAllAsync(request, null);

        public async Task<Station> GetByIdAsync(string id) => await _stationRepository.GetByIdAsync(id);

        public async Task<Station> GetOneAsync(ListResourceRequest request)
            => await _stationRepository.GetOneAsync(request);

        public async Task<Station> GetOneAsync(FilterDefinition<Station> filter)
            => await _stationRepository.GetOneAsync(filter);

        public async Task<int> CountAsync(ListResourceRequest request)
            => await _stationRepository.CountAsync(request);
    }
}
