using VietCommerce.Core.DTOs.Orders;

namespace VietCommerce.Core.DTOs.Payments;

/// <summary>
/// Request tính toán tổng đơn hàng
/// </summary>
public class CalculateOrderRequest
{
    public Guid? CustomerId { get; set; }
    public List<OrderItemRequest> Items { get; set; } = new();
    public string ShippingAddress { get; set; } = string.Empty;
    public string? VoucherCode { get; set; }
}
