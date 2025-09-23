using VietCommerce.Core.Common;

namespace VietCommerce.Core.Entities.Payments;

public class PaymentMethod : BaseEntity, ISoftDelete
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation propertiesa
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public bool IsDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? DeletedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int? DeletedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}