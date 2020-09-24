using System.Threading.Tasks;
using Api.Domain.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace Api.Services.Services
{
    public interface ICustomerPurchaseManager
    {
        Task CreateAsync(CustomerPurchase customerPurchase);

        Task UpdateByIdAsync(string id, CustomerPurchase newCustomerPurchase, JsonPatchDocument<CustomerPurchase> currentCustomerPurchase);

        Task DeleteByIdAsync(string id);
    }
}