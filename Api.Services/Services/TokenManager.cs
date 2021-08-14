using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Managers;
using MongoDB.Driver;

namespace Api.Services.Services
{
    public class TokenManager : ITokenManager
    {
        private readonly IManager<AccessToken> _tokenManager;

        public TokenManager(IManager<AccessToken> tokenManager)
            => _tokenManager = tokenManager;

        public async Task CreateAsync(AccessToken token)
            => await _tokenManager.CreateAsync(token);

        public async Task<DeleteResult> DeleteManyAsync(ListResourceRequest request)
            => await _tokenManager.DeleteManyAsync(request);

        public async Task<DeleteResult> DeleteManyAsync(FilterDefinition<AccessToken> filter)
            => await _tokenManager.DeleteManyAsync(filter);
    }
}