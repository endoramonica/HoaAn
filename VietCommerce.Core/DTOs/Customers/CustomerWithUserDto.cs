using VietCommerce.Core.DTOs.Users;

namespace VietCommerce.Core.DTOs.Customers;

/// <summary>
/// DTO for combined Customer + User information
/// Used in GET /api/v1/AdminCustomer/{id} endpoint
/// </summary>
public class CustomerWithUserDto
{
    public CustomerDetailDto Customer { get; set; } = new();
    public UserDetailDTO? User { get; set; }
}
