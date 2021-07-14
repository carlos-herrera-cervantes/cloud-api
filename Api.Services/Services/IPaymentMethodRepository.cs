using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;

namespace Api.Services.Services
{
    public interface IPaymentMethodRepository
    {
        Task<IEnumerable<PaymentMethod>> GetAllAsync(ListResourceRequest request);

        Task<PaymentMethod> GetByIdAsync(string id);

        Task<PaymentMethod> GetOneAsync(ListResourceRequest request);

        Task<int> CountAsync(ListResourceRequest request);
    }
}