using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Repositories;

namespace Api.Services.Services
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly IRepository<PaymentMethod> _paymentMethodRepository;

        public PaymentMethodRepository(IRepository<PaymentMethod> paymentMethodRepository)
            => _paymentMethodRepository = paymentMethodRepository;

        public async Task<IEnumerable<PaymentMethod>> GetAllAsync(ListResourceRequest request)
            => await _paymentMethodRepository.GetAllAsync(request, null);

        public async Task<PaymentMethod> GetByIdAsync(string id) => await _paymentMethodRepository.GetByIdAsync(id);

        public async Task<PaymentMethod> GetOneAsync(ListResourceRequest request) => await _paymentMethodRepository.GetOneAsync(request);

        public async Task<int> CountAsync(ListResourceRequest request) => await _paymentMethodRepository.CountAsync(request);
    }
}