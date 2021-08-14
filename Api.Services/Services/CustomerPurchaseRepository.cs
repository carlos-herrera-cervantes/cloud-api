using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Repositories;

namespace Api.Services.Services
{
    public class CustomerPurchaseRepository : ICustomerPurchaseRepository
    {
        private readonly IRepository<CustomerPurchase> _customerPurchaseRepository;

        public CustomerPurchaseRepository(IRepository<CustomerPurchase> customerPurchaseRepository)
            => _customerPurchaseRepository = customerPurchaseRepository;

        public async Task<IEnumerable<CustomerPurchase>> GetAllAsync(ListResourceRequest request)
            => await _customerPurchaseRepository.GetAllAsync(request, CustomerPurchase.Relations);

        public async Task<CustomerPurchase> GetByIdAsync(string id)
            => await _customerPurchaseRepository.GetByIdAsync(id);

        public async Task<CustomerPurchase> GetOneAsync(ListResourceRequest request)
            => await _customerPurchaseRepository.GetOneAsync(request);

        public async Task<int> CountAsync(ListResourceRequest request)
            => await _customerPurchaseRepository.CountAsync(request);
    }
}