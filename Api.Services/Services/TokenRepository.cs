using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Repositories;
using MongoDB.Driver;

namespace Api.Services.Services
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IRepository<AccessToken> _tokenRepository;

        public TokenRepository(IRepository<AccessToken> tokenRepository) => _tokenRepository = tokenRepository;

        public async Task<AccessToken> GetOneAsync(FilterDefinition<AccessToken> filter) => await _tokenRepository.GetOneAsync(filter);
    }
}