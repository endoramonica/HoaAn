
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Organization;

namespace VietCommerce.Core.Entities.Products;

public class Product : BaseEntity, ISoftDelete
{
    public Guid StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public int Stock { get; set; } = 0;
    // 🟡 Các trường từ ISoftDelete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
    public string SKU { get; set; } = string.Empty;       // thêm SKU
    
    public bool IsActive { get; set; } = true;

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
}