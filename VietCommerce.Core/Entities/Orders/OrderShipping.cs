using VietCommerce.Core.Common;
namespace VietCommerce.Core.Entities.Orders;
public class OrderShipping : BaseEntity
{
    public Guid OrderId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Ward { get; set; }
    public string? Phone { get; set; }
    public string? ShippingMethod { get; set; }
    public decimal ShippingCost { get; set; }
    // Navigation properties
    public virtual Order Order { get; set; } = null!;
}
