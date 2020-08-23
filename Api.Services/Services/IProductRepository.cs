using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;

namespace Api.Services.Services
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();

        Task<Product> GetByIdAsync(string id);

        Task<Product> GetOneAsync();
    }
}