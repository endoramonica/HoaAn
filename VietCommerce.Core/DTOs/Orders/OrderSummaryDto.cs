namespace VietCommerce.Core.DTOs.Orders;

/// <summary>
/// DTO tóm tắt đơn hàng trong payment
/// </summary>
public class OrderSummaryDto
{
    public Guid Id { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public DateTime CreatedAt { get; set; }
}

