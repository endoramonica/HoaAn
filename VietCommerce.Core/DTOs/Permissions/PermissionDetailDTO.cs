using VietCommerce.Core.DTOs.Roles;
namespace VietCommerce.Core.DTOs.Permissions;
public class PermissionDetailDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<RoleListDTO> Roles { get; set; } = new List<RoleListDTO>();
}
