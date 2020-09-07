using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Repositories;
using MongoDB.Driver;

namespace Api.Services.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly IRepository<Product> _productRepository;

        public ProductRepository(IRepository<Product> productRepository) => _productRepository = productRepository;

        public async Task<IEnumerable<Product>> GetAllAsync() => await _productRepository.GetAllAsync();

        public async Task<Product> GetByIdAsync(string id) => await _productRepository.GetByIdAsync(id);

        public async Task<Product> GetOneAsync(Request request) => await _productRepository.GetOneAsync(request);
    }
}