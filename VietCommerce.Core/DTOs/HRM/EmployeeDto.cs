using VietCommerce.Core.Entities.HRM;

namespace VietCommerce.Core.DTOs.HRM;

public class EmployeeDto
{
    // ✅ UserId là PK của Employee
    public Guid UserId { get; set; }

    // ✅ Thông tin từ User (mapped từ User entity)
    public string Name { get; set; } = string.Empty;          // User.Name
    public string Email { get; set; } = string.Empty;         // User.Email
    public string Phone { get; set; } = string.Empty;         // User.Phone
    public string? Avatar { get; set; }                       // User.AvatarUrl

    // ✅ Thông tin HR từ Employee
    public string Code { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public EmployeeStatus Status { get; set; }
    public string Skills { get; set; } = string.Empty;

    // ✅ Manager info
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }

    // ✅ Store info
    public Guid? StoreId { get; set; }
    public string? StoreName { get; set; }

    // ✅ Audit
    public DateTime CreatedAt { get; set; }
}