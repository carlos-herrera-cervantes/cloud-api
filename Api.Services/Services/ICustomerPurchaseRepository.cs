using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Domain.Models;

namespace Api.Services.Services
{
    public interface ICustomerPurchaseRepository
    {
        Task<IEnumerable<CustomerPurchase>> GetAllAsync(Request request);
         
        Task<CustomerPurchase> GetByIdAsync(string id);

        Task<CustomerPurchase> GetOneAsync(Request request);

        Task<int> CountAsync(Request request);
    }
}