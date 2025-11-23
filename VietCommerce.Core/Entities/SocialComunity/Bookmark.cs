using VietCommerce.Core.Entities.Customers;

public class Bookmark 
{
    public Guid PostId { get; set; }
    public virtual Post Post { get; set; }

    // ✅ SỬA: Dùng CustomerId thay vì UserId
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; }

    public DateTime BookmarkedOn { get; set; } = DateTime.UtcNow;
}