using System.Threading.Tasks;
using Api.Domain.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace Api.Repository.Managers
{
    public interface IManager<T> where T : BaseEntity
    {
        Task CreateAsync(T instance);

        Task UpdateByIdAsync(string id, T newInstance, JsonPatchDocument<T> currentInstance);

        Task DeleteByIdAsync(string id);
    }
}
