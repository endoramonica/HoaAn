using VietCommerce.Core.Enums;
using VietCommerce.Core.Enums.Payments;
namespace VietCommerce.Core.DTOs.Payments;
public class PaymentUpdateDTO
{
    public decimal? Amount { get; set; }
    public PaymentMethodType? Status { get; set; }
    public string? Description { get; set; }
}
