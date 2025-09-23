
using VietCommerce.Core.Common;

namespace VietCommerce.Core.Entities.Products;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; set; }
    public string Url { get; set; } = string.Empty;

    // Navigation properties
    public virtual Product Product { get; set; } = null!;
}