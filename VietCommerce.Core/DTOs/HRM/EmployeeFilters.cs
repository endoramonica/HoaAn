using VietCommerce.Core.Entities.HRM;

namespace VietCommerce.Core.DTOs.HRM;

public class EmployeeFilters
{
    public Guid? UserId { get; set; }
    public string? Search { get; set; }        // Tìm trong Name, Email, Code, Phone
    public string? Department { get; set; }
    public EmployeeStatus? Status { get; set; }
    public Guid? ManagerId { get; set; }
    public Guid? StoreId { get; set; }
    public DateTime? HireDateFrom { get; set; }
    public DateTime? HireDateTo { get; set; }  // ✅ Sửa lỗi thiếu set;
    public string? Position { get; set; }
}