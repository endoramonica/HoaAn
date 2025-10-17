using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Permissions;

public class AssignPermissionsToRoleDTO
{
    [Required(ErrorMessage = "Role ID is required")]
    public Guid RoleId { get; set; }

    [Required(ErrorMessage = "Permission IDs are required")]
    [MinLength(1, ErrorMessage = "At least one permission must be specified")]
    public List<Guid> PermissionIds { get; set; } = new List<Guid>();
}
