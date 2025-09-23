namespace VietCommerce.Core.DTOs.Users;

public class UserListDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? TenantId { get; set; }
}