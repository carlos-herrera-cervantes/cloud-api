using System.Threading.Tasks;
using Api.Domain.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace Api.Services.Services
{
    public interface IStationManager
    {
        Task CreateAsync(Station station);

        Task UpdateByIdAsync(string id, Station newStation, JsonPatchDocument<Station> currentStation);

        Task DeleteByIdAsync(string id);
    }
}
