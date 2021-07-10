using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;

namespace Api.Services.Services
{
    public interface ICustomerPurchaseRepository
    {
        Task<IEnumerable<CustomerPurchase>> GetAllAsync(ListResourceRequest request);
         
        Task<CustomerPurchase> GetByIdAsync(string id);

        Task<CustomerPurchase> GetOneAsync(ListResourceRequest request);

        Task<int> CountAsync(ListResourceRequest request);
    }
}