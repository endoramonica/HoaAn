namespace VietCommerce.Core.DTOs.Authorization;

public class BulkPermissionCheckResultDTO
{
    public Guid UserId { get; set; }
    public Dictionary<string, bool> PermissionResults { get; set; } = new Dictionary<string, bool>();
    public List<string> AllowedPermissions { get; set; } = new List<string>();
    public List<string> DeniedPermissions { get; set; } = new List<string>();
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
}
