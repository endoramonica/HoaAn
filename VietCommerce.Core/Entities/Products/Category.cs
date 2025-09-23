using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Organization;

namespace VietCommerce.Core.Entities.Products;

public class Category : BaseEntity, ISoftDelete
{
    public Guid StoreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Store Store { get; set; } = null!;
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public bool IsDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? DeletedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int? DeletedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}