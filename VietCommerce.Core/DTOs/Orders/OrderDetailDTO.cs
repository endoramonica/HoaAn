using System;
using System.Collections.Generic;
using VietCommerce.Core.Enums.Orders;

namespace VietCommerce.Core.DTOs.Orders;

public class OrderDetailDto
{
    // Basic Info
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid StoreId { get; set; }
    public string? StoreName { get; set; }
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; } // Added for contact
    public string? CustomerPhone { get; set; } // Added for contact
    public OrderStatus Status { get; set; }
    public string StatusText => Status.ToString(); // Human-readable
    public decimal SubTotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Notes { get; set; } = string.Empty; // Added from entity
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } // Added from AuditableEntity
    public DateTime? CompletedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public string? CreatedByName { get; set; } // Added for audit

    // Shipping Info
    public OrderShippingDto Shipping { get; set; } = null!;

    // Order Items
    public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    public int ItemsCount => Items?.Count ?? 0; // Calculated property

    // Status History
    public List<OrderStatusHistoryDTO> StatusHistories { get; set; } = new List<OrderStatusHistoryDTO>();

    // Payment Summary
    public bool IsPaid { get; set; } // Added for payment status
    public decimal? PaidAmount { get; set; } // Added for payment tracking
    public string? PaymentMethod { get; set; } // Added for payment details
    public DateTime? PaidAt { get; set; } // Added for payment timestamp
    public string? TransactionId { get; set; } // Added for payment reference

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

    /// <summary>
    /// Service location/address (only for type='service')
    /// </summary>
    public string? ServiceLocation { get; set; }

    /// <summary>
    /// Scheduled service date (only for type='service')
    /// </summary>
    public DateTime? ServiceDate { get; set; }

    /// <summary>
    /// Scheduled service time (only for type='service')
    /// </summary>
    /// public string? ServiceTime { get; set; 
    /// <summary>
    /// Additional notes for service (only for type='service')
    /// </summary>
    public string? ServiceNotes { get; set; }
}