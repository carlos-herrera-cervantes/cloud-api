using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;

namespace Api.Services.Services
{
    public interface IPaymentMethodRepository
    {
        Task<IEnumerable<PaymentMethod>> GetAllAsync();

        Task<PaymentMethod> GetByIdAsync(string id);

        Task<PaymentMethod> GetOneAsync(Request request);
    }
}