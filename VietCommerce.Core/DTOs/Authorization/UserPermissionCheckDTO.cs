namespace VietCommerce.Core.DTOs.Authorization;

public class UserPermissionCheckDTO
{
    public Guid UserId { get; set; }
    public string Permission { get; set; } = string.Empty;
    public bool HasPermission { get; set; }
    public List<string> GrantingRoles { get; set; } = new List<string>();
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
}
