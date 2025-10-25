namespace VietCommerce.Core.DTOs.Permissions;
public class PermissionListDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int RoleCount { get; set; }
}
