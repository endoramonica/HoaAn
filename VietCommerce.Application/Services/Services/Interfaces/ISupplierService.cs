using VietCommerce.Core.DTOs.Suppliers;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces;

/// <summary>
/// Service interface for managing suppliers
/// Provides business logic for supplier operations including CRUD, validation, and relationship management
/// </summary>
public interface ISupplierService
{
    /// <summary>
    /// Get paginated list of suppliers with filtering and sorting
    /// </summary>
    /// <param name="pagination">Pagination parameters (page, pageSize, sortBy, sortDescending)</param>
    /// <param name="filters">Filter criteria (name, email, status, etc.)</param>
    /// <returns>Paginated result containing supplier DTOs and total count</returns>
    Task<PaginatedResult<SupplierDto>> GetSuppliersAsync(
        PaginationParams pagination,
        SupplierFilters filters);

    /// <summary>
    /// Get supplier details by ID
    /// </summary>
    /// <param name="id">Supplier unique identifier</param>
    /// <returns>Supplier DTO with product and stock transfer counts</returns>
    /// <exception cref="KeyNotFoundException">When supplier not found</exception>
    Task<SupplierDto> GetSupplierByIdAsync(Guid id);

    /// <summary>
    /// Create a new supplier with validation
    /// </summary>
    /// <param name="request">Supplier creation request</param>
    /// <returns>Created supplier DTO</returns>
    /// <exception cref="InvalidOperationException">When name or email already exists</exception>
    /// <exception cref="ArgumentException">When validation fails</exception>
    Task<SupplierDto> CreateSupplierAsync(CreateSupplierRequest request);

    /// <summary>
    /// Update an existing supplier (partial update supported)
    /// </summary>
    /// <param name="id">Supplier unique identifier</param>
    /// <param name="request">Supplier update request (nullable fields for partial update)</param>
    /// <returns>Updated supplier DTO</returns>
    /// <exception cref="KeyNotFoundException">When supplier not found</exception>
    /// <exception cref="InvalidOperationException">When name or email already exists</exception>
    /// <exception cref="ArgumentException">When validation fails</exception>
    Task<SupplierDto> UpdateSupplierAsync(Guid id, UpdateSupplierRequest request);

    /// <summary>
    /// Delete a supplier (soft delete if has relationships)
    /// </summary>
    /// <param name="id">Supplier unique identifier</param>
    /// <returns>Task</returns>
    /// <exception cref="KeyNotFoundException">When supplier not found</exception>
    /// <exception cref="InvalidOperationException">When supplier has active products or stock transfers</exception>
    Task DeleteSupplierAsync(Guid id);

    /// <summary>
    /// Check if supplier can be deleted (no active relationships)
    /// </summary>
    /// <param name="id">Supplier unique identifier</param>
    /// <returns>True if can be deleted, false otherwise</returns>
    Task<bool> CanDeleteSupplierAsync(Guid id);

    /// <summary>
    /// Get supplier statistics (product count, stock transfer count)
    /// </summary>
    /// <param name="id">Supplier unique identifier</param>
    /// <returns>Dictionary with stats</returns>
    Task<Dictionary<string, int>> GetSupplierStatsAsync(Guid id);
}