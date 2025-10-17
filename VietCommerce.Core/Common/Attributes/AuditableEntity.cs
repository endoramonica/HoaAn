using VietCommerce.Core.Common;


public abstract class AuditableEntity : BaseEntity
{
    public Guid CreatedBy { get; set; } = Guid.NewGuid();
    public Guid? UpdatedBy { get; set; }
}