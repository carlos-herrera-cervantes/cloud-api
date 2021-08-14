using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Managers;
using Microsoft.AspNetCore.JsonPatch;

namespace Api.Services.Services
{
    public class PaymentMethodManager : IPaymentMethodManager
    {
        private readonly IManager<PaymentMethod> _paymentMethodManager;

        public PaymentMethodManager(IManager<PaymentMethod> paymentMethodManager)
            => _paymentMethodManager = paymentMethodManager;

        public async Task CreateAsync(PaymentMethod paymentMethod)
            => await _paymentMethodManager.CreateAsync(paymentMethod);

        public async Task UpdateByIdAsync
        (
            string id,
            PaymentMethod newPaymentMethod,
            JsonPatchDocument<PaymentMethod> currentPaymentMethod
        )
            => await _paymentMethodManager.UpdateByIdAsync(id, newPaymentMethod, currentPaymentMethod);

        public async Task DeleteByIdAsync(string id) => await _paymentMethodManager.DeleteByIdAsync(id);
    }
}