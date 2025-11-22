using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Enums.Payments;
namespace VietCommerce.Core.DTOs.Payments;

// 3. Filters
/// <summary>
/// Bộ lọc cho lịch sử thanh toán
/// </summary>
public class PaymentFilters
{
    public Guid? OrderId { get; set; }
    public PaymentMethodType? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public PaymentMethodType? PaymentType { get; set; }
    public string? TransactionId { get; set; }
}
