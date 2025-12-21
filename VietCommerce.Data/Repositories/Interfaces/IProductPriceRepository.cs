using VietCommerce.Core.Entities.Products;

namespace VietCommerce.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface for ProductPrice entity
/// Handles CRUD operations and queries for product pricing
/// Requirements: 1.4, 1.5, 3.1, 3.5, 3.6
/// </summary>
public interface IProductPriceRepository : IGenericRepository<ProductPrice>
{
    /// <summary>
    /// Get all prices for a product
    /// Requirements: 3.1, 3.5
    /// </summary>
    Task<List<ProductPrice>> GetByProductIdAsync(Guid productId);

    /// <summary>
    /// Get active prices for a product (where EffectiveTo > now or EffectiveTo is null)
    /// Requirements: 3.6
    /// </summary>
    Task<List<ProductPrice>> GetActivePricesByProductIdAsync(Guid productId);

    /// <summary>
    /// Get a specific price by ID
    /// </summary>
    Task<ProductPrice?> GetByIdAsync(Guid priceId);

    /// <summary>
    /// Delete a price by ID
    /// </summary>
    Task DeleteAsync(Guid priceId);
}