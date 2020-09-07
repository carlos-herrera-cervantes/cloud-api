using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Managers;

namespace Api.Services.Services
{
    public class TokenManager : ITokenManager
    {
        private readonly IManager<AccessToken> _tokenManager;

        public TokenManager(IManager<AccessToken> tokenManager) => _tokenManager = tokenManager;

        public async Task CreateAsync(AccessToken token) => await _tokenManager.CreateAsync(token);
    }
}