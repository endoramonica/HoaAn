namespace VietCommerce.Core.DTOs.Payments;

/// <summary>
/// Request cho thanh toán tiền mặt
/// </summary>
public class CashPaymentRequest
{
    public Guid OrderId { get; set; }
    public decimal AmountDue { get; set; }
    public decimal AmountReceived { get; set; }
    public decimal Change => AmountReceived - AmountDue;
}
