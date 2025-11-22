using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.DTOs.Payments;

/// <summary>
/// Request cho hoàn tiền
/// </summary>
public class RefundRequest
{
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Note { get; set; }
}