using VietCommerce.Core.DTOs.Customers;
using VietCommerce.Core.Entities.Customers;

namespace VietCommerce.Data.Repositories.Interfaces;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<Customer?> GetCustomerWithDetailsAsync(Guid id);
    Task<Customer?> GetCustomerByUserIdAsync(Guid userId);
    Task<bool> EmailExistsAsync(string email);
    Task<(IEnumerable<CustomerListDTO> customers, int totalCount)> GetCustomersPagedAsync(
        int pageNumber, int pageSize, string? searchTerm = null);
}
