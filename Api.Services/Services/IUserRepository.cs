using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;
using MongoDB.Driver;

namespace Api.Services.Services
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync(ListResourceRequest request);

        Task<User> GetByIdAsync(string id);

        Task<User> GetOneAsync(ListResourceRequest request);

        Task<User> GetOneAsync(FilterDefinition<User> filter);

        Task<int> CountAsync(ListResourceRequest request);

        Task<int> CountAsync();
    }
}