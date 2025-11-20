using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.CRM;

namespace VietCommerce.Data.Repositories.Interfaces;

/// <summary>
/// Repository interface for CRM Interactions
/// </summary>
public interface ICRMInteractionRepository : IGenericRepository<CRMInteraction>
{
    /// <summary>
    /// Get all interactions for a specific customer
    /// </summary>
    Task<List<CRMInteraction>> GetByCustomerIdAsync(Guid customerId);

    /// <summary>
    /// Get interactions with upcoming follow-up dates
    /// </summary>
    Task<List<CRMInteraction>> GetUpcomingFollowUpsAsync(DateTime? fromDate = null, DateTime? toDate = null);

    /// <summary>
    /// Get interactions by type
    /// </summary>
    Task<List<CRMInteraction>> GetByTypeAsync(CRMInteractionType type);

    /// <summary>
    /// Get interactions by status
    /// </summary>
    Task<List<CRMInteraction>> GetByStatusAsync(CRMInteractionStatus status);

    /// <summary>
    /// Get interactions created by specific user
    /// </summary>
    Task<List<CRMInteraction>> GetByCreatedByAsync(Guid userId);
}