using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Enums;
using VietCommerce.Core.Enums.Notifications;
namespace VietCommerce.Core.DTOs.Notifications;
public class NotificationCreateDTO
{
    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId { get; set; }
    [Required(ErrorMessage = "Type is required")]
    public NotificationType Type { get; set; }
    [StringLength(1000, ErrorMessage = "Title cannot exceed 1000 characters")]
    public string Title { get; set; } = string.Empty;
    [StringLength(5000, ErrorMessage = "Message cannot exceed 5000 characters")]
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public Guid? PostId { get; set; }
    public Guid? CommentId { get; set; }
    public Guid? ActorCustomerId { get; set; }
}
