using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums;
using VietCommerce.Core.Enums.Payments;

namespace VietCommerce.Core.Entities.Payments;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid? MethodId { get; set; }
    public decimal Amount { get; set; }
    public DateTime? PaidAt { get; set; }
    public PaymentMethodType? Status { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; } = null!;
    public virtual PaymentMethod? PaymentMethod { get; set; }
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
