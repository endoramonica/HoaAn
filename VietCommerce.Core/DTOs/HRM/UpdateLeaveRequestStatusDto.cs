// UpdateLeaveRequestStatusDto.cs
using VietCommerce.Core.Entities.HRM;

namespace VietCommerce.Core.DTOs.HRM;

public class UpdateLeaveRequestStatusDto
{
    public LeaveRequestStatus Status { get; set; }
    public string? Comments { get; set; }
}