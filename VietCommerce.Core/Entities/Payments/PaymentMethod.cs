using VietCommerce.Core.Common;

namespace VietCommerce.Core.Entities.Payments;

public class PaymentMethod : BaseEntity, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation propertiesa
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public bool IsDeleted {get;set; }
    public DateTime? DeletedAt {get;set; }
    public Guid? DeletedBy {get;set; }
}