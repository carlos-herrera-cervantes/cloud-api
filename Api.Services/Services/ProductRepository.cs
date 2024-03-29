using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Repositories;

namespace Api.Services.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly IRepository<Product> _productRepository;

        public ProductRepository(IRepository<Product> productRepository)
            => _productRepository = productRepository;

        public async Task<IEnumerable<Product>> GetAllAsync(ListResourceRequest request)
            => await _productRepository.GetAllAsync(request, null);

        public async Task<Product> GetByIdAsync(string id)
            => await _productRepository.GetByIdAsync(id);

        public async Task<Product> GetOneAsync(ListResourceRequest request)
            => await _productRepository.GetOneAsync(request);

        public async Task<int> CountAsync(ListResourceRequest request)
            => await _productRepository.CountAsync(request);
    }
}