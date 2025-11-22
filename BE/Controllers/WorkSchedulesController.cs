using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Application.Extensions;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Models;

namespace VietCommerce.AdminAPI.Controllers
{
    /// <summary>
    /// API Controller quản lý lịch làm việc (Work Schedule - HRM)
    ///
    /// PERMISSIONS:
    /// - hrm.schedule.view   : Xem lịch làm việc (của mình hoặc tất cả tùy role)
    /// - hrm.schedule.manage : Tạo, sửa, xóa lịch làm việc
    ///
    /// FEATURES:
    /// ✅ Danh sách phân trang + bộ lọc
    /// ✅ Chi tiết lịch làm việc
    /// ✅ Tạo mới lịch làm việc
    /// ✅ Cập nhật lịch làm việc
    /// ✅ Xóa lịch làm việc
    /// ✅ RBAC + Redis caching (nếu service có implement)
    /// ✅ Swagger docs chi tiết
    /// ✅ Sử dụng PaginatedResponse thay vì PaginatedResult
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class WorkSchedulesController : ControllerBase
    {
        private readonly IWorkScheduleService _workScheduleService;
        private readonly ILogger<WorkSchedulesController> _logger;

        public WorkSchedulesController(
            IWorkScheduleService workScheduleService,
            ILogger<WorkSchedulesController> logger)
        {
            _workScheduleService = workScheduleService ?? throw new ArgumentNullException(nameof(workScheduleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region GET Endpoints

        /// <summary>
        /// Lấy danh sách lịch làm việc (phân trang + lọc)
        /// </summary>
        /// <remarks>
        /// **Permissions Required:**
        /// - `hrm.schedule.view`
        ///
        /// **Query Parameters:**
        /// - Page, PageSize
        /// - EmployeeId, StoreId, Department
        /// - DateFrom, DateTo
        /// - ShiftType (Morning, Afternoon, FullDay, etc.)
        /// - Status (Approved, Pending, Rejected)
        /// - Search (tìm theo tên nhân viên, mã lịch...)
        ///
        /// **Response:** Paginated list of work schedules
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<WorkScheduleDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWorkSchedules(
            [FromQuery] PaginationParams pagination,
            [FromQuery] WorkScheduleFilters filters)
        {
            try
            {
                _logger.LogInformation("GET /api/workschedules - Lấy danh sách lịch làm việc");

                var result = await _workScheduleService.GetWorkSchedulesAsync(pagination, filters);

                var response = result.ToResponse(); // Sử dụng extension để chuyển sang PaginatedResponse

                return Ok(ApiResponse<PaginatedResponse<WorkScheduleDto>>.SuccessResponse(
                    response,
                    $"Lấy danh sách {response.Items.Count()} lịch làm việc thành công"
                ));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách lịch làm việc");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.FailureResponse("Có lỗi xảy ra khi lấy danh sách lịch làm việc"));
            }
        }

        /// <summary>
        /// Lấy chi tiết lịch làm việc theo ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<WorkScheduleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWorkScheduleById(Guid id)
        {
            try
            {
                _logger.LogInformation("GET /api/workschedules/{Id} - Lấy chi tiết lịch làm việc", id);

                var result = await _workScheduleService.GetWorkScheduleByIdAsync(id);

                return Ok(ApiResponse<WorkScheduleDto>.SuccessResponse(
                    result,
                    "Lấy thông tin lịch làm việc thành công"
                ));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Not Found: {Message}", ex.Message);
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết lịch làm việc {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.FailureResponse("Có lỗi xảy ra khi lấy thông tin lịch làm việc"));
            }
        }

        #endregion

        #region POST Endpoint

        /// <summary>
        /// Tạo mới lịch làm việc
        /// </summary>
        /// <remarks>
        /// **Permission Required:** `hrm.schedule.manage`
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<WorkScheduleDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateWorkSchedule([FromBody] CreateWorkScheduleRequest request)
        {
            try
            {
                _logger.LogInformation("POST /api/workschedules - Tạo lịch làm việc mới");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.FailureResponse(
                        "Dữ liệu không hợp lệ",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray()
                    ));
                }

                var result = await _workScheduleService.CreateWorkScheduleAsync(request);

                return CreatedAtAction(
                    nameof(GetWorkScheduleById),
                    new { id = result.Id },
                    ApiResponse<WorkScheduleDto>.SuccessResponse(result, "Tạo lịch làm việc thành công")
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Conflict: {Message}", ex.Message);
                return Conflict(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Bad Request: {Message}", ex.Message);
                return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo lịch làm việc");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.FailureResponse("Có lỗi xảy ra khi tạo lịch làm việc"));
            }
        }

        #endregion

        #region PUT Endpoint

        /// <summary>
        /// Cập nhật lịch làm việc
        /// </summary>
        /// <remarks>
        /// **Permission Required:** `hrm.schedule.manage`
        /// </remarks>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<WorkScheduleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateWorkSchedule(Guid id, [FromBody] UpdateWorkScheduleRequest request)
        {
            try
            {
                _logger.LogInformation("PUT /api/workschedules/{Id} - Cập nhật lịch làm việc", id);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.FailureResponse(
                        "Dữ liệu không hợp lệ",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray()
                    ));
                }

                var result = await _workScheduleService.UpdateWorkScheduleAsync(id, request);

                return Ok(ApiResponse<WorkScheduleDto>.SuccessResponse(
                    result,
                    "Cập nhật lịch làm việc thành công"
                ));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Not Found: {Message}", ex.Message);
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Conflict: {Message}", ex.Message);
                return Conflict(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Bad Request: {Message}", ex.Message);
                return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật lịch làm việc {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.FailureResponse("Có lỗi xảy ra khi cập nhật lịch làm việc"));
            }
        }

        #endregion

        #region DELETE Endpoint

        /// <summary>
        /// Xóa lịch làm việc
        /// </summary>
        /// <remarks>
        /// **Permission Required:** `hrm.schedule.manage`
        /// </remarks>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteWorkSchedule(Guid id)
        {
            try
            {
                _logger.LogInformation("DELETE /api/workschedules/{Id} - Xóa lịch làm việc", id);

                await _workScheduleService.DeleteWorkScheduleAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Not Found: {Message}", ex.Message);
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Conflict: {Message}", ex.Message);
                return Conflict(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa lịch làm việc {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.FailureResponse("Có lỗi xảy ra khi xóa lịch làm việc"));
            }
        }

        #endregion
    }
}