namespace VietCommerce.Core.DTOs.Orders;

/// <summary>
/// Item trong đơn hàng để tính toán
/// </summary>
public class OrderItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
