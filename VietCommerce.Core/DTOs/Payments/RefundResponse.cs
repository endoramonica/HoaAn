namespace VietCommerce.Core.DTOs.Payments;

/// <summary>
/// Response cho hoàn tiền
/// </summary>
public class RefundResponse
{
    public Guid RefundId { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal RefundedAmount { get; set; }
    public DateTime? RefundedAt { get; set; }
}
