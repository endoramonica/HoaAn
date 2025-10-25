using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.DTOs.UserRoles;
public class AssignRoleDTO
{
    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId { get; set; }
    [Required(ErrorMessage = "Role ID is required")]
    public Guid RoleId { get; set; }
}
