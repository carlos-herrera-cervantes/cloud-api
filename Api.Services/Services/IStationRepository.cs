using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;

namespace Api.Services.Services
{
    public interface IStationRepository
    {
        Task<IEnumerable<Station>> GetAllAsync();

        Task<Station> GetByIdAsync(string id);

        Task<Station> GetOneAsync(Request request);
    }
}