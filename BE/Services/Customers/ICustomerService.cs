using VietCommerce.Core.DTOs.Customers;
using VietCommerce.Core.Models;

namespace VietCommerce.Core.Services.Customers;

public interface ICustomerService
{
    Task<PaginatedResult<CustomerListDTO>> GetPaginatedAsync(int pageNumber = 1, int pageSize = 10);
    Task<CustomerListDTO> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CustomerCreateDTO dto);
    Task UpdateAsync(Guid id, CustomerUpdateDTO dto);
    Task DeleteAsync(Guid id);
}