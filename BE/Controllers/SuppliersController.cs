using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Suppliers;
using VietCommerce.Core.Models;

namespace VietCommerce.AdminApi.Controllers;

/// <summary>
/// API Controller for managing suppliers
/// Provides endpoints for CRUD operations, filtering, and supplier statistics
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Yêu cầu authentication cho tất cả endpoints
[Produces("application/json")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;
    private readonly ILogger<SuppliersController> _logger;

    public SuppliersController(
        ISupplierService supplierService,
        ILogger<SuppliersController> logger)
    {
        _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region GET Endpoints

    /// <summary>
    /// Get paginated list of suppliers with filtering
    /// </summary>
    /// <param name="pagination">Pagination parameters (page, pageSize, sortBy, sortDescending)</param>
    /// <param name="filters">Filter criteria (name, email, status, searchTerm, etc.)</param>
    /// <returns>Paginated list of suppliers</returns>
    /// <response code="200">Returns the paginated supplier list</response>
    /// <response code="400">Invalid pagination or filter parameters</response>
    /// <response code="401">Unauthorized - Missing or invalid token</response>
    /// <response code="403">Forbidden - User lacks permission (lrm.manage_suppliers)</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<SupplierDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSuppliers(
        [FromQuery] PaginationParams pagination,
        [FromQuery] SupplierFilters filters)
    {
        try
        {
            _logger.LogInformation("📋 GET /api/suppliers - Page: {Page}, PageSize: {PageSize}",
                pagination.Page, pagination.PageSize);

            var result = await _supplierService.GetSuppliersAsync(pagination, filters);

            return Ok(ApiResponse<PaginatedResult<SupplierDto>>.SuccessResponse(
                result,
                "Lấy danh sách suppliers thành công"));
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
            _logger.LogError(ex, "❌ Error getting suppliers");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.FailureResponse("Đã xảy ra lỗi khi lấy danh sách suppliers"));
        }
    }

    /// <summary>
    /// Get supplier details by ID
    /// </summary>
    /// <param name="id">Supplier unique identifier</param>
    /// <returns>Supplier details with product and stock transfer counts</returns>
    /// <response code="200">Returns the supplier details</response>
    /// <response code="400">Invalid supplier ID format</response>
    /// <response code="401">Unauthorized - Missing or invalid token</response>
    /// <response code="403">Forbidden - User lacks permission</response>
    /// <response code="404">Supplier not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSupplier(Guid id)
    {
        try
        {
            _logger.LogInformation("🔍 GET /api/suppliers/{SupplierId}", id);

            var supplier = await _supplierService.GetSupplierByIdAsync(id);

            return Ok(ApiResponse<SupplierDto>.SuccessResponse(
                supplier,
                "Lấy thông tin supplier thành công"));
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
                _logger.LogWarning("⚠️ Supplier not found: {SupplierId}", id);
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
            }

            _logger.LogWarning("⚠️ Permission denied: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting supplier {SupplierId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.FailureResponse("Đã xảy ra lỗi khi lấy thông tin supplier"));
        }
    }

    /// <summary>
    /// Get supplier statistics (product count, stock transfer count)
    /// </summary>
    /// <param name="id">Supplier unique identifier</param>
    /// <returns>Dictionary with supplier statistics</returns>
    /// <response code="200">Returns the supplier statistics</response>
    /// <response code="400">Invalid supplier ID format</response>
    /// <response code="401">Unauthorized - Missing or invalid token</response>
    /// <response code="403">Forbidden - User lacks permission</response>
    /// <response code="404">Supplier not found</response>
    [HttpGet("{id:guid}/stats")]
    [ProducesResponseType(typeof(ApiResponse<Dictionary<string, int>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSupplierStats(Guid id)
    {
        try
        {
            _logger.LogInformation("📊 GET /api/suppliers/{SupplierId}/stats", id);

            var stats = await _supplierService.GetSupplierStatsAsync(id);

            return Ok(ApiResponse<Dictionary<string, int>>.SuccessResponse(
                stats,
                "Lấy thống kê supplier thành công"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Invalid ID: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("⚠️ Permission denied or not found: {Message}", ex.Message);

            if (ex.Message.Contains("Không tìm thấy") || ex.Message.Contains("not found"))
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));

            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting supplier stats {SupplierId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.FailureResponse("Đã xảy ra lỗi khi lấy thống kê supplier"));
        }
    }

    /// <summary>
    /// Check if supplier can be deleted (no active relationships)
    /// </summary>
    /// <param name="id">Supplier unique identifier</param>
    /// <returns>Boolean indicating if supplier can be deleted</returns>
    /// <response code="200">Returns true if can delete, false otherwise</response>
    /// <response code="400">Invalid supplier ID format</response>
    /// <response code="401">Unauthorized - Missing or invalid token</response>
    /// <response code="403">Forbidden - User lacks permission</response>
    [HttpGet("{id:guid}/can-delete")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CanDeleteSupplier(Guid id)
    {
        try
        {
            _logger.LogInformation("🔍 GET /api/suppliers/{SupplierId}/can-delete", id);

            var canDelete = await _supplierService.CanDeleteSupplierAsync(id);

            return Ok(ApiResponse<bool>.SuccessResponse(
                canDelete,
                canDelete
                    ? "Supplier có thể xóa"
                    : "Supplier không thể xóa do có liên kết"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Invalid ID: {Message}", ex.Message);
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
            _logger.LogError(ex, "❌ Error checking if supplier can be deleted {SupplierId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.FailureResponse("Đã xảy ra lỗi khi kiểm tra supplier"));
        }
    }

    #endregion

    #region POST Endpoints

    /// <summary>
    /// Create a new supplier
    /// </summary>
    /// <param name="request">Supplier creation request</param>
    /// <returns>Created supplier details</returns>
    /// <response code="201">Supplier created successfully</response>
    /// <response code="400">Invalid request data or validation errors</response>
    /// <response code="401">Unauthorized - Missing or invalid token</response>
    /// <response code="403">Forbidden - User lacks permission</response>
    /// <response code="409">Conflict - Supplier name or email already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("⚠️ Invalid model state for CreateSupplier");
                return BadRequest(ApiResponse<object>.FailureResponse(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray()));
            }

            _logger.LogInformation("➕ POST /api/suppliers - Name: {Name}", request.Name);

            var supplier = await _supplierService.CreateSupplierAsync(request);

            return CreatedAtAction(
                nameof(GetSupplier),
                new { id = supplier.Id },
                ApiResponse<SupplierDto>.SuccessResponse(
                    supplier,
                    "Tạo supplier thành công"));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Validation error: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            // Check if it's a duplicate error or permission error
            if (ex.Message.Contains("đã tồn tại") || ex.Message.Contains("đã được sử dụng") ||
                ex.Message.Contains("already exists"))
            {
                _logger.LogWarning("⚠️ Duplicate supplier: {Message}", ex.Message);
                return Conflict(ApiResponse<object>.FailureResponse(ex.Message));
            }

            _logger.LogWarning("⚠️ Permission denied: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error creating supplier");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.FailureResponse("Đã xảy ra lỗi khi tạo supplier"));
        }
    }

    #endregion

    #region PUT Endpoints

    /// <summary>
    /// Update an existing supplier (partial update supported)
    /// </summary>
    /// <param name="id">Supplier unique identifier</param>
    /// <param name="request">Supplier update request (nullable fields for partial update)</param>
    /// <returns>Updated supplier details</returns>
    /// <response code="200">Supplier updated successfully</response>
    /// <response code="400">Invalid request data or validation errors</response>
    /// <response code="401">Unauthorized - Missing or invalid token</response>
    /// <response code="403">Forbidden - User lacks permission</response>
    /// <response code="404">Supplier not found</response>
    /// <response code="409">Conflict - Supplier name or email already exists</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SupplierDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateSupplier(
        Guid id,
        [FromBody] UpdateSupplierRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("⚠️ Invalid model state for UpdateSupplier");
                return BadRequest(ApiResponse<object>.FailureResponse(
                    "Dữ liệu không hợp lệ",
                    ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray()));
            }

            _logger.LogInformation("✏️ PUT /api/suppliers/{SupplierId}", id);

            var supplier = await _supplierService.UpdateSupplierAsync(id, request);

            return Ok(ApiResponse<SupplierDto>.SuccessResponse(
                supplier,
                "Cập nhật supplier thành công"));
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
                _logger.LogWarning("⚠️ Supplier not found: {SupplierId}", id);
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
            }

            if (ex.Message.Contains("đã tồn tại") || ex.Message.Contains("đã được sử dụng") ||
                ex.Message.Contains("already exists"))
            {
                _logger.LogWarning("⚠️ Duplicate supplier: {Message}", ex.Message);
                return Conflict(ApiResponse<object>.FailureResponse(ex.Message));
            }

            _logger.LogWarning("⚠️ Permission denied: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error updating supplier {SupplierId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.FailureResponse("Đã xảy ra lỗi khi cập nhật supplier"));
        }
    }

    #endregion

    #region DELETE Endpoints

    /// <summary>
    /// Delete a supplier (only if no active relationships)
    /// </summary>
    /// <param name="id">Supplier unique identifier</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Supplier deleted successfully</response>
    /// <response code="400">Invalid supplier ID format</response>
    /// <response code="401">Unauthorized - Missing or invalid token</response>
    /// <response code="403">Forbidden - User lacks permission</response>
    /// <response code="404">Supplier not found</response>
    /// <response code="409">Conflict - Supplier has active relationships (products or stock transfers)</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteSupplier(Guid id)
    {
        try
        {
            _logger.LogInformation("🗑️ DELETE /api/suppliers/{SupplierId}", id);

            await _supplierService.DeleteSupplierAsync(id);

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("⚠️ Invalid ID: {Message}", ex.Message);
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            // Check type of error
            if (ex.Message.Contains("Không tìm thấy") || ex.Message.Contains("not found"))
            {
                _logger.LogWarning("⚠️ Supplier not found: {SupplierId}", id);
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
            }

            if (ex.Message.Contains("Không thể xóa") || ex.Message.Contains("liên kết") ||
                ex.Message.Contains("cannot delete") || ex.Message.Contains("relationship"))
            {
                _logger.LogWarning("⚠️ Cannot delete supplier due to relationships: {SupplierId}", id);
                return Conflict(ApiResponse<object>.FailureResponse(ex.Message));
            }

            _logger.LogWarning("⚠️ Permission denied: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status403Forbidden,
                ApiResponse<object>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error deleting supplier {SupplierId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.FailureResponse("Đã xảy ra lỗi khi xóa supplier"));
        }
    }

    #endregion
}