using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.DTOs.Users;
public class UserUpdateDTO
{
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string? FullName { get; set; }
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string? Email { get; set; }
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? CustomerId { get; set; }
    public Guid? TenantId { get; set; }
}
