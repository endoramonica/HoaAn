using VietCommerce.Core.Common;
using VietCommerce.Core.Enums.Notifications;

namespace VietCommerce.Core.Entities.Notifications;

public class NotificationTemplate : BaseEntity, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // Unique identifier
    public NotificationType Type { get; set; }

    // Template content với placeholders
    // VD: "{{actorName}} đã thích bài viết của bạn"
    public string TitleTemplate { get; set; } = string.Empty;
    public string ContentTemplate { get; set; } = string.Empty;

    // Link template
    // VD: "/posts/{{postId}}"
    public string? LinkTemplate { get; set; }

    // ===== CHANNELS =====
    public bool EnableInApp { get; set; } = true;
    public bool EnableEmail { get; set; } = false;
    public bool EnableSms { get; set; } = false;
    public bool EnablePush { get; set; } = false;

    // Email template (nếu enable email)
    public string? EmailSubjectTemplate { get; set; }
    public string? EmailBodyTemplate { get; set; }

    // SMS template (nếu enable SMS)
    public string? SmsTemplate { get; set; }

    // ===== SETTINGS =====
    public int Priority { get; set; } = 0; // 0=low, 1=medium, 2=high, 3=urgent
    public bool RequiresAction { get; set; } = false;

    // ISoftDelete
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // Navigation
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}