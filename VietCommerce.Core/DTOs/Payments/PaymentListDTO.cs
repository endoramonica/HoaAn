using VietCommerce.Core.Enums;
using VietCommerce.Core.Enums.Payments;

namespace VietCommerce.Core.DTOs.Payments;

public class PaymentListDTO
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethodType Status { get; set; }
    public DateTime CreatedDate { get; set; }
}