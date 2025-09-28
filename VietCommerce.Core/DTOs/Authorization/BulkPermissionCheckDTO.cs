namespace VietCommerce.Core.DTOs.Authorization;

public class BulkPermissionCheckDTO
{
    public Guid UserId { get; set; }
    public List<string> Permissions { get; set; } = new List<string>();
}
