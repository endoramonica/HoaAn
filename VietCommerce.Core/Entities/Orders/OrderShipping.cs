using System;
using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Common;
using VietCommerce.Core.Enums.Orders;

namespace VietCommerce.Core.Entities.Orders;

public class OrderShipping : AuditableEntity, ISoftDelete
{
    public Guid OrderId { get; set; }
    [Required]
    public string RecipientName { get; set; } = string.Empty;
    [Required]
    [RegularExpression(@"^[0-9]{10,11}$")]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    public string Address { get; set; } = string.Empty;
    [Required]
    public string Ward { get; set; } = string.Empty;
    [Required]
    public string District { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [MaxLength(20)]
    public string PostalCode { get; set; } = string.Empty;
    [Required]
    public string ShippingMethod { get; set; } = string.Empty;
    public decimal ShippingCost { get; set; }
    public ShippingStatus Status { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; } = null!;
}