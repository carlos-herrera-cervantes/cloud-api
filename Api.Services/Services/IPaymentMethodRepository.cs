using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;

namespace Api.Services.Services
{
    public interface IPaymentMethodRepository
    {
        Task<IEnumerable<PaymentMethod>> GetAllAsync(Request request);

        Task<PaymentMethod> GetByIdAsync(string id);

        Task<PaymentMethod> GetOneAsync(Request request);

        Task<int> CountAsync(Request request);
    }
}