using VietCommerce.Core.DTOs.Permissions;

namespace VietCommerce.Core.DTOs.Roles;

public class RoleDetailDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<PermissionListDTO> Permissions { get; set; } = new List<PermissionListDTO>();
    public int UserCount { get; set; }
}
