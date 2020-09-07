using System.Threading.Tasks;
using Api.Domain.Models;

namespace Api.Services.Services
{
    public interface ITokenManager
    {
         Task CreateAsync(AccessToken token);
    }
}