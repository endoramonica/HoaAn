// CreateWorkScheduleRequest.cs
using VietCommerce.Core.Entities.HRM;

namespace VietCommerce.Core.DTOs.HRM;

public class CreateWorkScheduleRequest
{
    public Guid EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; } 
    public TimeSpan EndTime { get; set; } 
    public WorkScheduleType Type { get; set; } = WorkScheduleType.Regular;
    public string? Notes { get; set; }
}
