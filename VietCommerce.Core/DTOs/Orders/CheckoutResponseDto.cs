using VietCommerce.Core.Enums.Orders;

namespace VietCommerce.Core.DTOs.Orders;

public class CheckoutResponseDto
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public string StatusText => Status.ToString(); // Human-readable status
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid StoreId { get; set; } // Added for store context
    public string? StoreName { get; set; } // Added for display
    public Guid CustomerId { get; set; } // Added for customer reference
    public string? CustomerName { get; set; } // Added for display
    public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();
    public OrderShippingDto Shipping { get; set; } = null!;
    // Thêm
    public string? PaymentUrl { get; set; }
    public string? PaymentMethodUsed { get; set; }
}
