using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Users;
namespace VietCommerce.Core.Entities.Orders;

public class CartItem : AuditableEntity, ISoftDelete
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }

    /// <summary>
    /// JSON array of customizations for this cart item.
    /// Format: [{"optionId":"opt-xoi","quantity":10,"unitPrice":45000,"totalPrice":450000}]
    /// </summary>
    public string? CustomizationsJson { get; set; }

    /// <summary>
    /// Base price of the product before customizations.
    /// </summary>
    public decimal BasePrice { get; set; } = 0;

    /// <summary>
    /// Total price of all customizations applied to this cart item.
    /// </summary>
    public decimal CustomizationPrice { get; set; } = 0;

    /// <summary>
    /// Final price: BasePrice + CustomizationPrice
    /// </summary>
    public decimal FinalPrice { get; set; } = 0;

    // Navigation properties
    public virtual Cart Cart { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    // Soft delete properties (implements ISoftDelete)
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
