using VietCommerce.Core.Common;

namespace VietCommerce.Core.Entities.Notifications;

public class NotificationTemplate : BaseEntity, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public bool IsDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? DeletedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int? DeletedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}