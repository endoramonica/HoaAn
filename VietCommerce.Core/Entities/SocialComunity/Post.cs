using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Customers;

public class Post : AuditableEntity, ISoftDelete
{
    // ✅ Đúng: dùng CustomerId
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; }

    public string? Content { get; set; }
    public string? PhotoPath { get; set; }
    public string? PhotoUrl { get; set; }
    public string? PhotoHash { get; set; }
    public DateTime PostedOn { get; set; }
    public DateTime? NotificationOn { get; set; }

    // ISoftDelete
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }

    // Navigation collections
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    public virtual ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
}
