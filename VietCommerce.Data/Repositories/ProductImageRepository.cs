using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Products;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace VietCommerce.Data.Repositories
{
    public class ProductImageRepository : GenericRepository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy danh sách ảnh theo ProductId
        /// </summary>
        public async Task<List<ProductImage>> GetByProductIdAsync(Guid productId, bool onlyImages = true)
        {
            var query = _dbSet.Where(x => x.ProductId == productId);

            if (onlyImages)
            {
                query = query.Where(x => x.MediaType == MediaTypeEnum.Image);
            }

            return await query
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy ảnh chính của product
        /// </summary>
        public async Task<ProductImage?> GetMainImageAsync(Guid productId)
        {
            return await _dbSet
                .Where(x => x.ProductId == productId && x.IsMain)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Đặt ảnh chính cho product
        /// </summary>
        public async Task SetMainImageAsync(Guid productId, Guid newMainImageId)
        {
            // Bỏ IsMain của tất cả ảnh hiện tại
            var currentImages = await _dbSet
                .Where(x => x.ProductId == productId)
                .ToListAsync();

            foreach (var img in currentImages)
            {
                img.IsMain = false;
            }

            // Đặt ảnh mới làm main
            var newMainImage = currentImages.FirstOrDefault(x => x.Id == newMainImageId);
            if (newMainImage != null)
            {
                newMainImage.IsMain = true;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Lấy DisplayOrder tiếp theo cho product (để insert ảnh mới)
        /// </summary>
        public async Task<int> GetNextDisplayOrderAsync(Guid productId)
        {
            var maxOrder = await _dbSet
                .Where(x => x.ProductId == productId)
                .MaxAsync(x => (int?)x.DisplayOrder);

            return (maxOrder ?? -1) + 1;
        }

        public async Task DeleteAsync(Guid imageId)
        {
            var entity = await _dbSet.FindAsync(imageId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }


    }
}
