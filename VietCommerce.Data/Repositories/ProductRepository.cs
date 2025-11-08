using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Enums.Products;
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
    public override async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Prices)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
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


    public async Task<PaginatedResult<ProductListDto>> GetPaginatedDtoAsync(
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
        var now = DateTime.UtcNow;

        // ===============================
        // Step 1: Base product query
        // ===============================
        var query = _dbSet
            .Include(p => p.Category)
            .Include(p => p.Store)
            .Include(p => p.Images)
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

        // ===============================
        // Step 2: Pagination (only product ids)
        // ===============================
        var totalItems = await query.CountAsync();

        var productsPage = await query
            .OrderByDescending(p => p.CreatedAt) // default sort
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var productIds = productsPage.Select(p => p.Id).ToList();
        if (!productIds.Any())
            return new PaginatedResult<ProductListDto>(new List<ProductListDto>(), pageNumber, pageSize, totalItems);

        // ===============================
        // Step 3: Load Prices & Promotions optimized
        // ===============================
        var prices = await _context.ProductPrices
            .Where(pr => productIds.Contains(pr.ProductId)
                         && pr.IsActive
                         && pr.PriceType == PriceType.REGULAR)
            .GroupBy(pr => pr.ProductId)
            .Select(g => g
                .OrderByDescending(pr => pr.EffectiveFrom) // Lấy giá mới nhất
                .FirstOrDefault())
            .ToListAsync();

        // Tạo dictionary để tra nhanh
        var priceDict = prices
            .Where(p => p != null)
            .ToDictionary(p => p.ProductId, p => p.Price);

        var promotions = await _context.PromotionProducts
            .Include(pp => pp.Promotion)
            .Where(pp => productIds.Contains(pp.ProductId)
                         && pp.Promotion.IsActive
                         && pp.Promotion.StartDate <= now
                         && pp.Promotion.EndDate >= now)
            .ToListAsync();

        // Dictionary promotion theo ProductId
        var promotionDict = promotions
            .GroupBy(pp => pp.ProductId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(pp => pp.Promotion.DiscountValue)
                      .Select(pp => pp.Promotion)
                      .FirstOrDefault()
            );

        // ===============================
        // Step 4: Map to DTO (CurrentPrice + Promotion)
        // ===============================
        var dtos = productsPage.Select(p =>
        {
            decimal currentPrice = priceDict.ContainsKey(p.Id) ? priceDict[p.Id] : 0;

            var activePromotion = promotionDict.ContainsKey(p.Id) ? promotionDict[p.Id] : null;

            decimal discountAmount = 0;
            if (activePromotion != null)
            {
                discountAmount = activePromotion.PromotionType == PromotionType.PERCENTAGE
                    ? currentPrice * (activePromotion.DiscountValue / 100)
                    : Math.Min(currentPrice, activePromotion.DiscountValue);
            }

            var discountedPrice = currentPrice - discountAmount;

            return new ProductListDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                StockQuantity = p.Stock,
                CategoryName = p.Category?.Name,
                PrimaryImage = p.Images.FirstOrDefault()?.Url,
                ViewCount = (int)p.ViewCount,
                FavoriteCount = p.FavoriteCount,
                AverageRating = p.AvgRating,
                Price = discountedPrice,
                CompareAtPrice = currentPrice,
                DisplayPrice = new DisplayPriceResult
                {
                    OriginalPrice = currentPrice,
                    DiscountedPrice = discountedPrice,
                    DiscountAmount = discountAmount,
                    PromotionName = activePromotion?.PromotionName
                }
            };
        }).ToList();

        // ===============================
        // Step 5: Sorting (on DTOs)
        // ===============================
        dtos = sortBy?.ToLower() switch
        {
            "price" => isDescending ? dtos.OrderByDescending(x => x.Price).ToList() : dtos.OrderBy(x => x.Price).ToList(),
            "name" => isDescending ? dtos.OrderByDescending(x => x.Name).ToList() : dtos.OrderBy(x => x.Name).ToList(),
            "viewcount" => isDescending ? dtos.OrderByDescending(x => x.ViewCount).ToList() : dtos.OrderBy(x => x.ViewCount).ToList(),
            "purchasecount" => isDescending ? dtos.OrderByDescending(x => x.FavoriteCount).ToList() : dtos.OrderBy(x => x.FavoriteCount).ToList(),
            "rating" => isDescending ? dtos.OrderByDescending(x => x.AverageRating).ToList() : dtos.OrderBy(x => x.AverageRating).ToList(),
            "trending" => isDescending ? dtos.OrderByDescending(x => x.StockQuantity).ToList() : dtos.OrderBy(x => x.StockQuantity).ToList(),
            _ => isDescending ? dtos.OrderByDescending(x => x.Id).ToList() : dtos.OrderBy(x => x.Id).ToList()
        };

        return new PaginatedResult<ProductListDto>(dtos, pageNumber, pageSize, totalItems);
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

        var newQty = inventory.QuantityAvailable + quantity;
        if (newQty < 0) return false;

        inventory.QuantityAvailable = newQty;
        inventory.UpdatedAt = DateTime.UtcNow;

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product != null)
            product.Stock = newQty;

        await _context.SaveChangesAsync();
        return true;
        Console.WriteLine($"Check Inventory for ProductId: {productId}");

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
