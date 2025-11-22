using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.DTOs.Payments;

/// <summary>
/// Request cho thanh toán thẻ
/// </summary>
public class CardPaymentRequest
{
    public Guid OrderId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty; // MM/YY
    public string CVV { get; set; } = string.Empty;
}