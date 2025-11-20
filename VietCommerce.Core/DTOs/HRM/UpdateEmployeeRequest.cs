// UpdateEmployeeRequest.cs
using VietCommerce.Core.Entities.HRM;

namespace VietCommerce.Core.DTOs.HRM;

public class UpdateEmployeeRequest
{
    // ✅ Chỉ cho phép update thông tin HR, không update User info
    //Lưu ý: Nếu muốn update Name, Email, Phone, Avatar → Phải update qua User API.
    public string? Code { get; set; }
    public string? Position { get; set; }
    public string? Department { get; set; }
    public DateTime? HireDate { get; set; }
    public decimal? Salary { get; set; }
    public EmployeeStatus? Status { get; set; }
    public Guid? ManagerId { get; set; }
    public Guid? StoreId { get; set; }
    public string? Skills { get; set; }
}