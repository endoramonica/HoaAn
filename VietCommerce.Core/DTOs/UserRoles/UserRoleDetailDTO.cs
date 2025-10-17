using VietCommerce.Core.DTOs.Permissions;
using VietCommerce.Core.DTOs.Roles;
using VietCommerce.Core.DTOs.Users;

namespace VietCommerce.Core.DTOs.UserRoles;

public class UserRoleDetailDTO
{
    public UserListDTO User { get; set; } = null!;
    public List<RoleListDTO> Roles { get; set; } = new List<RoleListDTO>();
    public List<PermissionListDTO> EffectivePermissions { get; set; } = new List<PermissionListDTO>();
    public DateTime LastUpdated { get; set; }
}
