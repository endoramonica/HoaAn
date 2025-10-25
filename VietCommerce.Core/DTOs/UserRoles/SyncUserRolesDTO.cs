using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.DTOs.UserRoles;
public class SyncUserRolesDTO
{
    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId { get; set; }
    [Required(ErrorMessage = "Role IDs are required")]
    public List<Guid> RoleIds { get; set; } = new List<Guid>();
}
