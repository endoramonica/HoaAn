using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    public class ProductFavoriteRepository : GenericRepository<ProductFavorite>, IProductFavoriteRepository
    {
        public ProductFavoriteRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductFavorite>> GetByUserIdWithProductAsync(Guid userId)
        {
            return await _dbSet
                .Where(pf => pf.UserId == userId)
                .Include(pf => pf.Product)
                    .ThenInclude(p => p.Images)
                .Include(pf => pf.Product)
                    .ThenInclude(p => p.Prices)
                .OrderByDescending(pf => pf.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsByUserAndProductAsync(Guid userId, Guid productId)
        {
            return await _dbSet
                .AnyAsync(pf => pf.UserId == userId && pf.ProductId == productId);
        }

        public async Task<ProductFavorite?> GetByUserAndProductAsync(Guid userId, Guid productId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(pf => pf.UserId == userId && pf.ProductId == productId);
        }

        public async Task<bool> RemoveByUserAndProductAsync(Guid userId, Guid productId)
        {
            var item = await GetByUserAndProductAsync(userId, productId);
            if (item == null)
                return false;

            Delete(item);
            return true;
        }

        public async Task<int> ClearByUserIdAsync(Guid userId)
        {
            var items = await _dbSet
                .Where(pf => pf.UserId == userId)
                .ToListAsync();

            if (!items.Any())
                return 0;

            DeleteRange(items);
            return items.Count;
        }
        /// <summary>
        /// ✅ THÊM: Đếm số lượng wishlist items của user
        /// Dùng để hiển thị badge count trong Header
        /// </summary>
        public async Task<int> CountByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(pf => pf.UserId == userId)
                .CountAsync();
        }
    }
}
