using VietCommerce.Core.Entities.Customers;

public class Like 
{
    public Guid PostId { get; set; }
    public virtual Post Post { get; set; }

    // ✅ SỬA: Dùng CustomerId thay vì UserId
    public Guid CustomerId { get; set; }
    public virtual Customer Customer { get; set; }

    public DateTime LikedOn { get; set; } = DateTime.UtcNow;
}
