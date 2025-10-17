using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Permissions;

public class SyncRolePermissionsDTO
{
    [Required(ErrorMessage = "Role ID is required")]
    public Guid RoleId { get; set; }

    [Required(ErrorMessage = "Permission IDs are required")]
    public List<Guid> PermissionIds { get; set; } = new List<Guid>();
}
