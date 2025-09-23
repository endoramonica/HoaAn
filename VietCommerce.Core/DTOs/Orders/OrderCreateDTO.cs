using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.DTOs.Orders;

namespace VietCommerce.Core.DTOs.Orders;

public class OrderCreateDTO
{
    [Required(ErrorMessage = "Customer ID is required")]
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "Store ID is required")]
    public Guid StoreId { get; set; }

    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public List<OrderItemCreateDTO> Items { get; set; } = new();

    public OrderShippingCreateDTO? Shipping { get; set; }
}