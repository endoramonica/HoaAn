using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Logistics;
using VietCommerce.Core.Models;

namespace VietCommerce.AdminApi.Controllers;

/// <summary>
/// API Controller for managing stock transfers
/// Provides endpoints for CRUD operations, status updates, and inventory tracking
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Yêu cầu authentication cho tất cả endpoints
[Produces("application/json")]
public class StockTransfersController : ControllerBase
{
    private readonly IStockTransferService _stockTransferService;
    private readonly ILogger<StockTransfersController> _logger;

    public StockTransfersController(
        IStockTransferService stockTransferService,
        ILogger<StockTransfersController> logger)
    {
        _stockTransferService = stockTransferService ?? throw new ArgumentNullException(nameof(stockTransferService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region GET Endpoints

    /// <summary>
    /// Get paginated list of stock transfers with filtering
    /// </summary>
    /// <param name="pagination">Pagination parameters (page, pageSize, sortBy, sortDescending)</param>
    /// <param name="filters">Filter criteria (status, warehouse, dates, etc.)</param>
    /// <returns>Paginated list of stock transfers</returns>
    /// <response code="200">Returns the paginated stock transfer list</response>
    /// <response code="400">Invalid pagination or filter parameters</response>
    /// <response code="401">Unauthorized - Missing or invalid token</response>
    /// <response code="403">Forbidden - User lacks permission (lrm.view_stock_transfers)</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<StockTransferDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetStockTransfers(
        [FromQuery] PaginationParams pagination,
        [FromQuery] StockTransferFilters filters)
    {
        try
        {
            _logger.LogInformation("📋 GET /api/stocktransfers - Page: {Page}, PageSize: {PageSize}",
                pagination.Page, pagination.PageSize);

            var result = await _stockTransferService.GetStockTransfersAsync(pagination, filters);

            return Ok(ApiResponse<PaginatedResult<StockTransferDto>>.SuccessResponse(
                result,
                "Lấy danh sách phiếu chuyển kho thành công"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Invalid parameters: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("⚠️ Permission denied: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting stock transfers");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.FailureResponse("Đã xảy ra lỗi khi lấy danh sách phiếu chuyển kho"));
        }
    }

    /// <summary>
    /// Get stock transfer details by ID
    /// </summary>
    /// <param name="id">Stock transfer unique identifier</param>
    /// <returns>Stock transfer details with items and history</returns>
    /// <response code="200">Returns the stock transfer details</response>
    /// <response code="400">Invalid stock transfer ID format</response>
    /// <response code="401">Unauthorized - Missing or invalid token</response>
    /// <response code="403">Forbidden - User lacks permission</response>
    /// <response code="404">Stock transfer not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<StockTransferDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStockTransfer(Guid id)
    {
        try
        {
            _logger.LogInformation("🔍 GET /api/stocktransfers/{TransferId}", id);

            var transfer = await _stockTransferService.GetStockTransferByIdAsync(id);

            return Ok(ApiResponse<StockTransferDto>.SuccessResponse(
                transfer,
                "Lấy thông tin phiếu chuyển kho thành công"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Invalid ID: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            // Check if it's a "not found" or "permission denied" error
            if (ex.Message.Contains("Không tìm thấy") || ex.Message.Contains("not found"))
            {
                _logger.LogWarning("⚠️ Stock transfer not found: {TransferId}", id);
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
            }

            _logger.LogWarning("⚠️ Permission denied: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting stock transfer {TransferId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.FailureResponse("Đã xảy ra lỗi khi lấy thông tin phiếu chuyển kho"));
        }
    }

    /// <summary>
    /// Get stock transfer summary statistics
    /// </summary>
    /// <param name="filters">Optional filter criteria to calculate summary</param>
    /// <returns>Summary statistics (total, pending, in-transit, delivered, cancelled)</returns>
    /// <response code="200">Returns the summary statistics</response>
    /// <response code="400">Invalid filter parameters</response>
    /// <response code="401">Unauthorized - Missing or invalid token</response>
    /// <response code="403">Forbidden - User lacks permission</response>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<StockTransferSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetTransferSummary([FromQuery] StockTransferFilters? filters = null)
    {
        try
        {
            _logger.LogInformation("📊 GET /api/stocktransfers/summary");

            var summary = await _stockTransferService.GetTransferSummaryAsync(filters);

            return Ok(ApiResponse<StockTransferSummaryDto>.SuccessResponse(
                summary,
                "Lấy thống kê phiếu chuyển kho thành công"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Invalid parameters: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("⚠️ Permission denied: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting transfer summary");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.FailureResponse("Đã xảy ra lỗi khi lấy thống kê"));
        }
    }

    #endregion

    #region POST Endpoints

    /// <summary>
    /// Create a new stock transfer
    /// </summary>
    /// <param name="request">Stock transfer creation request with items</param>
    /// <returns>Created stock transfer details</returns>
    /// <response code="201">Stock transfer created successfully</response>
    /// <response code="400">Invalid request data or validation errors</response>
    /// <response code="401">Unauthorized - Missing or invalid token</response>
    /// <response code="403">Forbidden - User lacks permission (lrm.manage_stock_transfers)</response>
    /// <response code="409">Conflict - Business rule violation (e.g., same warehouse)</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StockTransferDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateStockTransfer([FromBody] CreateStockTransferRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("⚠️ Invalid model state for CreateStockTransfer");
                return BadRequest(ApiResponse<object>.FailureResponse(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray()));
            }

            _logger.LogInformation("➕ POST /api/stocktransfers - From: {From}, To: {To}",
                request.FromWarehouse, request.ToWarehouse);

            var transfer = await _stockTransferService.CreateStockTransferAsync(request);

            return CreatedAtAction(
                nameof(GetStockTransfer),
                new { id = transfer.Id },
                ApiResponse<StockTransferDto>.SuccessResponse(
                    transfer,
                    "Tạo phiếu chuyển kho thành công"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Validation error: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            // Check if it's a business rule violation or permission error
            if (ex.Message.Contains("phải khác nhau") ||
                ex.Message.Contains("không tồn tại") ||
                ex.Message.Contains("phải lớn hơn"))
            {
                _logger.LogWarning("⚠️ Business rule violation: {Message}", ex.Message);
                return Conflict(ApiResponse<object>.FailureResponse(ex.Message));
            }

            _logger.LogWarning("⚠️ Permission denied: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error creating stock transfer");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.FailureResponse("Đã xảy ra lỗi khi tạo phiếu chuyển kho"));
        }
    }

    #endregion

    #region PUT/PATCH Endpoints

    /// <summary>
    /// Update stock transfer status (approve, deliver, etc.)
    /// </summary>
    /// <param name="id">Stock transfer unique identifier</param>
    /// <param name="request">Status update request with optional received items</param>
    /// <returns>Updated stock transfer details</returns>
    /// <response code="200">Stock transfer status updated successfully</response>
    /// <response code="400">Invalid request data or validation errors</response>
    /// <response code="401">Unauthorized - Missing or invalid token</response>
    /// <response code="403">Forbidden - User lacks permission (lrm.manage_stock_transfers or lrm.approve_stock_transfers)</response>
    /// <response code="404">Stock transfer not found</response>
    /// <response code="409">Conflict - Invalid status transition</response>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<StockTransferDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStockTransferStatus(
        Guid id,
        [FromBody] UpdateStockTransferStatusRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("⚠️ Invalid model state for UpdateStockTransferStatus");
                return BadRequest(ApiResponse<object>.FailureResponse(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray()));
            }

            _logger.LogInformation("✏️ PUT /api/stocktransfers/{TransferId}/status - NewStatus: {Status}",
                id, request.Status);

            var transfer = await _stockTransferService.UpdateStockTransferStatusAsync(id, request);

            return Ok(ApiResponse<StockTransferDto>.SuccessResponse(
                transfer,
                "Cập nhật trạng thái phiếu chuyển kho thành công"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Validation error: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            // Check type of error
            if (ex.Message.Contains("Không tìm thấy") || ex.Message.Contains("not found"))
            {
                _logger.LogWarning("⚠️ Stock transfer not found: {TransferId}", id);
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
            }

            if (ex.Message.Contains("Không thể chuyển trạng thái") ||
                ex.Message.Contains("Cannot transition"))
            {
                _logger.LogWarning("⚠️ Invalid status transition: {Message}", ex.Message);
                return Conflict(ApiResponse<object>.FailureResponse(ex.Message));
            }

            _logger.LogWarning("⚠️ Permission denied: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error updating stock transfer status {TransferId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.FailureResponse("Đã xảy ra lỗi khi cập nhật trạng thái"));
        }
    }

    /// <summary>
    /// Cancel a stock transfer
    /// </summary>
    /// <param name="id">Stock transfer unique identifier</param>
    /// <param name="request">Cancellation request with reason</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Stock transfer cancelled successfully</response>
    /// <response code="400">Invalid request data or validation errors</response>
    /// <response code="401">Unauthorized - Missing or invalid token</response>
    /// <response code="403">Forbidden - User lacks permission (lrm.cancel_stock_transfers)</response>
    /// <response code="404">Stock transfer not found</response>
    /// <response code="409">Conflict - Cannot cancel (already delivered or cancelled)</response>
    [HttpPatch("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelStockTransfer(
        Guid id,
        [FromBody] CancelStockTransferRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("⚠️ Invalid model state for CancelStockTransfer");
                return BadRequest(ApiResponse<object>.FailureResponse(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray()));
            }

            _logger.LogInformation("🗑️ PATCH /api/stocktransfers/{TransferId}/cancel - Reason: {Reason}",
                id, request.Reason);

            await _stockTransferService.CancelStockTransferAsync(id, request);

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Validation error: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            // Check type of error
            if (ex.Message.Contains("Không tìm thấy") || ex.Message.Contains("not found"))
            {
                _logger.LogWarning("⚠️ Stock transfer not found: {TransferId}", id);
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
            }

            if (ex.Message.Contains("Không thể hủy") || ex.Message.Contains("Cannot cancel"))
            {
                _logger.LogWarning("⚠️ Cannot cancel stock transfer: {Message}", ex.Message);
                return Conflict(ApiResponse<object>.FailureResponse(ex.Message));
            }

            _logger.LogWarning("⚠️ Permission denied: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error cancelling stock transfer {TransferId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.FailureResponse("Đã xảy ra lỗi khi hủy phiếu chuyển kho"));
        }
    }

    #endregion
}