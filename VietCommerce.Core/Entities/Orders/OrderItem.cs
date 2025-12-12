using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Orders;

namespace VietCommerce.Core.Entities.Orders;

public class OrderItem : AuditableEntity, ISoftDelete
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    [Required]
    public string ProductName { get; set; } = string.Empty; // Snapshot of product name
    [Required]
    public string ProductCode { get; set; } = string.Empty; // Snapshot of product code
    public decimal UnitPrice { get; set; } // Snapshot of price at order time
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; } // UnitPrice * Quantity
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
    public virtual ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();

    // ========================================
    // 🔧 SERVICES-PRODUCT UNIFICATION - NEW FIELDS
    // ========================================

    /// <summary>
    /// Type discriminator: "product" or "service"
    /// Default: "product" for backward compatibility
    /// </summary>
    [MaxLength(50)]
    public string Type { get; set; } = "product";

    /// <summary>
    /// Service category (only for type='service')
    /// </summary>
    [MaxLength(100)]
    public string? ServiceCategory { get; set; }

    /// <summary>
    /// Service duration (only for type='service')
    /// </summary>
    [MaxLength(100)]
    public string? ServiceDuration { get; set; }

    // ========================================
    // 📦 PACKAGE & CUSTOMIZABLE PRODUCTS - NEW FIELDS
    // ========================================

    /// <summary>
    /// JSON snapshot of customizations from CartItem at order time
    /// Format: [{"optionId":"opt-xoi","quantity":10,"unitPrice":45000,"totalPrice":450000}]
    /// </summary>
    public string? CustomizationsJson { get; set; }

    /// <summary>
    /// Snapshot of base price from CartItem at order time
    /// </summary>
    public decimal BasePrice { get; set; }

    /// <summary>
    /// Snapshot of customization price from CartItem at order time
    /// </summary>
    public decimal CustomizationPrice { get; set; }
}