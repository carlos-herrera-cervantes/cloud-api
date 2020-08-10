using System.Threading.Tasks;
using Api.Domain.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace Api.Services.Services
{
    public interface IUserManager
    {
        Task CreateAsync(User station);

        Task UpdateByIdAsync(string id, User newUser, JsonPatchDocument<User> currentUser);

        Task DeleteByIdAsync(string id);
    }
}