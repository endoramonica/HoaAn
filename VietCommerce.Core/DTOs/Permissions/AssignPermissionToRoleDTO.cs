using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.DTOs.Permissions;
public class AssignPermissionToRoleDTO
{
    [Required(ErrorMessage = "Role ID is required")]
    public Guid RoleId { get; set; }
    [Required(ErrorMessage = "Permission ID is required")]
    public Guid PermissionId { get; set; }
}
