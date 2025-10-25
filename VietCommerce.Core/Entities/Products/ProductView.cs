using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Users;
namespace VietCommerce.Core.Entities.Products;
/// Log lượt xem sản phẩm
/// Dùng cho analytics và tracking user behavior
public class ProductView : BaseEntity
{
    public Guid ProductId { get; set; }
    /// UserId nếu user đã login, null nếu anonymous
    public Guid? UserId { get; set; }
    /// SessionId để track anonymous users
    public string SessionId { get; set; } = string.Empty;
    /// Thời gian xem (dùng CreatedAt từ BaseEntity)
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    /// IP Address (optional - cho analytics)
    public string? IpAddress { get; set; }
    /// User Agent (optional - cho analytics)
    public string? UserAgent { get; set; }
    // Navigation properties
    public virtual Product Product { get; set; } = null!;
    public virtual User? User { get; set; }
}
