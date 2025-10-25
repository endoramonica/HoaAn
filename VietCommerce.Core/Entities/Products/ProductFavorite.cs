using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Users;
namespace VietCommerce.Core.Entities.Products;
/// Sản phẩm yêu thích của user
/// Composite Key: UserId + ProductId
public class ProductFavorite : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
