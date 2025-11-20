using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.DTOs.Suppliers;
using VietCommerce.Core.Entities.Logistics;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Models;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

/// <summary>
/// Repository implementation for Supplier entity
/// </summary>
public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
{
    public SupplierRepository( AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get suppliers with pagination and filtering
    /// </summary>
    public async Task<(IEnumerable<Supplier> Items, int TotalCount)> GetSuppliersPagedAsync(
        PaginationParams pagination,
        SupplierFilters filters)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            var searchLower = filters.SearchTerm.ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(searchLower) ||
                s.Email.ToLower().Contains(searchLower) ||
                s.Phone.Contains(searchLower) ||
                s.Address.ToLower().Contains(searchLower));
        }

        if (!string.IsNullOrWhiteSpace(filters.Name))
        {
            query = query.Where(s => s.Name.ToLower().Contains(filters.Name.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(filters.Email))
        {
            query = query.Where(s => s.Email.ToLower().Contains(filters.Email.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(filters.Phone))
        {
            query = query.Where(s => s.Phone.Contains(filters.Phone));
        }

        if (filters.Status.HasValue)
        {
            query = query.Where(s => s.Status == filters.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.Address))
        {
            query = query.Where(s => s.Address.ToLower().Contains(filters.Address.ToLower()));
        }

        if (filters.CreatedFrom.HasValue)
        {
            query = query.Where(s => s.CreatedAt >= filters.CreatedFrom.Value);
        }

        if (filters.CreatedTo.HasValue)
        {
            query = query.Where(s => s.CreatedAt <= filters.CreatedTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.PaymentTerms))
        {
            query = query.Where(s => s.PaymentTerms.ToLower().Contains(filters.PaymentTerms.ToLower()));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = ApplySorting(query, pagination.SortBy, pagination.SortDescending);

        // Apply pagination
        var items = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    /// <summary>
    /// Get supplier by ID with related entities (Products, StockTransfers)
    /// </summary>
    public async Task<Supplier?> GetSupplierWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(s => s.Products)
            .Include(s => s.StockTransfers)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <summary>
    /// Check if supplier name exists (case-insensitive)
    /// </summary>
    public async Task<bool> IsNameExistsAsync(string name, Guid? excludeId = null)
    {
        var query = _dbSet.AsNoTracking()
            .Where(s => s.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// Check if supplier email exists (case-insensitive)
    /// </summary>
    public async Task<bool> IsEmailExistsAsync(string email, Guid? excludeId = null)
    {
        var query = _dbSet.AsNoTracking()
            .Where(s => s.Email.ToLower() == email.ToLower());

        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// Check if supplier has any associated products
    /// </summary>
    public async Task<bool> HasProductsAsync(Guid supplierId)
    {
        return await _context.Set<Product>()
            .AsNoTracking()
            .AnyAsync(p => p.Suppliers.Any(s => s.Id == supplierId));
    }

    /// <summary>
    /// Check if supplier has any associated stock transfers
    /// </summary>
    public async Task<bool> HasStockTransfersAsync(Guid supplierId)
    {
        return await _context.Set<StockTransfer>()
            .AsNoTracking()
            .AnyAsync(st => st.SupplierId == supplierId);
    }

    /// <summary>
    /// Get count of products associated with supplier
    /// </summary>
    public async Task<int> GetProductCountAsync(Guid supplierId)
    {
        return await _context.Set<Product>()
            .AsNoTracking()
            .CountAsync(p => p.Suppliers.Any(s => s.Id == supplierId));
    }

    /// <summary>
    /// Get count of stock transfers associated with supplier
    /// </summary>
    public async Task<int> GetStockTransferCountAsync(Guid supplierId)
    {
        return await _context.Set<StockTransfer>()
            .AsNoTracking()
            .CountAsync(st => st.SupplierId == supplierId);
    }

    /// <summary>
    /// Apply dynamic sorting based on field name
    /// </summary>
    private IQueryable<Supplier> ApplySorting(IQueryable<Supplier> query, string sortBy, bool descending)
    {
        return sortBy.ToLower() switch
        {
            "name" => descending ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
            "email" => descending ? query.OrderByDescending(s => s.Email) : query.OrderBy(s => s.Email),
            "status" => descending ? query.OrderByDescending(s => s.Status) : query.OrderBy(s => s.Status),
            "createdat" => descending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
            "updatedat" => descending ? query.OrderByDescending(s => s.UpdatedAt) : query.OrderBy(s => s.UpdatedAt),
            _ => descending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt)
        };
    }
}