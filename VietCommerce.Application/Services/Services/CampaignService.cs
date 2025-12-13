using AutoMapper;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Service implementation for Campaign management
    /// Handles all business logic for campaign CRUD operations
    /// Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 6.1, 6.2, 6.3, 6.4, 6.5
    /// </summary>
    public class CampaignService : BaseService, ICampaignService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // Cache TTL constants
        private static readonly TimeSpan CampaignCacheDuration = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan ListCacheDuration = TimeSpan.FromMinutes(5);

        public CampaignService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CampaignService> logger,
            ICacheService cacheService)
            : base(logger, cacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Create a new campaign
        /// Requirements: 1.1, 1.2, 1.3
        /// </summary>
        public async Task<ApiResponse<CampaignDto>> CreateCampaignAsync(CreateCampaignDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"➕ Creating new campaign: {dto.CampaignName}");

                // Validate input
                ValidateNotEmpty(dto.CampaignName, nameof(dto.CampaignName));
                ValidateId(dto.StoreId, nameof(dto.StoreId));

                // Validate date range (Requirement: 1.2)
                if (dto.EndDate <= dto.StartDate)
                {
                    LogWarning($"⚠️ Invalid date range: EndDate ({dto.EndDate}) <= StartDate ({dto.StartDate})");
                    throw new ArgumentException("EndDate must be greater than StartDate", nameof(dto.EndDate));
                }

                // Validate budget (Requirement: 1.3)
                if (dto.Budget < 0)
                {
                    LogWarning($"⚠️ Invalid budget: {dto.Budget} < 0");
                    throw new ArgumentException("Budget must be non-negative", nameof(dto.Budget));
                }

                // Map DTO to entity
                var campaign = _mapper.Map<Campaign>(dto);

                // Set default values (Requirement: 6.1)
                campaign.Id = Guid.NewGuid();
                campaign.Status = CampaignStatus.DRAFT;
                campaign.CreatedAt = DateTime.UtcNow;
                campaign.UpdatedAt = DateTime.UtcNow;
                campaign.ActualCost = 0;

                // Create campaign
                var createdCampaign = await _unitOfWork.Campaigns.AddAsync(campaign);
                await _unitOfWork.SaveChangesAsync();

                // Map to DTO
                var result = _mapper.Map<CampaignDto>(createdCampaign);

                LogInfo($"✅ Created campaign: {createdCampaign.Id} - {createdCampaign.CampaignName}");

                // Invalidate cache
                await InvalidateCacheByPrefixAsync($"campaigns:{dto.StoreId}");

                return result;

            }, "CreateCampaignAsync", "Campaign created successfully");
        }

        /// <summary>
        /// Get campaigns with pagination and filtering
        /// Requirements: 1.5
        /// </summary>
        public async Task<ApiResponse<PaginatedResult<CampaignDto>>> GetCampaignsAsync(GetCampaignsQueryDto query)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"📋 Getting campaigns - Page: {query.PageNumber}, Size: {query.PageSize}");

                // Validate pagination parameters
                if (query.PageNumber < 1)
                {
                    throw new ArgumentException("Page number must be greater than 0", nameof(query.PageNumber));
                }

                if (query.PageSize < 1 || query.PageSize > 100)
                {
                    throw new ArgumentException("Page size must be between 1 and 100", nameof(query.PageSize));
                }

                // Get current user's store ID (from context)
                var storeId = GetCurrentStoreId();

                // Get campaigns from repository with filters
                var result = await _unitOfWork.Campaigns.GetCampaignsByStoreAsync(
                    storeId: storeId,
                    pageNumber: query.PageNumber,
                    pageSize: query.PageSize,
                    status: query.Status,
                    fromDate: query.FromDate,
                    toDate: query.ToDate,
                    searchTerm: query.SearchTerm,
                    sortBy: query.SortBy,
                    isDescending: query.IsDescending
                );

                // Map to DTOs
                var dtos = _mapper.Map<List<CampaignDto>>(result.Items);

                var paginatedResult = new PaginatedResult<CampaignDto>
                {
                    Items = dtos,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize,
                    TotalItems = result.TotalItems,
                    TotalPages = result.TotalPages
                };

                LogInfo($"✅ Retrieved {dtos.Count} campaigns (Total: {result.TotalItems})");
                return paginatedResult;

            }, "GetCampaignsAsync", "Campaigns retrieved successfully");
        }

        /// <summary>
        /// Get campaign by ID
        /// </summary>
        public async Task<ApiResponse<CampaignDto>> GetCampaignByIdAsync(Guid id)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"🔍 Getting campaign by ID: {id}");

                // Validate campaign ID
                ValidateId(id, nameof(id));

                // Retrieve campaign with details
                var campaign = await _unitOfWork.Campaigns.GetCampaignWithDetailsAsync(id);

                if (campaign == null)
                {
                    LogWarning($"⚠️ Campaign not found: {id}");
                    throw new KeyNotFoundException($"Campaign with ID '{id}' not found");
                }

                // Map to DTO
                var dto = _mapper.Map<CampaignDto>(campaign);

                LogInfo($"✅ Retrieved campaign: {campaign.CampaignName}");
                return dto;

            }, "GetCampaignByIdAsync", "Campaign retrieved successfully");
        }

        /// <summary>
        /// Update a campaign
        /// Requirements: 1.4
        /// </summary>
        public async Task<ApiResponse<CampaignDto>> UpdateCampaignAsync(Guid id, UpdateCampaignDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"✏️ Updating campaign: {id}");

                // Validate campaign ID
                ValidateId(id, nameof(id));

                // Retrieve existing campaign
                var existingCampaign = await _unitOfWork.Campaigns.GetByIdAsync(id);

                if (existingCampaign == null)
                {
                    LogWarning($"⚠️ Campaign not found: {id}");
                    throw new KeyNotFoundException($"Campaign with ID '{id}' not found");
                }

                // Validate date range if dates are being updated
                if (dto.StartDate.HasValue || dto.EndDate.HasValue)
                {
                    var startDate = dto.StartDate ?? existingCampaign.StartDate;
                    var endDate = dto.EndDate ?? existingCampaign.EndDate;

                    if (endDate <= startDate)
                    {
                        LogWarning($"⚠️ Invalid date range: EndDate ({endDate}) <= StartDate ({startDate})");
                        throw new ArgumentException("EndDate must be greater than StartDate", nameof(dto.EndDate));
                    }
                }

                // Validate budget if being updated
                if (dto.Budget.HasValue && dto.Budget.Value < 0)
                {
                    LogWarning($"⚠️ Invalid budget: {dto.Budget} < 0");
                    throw new ArgumentException("Budget must be non-negative", nameof(dto.Budget));
                }

                // Update only provided fields
                _mapper.Map(dto, existingCampaign);

                // Update timestamp
                existingCampaign.UpdatedAt = DateTime.UtcNow;

                // Save changes
                _unitOfWork.Campaigns.Update(existingCampaign);
                await _unitOfWork.SaveChangesAsync();

                // Map to DTO
                var result = _mapper.Map<CampaignDto>(existingCampaign);

                LogInfo($"✅ Updated campaign: {existingCampaign.Id} - {existingCampaign.CampaignName}");

                // Invalidate cache
                await InvalidateCacheByPrefixAsync($"campaigns:{existingCampaign.StoreId}");

                return result;

            }, "UpdateCampaignAsync", "Campaign updated successfully");
        }

        /// <summary>
        /// Delete a campaign
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteCampaignAsync(Guid id)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"🗑️ Deleting campaign: {id}");

                // Validate campaign ID
                ValidateId(id, nameof(id));

                // Retrieve campaign
                var campaign = await _unitOfWork.Campaigns.GetByIdAsync(id);

                if (campaign == null)
                {
                    LogWarning($"⚠️ Campaign not found: {id}");
                    throw new KeyNotFoundException($"Campaign with ID '{id}' not found");
                }

                // Delete campaign
                _unitOfWork.Campaigns.Delete(campaign);
                await _unitOfWork.SaveChangesAsync();

                LogInfo($"✅ Deleted campaign: {id} - {campaign.CampaignName}");

                // Invalidate cache
                await InvalidateCacheByPrefixAsync($"campaigns:{campaign.StoreId}");

                return true;

            }, "DeleteCampaignAsync", "Campaign deleted successfully");
        }

        /// <summary>
        /// Change campaign status
        /// Requirements: 6.1, 6.2, 6.3, 6.4, 6.5
        /// </summary>
        public async Task<ApiResponse<CampaignDto>> ChangeCampaignStatusAsync(Guid id, int newStatus)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"🔄 Changing campaign status: {id} to {newStatus}");

                // Validate campaign ID
                ValidateId(id, nameof(id));

                // Validate status value
                if (!Enum.IsDefined(typeof(CampaignStatus), newStatus))
                {
                    throw new ArgumentException($"Invalid campaign status: {newStatus}", nameof(newStatus));
                }

                var targetStatus = (CampaignStatus)newStatus;

                // Retrieve campaign
                var campaign = await _unitOfWork.Campaigns.GetByIdAsync(id);

                if (campaign == null)
                {
                    LogWarning($"⚠️ Campaign not found: {id}");
                    throw new KeyNotFoundException($"Campaign with ID '{id}' not found");
                }

                // Validate status transition (Requirement: 6.3)
                if (campaign.Status == CampaignStatus.DRAFT && targetStatus == CampaignStatus.ACTIVE)
                {
                    // Validate all required fields are set
                    if (string.IsNullOrWhiteSpace(campaign.CampaignName) ||
                        campaign.Budget < 0 ||
                        campaign.EndDate <= campaign.StartDate)
                    {
                        LogWarning($"⚠️ Cannot activate campaign {id} - missing required fields");
                        throw new InvalidOperationException(
                            "Cannot activate campaign. Ensure CampaignName is set, Budget >= 0, and EndDate > StartDate");
                    }
                }

                // Prevent adding promotions to non-DRAFT campaigns (Requirement: 6.4)
                // This is enforced in PromotionService

                // Update status
                campaign.Status = targetStatus;
                campaign.UpdatedAt = DateTime.UtcNow;

                // Save changes
                _unitOfWork.Campaigns.Update(campaign);
                await _unitOfWork.SaveChangesAsync();

                // Map to DTO
                var result = _mapper.Map<CampaignDto>(campaign);

                LogInfo($"✅ Changed campaign status: {id} to {targetStatus}");

                // Invalidate cache
                await InvalidateCacheByPrefixAsync($"campaigns:{campaign.StoreId}");

                return result;

            }, "ChangeCampaignStatusAsync", "Campaign status changed successfully");
        }

        /// <summary>
        /// Helper method to get current store ID from context
        /// </summary>
        private Guid GetCurrentStoreId()
        {
            // TODO: Implement based on your authentication context
            // For now, return a placeholder
            return Guid.Empty;
        }
    }
}
