using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;
using MongoDB.Driver;

namespace Api.Services.Services
{
    public interface IStationRepository
    {
        Task<IEnumerable<Station>> GetAllAsync(ListResourceRequest request);

        Task<Station> GetByIdAsync(string id);

        Task<Station> GetOneAsync(ListResourceRequest request);

        Task<Station> GetOneAsync(FilterDefinition<Station> filter);

        Task<int> CountAsync(ListResourceRequest request);
    }
}