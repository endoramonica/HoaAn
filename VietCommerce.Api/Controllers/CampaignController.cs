using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Controllers;

/// <summary>
/// Campaign Management API Controller
/// 
/// Handles all campaign-related operations including CRUD and status management.
/// Campaigns are marketing initiatives with budgets, validity periods, and targeting rules.
/// 
/// **Key Features:**
/// - Create campaigns with budget and date validation
/// - Retrieve campaigns with pagination and filtering by status/date range
/// - Update campaign details
/// - Delete campaigns (soft delete)
/// - Manage campaign status lifecycle (DRAFT → ACTIVE → COMPLETED)
/// 
/// **Requirements:** 1.1, 1.2, 1.3, 1.4, 1.5, 8.1, 8.2, 8.3, 8.4
/// 
/// **Example Campaign Lifecycle:**
/// 1. Create campaign in DRAFT status
/// 2. Add promotions while in DRAFT
/// 3. Transition to ACTIVE when ready
/// 4. System automatically transitions to COMPLETED when end date is reached
/// 
/// **Authentication:** Requires JWT Bearer token with admin privileges
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Tags("Campaign Management")]
public class CampaignController : ControllerBase
{
    private readonly ICampaignService _campaignService;
    private readonly ILogger<CampaignController> _logger;

    public CampaignController(
        ICampaignService campaignService,
        ILogger<CampaignController> logger)
    {
        _campaignService = campaignService ?? throw new ArgumentNullException(nameof(campaignService));
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
    /// Helper method to get current store ID from claims
    /// </summary>
    private Guid? GetCurrentStoreId()
    {
        var storeIdClaim = User.FindFirst("StoreId")?.Value;
        return Guid.TryParse(storeIdClaim, out var storeId) ? storeId : null;
    }

    /// <summary>
    /// Create a new campaign
    /// 
    /// Creates a new marketing campaign with the specified details. The campaign is created in DRAFT status
    /// and can have promotions added to it. Once all details are configured, the campaign can be transitioned
    /// to ACTIVE status.
    /// 
    /// **Validation Rules:**
    /// - CampaignName: Required, max 255 characters
    /// - Budget: Must be >= 0
    /// - EndDate: Must be greater than StartDate
    /// - StartDate: Must be in the future or today
    /// 
    /// **Requirements:** 1.1, 1.2, 1.3
    /// 
    /// **Example Request:**
    /// ```json
    /// {
    ///   "campaignName": "Summer Sale 2025",
    ///   "description": "50% off all summer items",
    ///   "campaignType": 1,
    ///   "startDate": "2025-06-01T00:00:00Z",
    ///   "endDate": "2025-08-31T23:59:59Z",
    ///   "budget": 10000.00,
    ///   "targetingRules": "{\"pages\": [\"home\", \"products\"], \"frequency\": \"once-per-session\"}"
    /// }
    /// ```
    /// </summary>
    /// <param name="dto">Campaign creation data</param>
    /// <returns>Created campaign with 200 status</returns>
    /// <response code="200">Campaign created successfully with all fields persisted</response>
    /// <response code="400">Validation error (invalid dates, negative budget, missing required fields, etc.)</response>
    /// <response code="401">Unauthorized - missing or invalid JWT token</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignDto dto)
    {
        try
        {
            _logger.LogInformation($"📝 POST /api/v1/campaign - Creating campaign: {dto.CampaignName}");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized campaign creation attempt");
                return Unauthorized(ApiResponse<CampaignDto>.FailureResponse("User not authenticated"));
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(dto.CampaignName))
            {
                _logger.LogWarning("⚠️ Campaign name is required");
                return BadRequest(ApiResponse<CampaignDto>.FailureResponse(
                    "Campaign name is required",
                    new[] { "CampaignName is required" }));
            }

            // Validate date range (Requirement: 1.2)
            if (dto.EndDate <= dto.StartDate)
            {
                _logger.LogWarning($"⚠️ Invalid date range: EndDate ({dto.EndDate}) <= StartDate ({dto.StartDate})");
                return BadRequest(ApiResponse<CampaignDto>.FailureResponse(
                    "EndDate must be greater than StartDate",
                    new[] { "EndDate must be greater than StartDate" }));
            }

            // Validate budget (Requirement: 1.3)
            if (dto.Budget < 0)
            {
                _logger.LogWarning($"⚠️ Invalid budget: {dto.Budget} < 0");
                return BadRequest(ApiResponse<CampaignDto>.FailureResponse(
                    "Budget must be non-negative",
                    new[] { "Budget must be non-negative" }));
            }

            // Call service
            var result = await _campaignService.CreateCampaignAsync(dto);

            if (!result.Success)
            {
                _logger.LogWarning($"⚠️ Campaign creation failed: {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"✅ Campaign created successfully: {result.Data?.Id}");
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"⚠️ Validation error: {ex.Message}");
            return BadRequest(ApiResponse<CampaignDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error creating campaign: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<CampaignDto>.FailureResponse("An error occurred while creating the campaign"));
        }
    }

    /// <summary>
    /// Get campaigns with pagination and filtering
    /// 
    /// Retrieves a paginated list of campaigns with optional filtering by status, date range, and search term.
    /// Results can be sorted by any campaign field.
    /// 
    /// **Query Parameters:**
    /// - pageNumber: 1-based page index (default: 1)
    /// - pageSize: Items per page, 1-100 (default: 10)
    /// - status: Filter by CampaignStatus (DRAFT, ACTIVE, PAUSED, COMPLETED, ARCHIVED)
    /// - fromDate: Include campaigns with StartDate >= this date
    /// - toDate: Include campaigns with EndDate <= this date
    /// - searchTerm: Search campaigns by name (case-insensitive substring match)
    /// - sortBy: Field to sort by (default: CreatedAt)
    /// - isDescending: Sort order (default: true for newest first)
    /// 
    /// **Requirements:** 1.5
    /// 
    /// **Example Request:**
    /// ```
    /// GET /api/v1/campaigns?pageNumber=1&pageSize=10&status=1&fromDate=2025-01-01&sortBy=CreatedAt&isDescending=true
    /// ```
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1, min: 1)</param>
    /// <param name="pageSize">Page size (default: 10, range: 1-100)</param>
    /// <param name="status">Filter by campaign status (optional)</param>
    /// <param name="fromDate">Filter campaigns starting from this date (optional)</param>
    /// <param name="toDate">Filter campaigns ending before this date (optional)</param>
    /// <param name="searchTerm">Search by campaign name (optional, case-insensitive)</param>
    /// <param name="sortBy">Sort field (default: CreatedAt)</param>
    /// <param name="isDescending">Sort order (default: true for descending)</param>
    /// <returns>Paginated list of campaigns matching filters</returns>
    /// <response code="200">Campaigns retrieved successfully with pagination metadata</response>
    /// <response code="400">Invalid pagination parameters (page < 1, pageSize out of range, etc.)</response>
    /// <response code="401">Unauthorized - missing or invalid JWT token</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<CampaignDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<CampaignDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCampaigns(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] CampaignStatus? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string sortBy = "CreatedAt",
        [FromQuery] bool isDescending = true)
    {
        try
        {
            _logger.LogInformation($"📋 GET /api/v1/campaign - Page: {pageNumber}, Size: {pageSize}");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized campaign retrieval attempt");
                return Unauthorized(ApiResponse<PaginatedResult<CampaignDto>>.FailureResponse("User not authenticated"));
            }

            // Validate pagination parameters
            if (pageNumber < 1)
            {
                _logger.LogWarning("⚠️ Invalid page number");
                return BadRequest(ApiResponse<PaginatedResult<CampaignDto>>.FailureResponse(
                    "Page number must be greater than 0",
                    new[] { "PageNumber must be greater than 0" }));
            }

            if (pageSize < 1 || pageSize > 100)
            {
                _logger.LogWarning("⚠️ Invalid page size");
                return BadRequest(ApiResponse<PaginatedResult<CampaignDto>>.FailureResponse(
                    "Page size must be between 1 and 100",
                    new[] { "PageSize must be between 1 and 100" }));
            }

            // Build query DTO
            var query = new GetCampaignsQueryDto
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
            var result = await _campaignService.GetCampaignsAsync(query);

            if (!result.Success)
            {
                _logger.LogWarning($"⚠️ Campaign retrieval failed: {result.Message}");
                return BadRequest(result);
            }

            var itemCount = result.Data?.Items?.Count() ?? 0;
            _logger.LogInformation($"✅ Retrieved {itemCount} campaigns");
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"⚠️ Validation error: {ex.Message}");
            return BadRequest(ApiResponse<PaginatedResult<CampaignDto>>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error retrieving campaigns: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<PaginatedResult<CampaignDto>>.FailureResponse("An error occurred while retrieving campaigns"));
        }
    }

    /// <summary>
    /// Get campaign by ID
    /// 
    /// Retrieves detailed information about a specific campaign including all its properties,
    /// status, budget, and targeting rules.
    /// 
    /// **Example Request:**
    /// ```
    /// GET /api/v1/campaigns/550e8400-e29b-41d4-a716-446655440000
    /// ```
    /// </summary>
    /// <param name="id">Campaign ID (GUID format)</param>
    /// <returns>Campaign details with all properties</returns>
    /// <response code="200">Campaign retrieved successfully</response>
    /// <response code="404">Campaign not found with the specified ID</response>
    /// <response code="401">Unauthorized - missing or invalid JWT token</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCampaignById(Guid id)
    {
        try
        {
            _logger.LogInformation($"🔍 GET /api/v1/campaign/{id}");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized campaign retrieval attempt");
                return Unauthorized(ApiResponse<CampaignDto>.FailureResponse("User not authenticated"));
            }

            // Validate ID
            if (id == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<CampaignDto>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "Id is invalid" }));
            }

            // Call service
            var result = await _campaignService.GetCampaignByIdAsync(id);

            if (!result.Success)
            {
                _logger.LogWarning($"⚠️ Campaign not found: {id}");
                return NotFound(result);
            }

            _logger.LogInformation($"✅ Campaign retrieved: {result.Data?.CampaignName}");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Campaign not found: {ex.Message}");
            return NotFound(ApiResponse<CampaignDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error retrieving campaign: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<CampaignDto>.FailureResponse("An error occurred while retrieving the campaign"));
        }
    }

    /// <summary>
    /// Update a campaign
    /// 
    /// Updates one or more fields of an existing campaign. All fields are optional - only provided
    /// fields will be updated. Validation rules apply to any date or budget fields being updated.
    /// 
    /// **Validation Rules:**
    /// - If both StartDate and EndDate are provided: EndDate must be > StartDate
    /// - If Budget is provided: must be >= 0
    /// - Campaign must exist
    /// 
    /// **Requirements:** 1.4
    /// 
    /// **Example Request:**
    /// ```json
    /// {
    ///   "campaignName": "Updated Summer Sale",
    ///   "budget": 15000.00,
    ///   "endDate": "2025-09-30T23:59:59Z"
    /// }
    /// ```
    /// </summary>
    /// <param name="id">Campaign ID (GUID format)</param>
    /// <param name="dto">Campaign update data (all fields optional)</param>
    /// <returns>Updated campaign with new values</returns>
    /// <response code="200">Campaign updated successfully with all changes persisted</response>
    /// <response code="400">Validation error (invalid dates, negative budget, etc.)</response>
    /// <response code="404">Campaign not found with the specified ID</response>
    /// <response code="401">Unauthorized - missing or invalid JWT token</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateCampaign(Guid id, [FromBody] UpdateCampaignDto dto)
    {
        try
        {
            _logger.LogInformation($"✏️ PUT /api/v1/campaign/{id}");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized campaign update attempt");
                return Unauthorized(ApiResponse<CampaignDto>.FailureResponse("User not authenticated"));
            }

            // Validate ID
            if (id == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<CampaignDto>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "Id is invalid" }));
            }

            // Validate date range if dates are provided
            if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.EndDate <= dto.StartDate)
            {
                _logger.LogWarning($"⚠️ Invalid date range: EndDate <= StartDate");
                return BadRequest(ApiResponse<CampaignDto>.FailureResponse(
                    "EndDate must be greater than StartDate",
                    new[] { "EndDate must be greater than StartDate" }));
            }

            // Validate budget if provided
            if (dto.Budget.HasValue && dto.Budget.Value < 0)
            {
                _logger.LogWarning($"⚠️ Invalid budget: {dto.Budget} < 0");
                return BadRequest(ApiResponse<CampaignDto>.FailureResponse(
                    "Budget must be non-negative",
                    new[] { "Budget must be non-negative" }));
            }

            // Call service
            var result = await _campaignService.UpdateCampaignAsync(id, dto);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                {
                    _logger.LogWarning($"⚠️ Campaign not found: {id}");
                    return NotFound(result);
                }

                _logger.LogWarning($"⚠️ Campaign update failed: {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"✅ Campaign updated: {result.Data?.Id}");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Campaign not found: {ex.Message}");
            return NotFound(ApiResponse<CampaignDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"⚠️ Validation error: {ex.Message}");
            return BadRequest(ApiResponse<CampaignDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error updating campaign: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<CampaignDto>.FailureResponse("An error occurred while updating the campaign"));
        }
    }

    /// <summary>
    /// Delete a campaign
    /// 
    /// Deletes a campaign using soft delete (marks as deleted without removing from database).
    /// Deleted campaigns will not appear in list queries but can be recovered if needed.
    /// 
    /// **Note:** Deleting a campaign also soft-deletes all associated promotions and vouchers.
    /// 
    /// **Example Request:**
    /// ```
    /// DELETE /api/v1/campaigns/550e8400-e29b-41d4-a716-446655440000
    /// ```
    /// </summary>
    /// <param name="id">Campaign ID (GUID format)</param>
    /// <returns>Success status</returns>
    /// <response code="200">Campaign deleted successfully (soft delete)</response>
    /// <response code="404">Campaign not found with the specified ID</response>
    /// <response code="401">Unauthorized - missing or invalid JWT token</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteCampaign(Guid id)
    {
        try
        {
            _logger.LogInformation($"🗑️ DELETE /api/v1/campaign/{id}");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized campaign deletion attempt");
                return Unauthorized(ApiResponse<bool>.FailureResponse("User not authenticated"));
            }

            // Validate ID
            if (id == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "Id is invalid" }));
            }

            // Call service
            var result = await _campaignService.DeleteCampaignAsync(id);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                {
                    _logger.LogWarning($"⚠️ Campaign not found: {id}");
                    return NotFound(result);
                }

                _logger.LogWarning($"⚠️ Campaign deletion failed: {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"✅ Campaign deleted: {id}");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Campaign not found: {ex.Message}");
            return NotFound(ApiResponse<bool>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error deleting campaign: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<bool>.FailureResponse("An error occurred while deleting the campaign"));
        }
    }

    /// <summary>
    /// Change campaign status
    /// 
    /// Transitions a campaign to a new status. Status transitions follow a specific lifecycle:
    /// - DRAFT: Initial state, allows adding/removing promotions
    /// - ACTIVE: Campaign is live, no new promotions can be added
    /// - PAUSED: Campaign is temporarily paused
    /// - COMPLETED: Campaign end date has passed (automatic transition)
    /// - ARCHIVED: Campaign is archived for historical reference
    /// 
    /// **Validation Rules:**
    /// - DRAFT → ACTIVE: All required fields must be set (name, budget, dates, at least one promotion)
    /// - Cannot transition from COMPLETED or ARCHIVED
    /// - Status value must be valid enum value
    /// 
    /// **Status Enum Values:**
    /// - 0 = DRAFT
    /// - 1 = ACTIVE
    /// - 2 = PAUSED
    /// - 3 = COMPLETED
    /// - 4 = ARCHIVED
    /// 
    /// **Requirements:** 6.1, 6.2, 6.3, 6.4, 6.5
    /// 
    /// **Example Request:**
    /// ```json
    /// {
    ///   "newStatus": 1
    /// }
    /// ```
    /// </summary>
    /// <param name="id">Campaign ID (GUID format)</param>
    /// <param name="newStatus">New campaign status (0-4)</param>
    /// <returns>Updated campaign with new status</returns>
    /// <response code="200">Campaign status changed successfully</response>
    /// <response code="400">Invalid status value or validation error (missing required fields)</response>
    /// <response code="404">Campaign not found with the specified ID</response>
    /// <response code="409">Business rule violation (e.g., cannot activate incomplete campaign, invalid transition)</response>
    /// <response code="401">Unauthorized - missing or invalid JWT token</response>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<CampaignDto>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangeCampaignStatus(Guid id, [FromBody] ChangeCampaignStatusDto dto)
    {
        try
        {
            _logger.LogInformation($"🔄 PATCH /api/v1/campaign/{id}/status - New status: {dto.NewStatus}");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized campaign status change attempt");
                return Unauthorized(ApiResponse<CampaignDto>.FailureResponse("User not authenticated"));
            }

            // Validate ID
            if (id == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<CampaignDto>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "Id is invalid" }));
            }

            // Validate status value
            if (!Enum.IsDefined(typeof(CampaignStatus), dto.NewStatus))
            {
                _logger.LogWarning($"⚠️ Invalid campaign status: {dto.NewStatus}");
                return BadRequest(ApiResponse<CampaignDto>.FailureResponse(
                    $"Invalid campaign status: {dto.NewStatus}",
                    new[] { "NewStatus is invalid" }));
            }

            // Call service
            var result = await _campaignService.ChangeCampaignStatusAsync(id, dto.NewStatus);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                {
                    _logger.LogWarning($"⚠️ Campaign not found: {id}");
                    return NotFound(result);
                }

                if (result.Message.Contains("Cannot activate") || result.Message.Contains("missing required fields"))
                {
                    _logger.LogWarning($"⚠️ Business rule violation: {result.Message}");
                    return Conflict(result);
                }

                _logger.LogWarning($"⚠️ Status change failed: {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"✅ Campaign status changed: {result.Data?.Id} to {result.Data?.Status}");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Campaign not found: {ex.Message}");
            return NotFound(ApiResponse<CampaignDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"⚠️ Business rule violation: {ex.Message}");
            return Conflict(ApiResponse<CampaignDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"⚠️ Validation error: {ex.Message}");
            return BadRequest(ApiResponse<CampaignDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error changing campaign status: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<CampaignDto>.FailureResponse("An error occurred while changing the campaign status"));
        }
    }
}
