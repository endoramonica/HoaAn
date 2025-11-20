// UpdateWorkScheduleRequest.cs
using VietCommerce.Core.Entities.HRM;

namespace VietCommerce.Core.DTOs.HRM;

public class UpdateWorkScheduleRequest
{
    public DateTime Date { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public WorkScheduleType Type { get; set; }
    public WorkScheduleStatus Status { get; set; }
    public string? Notes { get; set; }
}