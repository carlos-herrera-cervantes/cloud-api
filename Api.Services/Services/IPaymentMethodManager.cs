using System.Threading.Tasks;
using Api.Domain.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace Api.Services.Services
{
    public interface IPaymentMethodManager
    {
        Task CreateAsync(PaymentMethod paymentMethod);

        Task UpdateByIdAsync(string id, PaymentMethod newPaymentMethod, JsonPatchDocument<PaymentMethod> currentPaymentMethod);

        Task DeleteByIdAsync(string id);
    }
}