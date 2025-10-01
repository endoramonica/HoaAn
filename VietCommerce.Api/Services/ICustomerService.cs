using VietCommerce.Core.DTOs.Customers;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Services.Interfaces;

public interface ICustomerService : IGenericServices<Customer>
{
    Task<ApiResponse<CustomerDetailDTO>> GetCustomerByIdAsync(Guid id);
    Task<ApiResponse<PaginatedResult<CustomerListDTO>>> GetCustomersAsync(int pageNumber, int pageSize, string? searchTerm = null);
    Task<ApiResponse<CustomerDetailDTO>> CreateCustomerAsync(CustomerCreateDTO request);
    Task<ApiResponse<CustomerDetailDTO>> UpdateCustomerAsync(Guid id, CustomerUpdateDTO request);
    Task<ApiResponse<bool>> DeleteCustomerAsync(Guid id);
    Task<ApiResponse<CustomerDetailDTO>> GetCustomerByUserIdAsync(Guid userId);
    Task<ApiResponse<bool>> LinkCustomerToUserAsync(Guid customerId, Guid userId);
    Task<ApiResponse<bool>> UnlinkCustomerFromUserAsync(Guid customerId);
}
