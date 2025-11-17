using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface IProductFavoriteRepository : IGenericRepository<ProductFavorite>
    {
        /// <summary>
        /// Lấy tất cả wishlist items của user (include Product)
        /// </summary>
        Task<IEnumerable<ProductFavorite>> GetByUserIdWithProductAsync(Guid userId);

        /// <summary>
        /// Kiểm tra sản phẩm có trong wishlist không
        /// </summary>
        Task<bool> ExistsByUserAndProductAsync(Guid userId, Guid productId);

        /// <summary>
        /// Lấy wishlist item theo userId và productId
        /// </summary>
        Task<ProductFavorite?> GetByUserAndProductAsync(Guid userId, Guid productId);

        /// <summary>
        /// Xóa wishlist item theo userId và productId
        /// </summary>
        Task<bool> RemoveByUserAndProductAsync(Guid userId, Guid productId);

        /// <summary>
        /// Xóa toàn bộ wishlist của user
        /// </summary>
        Task<int> ClearByUserIdAsync(Guid userId);

        Task<int> CountByUserIdAsync(Guid userId);
    }
}
