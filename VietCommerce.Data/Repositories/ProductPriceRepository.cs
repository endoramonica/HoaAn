using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

/// <summary>
/// Repository for ProductPrice entity
/// Handles CRUD operations and queries for product pricing
/// Requirements: 1.4, 1.5, 3.1, 3.5, 3.6
/// </summary>
public class ProductPriceRepository : GenericRepository<ProductPrice>, IProductPriceRepository
{
    public ProductPriceRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get all prices for a product
    /// Requirements: 3.1, 3.5
    /// </summary>
    public async Task<List<ProductPrice>> GetByProductIdAsync(Guid productId)
    {
        return await _dbSet
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.EffectiveFrom)
            .ToListAsync();
    }

    /// <summary>
    /// /// Get active prices for a product (where EffectiveTo > now or EffectiveTo is null)
    /// Requirements: 3.6
    /// </summary>
    public async Task<List<ProductPrice>> GetActivePricesByProductIdAsync(Guid productId)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Where(x => x.ProductId == productId
                && x.IsActive
                && (x.EffectiveTo == null || x.EffectiveTo > now))
            .OrderBy(x => x.EffectiveFrom)
            .ToListAsync();
    }

    /// <summary>
    /// Get a specific price by ID
    /// </summary>
    public async Task<ProductPrice?> GetByIdAsync(Guid priceId)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == priceId);
    }

    /// <summary>
    /// Delete a price by ID
    /// </summary>
    public async Task DeleteAsync(Guid priceId)
    {
        var entity = await _dbSet.FindAsync(priceId);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
