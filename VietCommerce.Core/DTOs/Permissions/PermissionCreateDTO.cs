using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Permissions;

public class PermissionCreateDTO
{
    [Required(ErrorMessage = "Permission name is required")]
    [StringLength(100, ErrorMessage = "Permission name cannot exceed 100 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_\-\.]+$", ErrorMessage = "Permission name can only contain letters, numbers, underscores, hyphens, and dots")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}
