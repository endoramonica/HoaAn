using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Campaign entity
    /// Handles all data access operations for campaigns
    /// </summary>
    public interface ICampaignRepository : IGenericRepository<Campaign>
    {
        /// <summary>
        /// Get campaigns by store ID with pagination and filtering
        /// </summary>
        Task<PaginatedResult<Campaign>> GetCampaignsByStoreAsync(
            Guid storeId,
            int pageNumber,
            int pageSize,
            CampaignStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? searchTerm = null,
            string? sortBy = null,
            bool isDescending = false);

        /// <summary>
        /// Get active campaigns for a store
        /// </summary>
        Task<IEnumerable<Campaign>> GetActiveCampaignsAsync(Guid storeId);

        /// <summary>
        /// Get campaigns by status
        /// </summary>
        Task<IEnumerable<Campaign>> GetCampaignsByStatusAsync(Guid storeId, CampaignStatus status);

        /// <summary>
        /// Check if campaign exists by ID
        /// </summary>
        Task<bool> CampaignExistsAsync(Guid id);

        /// <summary>
        /// Get campaign with all related data (promotions, impressions, clicks)
        /// </summary>
        Task<Campaign?> GetCampaignWithDetailsAsync(Guid id);
    }
}
