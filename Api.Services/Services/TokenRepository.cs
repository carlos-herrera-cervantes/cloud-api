using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Repositories;

namespace Api.Services.Services
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IRepository<AccessToken> _tokenRepository;

        public TokenRepository(IRepository<AccessToken> tokenRepository) => _tokenRepository = tokenRepository;

        public async Task<AccessToken> GetOneAsync(Request request) => await _tokenRepository.GetOneAsync(request);
    }
}