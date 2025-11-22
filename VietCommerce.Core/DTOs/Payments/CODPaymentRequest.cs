namespace VietCommerce.Core.DTOs.Payments;

/// <summary>
/// Request cho thanh toán COD (Cash On Delivery)
/// </summary>
public class CODPaymentRequest
{
    public Guid OrderId { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public bool IsDelivered { get; set; } // true = đã giao hàng và thu tiền
    public string? Note { get; set; }
}

