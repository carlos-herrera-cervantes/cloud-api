using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;

namespace Api.Services.Services
{
    public interface IUserRepository
    {
         Task<IEnumerable<User>> GetAllAsync();

        Task<User> GetByIdAsync(string id);

        Task<User> GetOneAsync();
    }
}