using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Controllers;

/// <summary>
/// Voucher Management API Controller
/// 
/// Handles all voucher-related operations including generation, validation, and application.
/// Vouchers are redeemable codes (coupons) that apply a promotion's discount to customer orders.
/// 
/// **Key Features:**
/// - Generate unique voucher codes in bulk
/// - Retrieve vouchers with pagination
/// - Delete vouchers (soft delete)
/// - Validate voucher codes before application
/// - Track voucher usage and redemptions
/// 
/// **Business Rules:**
/// - Voucher codes must be unique
/// - Voucher codes must have an expiry date in the future
/// - Voucher usage is tracked and limited by promotion settings
/// - Expired vouchers cannot be applied
/// - Vouchers can only be applied to valid promotions
/// 
/// **Requirements:** 3.1, 3.2, 3.3, 3.4, 3.5, 8.1, 8.2, 8.3, 8.4
/// 
/// **Authentication:** Requires JWT Bearer token with admin privileges (except validation)
/// </summary>
[ApiController]
[Route("api/v1/promotions/{promotionId}/[controller]")]
[Authorize]
[Tags("Voucher Management")]
public class VoucherController : ControllerBase
{
    private readonly IVoucherService _voucherService;
    private readonly ILogger<VoucherController> _logger;

    public VoucherController(
        IVoucherService voucherService,
        ILogger<VoucherController> logger)
    {
        _voucherService = voucherService ?? throw new ArgumentNullException(nameof(voucherService));
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
    /// Generate voucher codes for a promotion
    /// 
    /// Generates a batch of unique voucher codes linked to a promotion. Each code is unique and
    /// can be distributed to customers. Codes are generated with a prefix and random suffix for
    /// easy identification and distribution.
    /// 
    /// **Validation Rules:**
    /// - Promotion must exist
    /// - Quantity: Must be > 0 (recommended: 1-10000 per request)
    /// - ExpiryDate: Must be in the future
    /// - Prefix: Optional, max 10 characters (default: auto-generated)
    /// 
    /// **Code Format:**
    /// - Format: [PREFIX]-[RANDOM_SUFFIX]
    /// - Example: SUMMER2025-A7K9M2X1
    /// - All codes are uppercase
    /// 
    /// **Requirements:** 3.1
    /// 
    /// **Example Request:**
    /// ```json
    /// {
    ///   "quantity": 100,
    ///   "expiryDate": "2025-12-31T23:59:59Z",
    ///   "prefix": "SUMMER25"
    /// }
    /// ```
    /// </summary>
    /// <param name="promotionId">Promotion ID (GUID format)</param>
    /// <param name="dto">Voucher generation parameters</param>
    /// <returns>Generated voucher codes with metadata</returns>
    /// <response code="200">Vouchers generated successfully with list of codes</response>
    /// <response code="400">Validation error (invalid quantity, expiry date in past, etc.)</response>
    /// <response code="404">Promotion not found with the specified ID</response>
    /// <response code="401">Unauthorized - missing or invalid JWT token</response>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(ApiResponse<GenerateVouchersResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<GenerateVouchersResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<GenerateVouchersResultDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GenerateVouchers(Guid promotionId, [FromBody] GenerateVouchersDto dto)
    {
        try
        {
            _logger.LogInformation($"📝 POST /api/v1/promotions/{promotionId}/vouchers/generate - Generating {dto.Quantity} vouchers");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized voucher generation attempt");
                return Unauthorized(ApiResponse<GenerateVouchersResultDto>.FailureResponse("User not authenticated"));
            }

            // Validate promotion ID
            if (promotionId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid promotion ID");
                return BadRequest(ApiResponse<GenerateVouchersResultDto>.FailureResponse(
                    "Promotion ID cannot be empty",
                    new[] { "PromotionId is invalid" }));
            }

            // Validate input
            if (dto.Quantity <= 0)
            {
                _logger.LogWarning($"⚠️ Invalid quantity: {dto.Quantity} <= 0");
                return BadRequest(ApiResponse<GenerateVouchersResultDto>.FailureResponse(
                    "Quantity must be greater than 0",
                    new[] { "Quantity must be greater than 0" }));
            }

            // Validate expiry date (Requirement: 3.1)
            if (dto.ExpiryDate <= DateTime.UtcNow)
            {
                _logger.LogWarning($"⚠️ Invalid expiry date: {dto.ExpiryDate} <= now");
                return BadRequest(ApiResponse<GenerateVouchersResultDto>.FailureResponse(
                    "ExpiryDate must be in the future",
                    new[] { "ExpiryDate must be in the future" }));
            }

            // Call service
            var result = await _voucherService.GenerateVouchersAsync(promotionId, dto);

            if (!result.Success)
            {
                _logger.LogWarning($"⚠️ Voucher generation failed: {result.Message}");
                
                if (result.Message.Contains("not found"))
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }

            _logger.LogInformation($"✅ Vouchers generated successfully: {result.Data?.GeneratedCount} codes");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Promotion not found: {ex.Message}");
            return NotFound(ApiResponse<GenerateVouchersResultDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"⚠️ Validation error: {ex.Message}");
            return BadRequest(ApiResponse<GenerateVouchersResultDto>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error generating vouchers: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<GenerateVouchersResultDto>.FailureResponse("An error occurred while generating vouchers"));
        }
    }

    /// <summary>
    /// Get vouchers for a promotion with pagination
    /// 
    /// Retrieves a paginated list of all vouchers generated for a specific promotion.
    /// Includes usage statistics and expiry information for each voucher.
    /// 
    /// **Query Parameters:**
    /// - pageNumber: 1-based page index (default: 1)
    /// - pageSize: Items per page, 1-100 (default: 10)
    /// 
    /// **Requirements:** 3.1
    /// 
    /// **Example Request:**
    /// ```
    /// GET /api/v1/promotions/550e8400-e29b-41d4-a716-446655440000/vouchers?pageNumber=1&pageSize=20
    /// ```
    /// </summary>
    /// <param name="promotionId">Promotion ID (GUID format)</param>
    /// <param name="pageNumber">Page number (default: 1, min: 1)</param>
    /// <param name="pageSize">Page size (default: 10, range: 1-100)</param>
    /// <returns>Paginated list of vouchers with usage statistics</returns>
    /// <response code="200">Vouchers retrieved successfully with pagination metadata</response>
    /// <response code="400">Invalid pagination parameters (page < 1, pageSize out of range, etc.)</response>
    /// <response code="404">Promotion not found with the specified ID</response>
    /// <response code="401">Unauthorized - missing or invalid JWT token</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<VoucherDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<VoucherDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<VoucherDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetVouchers(
        Guid promotionId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            _logger.LogInformation($"📋 GET /api/v1/promotions/{promotionId}/vouchers - Page: {pageNumber}, Size: {pageSize}");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized voucher retrieval attempt");
                return Unauthorized(ApiResponse<PaginatedResult<VoucherDto>>.FailureResponse("User not authenticated"));
            }

            // Validate promotion ID
            if (promotionId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid promotion ID");
                return BadRequest(ApiResponse<PaginatedResult<VoucherDto>>.FailureResponse(
                    "Promotion ID cannot be empty",
                    new[] { "PromotionId is invalid" }));
            }

            // Validate pagination parameters
            if (pageNumber < 1)
            {
                _logger.LogWarning("⚠️ Invalid page number");
                return BadRequest(ApiResponse<PaginatedResult<VoucherDto>>.FailureResponse(
                    "Page number must be greater than 0",
                    new[] { "PageNumber must be greater than 0" }));
            }

            if (pageSize < 1 || pageSize > 100)
            {
                _logger.LogWarning("⚠️ Invalid page size");
                return BadRequest(ApiResponse<PaginatedResult<VoucherDto>>.FailureResponse(
                    "Page size must be between 1 and 100",
                    new[] { "PageSize must be between 1 and 100" }));
            }

            // Call service
            var result = await _voucherService.GetVouchersForPromotionAsync(promotionId, pageNumber, pageSize);

            if (!result.Success)
            {
                _logger.LogWarning($"⚠️ Voucher retrieval failed: {result.Message}");
                
                if (result.Message.Contains("not found"))
                {
                    return NotFound(result);
                }

                return BadRequest(result);
            }

            var itemCount = result.Data?.Items?.Count() ?? 0;
            _logger.LogInformation($"✅ Retrieved {itemCount} vouchers");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Promotion not found: {ex.Message}");
            return NotFound(ApiResponse<PaginatedResult<VoucherDto>>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning($"⚠️ Validation error: {ex.Message}");
            return BadRequest(ApiResponse<PaginatedResult<VoucherDto>>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error retrieving vouchers: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<PaginatedResult<VoucherDto>>.FailureResponse("An error occurred while retrieving vouchers"));
        }
    }

    /// <summary>
    /// Delete a voucher (soft delete)
    /// </summary>
    /// <param name="promotionId">Promotion ID</param>
    /// <param name="voucherId">Voucher ID</param>
    /// <returns>Success status</returns>
    /// <response code="200">Voucher deleted successfully</response>
    /// <response code="404">Voucher or promotion not found</response>
    /// <response code="401">Unauthorized</response>
    [HttpDelete("{voucherId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteVoucher(Guid promotionId, Guid voucherId)
    {
        try
        {
            _logger.LogInformation($"🗑️ DELETE /api/v1/promotions/{promotionId}/vouchers/{voucherId}");

            // Validate authentication
            if (!GetCurrentUserId().HasValue)
            {
                _logger.LogWarning("⚠️ Unauthorized voucher deletion attempt");
                return Unauthorized(ApiResponse<bool>.FailureResponse("User not authenticated"));
            }

            // Validate IDs
            if (promotionId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid promotion ID");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Promotion ID cannot be empty",
                    new[] { "PromotionId is invalid" }));
            }

            if (voucherId == Guid.Empty)
            {
                _logger.LogWarning("⚠️ Invalid voucher ID");
                return BadRequest(ApiResponse<bool>.FailureResponse(
                    "Voucher ID cannot be empty",
                    new[] { "VoucherId is invalid" }));
            }

            // Call service
            var result = await _voucherService.DeleteVoucherAsync(voucherId);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                {
                    _logger.LogWarning($"⚠️ Voucher not found: {voucherId}");
                    return NotFound(result);
                }

                _logger.LogWarning($"⚠️ Voucher deletion failed: {result.Message}");
                return BadRequest(result);
            }

            _logger.LogInformation($"✅ Voucher deleted: {voucherId}");
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"⚠️ Voucher not found: {ex.Message}");
            return NotFound(ApiResponse<bool>.FailureResponse(
                ex.Message,
                new[] { ex.Message }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Error deleting voucher: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<bool>.FailureResponse("An error occurred while deleting the voucher"));
        }
    }
}
