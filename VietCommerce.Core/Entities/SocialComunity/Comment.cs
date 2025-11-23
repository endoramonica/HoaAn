using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Customers;

public class Comment : AuditableEntity, ISoftDelete
{
    public Guid PostId { get; set; }
    public virtual Post Post { get; set; }

    // ✅ SỬA: Dùng CustomerId thay vì UserId
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; }

    public string Content { get; set; }
    public DateTime AddedOn { get; set; }

    // ISoftDelete
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // Optional: Support nested comments (reply)
    public Guid? ParentCommentId { get; set; }
    public virtual Comment? ParentComment { get; set; }
    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
