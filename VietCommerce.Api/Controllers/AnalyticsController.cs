using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Controllers;

/// <summary>
/// Campaign Analytics API Controller
/// 
/// Handles tracking and reporting of campaign performance metrics including impressions,
/// clicks, and redemptions. Provides comprehensive analytics for measuring campaign ROI.
/// 
/// **Key Features:**
/// - Track campaign impressions (when campaign is shown to users)
/// - Track campaign clicks (when users interact with campaign)
/// - Track voucher redemptions (when discounts are applied)
/// - Generate campaign statistics with calculated metrics
/// - Filter analytics by date range
/// 
/// **Metrics Calculated:**
/// - Impressions: Total times campaign was shown
/// - Clicks: Total times campaign was clicked
/// - Click-Through Rate (CTR): clicks / impressions × 100
/// - Redemptions: Total vouchers redeemed
/// - Redemption Rate: redemptions / clicks × 100
/// - Total Revenue: Sum of all discounts applied
/// - Conversion Rate: redemptions / impressions × 100
/// 
/// **Requirements:** 7.1, 7.2, 7.3, 7.4, 7.5, 8.1, 8.2, 8.3, 8.4
/// 
/// **Note:** Impression and click tracking endpoints are public (no authentication required)
/// to allow frontend tracking. Stats retrieval requires authentication.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Tags("Campaign Analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(
        IAnalyticsService analyticsService,
        ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
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
    /// Track a campaign impression (when campaign is shown to user)
    /// 
    /// Records an impression event when a campaign is displayed to a user. This endpoint is called
    /// by the frontend when the AdPopup component renders a campaign. Multiple impressions from the
    /// same session on the same page are tracked separately (frequency rules are enforced on frontend).
    /// 
    /// **Validation Rules:**
    /// - Campaign must exist and be ACTIVE
    /// - SessionId: Required, max 100 characters (typically browser session ID)
    /// - Page: Required, max 100 characters (e.g., 'home', 'products', 'checkout')
    /// 
    /// **Requirements:** 7.1
    /// 
    /// **Example Request:**
    /// ```json
    /// {
    ///   "sessionId": "sess_abc123def456",
    ///   "page": "home",
    ///   "timestamp": "2025-01-15T10:30:00Z"
    /// }
    /// ```
    /// 
    /// **Note:** This endpoint is public (no authentication required) to allow frontend tracking.
    /// </summary>
    /// <param name="campaignId">Campaign ID (GUID format)</param>
    /// <param name="dto">Impression tracking data (sessionId, page, timestamp)</param>
    /// <returns>Success status</returns>
    /// <response code="200">Impression tracked successfully</response>
    /// <response code="400">Validation error (missing sessionId, page, etc.)</response>
    /// <response code="404">Campaign not found with the specified ID</response>
    [HttpPost("{campaignId}/track-impression")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TrackImpression(Guid campaignId, [FromBody] TrackImpressionDto dto)
    {
        try
        {
            _logger.LogInformation($"📊 POST /api/v1/analytics/{campaignId}/track-impression");

            // Validate campaign ID
            if (campaignId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "CampaignId is invalid" }));
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(dto.SessionId))
            {
                _logger.LogWarning("⚠️ Session ID is required");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Session ID is required",
                    new[] { "SessionId is required" }));
            }

            if (string.IsNullOrWhiteSpace(dto.Page))
            {
                _logger.LogWarning("⚠️ Page is required");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Page is required",
                    new[] { "Page is required" }));
            }

            // Call service
            var result = await _analyticsService.TrackImpressionAsync(campaignId, dto);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                {
                    _logger.LogWarning($"⚠️ Campaign not found: {campaignId}");
                    return NotFound(result);
                }

                _logger.LogWarning($"⚠️ Impression tracking failed: {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"✅ Impression tracked for campaign: {campaignId}");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Campaign not found: {ex.Message}");
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
            _logger.LogError($"❌ Error tracking impression: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<bool>.FailureResponse("An error occurred while tracking the impression"));
        }
    }

    /// <summary>
    /// Track a campaign click (when user clicks on campaign)
    /// 
    /// Records a click event when a user interacts with a campaign (e.g., clicks the CTA button).
    /// This endpoint is called by the frontend when the user engages with the AdPopup component.
    /// 
    /// **Validation Rules:**
    /// - Campaign must exist and be ACTIVE
    /// - SessionId: Required, max 100 characters (typically browser session ID)
    /// - Page: Required, max 100 characters (e.g., 'home', 'products', 'checkout')
    /// 
    /// **Requirements:** 7.2
    /// 
    /// **Example Request:**
    /// ```json
    /// {
    ///   "sessionId": "sess_abc123def456",
    ///   "page": "home",
    ///   "timestamp": "2025-01-15T10:30:15Z"
    /// }
    /// ```
    /// 
    /// **Note:** This endpoint is public (no authentication required) to allow frontend tracking.
    /// </summary>
    /// <param name="campaignId">Campaign ID (GUID format)</param>
    /// <param name="dto">Click tracking data (sessionId, page, timestamp)</param>
    /// <returns>Success status</returns>
    /// <response code="200">Click tracked successfully</response>
    /// <response code="400">Validation error (missing sessionId, page, etc.)</response>
    /// <response code="404">Campaign not found with the specified ID</response>
    [HttpPost("{campaignId}/track-click")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> TrackClick(Guid campaignId, [FromBody] TrackClickDto dto)
    {
        try
        {
            _logger.LogInformation($"📊 POST /api/v1/analytics/{campaignId}/track-click");

            // Validate campaign ID
            if (campaignId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "CampaignId is invalid" }));
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(dto.SessionId))
            {
                _logger.LogWarning("⚠️ Session ID is required");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Session ID is required",
                    new[] { "SessionId is required" }));
            }

            if (string.IsNullOrWhiteSpace(dto.Page))
            {
                _logger.LogWarning("⚠️ Page is required");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Page is required",
                    new[] { "Page is required" }));
            }

            // Call service
            var result = await _analyticsService.TrackClickAsync(campaignId, dto);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                {
                    _logger.LogWarning($"⚠️ Campaign not found: {campaignId}");
                    return NotFound(result);
                }

                _logger.LogWarning($"⚠️ Click tracking failed: {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"✅ Click tracked for campaign: {campaignId}");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Campaign not found: {ex.Message}");
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
            _logger.LogError($"❌ Error tracking click: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<bool>.FailureResponse("An error occurred while tracking the click"));
        }
    }

    /// <summary>
    /// Get campaign statistics including impressions, clicks, and redemptions
    /// 
    /// Retrieves comprehensive campaign performance statistics including impressions, clicks,
    /// redemptions, and calculated metrics like CTR and conversion rate. Results can be filtered
    /// by date range to analyze performance over specific periods.
    /// 
    /// **Calculated Metrics:**
    /// - Impressions: Total times campaign was shown
    /// - Clicks: Total times campaign was clicked
    /// - Click-Through Rate (CTR): (clicks / impressions) × 100 %
    /// - Redemptions: Total vouchers redeemed from this campaign
    /// - Redemption Rate: (redemptions / clicks) × 100 %
    /// - Conversion Rate: (redemptions / impressions) × 100 %
    /// - Total Revenue: Sum of all discount amounts applied
    /// - Total Discount: Sum of all discount amounts
    /// 
    /// **Query Parameters:**
    /// - fromDate: Include events from this date onwards (optional, ISO 8601 format)
    /// - toDate: Include events up to this date (optional, ISO 8601 format)
    /// - If no dates provided: returns all-time statistics
    /// 
    /// **Requirements:** 7.4, 7.5
    /// 
    /// **Example Request:**
    /// ```
    /// GET /api/v1/analytics/550e8400-e29b-41d4-a716-446655440000/stats?fromDate=2025-01-01&toDate=2025-01-31
    /// ```
    /// 
    /// **Example Response:**
    /// ```json
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "campaignId": "550e8400-e29b-41d4-a716-446655440000",
    ///     "impressions": 5000,
    ///     "clicks": 250,
    ///     "clickThroughRate": 5.0,
    ///     "redemptions": 150,
    ///     "redemptionRate": 60.0,
    ///     "conversionRate": 3.0,
    ///     "totalRevenue": 7500000,
    ///     "totalDiscount": 7500000
    ///   }
    /// }
    /// ```
    /// </summary>
    /// <param name="campaignId">Campaign ID (GUID format)</param>
    /// <param name="fromDate">Start date for filtering statistics (optional, ISO 8601 format)</param>
    /// <param name="toDate">End date for filtering statistics (optional, ISO 8601 format)</param>
    /// <returns>Campaign statistics with calculated metrics</returns>
    /// <response code="200">Campaign statistics retrieved successfully</response>
    /// <response code="400">Validation error (invalid date format, etc.)</response>
    /// <response code="404">Campaign not found with the specified ID</response>
    [HttpGet("{campaignId}/stats")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<CampaignStatsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CampaignStatsDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CampaignStatsDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCampaignStats(
        Guid campaignId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            _logger.LogInformation($"📊 GET /api/v1/analytics/{campaignId}/stats");

            // Validate campaign ID
            if (campaignId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid campaign ID");
                return BadRequest(ApiResponse<CampaignStatsDto>.FailureResponse(
                    "Campaign ID cannot be empty",
                    new[] { "CampaignId is invalid" }));
            }

            // Build query DTO
            var query = new GetCampaignStatsQueryDto
            {
                FromDate = fromDate,
                ToDate = toDate
            };

            // Call service
            var result = await _analyticsService.GetCampaignStatsAsync(campaignId, query);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                {
                    _logger.LogWarning($"⚠️ Campaign not found: {campaignId}");
                    return NotFound(result);
                }

                _logger.LogWarning($"⚠️ Stats retrieval failed: {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"✅ Campaign stats retrieved: {campaignId}");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Campaign not found: {ex.Message}");
            return NotFound(ApiResponse<CampaignStatsDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"⚠️ Validation error: {ex.Message}");
            return BadRequest(ApiResponse<CampaignStatsDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error retrieving campaign stats: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<CampaignStatsDto>.FailureResponse("An error occurred while retrieving campaign statistics"));
        }
    }
}
