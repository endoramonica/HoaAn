using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Models;
namespace VietCommerce.Data.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    // Lookup
    Task<Product?> GetBySlugAsync(string slug);
    Task<Product?> GetByCodeAsync(string code);
    Task<IEnumerable<Product>> GetByStoreIdAsync(Guid storeId);
    Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);

    // Pagination + Filtering
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

    // Business use cases
    Task<IEnumerable<Product>> GetPromotedProductsAsync(int count = 10);
    Task<IEnumerable<Product>> GetRelatedProductsAsync(Guid productId, int count = 5);
    Task<IEnumerable<Product>> GetRecentlyViewedAsync(Guid userId, int count = 10);
    Task<IEnumerable<Product>> GetFavoriteProductsAsync(Guid userId);

    // Validation
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null);
    Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null);

    // Inventory & pricing
    Task<int> GetStockQuantityAsync(Guid productId);
    Task<bool> UpdateStockAsync(Guid productId, int quantity);
    Task<decimal?> GetCurrentPriceAsync(Guid productId);

    // Statistics / Favorites / Trending
    Task<bool> IncrementViewCountAsync(Guid productId);
    Task<bool> UpdateFavoriteStatusAsync(Guid userId, Guid productId, bool isFavorite);
    Task<IEnumerable<Product>> GetTrendingProductsAsync(int top = 10);

    // Soft delete (business-specific)
    Task<bool> SoftDeleteAsync(Guid id, Guid deletedBy);
}
