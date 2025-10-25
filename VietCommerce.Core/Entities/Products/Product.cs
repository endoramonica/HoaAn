using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Logistics;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Users;
namespace VietCommerce.Core.Entities.Products;
public class Product : AuditableEntity , ISoftDelete 
{
    public Guid StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    // ?? Thêm Code và Slug
    // code duy nh?t trên toàn h? th?ng
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty; 
    [MaxLength(150)]
    public string Slug { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public int Stock { get; set; } = 0;
    // ?? Các tru?ng t? ISoftDelete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public string SKU { get; set; } = string.Empty; // SKU  - stock keeping unit - mã hàng t?n kho
    public bool IsActive { get; set; } = true;
// ========================================
    // ?? PRODUCT STATS (Embedded) - M?I THÊM
    // ========================================
    /// T?ng s? lu?t xem
    /// Update: Batch job t? ProductView events
    public long ViewCount { get; set; } = 0;
    /// T?ng s? lu?t yêu thích
    /// Update: Realtime + eventual consistency
    public int FavoriteCount { get; set; } = 0;
    /// T?ng s? lu?ng dã bán
    /// Update: Khi Order.Status == Completed
    public int PurchaseCount { get; set; } = 0;
    /// T?ng s? review
    public int ReviewCount { get; set; } = 0;
    /// Ði?m dánh giá trung bình (1-5)
    [Column(TypeName = "decimal(3,2)")]
    public decimal AvgRating { get; set; } = 0;
    /// Trending Score - tính d?a trên views, purchases, favorites trong 7 ngày g?n nh?t
    /// Formula: (ViewCount * 0.1) + (FavoriteCount * 0.3) + (PurchaseCount * 0.6)
    /// Update: Background job daily/hourly
    [Column(TypeName = "decimal(18,2)")]
    public decimal TrendingScore { get; set; } = 0;
    /// L?n cu?i c?p nh?t stats
    public DateTime? StatsUpdatedAt { get; set; }
    // Navigation properties
    public virtual Store Store { get; set; } = null!;
    public virtual Category? Category { get; set; }
    public virtual ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();
    public virtual ICollection<ProductPrice> Prices { get; set; } = new List<ProductPrice>();
    public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
    public virtual ICollection<ProductFavorite> Favorites { get; set; } = new List<ProductFavorite>();
    public virtual ICollection<ProductView> Views { get; set; } = new List<ProductView>();
    public virtual ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
    [InverseProperty(nameof(TransferItem.Product))]
    public ICollection<TransferItem> TransferItems { get; set; } = new List<TransferItem>();
    // Many-to-many v?i Supplier
    public ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
    [ForeignKey(nameof(CreatedBy))]
    public virtual User? CreatedByUser { get; set; }

    [ForeignKey(nameof(UpdatedBy))]
    public virtual User? UpdatedByUser { get; set; }

}
