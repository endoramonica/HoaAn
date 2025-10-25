namespace VietCommerce.Core.DTOs.Roles;
public class RoleListDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UserCount { get; set; }
    public int PermissionCount { get; set; }
}
