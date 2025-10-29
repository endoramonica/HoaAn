using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Payments;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Orders;

namespace VietCommerce.Core.Entities.Orders;

public class Order : AuditableEntity, ISoftDelete
{
    public Guid StoreId { get; set; }
    [Required]
    public Guid CustomerId { get; set; } // Changed from nullable to required as per rule
    [Required]
    public string OrderNumber { get; set; } = string.Empty; /// Mã đơn hàng duy nhất, sinh tại CheckoutService
    public OrderStatus Status { get; set; }
    public decimal SubTotal { get; set; } // Total product amount
    public decimal ShippingFee { get; set; } // Shipping cost
    public decimal TaxAmount { get; set; } // Tax amount
    public decimal DiscountAmount { get; set; } // Discount amount
    public decimal TotalAmount { get; set; } // Grand total
    public string Notes { get; set; } = string.Empty; // Order notes
    public DateTime? CompletedAt { get; set; } // Completion date
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // Navigation properties
    public virtual Store Store { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
    [ForeignKey(nameof(CreatedById))]
    public virtual User? CreatedByUser { get; set; }
    public Guid? CreatedById { get; set; }
    [NotMapped]
    public new Guid CreatedBy { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
    public virtual OrderShipping? OrderShipping { get; set; }
}