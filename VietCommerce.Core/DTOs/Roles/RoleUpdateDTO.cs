using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.DTOs.Roles;
public class RoleUpdateDTO
{
    [Required(ErrorMessage = "Role name is required")]
    [StringLength(100, ErrorMessage = "Role name cannot exceed 100 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_\-\s]+$", ErrorMessage = "Role name can only contain letters, numbers, underscores, hyphens, and spaces")]
    public string Name { get; set; } = string.Empty;
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
}
