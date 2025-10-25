using VietCommerce.Core.Common;
using VietCommerce.Core.Enums.Payments;
namespace VietCommerce.Core.Entities.Payments;
public class PaymentTransaction : BaseEntity
{
    public Guid PaymentId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public PaymentMethodType Status { get; set; } 
    public string? GatewayResponse { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    // Navigation properties
    public virtual Payment Payment { get; set; } = null!;
}
