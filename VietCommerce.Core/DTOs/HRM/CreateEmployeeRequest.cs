// CreateEmployeeRequest.cs
namespace VietCommerce.Core.DTOs.HRM;

public class CreateEmployeeRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty; // Mã nhân viên
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public Guid? ManagerId { get; set; }
    public Guid? StoreId { get; set; }
    public string Skills { get; set; } = string.Empty;
}
