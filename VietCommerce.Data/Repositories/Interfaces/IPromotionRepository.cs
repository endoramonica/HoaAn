using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Promotion entity
    /// Handles all data access operations for promotions
    /// Requirements: 2.1, 2.2, 2.3, 2.4, 2.5
    /// </summary>
    public interface IPromotionRepository : IGenericRepository<Promotion>
    {
        /// <summary>
        /// Get promotions by campaign ID with pagination and filtering
        /// Requirements: 2.5
        /// </summary>
        Task<PaginatedResult<Promotion>> GetPromotionsByCampaignAsync(
            Guid campaignId,
            int pageNumber,
            int pageSize,
            PromotionStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? searchTerm = null,
            string? sortBy = null,
            bool isDescending = false);

        /// <summary>
        /// Get active promotions for a campaign
        /// </summary>
        Task<IEnumerable<Promotion>> GetActivePromotionsByCampaignAsync(Guid campaignId);

        /// <summary>
        /// Get promotions by status
        /// </summary>
        Task<IEnumerable<Promotion>> GetPromotionsByStatusAsync(Guid campaignId, PromotionStatus status);

        /// <summary>
        /// Check if promotion exists by ID
        /// </summary>
        Task<bool> PromotionExistsAsync(Guid id);

        /// <summary>
        /// Get promotion with all related data (products, vouchers)
        /// </summary>
        Task<Promotion?> GetPromotionWithDetailsAsync(Guid id);

        /// <summary>
        /// Get promotions for a campaign (non-deleted)
        /// </summary>
        Task<IEnumerable<Promotion>> GetPromotionsForCampaignAsync(Guid campaignId);

        /// <summary>
        /// Check if promotion exists for a campaign
        /// </summary>
        Task<bool> PromotionExistsForCampaignAsync(Guid campaignId, Guid promotionId);
    }
}
