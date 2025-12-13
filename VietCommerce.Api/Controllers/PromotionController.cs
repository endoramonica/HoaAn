using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Controllers;

/// <summary>
/// Promotion Management API Controller
/// 
/// Handles all promotion-related operations including CRUD and product linking.
/// Promotions are discount offers linked to campaigns and can only be added to DRAFT campaigns.
/// 
/// **Key Features:**
/// - Create promotions with discount value and validity period
/// - Retrieve promotions with pagination and filtering
/// - Update promotion details
/// - Delete promotions (soft delete)
/// - Link/unlink products to promotions for targeted discounts
/// 
/// **Business Rules:**
/// - Promotions can only be created for DRAFT campaigns
/// - Cannot add promotions to ACTIVE, PAUSED, COMPLETED, or ARCHIVED campaigns
/// - Discount value must be > 0
/// - EndDate must be > StartDate
/// 
/// **Requirements:** 2.1, 2.2, 2.3, 2.4, 2.5, 8.1, 8.2, 8.3, 8.4
/// 
/// **Authentication:** Requires JWT Bearer token with admin privileges
/// </summary>
[ApiController]
[Route("api/v1/campaigns/{campaignId}/[controller]")]
[Authorize]
[Tags("Promotion Management")]
public class PromotionController : ControllerBase
{
    private readonly IPromotionService _promotionService;
    private readonly ILogger<PromotionController> _logger;

    public PromotionController(
        IPromotionService promotionService,
        ILogger<PromotionController> logger)
    {
        _promotionService = promotionService ?? throw new ArgumentNullException(nameof(promotionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Helper method to get current user ID from JWT claims
    /// </summary>
    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    /// <summary>
    /// Create a new promotion for a campaign
    /// 
    /// Creates a new promotion linked to a campaign. Promotions can only be created for campaigns
    /// in DRAFT status. Once a campaign transitions to ACTIVE, no new promotions can be added.
    /// 
    /// **Validation Rules:**
    /// - Campaign must exist and be in DRAFT status
    /// - PromotionName: Required, max 255 characters
    /// - DiscountValue: Must be > 0
    /// - DiscountType: Must be 'percentage' or 'fixed'
    /// - EndDate: Must be > StartDate
    /// - MinOrderAmount: Optional, must be >= 0 if provided
    /// - UsageLimit: Optional, must be > 0 if provided
    /// 
    /// **Requirements:** 2.1, 2.2, 2.3, 2.4
    /// 
    /// **Example Request:**
    /// ```json
    /// {
    ///   "promotionName": "50% Off Summer Items",
    ///   "promotionType": 1,
    ///   "discountValue": 50,
    ///   "discountType": "percentage",
    ///   "startDate": "2025-06-01T00:00:00Z",
    ///   "endDate": "2025-08-31T23:59:59Z",
    ///   "minOrderAmount": 100000,
    ///   "usageLimit": 1000
    /// }
    /// ```
    /// </summary>
    /// <param name="campaignId">Campaign ID (GUID format)</param>
    /// <param name="dto">Promotion creation data</param>
    /// <returns>Created promotion with all fields persisted</returns>
    /// <response code="200">Promotion created successfully</response>
    /// <response code="400">Validation error (invalid dates, negative discount, missing required fields, etc.)</response>
    /// <response code="404">Campaign not found with the specified ID</response>
    /// <response code="409">Business rule violation (campaign not in DRAFT status)</response>
    /// <response code="401">Unauthorized - missing or invalid JWT token</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PromotionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PromotionDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<PromotionDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<PromotionDto>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreatePromotion(Guid campaignId, [FromBody] CreatePromotionDto dto)
    {
        try
        {
            _logger.LogInformation($"📝 POST /api/v1/campaigns/{campaignId}/promotions - Creating promotion: {dto.PromotionName}");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized promotion creation attempt");
                return Unauthorized(ApiResponse<PromotionDto>.FailureResponse("User not authenticated"));
            }

            // Validate campaign ID
            if (campaignId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<PromotionDto>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "CampaignId is invalid" }));
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(dto.PromotionName))
            {
                _logger.LogWarning("⚠️ Promotion name is required");
                return BadRequest(ApiResponse<PromotionDto>.FailureResponse(
                    "Promotion name is required",
                    new[] { "PromotionName is required" }));
            }

            // Validate discount value (Requirement: 2.3)
            if (dto.DiscountValue <= 0)
            {
                _logger.LogWarning($"⚠️ Invalid discount value: {dto.DiscountValue} <= 0");
                return BadRequest(ApiResponse<PromotionDto>.FailureResponse(
                    "DiscountValue must be greater than 0",
                    new[] { "DiscountValue must be greater than 0" }));
            }

            // Validate date range (Requirement: 2.4)
            if (dto.EndDate <= dto.StartDate)
            {
                _logger.LogWarning($"⚠️ Invalid date range: EndDate ({dto.EndDate}) <= StartDate ({dto.StartDate})");
                return BadRequest(ApiResponse<PromotionDto>.FailureResponse(
                    "EndDate must be greater than StartDate",
                    new[] { "EndDate must be greater than StartDate" }));
            }

            // Call service
            var result = await _promotionService.CreatePromotionAsync(campaignId, dto);

            if (!result.Success)
            {
                _logger.LogWarning($"⚠️ Promotion creation failed: {result.Message}");
                
                // Check if it's a campaign not found error
                if (result.Message.Contains("not found"))
                {
                    return NotFound(result);
                }

                // Check if it's a business rule violation (campaign not DRAFT)
                if (result.Message.Contains("DRAFT"))
                {
                    return Conflict(result);
                }

                return BadRequest(result);
            }

            _logger.LogInformation($"✅ Promotion created successfully: {result.Data?.Id}");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Campaign not found: {ex.Message}");
            return NotFound(ApiResponse<PromotionDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"⚠️ Business rule violation: {ex.Message}");
            return Conflict(ApiResponse<PromotionDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"⚠️ Validation error: {ex.Message}");
            return BadRequest(ApiResponse<PromotionDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error creating promotion: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<PromotionDto>.FailureResponse("An error occurred while creating the promotion"));
        }
    }


    /// <summary>
    /// Get promotions for a campaign with pagination and filtering
    /// 
    /// Retrieves a paginated list of promotions for a specific campaign with optional filtering
    /// by status, date range, and search term. Results can be sorted by any promotion field.
    /// 
    /// **Query Parameters:**
    /// - pageNumber: 1-based page index (default: 1)
    /// - pageSize: Items per page, 1-100 (default: 10)
    /// - status: Filter by PromotionStatus (ACTIVE, INACTIVE, EXPIRED)
    /// - fromDate: Include promotions with StartDate >= this date
    /// - toDate: Include promotions with EndDate <= this date
    /// - searchTerm: Search promotions by name (case-insensitive substring match)
    /// - sortBy: Field to sort by (default: CreatedAt)
    /// - isDescending: Sort order (default: true for newest first)
    /// 
    /// **Requirements:** 2.5
    /// 
    /// **Example Request:**
    /// ```
    /// GET /api/v1/campaigns/550e8400-e29b-41d4-a716-446655440000/promotions?pageNumber=1&pageSize=10&status=0
    /// ```
    /// </summary>
    /// <param name="campaignId">Campaign ID (GUID format)</param>
    /// <param name="pageNumber">Page number (default: 1, min: 1)</param>
    /// <param name="pageSize">Page size (default: 10, range: 1-100)</param>
    /// <param name="status">Filter by promotion status (optional)</param>
    /// <param name="fromDate">Filter promotions starting from this date (optional)</param>
    /// <param name="toDate">Filter promotions ending before this date (optional)</param>
    /// <param name="searchTerm">Search by promotion name (optional, case-insensitive)</param>
    /// <param name="sortBy">Sort field (default: CreatedAt)</param>
    /// <param name="isDescending">Sort order (default: true for descending)</param>
    /// <returns>Paginated list of promotions matching filters</returns>
    /// <response code="200">Promotions retrieved successfully with pagination metadata</response>
    /// <response code="400">Invalid pagination parameters (page < 1, pageSize out of range, etc.)</response>
    /// <response code="404">Campaign not found with the specified ID</response>
    /// <response code="401">Unauthorized - missing or invalid JWT token</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PromotionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PromotionDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<PromotionDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPromotions(
        Guid campaignId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] PromotionStatus? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] bool isDescending = true)
    {
        try
        {
            _logger.LogInformation($"📋 GET /api/v1/campaigns/{campaignId}/promotions - Page: {pageNumber}, Size: {pageSize}");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized promotion retrieval attempt");
                return Unauthorized(ApiResponse<PaginatedResult<PromotionDto>>.FailureResponse("User not authenticated"));
            }

            // Validate campaign ID
            if (campaignId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<PaginatedResult<PromotionDto>>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "CampaignId is invalid" }));
            }

            // Validate pagination parameters
            if (pageNumber < 1)
            {
                _logger.LogWarning("⚠️ Invalid page number");
                return BadRequest(ApiResponse<PaginatedResult<PromotionDto>>.FailureResponse(
                    "Page number must be greater than 0",
                    new[] { "PageNumber must be greater than 0" }));
            }

            if (pageSize < 1 || pageSize > 100)
            {
                _logger.LogWarning("⚠️ Invalid page size");
                return BadRequest(ApiResponse<PaginatedResult<PromotionDto>>.FailureResponse(
                    "Page size must be between 1 and 100",
                    new[] { "PageSize must be between 1 and 100" }));
            }

            // Build query DTO
            var query = new GetPromotionsQueryDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                IsDescending = isDescending
            };

            // Call service
            var result = await _promotionService.GetPromotionsByCampaignAsync(campaignId, query);

            if (!result.Success)
            {
                _logger.LogWarning($"⚠️ Promotion retrieval failed: {result.Message}");
                
                if (result.Message.Contains("not found"))
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }

            var itemCount = result.Data?.Items?.Count() ?? 0;
            _logger.LogInformation($"✅ Retrieved {itemCount} promotions");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Campaign not found: {ex.Message}");
            return NotFound(ApiResponse<PaginatedResult<PromotionDto>>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"⚠️ Validation error: {ex.Message}");
            return BadRequest(ApiResponse<PaginatedResult<PromotionDto>>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error retrieving promotions: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<PaginatedResult<PromotionDto>>.FailureResponse("An error occurred while retrieving promotions"));
        }
    }

    /// <summary>
    /// Get promotion by ID
    /// </summary>
    /// <param name="campaignId">Campaign ID</param>
    /// <param name="id">Promotion ID</param>
    /// <returns>Promotion details</returns>
    /// <response code="200">Promotion retrieved successfully</response>
    /// <response code="404">Promotion or campaign not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PromotionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PromotionDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPromotionById(Guid campaignId, Guid id)
    {
        try
        {
            _logger.LogInformation($"🔍 GET /api/v1/campaigns/{campaignId}/promotions/{id}");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized promotion retrieval attempt");
                return Unauthorized(ApiResponse<PromotionDto>.FailureResponse("User not authenticated"));
            }

            // Validate IDs
            if (campaignId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<PromotionDto>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "CampaignId is invalid" }));
            }

            if (id == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid promotion ID");
                return BadRequest(ApiResponse<PromotionDto>.FailureResponse(
                    "Promotion ID cannot be empty",
                    new[] { "Id is invalid" }));
            }

            // Call service
            var result = await _promotionService.GetPromotionByIdAsync(id);

            if (!result.Success)
            {
                _logger.LogWarning($"⚠️ Promotion not found: {id}");
                return NotFound(result);
            }

            _logger.LogInformation($"✅ Promotion retrieved: {result.Data?.PromotionName}");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Promotion not found: {ex.Message}");
            return NotFound(ApiResponse<PromotionDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error retrieving promotion: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<PromotionDto>.FailureResponse("An error occurred while retrieving the promotion"));
        }
    }


    /// <summary>
    /// Update a promotion
    /// </summary>
    /// <param name="campaignId">Campaign ID</param>
    /// <param name="id">Promotion ID</param>
    /// <param name="dto">Promotion update data</param>
    /// <returns>Updated promotion</returns>
    /// <response code="200">Promotion updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="404">Promotion or campaign not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PromotionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PromotionDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<PromotionDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdatePromotion(Guid campaignId, Guid id, [FromBody] UpdatePromotionDto dto)
    {
        try
        {
            _logger.LogInformation($"✏️ PUT /api/v1/campaigns/{campaignId}/promotions/{id}");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized promotion update attempt");
                return Unauthorized(ApiResponse<PromotionDto>.FailureResponse("User not authenticated"));
            }

            // Validate IDs
            if (campaignId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<PromotionDto>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "CampaignId is invalid" }));
            }

            if (id == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid promotion ID");
                return BadRequest(ApiResponse<PromotionDto>.FailureResponse(
                    "Promotion ID cannot be empty",
                    new[] { "Id is invalid" }));
            }

            // Validate discount value if being updated
            if (dto.DiscountValue.HasValue && dto.DiscountValue.Value <= 0)
            {
                _logger.LogWarning($"⚠️ Invalid discount value: {dto.DiscountValue} <= 0");
                return BadRequest(ApiResponse<PromotionDto>.FailureResponse(
                    "DiscountValue must be greater than 0",
                    new[] { "DiscountValue must be greater than 0" }));
            }

            // Validate date range if dates are provided
            if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.EndDate <= dto.StartDate)
            {
                _logger.LogWarning($"⚠️ Invalid date range: EndDate <= StartDate");
                return BadRequest(ApiResponse<PromotionDto>.FailureResponse(
                    "EndDate must be greater than StartDate",
                    new[] { "EndDate must be greater than StartDate" }));
            }

            // Call service
            var result = await _promotionService.UpdatePromotionAsync(id, dto);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                {
                    _logger.LogWarning($"⚠️ Promotion not found: {id}");
                    return NotFound(result);
                }

                _logger.LogWarning($"⚠️ Promotion update failed: {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"✅ Promotion updated: {result.Data?.Id}");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Promotion not found: {ex.Message}");
            return NotFound(ApiResponse<PromotionDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"⚠️ Validation error: {ex.Message}");
            return BadRequest(ApiResponse<PromotionDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error updating promotion: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<PromotionDto>.FailureResponse("An error occurred while updating the promotion"));
        }
    }

    /// <summary>
    /// Delete a promotion
    /// </summary>
    /// <param name="campaignId">Campaign ID</param>
    /// <param name="id">Promotion ID</param>
    /// <returns>Success status</returns>
    /// <response code="200">Promotion deleted successfully</response>
    /// <response code="404">Promotion or campaign not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeletePromotion(Guid campaignId, Guid id)
    {
        try
        {
            _logger.LogInformation($"🗑️ DELETE /api/v1/campaigns/{campaignId}/promotions/{id}");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized promotion deletion attempt");
                return Unauthorized(ApiResponse<bool>.FailureResponse("User not authenticated"));
            }

            // Validate IDs
            if (campaignId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "CampaignId is invalid" }));
            }

            if (id == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid promotion ID");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Promotion ID cannot be empty",
                    new[] { "Id is invalid" }));
            }

            // Call service
            var result = await _promotionService.DeletePromotionAsync(id);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                {
                    _logger.LogWarning($"⚠️ Promotion not found: {id}");
                    return NotFound(result);
                }

                _logger.LogWarning($"⚠️ Promotion deletion failed: {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"✅ Promotion deleted: {id}");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Promotion not found: {ex.Message}");
            return NotFound(ApiResponse<bool>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error deleting promotion: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<bool>.FailureResponse("An error occurred while deleting the promotion"));
        }
    }

    /// <summary>
    /// Link products to a promotion
    /// </summary>
    /// <param name="campaignId">Campaign ID</param>
    /// <param name="promotionId">Promotion ID</param>
    /// <param name="dto">Product IDs to link</param>
    /// <returns>Success status</returns>
    /// <response code="200">Products linked successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="404">Promotion or product not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpPost("{promotionId}/products")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LinkProducts(Guid campaignId, Guid promotionId, [FromBody] LinkProductsDto dto)
    {
        try
        {
            _logger.LogInformation($"🔗 POST /api/v1/campaigns/{campaignId}/promotions/{promotionId}/products");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized product linking attempt");
                return Unauthorized(ApiResponse<bool>.FailureResponse("User not authenticated"));
            }

            // Validate IDs
            if (campaignId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "CampaignId is invalid" }));
            }

            if (promotionId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid promotion ID");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Promotion ID cannot be empty",
                    new[] { "PromotionId is invalid" }));
            }

            // Validate product IDs
            if (dto.ProductIds == null || dto.ProductIds.Count == 0)
            {
                _logger.LogWarning("⚠️ No product IDs provided");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "At least one product ID must be provided",
                    new[] { "ProductIds cannot be empty" }));
            }

            // Call service
            var result = await _promotionService.LinkProductsAsync(promotionId, dto);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                {
                    _logger.LogWarning($"⚠️ Resource not found: {result.Message}");
                    return NotFound(result);
                }

                _logger.LogWarning($"⚠️ Product linking failed: {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"✅ Products linked to promotion: {promotionId}");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Resource not found: {ex.Message}");
            return NotFound(ApiResponse<bool>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"⚠️ Validation error: {ex.Message}");
            return BadRequest(ApiResponse<bool>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error linking products: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<bool>.FailureResponse("An error occurred while linking products"));
        }
    }

    /// <summary>
    /// Get linked products for a promotion
    /// </summary>
    /// <param name="campaignId">Campaign ID</param>
    /// <param name="promotionId">Promotion ID</param>
    /// <returns>List of linked products</returns>
    /// <response code="200">Products retrieved successfully</response>
    /// <response code="404">Promotion not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpGet("{promotionId}/products")]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLinkedProducts(Guid campaignId, Guid promotionId)
    {
        try
        {
            _logger.LogInformation($"📦 GET /api/v1/campaigns/{campaignId}/promotions/{promotionId}/products");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized product retrieval attempt");
                return Unauthorized(ApiResponse<List<object>>.FailureResponse("User not authenticated"));
            }

            // Validate IDs
            if (campaignId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<List<object>>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "CampaignId is invalid" }));
            }

            if (promotionId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid promotion ID");
                return BadRequest(ApiResponse<List<object>>.FailureResponse(
                    "Promotion ID cannot be empty",
                    new[] { "PromotionId is invalid" }));
            }

            // Call service
            var result = await _promotionService.GetLinkedProductsAsync(promotionId);

            if (!result.Success)
            {
                _logger.LogWarning($"⚠️ Product retrieval failed: {result.Message}");
                return NotFound(result);
            }

            _logger.LogInformation($"✅ Retrieved {result.Data?.Count ?? 0} linked products");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Promotion not found: {ex.Message}");
            return NotFound(ApiResponse<List<object>>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error retrieving linked products: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<List<object>>.FailureResponse("An error occurred while retrieving linked products"));
        }
    }

    /// <summary>
    /// Unlink products from a promotion
    /// </summary>
    /// <param name="campaignId">Campaign ID</param>
    /// <param name="promotionId">Promotion ID</param>
    /// <param name="productIds">Product IDs to unlink</param>
    /// <returns>Success status</returns>
    /// <response code="200">Products unlinked successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="404">Promotion not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpDelete("{promotionId}/products")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnlinkProducts(Guid campaignId, Guid promotionId, [FromQuery] List<Guid> productIds)
    {
        try
        {
            _logger.LogInformation($"🔓 DELETE /api/v1/campaigns/{campaignId}/promotions/{promotionId}/products");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized product unlinking attempt");
                return Unauthorized(ApiResponse<bool>.FailureResponse("User not authenticated"));
            }

            // Validate IDs
            if (campaignId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "CampaignId is invalid" }));
            }

            if (promotionId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid promotion ID");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Promotion ID cannot be empty",
                    new[] { "PromotionId is invalid" }));
            }

            // Validate product IDs
            if (productIds == null || productIds.Count == 0)
            {
                _logger.LogWarning("⚠️ No product IDs provided");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "At least one product ID must be provided",
                    new[] { "ProductIds cannot be empty" }));
            }

            // Call service
            var result = await _promotionService.UnlinkProductsAsync(promotionId, productIds);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                {
                    _logger.LogWarning($"⚠️ Promotion not found: {result.Message}");
                    return NotFound(result);
                }

                _logger.LogWarning($"⚠️ Product unlinking failed: {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"✅ Products unlinked from promotion: {promotionId}");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Promotion not found: {ex.Message}");
            return NotFound(ApiResponse<bool>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"⚠️ Validation error: {ex.Message}");
            return BadRequest(ApiResponse<bool>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error unlinking products: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<bool>.FailureResponse("An error occurred while unlinking products"));
        }
    }
}
