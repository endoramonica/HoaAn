namespace VietCommerce.Core.DTOs.Payments;

/// <summary>
/// DTO lịch sử thay đổi payment
/// </summary>
public class PaymentHistoryDto
{
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
}

