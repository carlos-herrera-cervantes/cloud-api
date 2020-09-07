using System.Threading.Tasks;
using Api.Domain.Models;

namespace Api.Services.Services
{
    public interface ITokenRepository
    {
         Task<AccessToken> GetOneAsync(Request request);
    }
}