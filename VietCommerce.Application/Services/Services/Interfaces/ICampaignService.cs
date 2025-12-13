using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Service interface for Campaign management
    /// Handles all business logic for campaign CRUD operations
    /// </summary>
    public interface ICampaignService
    {
        /// <summary>
        /// Create a new campaign
        /// Requirements: 1.1, 1.2, 1.3
        /// </summary>
        Task<ApiResponse<CampaignDto>> CreateCampaignAsync(CreateCampaignDto dto);

        /// <summary>
        /// Get campaigns with pagination and filtering
        /// Requirements: 1.5
        /// </summary>
        Task<ApiResponse<PaginatedResult<CampaignDto>>> GetCampaignsAsync(GetCampaignsQueryDto query);

        /// <summary>
        /// Get campaign by ID
        /// </summary>
        Task<ApiResponse<CampaignDto>> GetCampaignByIdAsync(Guid id);

        /// <summary>
        /// Update a campaign
        /// Requirements: 1.4
        /// </summary>
        Task<ApiResponse<CampaignDto>> UpdateCampaignAsync(Guid id, UpdateCampaignDto dto);

        /// <summary>
        /// Delete a campaign
        /// </summary>
        Task<ApiResponse<bool>> DeleteCampaignAsync(Guid id);

        /// <summary>
        /// Change campaign status
        /// Requirements: 6.1, 6.2, 6.3, 6.4, 6.5
        /// </summary>
        Task<ApiResponse<CampaignDto>> ChangeCampaignStatusAsync(Guid id, int newStatus);
    }
}
