using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.DTOs.Orders;

namespace VietCommerce.Core.DTOs.Orders;

public class OrderCreateDTO
{
    [Required(ErrorMessage = "Customer ID is required")]
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "Store ID is required")]
    public Guid StoreId { get; set; }

    [Required(ErrorMessage = "At least one item is required")]
    [MinLength(1, ErrorMessage = "Order must have at least one item")]
    public List<OrderItemCreateDTO> Items { get; set; } = new();

    public OrderShippingDto? Shipping { get; set; }
}
