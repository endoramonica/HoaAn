using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Enums.Payments;

namespace VietCommerce.Core.DTOs.Payments;

/// <summary>
/// DTO hiển thị thông tin thanh toán
/// </summary>
public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string OrderCode { get; set; } = string.Empty;

    public Guid? MethodId { get; set; }
    public string MethodName { get; set; } = string.Empty;
    public PaymentMethodType PaymentType { get; set; }

    public decimal Amount { get; set; }
    public PaymentMethodType Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;

    public string TransactionId { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }

    // Refund info
    public decimal? RefundedAmount { get; set; }
    public string? RefundReason { get; set; }
    public DateTime? RefundedAt { get; set; }

    public string? Notes { get; set; }

    public Guid CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
