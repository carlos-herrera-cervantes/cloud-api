using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Repositories;

namespace Api.Services.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IRepository<User> _userRepository;

        public UserRepository(IRepository<User> userRepository) => _userRepository = userRepository;

        public async Task<IEnumerable<User>> GetAllAsync() => await _userRepository.GetAllAsync();

        public async Task<User> GetByIdAsync(string id) => await _userRepository.GetByIdAsync(id);

        public async Task<User> GetOneAsync() => await _userRepository.GetOneAsync();
    }
}