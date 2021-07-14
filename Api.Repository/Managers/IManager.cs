using System.Threading.Tasks;
using Api.Domain.Models;
using Microsoft.AspNetCore.JsonPatch;
using MongoDB.Driver;

namespace Api.Repository.Managers
{
    public interface IManager<T> where T : BaseEntity
    {
        Task CreateAsync(T instance);

        Task UpdateByIdAsync(string id, T newInstance, JsonPatchDocument<T> currentInstance);

        Task DeleteByIdAsync(string id);

        Task<DeleteResult> DeleteManyAsync(ListResourceRequest request);

        Task<DeleteResult> DeleteManyAsync(FilterDefinition<T> filter);
    }
}
