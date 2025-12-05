using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Enums.Marketing;

namespace VietCommerce.Core.DTOs.Marketing;

/// <summary>
/// DTO for bulk status update operation
/// </summary>
public class BulkUpdateStatusDto
{
    [Required(ErrorMessage = "Post IDs are required")]
    [MinLength(1, ErrorMessage = "At least one post ID is required")]
    public List<Guid> PostIds { get; set; } = new();

    [Required(ErrorMessage = "Status is required")]
    public MarketingPostStatus Status { get; set; }
}
