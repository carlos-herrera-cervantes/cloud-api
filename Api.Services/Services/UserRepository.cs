using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Repositories;
using MongoDB.Driver;

namespace Api.Services.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IRepository<User> _userRepository;

        public UserRepository(IRepository<User> userRepository) => _userRepository = userRepository;

        public async Task<IEnumerable<User>> GetAllAsync(Request request) => await _userRepository.GetAllAsync(request, null);

        public async Task<User> GetByIdAsync(string id) => await _userRepository.GetByIdAsync(id);

        public async Task<User> GetOneAsync(Request request) => await _userRepository.GetOneAsync(request);

        public async Task<User> GetOneAsync(FilterDefinition<User> filter) => await _userRepository.GetOneAsync(filter);

        public async Task<int> CountAsync(Request request) => await _userRepository.CountAsync(request);
    }
}