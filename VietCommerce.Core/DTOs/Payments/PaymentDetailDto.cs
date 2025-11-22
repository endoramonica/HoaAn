using VietCommerce.Core.DTOs.Orders;

namespace VietCommerce.Core.DTOs.Payments;

/// <summary>
/// DTO chi tiết thanh toán (bao gồm order info)
/// </summary>
public class PaymentDetailDto : PaymentDto
{
    public OrderSummaryDto Order { get; set; } = new();
    public List<PaymentHistoryDto> History { get; set; } = new();
}

