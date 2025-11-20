using VietCommerce.Core.DTOs.Suppliers;
using VietCommerce.Core.Entities.Logistics;
using VietCommerce.Core.Models;

namespace VietCommerce.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface for Supplier entity with custom queries
/// </summary>
public interface ISupplierRepository : IGenericRepository<Supplier>
{
    /// <summary>
    /// Get suppliers with pagination and filtering
    /// </summary>
    Task<(IEnumerable<Supplier> Items, int TotalCount)> GetSuppliersPagedAsync(
        PaginationParams pagination,
        SupplierFilters filters);

    /// <summary>
    /// Get supplier by ID with related entities
    /// </summary>
    Task<Supplier?> GetSupplierWithDetailsAsync(Guid id);

    /// <summary>
    /// Check if supplier name exists (for uniqueness validation)
    /// </summary>
    Task<bool> IsNameExistsAsync(string name, Guid? excludeId = null);

    /// <summary>
    /// Check if supplier email exists (for uniqueness validation)
    /// </summary>
    Task<bool> IsEmailExistsAsync(string email, Guid? excludeId = null);

    /// <summary>
    /// Check if supplier has any associated products
    /// </summary>
    Task<bool> HasProductsAsync(Guid supplierId);

    /// <summary>
    /// Check if supplier has any associated stock transfers
    /// </summary>
    Task<bool> HasStockTransfersAsync(Guid supplierId);

    /// <summary>
    /// Get count of products associated with supplier
    /// </summary>
    Task<int> GetProductCountAsync(Guid supplierId);

    /// <summary>
    /// Get count of stock transfers associated with supplier
    /// </summary>
    Task<int> GetStockTransferCountAsync(Guid supplierId);
}