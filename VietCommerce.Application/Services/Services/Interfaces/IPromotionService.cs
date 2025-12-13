using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Service interface for Promotion management
    /// Handles all business logic for promotion CRUD operations
    /// Requirements: 2.1, 2.2, 2.3, 2.4, 2.5
    /// </summary>
    public interface IPromotionService
    {
        /// <summary>
        /// Create a new promotion for a campaign
        /// Requirements: 2.1, 2.2, 2.3, 2.4
        /// </summary>
        Task<ApiResponse<PromotionDto>> CreatePromotionAsync(Guid campaignId, CreatePromotionDto dto);

        /// <summary>
        /// Get promotions for a campaign with pagination and filtering
        /// Requirements: 2.5
        /// </summary>
        Task<ApiResponse<PaginatedResult<PromotionDto>>> GetPromotionsByCampaignAsync(
            Guid campaignId,
            GetPromotionsQueryDto query);

        /// <summary>
        /// Get promotion by ID
        /// </summary>
        Task<ApiResponse<PromotionDto>> GetPromotionByIdAsync(Guid id);

        /// <summary>
        /// Update a promotion
        /// </summary>
        Task<ApiResponse<PromotionDto>> UpdatePromotionAsync(Guid id, UpdatePromotionDto dto);

        /// <summary>
        /// Delete a promotion (soft delete)
        /// </summary>
        Task<ApiResponse<bool>> DeletePromotionAsync(Guid id);

        /// <summary>
        /// Link products to a promotion
        /// </summary>
        Task<ApiResponse<bool>> LinkProductsAsync(Guid promotionId, LinkProductsDto dto);

        /// <summary>
        /// Get linked products for a promotion
        /// </summary>
        Task<ApiResponse<List<ProductListDto>>> GetLinkedProductsAsync(Guid promotionId);

        /// <summary>
        /// Unlink products from a promotion
        /// </summary>
        Task<ApiResponse<bool>> UnlinkProductsAsync(Guid promotionId, List<Guid> productIds);
    }
}
