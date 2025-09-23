using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Core.Entities.Audit;

public class AuditLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? TargetType { get; set; }
    public Guid? TargetId { get; set; }
    public string? Meta { get; set; }
    public DateTime Timestamp { get; set; }   // <-- thêm vào

    // Navigation properties
    public virtual User User { get; set; } = null!;
}