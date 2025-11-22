namespace VietCommerce.Core.DTOs.Payments;

/// <summary>
/// DTO thống kê theo thời gian
/// </summary>
public class PaymentRevenueDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int TransactionCount { get; set; }
    public decimal AverageOrderValue => TransactionCount > 0
        ? Revenue / TransactionCount
        : 0;
}

