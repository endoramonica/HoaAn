using VietCommerce.Core.Common;
namespace VietCommerce.Core.Entities.Users;
public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid? TenantId { get; set; }  // Added TenantId for multi-tenancy support
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}
