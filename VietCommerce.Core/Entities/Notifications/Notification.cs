using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Notifications;
using System.ComponentModel.DataAnnotations.Schema;

namespace VietCommerce.Core.Entities.Notifications;

public class Notification : BaseEntity, ISoftDelete
{
    // ===== RECIPIENT (Người nhận) =====
    // Dùng CustomerId cho social + buyer notifications
    public Guid? CustomerId { get; set; }
    public virtual Customer? Customer { get; set; }

    // Dùng UserId cho admin/employee notifications
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }

    // ===== CONTENT =====
    public Guid? TemplateId { get; set; }
    public virtual NotificationTemplate? Template { get; set; }

    public string? Title { get; set; }
    public string? Message { get; set; }
    public NotificationType Type { get; set; }

    // ===== METADATA =====
    // Actor: Người tạo ra notification (ví dụ: ai đó like post của bạn)
    public Guid? ActorCustomerId { get; set; }
    public virtual Customer? ActorCustomer { get; set; }

    // Related entities (polymorphic references)
    public Guid? PostId { get; set; }
    public virtual Post? Post { get; set; }

    public Guid? OrderId { get; set; }
    public virtual Order? Order { get; set; }

    public Guid? CommentId { get; set; }
    // Note: Comment navigation nếu cần

    // Link to redirect
    public string? Link { get; set; }

    // Additional data (JSON)
    [Column(TypeName = "nvarchar(max)")]
    public string? Data { get; set; }

    // ===== STATUS =====
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public DateTime NotifiedOn { get; set; } = DateTime.UtcNow;

    // ISoftDelete
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // ===== DELIVERY CHANNELS =====
    public bool SentViaInApp { get; set; } = true;
    public bool SentViaEmail { get; set; } = false;
    public bool SentViaSms { get; set; } = false;
    public bool SentViaPush { get; set; } = false;
}