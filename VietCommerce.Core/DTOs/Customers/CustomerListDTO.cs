namespace VietCommerce.Core.DTOs.Customers;
public class CustomerListDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public Guid? UserId { get; set; }
    public DateTime CreatedDate { get; set; }
}
