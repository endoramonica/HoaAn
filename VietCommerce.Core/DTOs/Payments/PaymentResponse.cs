using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Enums.Payments;

namespace VietCommerce.Core.DTOs.Payments;


/// <summary>
/// Response chung cho các thao tác thanh toán
/// </summary>
public class PaymentResponse
{
    public Guid PaymentId { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public PaymentMethodType? Status { get; set; }
}

