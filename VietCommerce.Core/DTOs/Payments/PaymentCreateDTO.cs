using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.DTOs.Payments;
public class PaymentCreateDTO
{
    [Required(ErrorMessage = "Order ID is required")]
    public Guid OrderId { get; set; }
    [Required(ErrorMessage = "Payment method ID is required")]
    public Guid PaymentMethodId { get; set; }
    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}
