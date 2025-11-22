using VietCommerce.Core.Enums.Payments;

namespace VietCommerce.Core.DTOs.Orders;

/// <summary>
/// Base request cho tất cả các loại thanh toán
/// </summary>
public class PaymentRequest
{
    public Guid OrderId { get; set; }
    public PaymentMethodType PaymentType { get; set; }
    public decimal Amount { get; set; }

    // Optional fields cho các loại thanh toán khác nhau
    public string? CardNumber { get; set; }
    public string? CVV { get; set; }
    public string? ShippingAddress { get; set; }
}
