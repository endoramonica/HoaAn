// UpdateWorkScheduleRequest.cs
using VietCommerce.Core.Entities.HRM;

namespace VietCommerce.Core.DTOs.HRM;

public class UpdateWorkScheduleRequest
{
    public DateTime Date { get; set; }
    public TimeSpan? StartTime { get; set; } 
    public TimeSpan? EndTime { get; set; } 
    public string? ShiftName { get; set; }
    public WorkScheduleType Type { get; set; }
    public WorkScheduleStatus Status { get; set; }
    public string? Notes { get; set; }
}