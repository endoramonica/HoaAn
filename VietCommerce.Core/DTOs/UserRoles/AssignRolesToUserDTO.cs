using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.UserRoles;

public class AssignRolesToUserDTO
{
    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "Role IDs are required")]
    [MinLength(1, ErrorMessage = "At least one role must be specified")]
    public List<Guid> RoleIds { get; set; } = new List<Guid>();
}
