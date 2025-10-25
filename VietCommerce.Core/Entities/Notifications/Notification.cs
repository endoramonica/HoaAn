using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Notifications;
namespace VietCommerce.Core.Entities.Notifications;
public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? TemplateId { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public NotificationType? Type { get; set; }
    public bool Read { get; set; } = false;
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual NotificationTemplate? Template { get; set; }
}
