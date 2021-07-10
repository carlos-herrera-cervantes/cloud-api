using System.Threading.Tasks;
using Api.Domain.Models;
using MongoDB.Driver;

namespace Api.Services.Services
{
    public interface ITokenManager
    {
         Task CreateAsync(AccessToken token);

         Task<DeleteResult> DeleteManyAsync(ListResourceRequest request);

         Task<DeleteResult> DeleteManyAsync(FilterDefinition<AccessToken> filter);
    }
}