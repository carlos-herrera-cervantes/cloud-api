using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Managers;
using Microsoft.AspNetCore.JsonPatch;

namespace Api.Services.Services
{
    public class UserManager : IUserManager
    {
        private readonly IManager<User> _userRepository;

        public UserManager(IManager<User> userRepostitory) => _userRepository = userRepostitory;

        public async Task CreateAsync(User user) => await _userRepository.CreateAsync(user);

        public async Task UpdateByIdAsync(string id, User newUser, JsonPatchDocument<User> currentUser)
            => await _userRepository.UpdateByIdAsync(id, newUser, currentUser);

        public async Task DeleteByIdAsync(string id) => await _userRepository.DeleteByIdAsync(id);
    }
}