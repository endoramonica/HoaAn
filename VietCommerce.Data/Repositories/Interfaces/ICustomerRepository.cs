using VietCommerce.Core.Entities.Customers;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer?> GetByUserIdAsync(Guid userId);
        Task<bool> EmailExistsAsync(string email);
        Task<Customer> EnsureCustomerExistsAsync(Guid userId, Guid storeId);

    }
}
