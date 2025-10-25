using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Users;
namespace VietCommerce.Core.Entities.Products;
/// Đánh giá sản phẩm
/// Business Rule: Chỉ review được khi Order.Status == Completed
/// Constraint: 1 review per user per product per order
[Table("ProductReviews")]
public class ProductReview : BaseEntity, ISoftDelete
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    /// Link đến OrderItem để đảm bảo user đã mua sản phẩm
    public Guid OrderItemId { get; set; }
    /// Đánh giá từ 1-5 sao
    /// Business: Hiển thị ưu tiên rating >= 3, nhưng KHÔNG loại bỏ < 3
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
    /// Nội dung đánh giá
    [MaxLength(2000)]
    public string? Comment { get; set; }
    /// Media URLs (ảnh/video) - JSON array
    /// Format: ["url1", "url2", "url3"]
    public string? MediaUrls { get; set; }
    /// Admin có thể ẩn review không phù hợp
    public bool IsVisible { get; set; } = true;
    /// Số lượng helpful votes
    public int HelpfulCount { get; set; } = 0;
    /// Admin reply (nếu có)
    [MaxLength(1000)]
    public string? AdminReply { get; set; }
    public DateTime? AdminRepliedAt { get; set; }
    // ISoftDelete implementation
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual OrderItem OrderItem { get; set; } = null!;
}
