using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Marketing;

/// <summary>
/// DTO for scheduling a marketing post
/// </summary>
public class SchedulePostDto
{
    [Required(ErrorMessage = "Scheduled date is required")]
    public DateTime ScheduledDate { get; set; }
}
