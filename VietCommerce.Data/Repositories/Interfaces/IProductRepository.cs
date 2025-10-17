using VietCommerce.Data.Entities.Products; 
namespace VietCommerce.Data.Repositories.Interfaces;
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> GetBySlugAsync(string slug);
        Task<Product?> GetByCodeAsync(string code);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<PaginatedResult<Product>> GetPaginatedAsync(
            int pageNumber, 
            int pageSize, 
            string? searchTerm = null,
            Guid? categoryId = null,
            Guid? storeId = null,
            bool? isActive = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortBy = null,
            bool isDescending = false
        );
        Task<IEnumerable<Product>> GetByStoreIdAsync(Guid storeId);
        Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
        Task<IEnumerable<Product>> GetPromotedProductsAsync(int count = 10);
        Task<IEnumerable<Product>> GetRelatedProductsAsync(Guid productId, int count = 5);
        Task<IEnumerable<Product>> GetRecentlyViewedAsync(Guid userId, int count = 10);
        Task<IEnumerable<Product>> GetFavoriteProductsAsync(Guid userId);
        Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null);
        Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> SoftDeleteAsync(Guid id, Guid deletedBy);
        Task<int> GetStockQuantityAsync(Guid productId);
        Task<bool> UpdateStockAsync(Guid productId, int quantity);
        Task<decimal?> GetCurrentPriceAsync(Guid productId);
    }

