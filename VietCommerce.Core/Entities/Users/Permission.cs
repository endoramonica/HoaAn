using VietCommerce.Core.Common;

namespace VietCommerce.Core.Entities.Users;

public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}