using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.CRM;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

/// <summary>
/// Repository implementation for CRM Interactions
/// </summary>
public class CRMInteractionRepository : GenericRepository<CRMInteraction>, ICRMInteractionRepository
{
    public CRMInteractionRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get all interactions for a specific customer with related data
    /// </summary>
    public async Task<List<CRMInteraction>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.CreatedByUser)
            .Where(i => i.CustomerId == customerId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get interactions with upcoming follow-up dates
    /// </summary>
    public async Task<List<CRMInteraction>> GetUpcomingFollowUpsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = _dbSet
            .Include(i => i.Customer)
            .Include(i => i.CreatedByUser)
            .Where(i => i.FollowUpDate.HasValue);

        // Apply date filters
        if (fromDate.HasValue)
        {
            query = query.Where(i => i.FollowUpDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(i => i.FollowUpDate <= toDate.Value);
        }

        // Only pending interactions
        query = query.Where(i => i.Status == CRMInteractionStatus.Pending);

        return await query
            .OrderBy(i => i.FollowUpDate)
            .ToListAsync();
    }

    /// <summary>
    /// Get interactions by type
    /// </summary>
    public async Task<List<CRMInteraction>> GetByTypeAsync(CRMInteractionType type)
    {
        return await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.CreatedByUser)
            .Where(i => i.Type == type)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get interactions by status
    /// </summary>
    public async Task<List<CRMInteraction>> GetByStatusAsync(CRMInteractionStatus status)
    {
        return await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.CreatedByUser)
            .Where(i => i.Status == status)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get interactions created by specific user
    /// </summary>
    public async Task<List<CRMInteraction>> GetByCreatedByAsync(Guid userId)
    {
        return await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.CreatedByUser)
            .Where(i => i.CreatedBy == userId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }
}