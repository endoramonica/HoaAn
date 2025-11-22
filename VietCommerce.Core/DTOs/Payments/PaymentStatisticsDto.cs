using VietCommerce.Core.Enums.Payments;

namespace VietCommerce.Core.DTOs.Payments;

/// <summary>
/// DTO thống kê thanh toán
/// </summary>
public class PaymentStatisticsDto
{
    public int TotalPayments { get; set; }
    public decimal TotalAmount { get; set; }

    public int CompletedPayments { get; set; }
    public decimal CompletedAmount { get; set; }

    public int PendingPayments { get; set; }
    public decimal PendingAmount { get; set; }

    public int RefundedPayments { get; set; }
    public decimal RefundedAmount { get; set; }

    public Dictionary<PaymentMethodType, int> PaymentsByMethod { get; set; } = new();
    public Dictionary<PaymentMethodType, decimal> AmountByMethod { get; set; } = new();
}

