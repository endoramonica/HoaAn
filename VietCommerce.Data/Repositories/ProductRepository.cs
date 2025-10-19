using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Models;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    // ========================================
    // LOOKUP BY RELATION
    // ========================================

    public async Task<Product?> GetBySlugAsync(string slug)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Store)
            .Include(p => p.Images)
            .Include(p => p.Prices.Where(pr => pr.IsActive && pr.EffectiveFrom <= DateTime.UtcNow))
            .FirstOrDefaultAsync(p => p.Slug == slug && !p.IsDeleted);
    }

    public async Task<Product?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Store)
            .FirstOrDefaultAsync(p => p.Code == code && !p.IsDeleted);
    }

    public async Task<IEnumerable<Product>> GetByStoreIdAsync(Guid storeId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.StoreId == storeId && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId)
    {
        return await _dbSet
            .Include(p => p.Store)
            .Include(p => p.Images)
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .OrderByDescending(p => p.TrendingScore)
            .ToListAsync();
    }
    // ========================================
    // PAGINATION + FILTERING
    // ========================================

    public async Task<PaginatedResult<Product>> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        Guid? categoryId = null,
        Guid? storeId = null,
        bool? isActive = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? sortBy = null,
        bool isDescending = false)
    {
        var query = _dbSet
            .Include(p => p.Category)
            .Include(p => p.Store)
            .Include(p => p.Images.Take(1))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.Code.ToLower().Contains(search) ||
                p.SKU.ToLower().Contains(search));
        }

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (storeId.HasValue)
            query = query.Where(p => p.StoreId == storeId.Value);

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        if (minPrice.HasValue || maxPrice.HasValue)
        {
            var now = DateTime.UtcNow;
            query = query.Where(p => p.Prices.Any(pr =>
                pr.IsActive &&
                pr.EffectiveFrom <= now &&
                (pr.EffectiveTo == null || pr.EffectiveTo >= now) &&
                (!minPrice.HasValue || pr.Price >= minPrice.Value) &&
                (!maxPrice.HasValue || pr.Price <= maxPrice.Value)));
        }

        query = sortBy?.ToLower() switch
        {
            "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "price" => isDescending
                ? query.OrderByDescending(p => p.Prices
                    .Where(pr => pr.IsActive && pr.EffectiveFrom <= DateTime.UtcNow)
                    .OrderBy(pr => pr.PriceType)
                    .Select(pr => pr.Price)
                    .FirstOrDefault())
                : query.OrderBy(p => p.Prices
                    .Where(pr => pr.IsActive && pr.EffectiveFrom <= DateTime.UtcNow)
                    .OrderBy(pr => pr.PriceType)
                    .Select(pr => pr.Price)
                    .FirstOrDefault()),
            "viewcount" => isDescending ? query.OrderByDescending(p => p.ViewCount) : query.OrderBy(p => p.ViewCount),
            "purchasecount" => isDescending ? query.OrderByDescending(p => p.PurchaseCount) : query.OrderBy(p => p.PurchaseCount),
            "rating" => isDescending ? query.OrderByDescending(p => p.AvgRating) : query.OrderBy(p => p.AvgRating),
            "trending" => isDescending ? query.OrderByDescending(p => p.TrendingScore) : query.OrderBy(p => p.TrendingScore),
            _ => isDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
        };

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Product>(items, pageNumber, pageSize, totalItems);
    }

    // ========================================
    // BUSINESS USE CASES
    // ========================================

    public async Task<IEnumerable<Product>> GetPromotedProductsAsync(int count = 10)
    {
        var now = DateTime.UtcNow;

        return await _dbSet
            .Include(p => p.Store)
            .Include(p => p.Images.Take(1))
            .Include(p => p.Prices)
            .Where(p => p.IsActive &&
                        p.PromotionProducts.Any(pp =>
                            pp.Promotion.IsActive &&
                            pp.Promotion.StartDate <= now &&
                            pp.Promotion.EndDate >= now))
            .OrderByDescending(p => p.TrendingScore)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetRelatedProductsAsync(Guid productId, int count = 5)
    {
        var product = await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null || product.CategoryId == null)
            return Enumerable.Empty<Product>();

        return await _dbSet
            .Include(p => p.Store)
            .Include(p => p.Images.Take(1))
            .Where(p => p.Id != productId &&
                        p.CategoryId == product.CategoryId &&
                        p.IsActive)
            .OrderByDescending(p => p.TrendingScore)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetRecentlyViewedAsync(Guid userId, int count = 10)
    {
        var recentViewedIds = await _context.ProductViews
            .Where(pv => pv.UserId == userId)
            .OrderByDescending(pv => pv.ViewedAt)
            .Select(pv => pv.ProductId)
            .Distinct()
            .Take(count)
            .ToListAsync();

        if (!recentViewedIds.Any())
            return Enumerable.Empty<Product>();

        var products = await _dbSet
            .Include(p => p.Store)
            .Include(p => p.Images.Take(1))
            .Where(p => recentViewedIds.Contains(p.Id) && p.IsActive)
            .ToListAsync();

        // Preserve order
        return recentViewedIds
            .Select(id => products.FirstOrDefault(p => p.Id == id))
            .Where(p => p != null)
            .Cast<Product>();
    }

    public async Task<IEnumerable<Product>> GetFavoriteProductsAsync(Guid userId)
    {
        var favoriteIds = await _context.ProductFavorites
            .Where(pf => pf.UserId == userId)
            .OrderByDescending(pf => pf.CreatedAt)
            .Select(pf => pf.ProductId)
            .ToListAsync();

        if (!favoriteIds.Any())
            return Enumerable.Empty<Product>();

        return await _dbSet
            .Include(p => p.Store)
            .Include(p => p.Images.Take(1))
            .Include(p => p.Category)
            .Where(p => favoriteIds.Contains(p.Id) && p.IsActive)
            .ToListAsync();
    }

    // ========================================
    // VALIDATION HELPERS
    // ========================================

    public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null)
    {
        var query = _dbSet.Where(p => p.Code == code);
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    public async Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null)
    {
        var query = _dbSet.Where(p => p.Slug == slug);
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);
        return await query.AnyAsync();
    }

    // ========================================
    // INVENTORY & PRICING
    // ========================================

    public async Task<int> GetStockQuantityAsync(Guid productId)
    {
        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId && !i.IsDeleted);
        return inventory?.QuantityAvailable ?? 0;
    }

    public async Task<bool> UpdateStockAsync(Guid productId, int quantity)
    {
        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId && !i.IsDeleted);

        if (inventory == null) return false;

        inventory.QuantityAvailable = quantity;
        inventory.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal?> GetCurrentPriceAsync(Guid productId)
    {
        var now = DateTime.UtcNow;

        return await _context.ProductPrices
            .Where(pp => pp.ProductId == productId &&
                         pp.IsActive &&
                         pp.EffectiveFrom <= now &&
                         (pp.EffectiveTo == null || pp.EffectiveTo >= now))
            .OrderBy(pp => pp.PriceType)
            .Select(pp => pp.Price)
            .FirstOrDefaultAsync();
    }

    // ========================================
    // STATISTICS / FAVORITES / TRENDING
    // ========================================

    public async Task<bool> IncrementViewCountAsync(Guid productId)
    {
        var product = await _dbSet.FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null) return false;

        product.ViewCount++;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateFavoriteStatusAsync(Guid userId, Guid productId, bool isFavorite)
    {
        var existingFavorite = await _context.ProductFavorites
            .FirstOrDefaultAsync(pf => pf.UserId == userId && pf.ProductId == productId);

        var product = await _dbSet.FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null) return false;

        if (isFavorite)
        {
            if (existingFavorite == null)
            {
                _context.ProductFavorites.Add(new()
                {
                    UserId = userId,
                    ProductId = productId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                product.FavoriteCount++;
            }
        }
        else if (existingFavorite != null)
        {
            _context.ProductFavorites.Remove(existingFavorite);
            product.FavoriteCount = Math.Max(0, product.FavoriteCount - 1);
        }

        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Product>> GetTrendingProductsAsync(int top = 10)
    {
        return await _dbSet
            .Include(p => p.Store)
            .Include(p => p.Images.Take(1))
            .Include(p => p.Category)
            .Where(p => p.IsActive && p.TrendingScore > 0)
            .OrderByDescending(p => p.TrendingScore)
            .ThenByDescending(p => p.ViewCount)
            .Take(top)
            .ToListAsync();
    }

    // ========================================
    // SOFT DELETE (business specific)
    // ========================================

    public async Task<bool> SoftDeleteAsync(Guid id, Guid deletedBy)
    {
        var product = await _dbSet.IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return false;

        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;
        product.DeletedBy = deletedBy;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}
