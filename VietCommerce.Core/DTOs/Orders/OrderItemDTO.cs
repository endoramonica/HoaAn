using System;

namespace VietCommerce.Core.DTOs.Orders;

public class OrderItemDTO
{
    public Guid Id { get; set; }  // ✅ Added OrderItem ID
    public Guid OrderId { get; set; }  // ✅ Added for reference
    public Guid ProductId { get; set; }

    // Product snapshot (at order time)
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSKU { get; set; }  // ✅ Added for reference
    public string? ProductImageUrl { get; set; }

    // Pricing
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }// UnitPrice * Quantity

    // Calculated properties
    public decimal Subtotal => Quantity * UnitPrice;  // ✅ For display

    // ========================================
    // 🔧 SERVICES-PRODUCT UNIFICATION - NEW FIELDS
    // ========================================

    /// <summary>
    /// Type discriminator: "product" or "service"
    /// </summary>
    public string Type { get; set; } = "product";

    /// <summary>
    /// Service category (only for type='service')
    /// </summary>
    public string? ServiceCategory { get; set; }

    /// <summary>
    /// Service duration (only for type='service')
    /// </summary>
    public string? ServiceDuration { get; set; }
}
