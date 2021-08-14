using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Repository.Managers;
using Microsoft.AspNetCore.JsonPatch;

namespace Api.Services.Services
{
    public class CustomerPurchaseManager : ICustomerPurchaseManager
    {
        private readonly IManager<CustomerPurchase> _customerPurchaseManager;

        public CustomerPurchaseManager(IManager<CustomerPurchase> customerPurchaseManager)
            => _customerPurchaseManager = customerPurchaseManager;

        public async Task CreateAsync(CustomerPurchase customerPurchase)
            => await _customerPurchaseManager.CreateAsync(customerPurchase);

        public async Task UpdateByIdAsync
        (
            string id,
            CustomerPurchase newCustomerPurchase,
            JsonPatchDocument<CustomerPurchase> currentCustomerPurchase
        )
            => await _customerPurchaseManager.UpdateByIdAsync(id, newCustomerPurchase, currentCustomerPurchase);

        public async Task DeleteByIdAsync(string id)
            => await _customerPurchaseManager.DeleteByIdAsync(id);
    }
}