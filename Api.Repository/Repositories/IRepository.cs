using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;

namespace Api.Repository.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync(Request request, List<Relation> relations);

        Task<T> GetByIdAsync(string id);

        Task<T> GetOneAsync(Request request);

        Task<int> CountAsync(Request request);
    }
}
