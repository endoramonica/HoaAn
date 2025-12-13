using AutoMapper;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Service implementation for Promotion management
    /// Handles all business logic for promotion CRUD operations
    /// Requirements: 2.1, 2.2, 2.3, 2.4, 2.5
    /// </summary>
    public class PromotionService : BaseService, IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PromotionService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PromotionService> logger,
            ICacheService cacheService)
            : base(logger, cacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Create a new promotion for a campaign
        /// Requirements: 2.1, 2.2, 2.3, 2.4
        /// </summary>
        public async Task<ApiResponse<PromotionDto>> CreatePromotionAsync(Guid campaignId, CreatePromotionDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"➕ Creating new promotion for campaign: {campaignId}");

                // Validate campaign ID
                ValidateId(campaignId, nameof(campaignId));

                // Validate input
                ValidateNotEmpty(dto.PromotionName, nameof(dto.PromotionName));

                // Requirement 2.1, 2.2: Campaign must be DRAFT
                var campaign = await _unitOfWork.Campaigns.GetByIdAsync(campaignId);
                if (campaign == null)
                {
                    LogWarning($"⚠️ Campaign not found: {campaignId}");
                    throw new KeyNotFoundException($"Campaign with ID '{campaignId}' not found");
                }

                if (campaign.Status != CampaignStatus.DRAFT)
                {
                    LogWarning($"⚠️ Cannot add promotion to non-DRAFT campaign: {campaignId} (Status: {campaign.Status})");
                    throw new InvalidOperationException(
                        $"Promotions can only be added to DRAFT campaigns. Current status: {campaign.Status}");
                }

                // Requirement 2.3: Validate required fields
                if (dto.DiscountValue <= 0)
                {
                    LogWarning($"⚠️ Invalid discount value: {dto.DiscountValue} <= 0");
                    throw new ArgumentException("DiscountValue must be greater than 0", nameof(dto.DiscountValue));
                }

                // Requirement 2.4: Validate date range
                if (dto.EndDate <= dto.StartDate)
                {
                    LogWarning($"⚠️ Invalid date range: EndDate ({dto.EndDate}) <= StartDate ({dto.StartDate})");
                    throw new ArgumentException("EndDate must be greater than StartDate", nameof(dto.EndDate));
                }

                // Map DTO to entity
                var promotion = _mapper.Map<Promotion>(dto);

                // Set default values
                promotion.Id = Guid.NewGuid();
                promotion.CampaignId = campaignId;
                promotion.Status = PromotionStatus.ACTIVE;
                promotion.CreatedAt = DateTime.UtcNow;
                promotion.UpdatedAt = DateTime.UtcNow;
                promotion.UsedCount = 0;
                promotion.IsActive = true;
                promotion.IsDeleted = false;

                // Create promotion
                var createdPromotion = await _unitOfWork.Promotions.AddAsync(promotion);
                await _unitOfWork.SaveChangesAsync();

                // Map to DTO
                var result = _mapper.Map<PromotionDto>(createdPromotion);

                LogInfo($"✅ Created promotion: {createdPromotion.Id} - {createdPromotion.PromotionName}");

                // Invalidate cache
                await InvalidateCacheByPrefixAsync($"promotions:{campaignId}");

                return result;

            }, "CreatePromotionAsync", "Promotion created successfully");
        }

        /// <summary>
        /// Get promotions for a campaign with pagination and filtering
        /// Requirements: 2.5
        /// </summary>
        public async Task<ApiResponse<PaginatedResult<PromotionDto>>> GetPromotionsByCampaignAsync(
            Guid campaignId,
            GetPromotionsQueryDto query)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"📋 Getting promotions for campaign: {campaignId} - Page: {query.PageNumber}, Size: {query.PageSize}");

                // Validate campaign ID
                ValidateId(campaignId, nameof(campaignId));

                // Validate pagination parameters
                if (query.PageNumber < 1)
                {
                    throw new ArgumentException("Page number must be greater than 0", nameof(query.PageNumber));
                }

                if (query.PageSize < 1 || query.PageSize > 100)
                {
                    throw new ArgumentException("Page size must be between 1 and 100", nameof(query.PageSize));
                }

                // Verify campaign exists
                var campaignExists = await _unitOfWork.Campaigns.CampaignExistsAsync(campaignId);
                if (!campaignExists)
                {
                    LogWarning($"⚠️ Campaign not found: {campaignId}");
                    throw new KeyNotFoundException($"Campaign with ID '{campaignId}' not found");
                }

                // Get promotions from repository with filters
                var result = await _unitOfWork.Promotions.GetPromotionsByCampaignAsync(
                    campaignId: campaignId,
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
                var dtos = _mapper.Map<List<PromotionDto>>(result.Items);

                var paginatedResult = new PaginatedResult<PromotionDto>
                {
                    Items = dtos,
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize,
                    TotalItems = result.TotalItems,
                    TotalPages = result.TotalPages
                };

                LogInfo($"✅ Retrieved {dtos.Count} promotions (Total: {result.TotalItems})");
                return paginatedResult;

            }, "GetPromotionsByCampaignAsync", "Promotions retrieved successfully");
        }

        /// <summary>
        /// Get promotion by ID
        /// </summary>
        public async Task<ApiResponse<PromotionDto>> GetPromotionByIdAsync(Guid id)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"🔍 Getting promotion by ID: {id}");

                // Validate promotion ID
                ValidateId(id, nameof(id));

                // Retrieve promotion with details
                var promotion = await _unitOfWork.Promotions.GetPromotionWithDetailsAsync(id);

                if (promotion == null)
                {
                    LogWarning($"⚠️ Promotion not found: {id}");
                    throw new KeyNotFoundException($"Promotion with ID '{id}' not found");
                }

                // Map to DTO
                var dto = _mapper.Map<PromotionDto>(promotion);

                LogInfo($"✅ Retrieved promotion: {promotion.PromotionName}");
                return dto;

            }, "GetPromotionByIdAsync", "Promotion retrieved successfully");
        }

        /// <summary>
        /// Update a promotion
        /// </summary>
        public async Task<ApiResponse<PromotionDto>> UpdatePromotionAsync(Guid id, UpdatePromotionDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"✏️ Updating promotion: {id}");

                // Validate promotion ID
                ValidateId(id, nameof(id));

                // Retrieve existing promotion
                var existingPromotion = await _unitOfWork.Promotions.GetByIdAsync(id);

                if (existingPromotion == null)
                {
                    LogWarning($"⚠️ Promotion not found: {id}");
                    throw new KeyNotFoundException($"Promotion with ID '{id}' not found");
                }

                // Validate discount value if being updated
                if (dto.DiscountValue.HasValue && dto.DiscountValue.Value <= 0)
                {
                    LogWarning($"⚠️ Invalid discount value: {dto.DiscountValue} <= 0");
                    throw new ArgumentException("DiscountValue must be greater than 0", nameof(dto.DiscountValue));
                }

                // Validate date range if dates are being updated
                if (dto.StartDate.HasValue || dto.EndDate.HasValue)
                {
                    var startDate = dto.StartDate ?? existingPromotion.StartDate;
                    var endDate = dto.EndDate ?? existingPromotion.EndDate;

                    if (endDate <= startDate)
                    {
                        LogWarning($"⚠️ Invalid date range: EndDate ({endDate}) <= StartDate ({startDate})");
                        throw new ArgumentException("EndDate must be greater than StartDate", nameof(dto.EndDate));
                    }
                }

                // Update only provided fields
                _mapper.Map(dto, existingPromotion);

                // Update timestamp
                existingPromotion.UpdatedAt = DateTime.UtcNow;

                // Save changes
                _unitOfWork.Promotions.Update(existingPromotion);
                await _unitOfWork.SaveChangesAsync();

                // Map to DTO
                var result = _mapper.Map<PromotionDto>(existingPromotion);

                LogInfo($"✅ Updated promotion: {existingPromotion.Id} - {existingPromotion.PromotionName}");

                // Invalidate cache
                await InvalidateCacheByPrefixAsync($"promotions:{existingPromotion.CampaignId}");

                return result;

            }, "UpdatePromotionAsync", "Promotion updated successfully");
        }

        /// <summary>
        /// Delete a promotion (soft delete)
        /// </summary>
        public async Task<ApiResponse<bool>> DeletePromotionAsync(Guid id)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"🗑️ Deleting promotion: {id}");

                // Validate promotion ID
                ValidateId(id, nameof(id));

                // Retrieve promotion
                var promotion = await _unitOfWork.Promotions.GetByIdAsync(id);

                if (promotion == null)
                {
                    LogWarning($"⚠️ Promotion not found: {id}");
                    throw new KeyNotFoundException($"Promotion with ID '{id}' not found");
                }

                // Soft delete
                promotion.IsDeleted = true;
                promotion.DeletedAt = DateTime.UtcNow;

                // Save changes
                _unitOfWork.Promotions.Update(promotion);
                await _unitOfWork.SaveChangesAsync();

                LogInfo($"✅ Deleted promotion: {id} - {promotion.PromotionName}");

                // Invalidate cache
                await InvalidateCacheByPrefixAsync($"promotions:{promotion.CampaignId}");

                return true;

            }, "DeletePromotionAsync", "Promotion deleted successfully");
        }

        /// <summary>
        /// Link products to a promotion
        /// </summary>
        public async Task<ApiResponse<bool>> LinkProductsAsync(Guid promotionId, LinkProductsDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"🔗 Linking products to promotion: {promotionId}");

                // Validate promotion ID
                ValidateId(promotionId, nameof(promotionId));

                // Validate product IDs
                if (dto.ProductIds == null || dto.ProductIds.Count == 0)
                {
                    throw new ArgumentException("At least one product ID must be provided", nameof(dto.ProductIds));
                }

                // Retrieve promotion
                var promotion = await _unitOfWork.Promotions.GetByIdAsync(promotionId);

                if (promotion == null)
                {
                    LogWarning($"⚠️ Promotion not found: {promotionId}");
                    throw new KeyNotFoundException($"Promotion with ID '{promotionId}' not found");
                }

                // Verify all products exist
                foreach (var productId in dto.ProductIds)
                {
                    var productExists = await _unitOfWork.Products.GetByIdAsync(productId);
                    if (productExists == null)
                    {
                        LogWarning($"⚠️ Product not found: {productId}");
                        throw new KeyNotFoundException($"Product with ID '{productId}' not found");
                    }
                }

                // Link products
                foreach (var productId in dto.ProductIds)
                {
                    // Check if already linked
                    var existingLink = promotion.PromotionProducts
                        .FirstOrDefault(pp => pp.ProductId == productId && !pp.IsDeleted);

                    if (existingLink == null)
                    {
                        var promotionProduct = new PromotionProduct
                        {
                            Id = Guid.NewGuid(),
                            PromotionId = promotionId,
                            ProductId = productId,
                            DiscountOverride = dto.DiscountOverride,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedAt = DateTime.UtcNow
                        };

                        promotion.PromotionProducts.Add(promotionProduct);
                    }
                }

                // Save changes
                _unitOfWork.Promotions.Update(promotion);
                await _unitOfWork.SaveChangesAsync();

                LogInfo($"✅ Linked {dto.ProductIds.Count} products to promotion: {promotionId}");

                // Invalidate cache
                await InvalidateCacheByPrefixAsync($"promotion:{promotionId}");

                return true;

            }, "LinkProductsAsync", "Products linked successfully");
        }

        /// <summary>
        /// Get linked products for a promotion
        /// </summary>
        public async Task<ApiResponse<List<ProductListDto>>> GetLinkedProductsAsync(Guid promotionId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"📦 Getting linked products for promotion: {promotionId}");

                // Validate promotion ID
                ValidateId(promotionId, nameof(promotionId));

                // Retrieve promotion with products
                var promotion = await _unitOfWork.Promotions.GetPromotionWithDetailsAsync(promotionId);

                if (promotion == null)
                {
                    LogWarning($"⚠️ Promotion not found: {promotionId}");
                    throw new KeyNotFoundException($"Promotion with ID '{promotionId}' not found");
                }

                // Get linked products (non-deleted)
                var linkedProducts = promotion.PromotionProducts
                    .Where(pp => !pp.IsDeleted)
                    .Select(pp => pp.Product)
                    .ToList();

                // Map to DTOs
                var dtos = _mapper.Map<List<ProductListDto>>(linkedProducts);

                LogInfo($"✅ Retrieved {dtos.Count} linked products for promotion: {promotionId}");
                return dtos;

            }, "GetLinkedProductsAsync", "Linked products retrieved successfully");
        }

        /// <summary>
        /// Unlink products from a promotion
        /// </summary>
        public async Task<ApiResponse<bool>> UnlinkProductsAsync(Guid promotionId, List<Guid> productIds)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo($"🔓 Unlinking products from promotion: {promotionId}");

                // Validate promotion ID
                ValidateId(promotionId, nameof(promotionId));

                // Validate product IDs
                if (productIds == null || productIds.Count == 0)
                {
                    throw new ArgumentException("At least one product ID must be provided", nameof(productIds));
                }

                // Retrieve promotion
                var promotion = await _unitOfWork.Promotions.GetByIdAsync(promotionId);

                if (promotion == null)
                {
                    LogWarning($"⚠️ Promotion not found: {promotionId}");
                    throw new KeyNotFoundException($"Promotion with ID '{promotionId}' not found");
                }

                // Unlink products (soft delete)
                foreach (var productId in productIds)
                {
                    var promotionProduct = promotion.PromotionProducts
                        .FirstOrDefault(pp => pp.ProductId == productId && !pp.IsDeleted);

                    if (promotionProduct != null)
                    {
                        promotionProduct.IsDeleted = true;
                        promotionProduct.DeletedAt = DateTime.UtcNow;
                    }
                }

                // Save changes
                _unitOfWork.Promotions.Update(promotion);
                await _unitOfWork.SaveChangesAsync();

                LogInfo($"✅ Unlinked {productIds.Count} products from promotion: {promotionId}");

                // Invalidate cache
                await InvalidateCacheByPrefixAsync($"promotion:{promotionId}");

                return true;

            }, "UnlinkProductsAsync", "Products unlinked successfully");
        }
    }
}
