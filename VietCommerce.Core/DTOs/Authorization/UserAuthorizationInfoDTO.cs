using VietCommerce.Core.DTOs.Permissions;
using VietCommerce.Core.DTOs.Roles;
namespace VietCommerce.Core.DTOs.Authorization;
public class UserAuthorizationInfoDTO
{
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<RoleListDTO> Roles { get; set; } = new List<RoleListDTO>();
    public List<PermissionListDTO> Permissions { get; set; } = new List<PermissionListDTO>();
    public DateTime LastUpdated { get; set; }
}
