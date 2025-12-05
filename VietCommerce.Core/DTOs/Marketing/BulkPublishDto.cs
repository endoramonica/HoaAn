using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Marketing;

/// <summary>
/// DTO for bulk publish operation
/// </summary>
public class BulkPublishDto
{
    [Required(ErrorMessage = "Post IDs are required")]
    [MinLength(1, ErrorMessage = "At least one post ID is required")]
    public List<Guid> PostIds { get; set; } = new();
}
