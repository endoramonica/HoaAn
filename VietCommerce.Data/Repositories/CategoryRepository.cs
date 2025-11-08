using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;
namespace VietCommerce.Data.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }
        public async Task<IEnumerable<Category>> GetByStoreIdAsync(Guid storeId)
        {
            return await _dbSet
                .Where(c => c.StoreId == storeId && !c.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid parentId)
        {
            return await _dbSet
                .Where(c => c.ParentId == parentId && !c.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.Name)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<bool> ExistsByNameAsync(Guid storeId, string name)
        {
            return await _dbSet
                .AnyAsync(c => c.StoreId == storeId
                            && c.Name.ToLower() == name.ToLower()
                            && !c.IsDeleted);
        }
        public async Task<Category?> GetWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.SubCategories)
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
        public async Task<int> CountByStoreAsync(Guid storeId)
        {
            return await _dbSet.CountAsync(c => c.StoreId == storeId && !c.IsDeleted);
        }
    }
}