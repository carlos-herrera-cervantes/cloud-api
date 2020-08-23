using System.Threading.Tasks;
using Api.Domain.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace Api.Services.Services
{
    public interface IProductManager
    {
        Task CreateAsync(Product product);

        Task UpdateByIdAsync(string id, Product newProduct, JsonPatchDocument<Product> currentProduct);

        Task DeleteByIdAsync(string id);
    }
}