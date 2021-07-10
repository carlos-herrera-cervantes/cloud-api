using System.Threading.Tasks;
using Api.Domain.Models;
using MongoDB.Driver;

namespace Api.Services.Services
{
    public interface ITokenRepository
    {
         Task<AccessToken> GetOneAsync(FilterDefinition<AccessToken> filter);
    }
}