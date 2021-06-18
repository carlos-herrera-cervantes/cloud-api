using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;
using MongoDB.Driver;

namespace Api.Services.Services
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync(Request request);

        Task<User> GetByIdAsync(string id);

        Task<User> GetOneAsync(Request request);

        Task<User> GetOneAsync(FilterDefinition<User> filter);

        Task<int> CountAsync(Request request);
    }
}