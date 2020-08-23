using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Managers;
using Microsoft.AspNetCore.JsonPatch;

namespace Api.Services.Services
{
    public class ProductManager : IProductManager
    {
        private readonly IManager<Product> _productManager;

        public ProductManager(IManager<Product> productManager) => _productManager = productManager;

        public async Task CreateAsync(Product product) => await _productManager.CreateAsync(product);

        public async Task UpdateByIdAsync(string id, Product newProduct, JsonPatchDocument<Product> currentProduct)
            => await _productManager.UpdateByIdAsync(id, newProduct, currentProduct);

        public async Task DeleteByIdAsync(string id) => await _productManager.DeleteByIdAsync(id);
    }
}