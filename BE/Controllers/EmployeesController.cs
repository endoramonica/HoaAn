using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Models;

namespace VietCommerce.AdminAPI.Controllers
{
    /// <summary>
    /// API Controller for managing employees (HR Management)
    /// 
    /// PERMISSIONS:
    /// - hrm.view_own: View own employee data
    /// - hrm.view_all: View all employees
    /// - hrm.manage: Create, Update, Delete employees
    /// 
    /// FEATURES:
    /// ✅ Get paginated list with filters
    /// ✅ Get employee details
    /// ✅ Create employee profile
    /// ✅ Update employee info (HR fields only)
    /// ✅ Delete employee profile
    /// ✅ RBAC permission checks
    /// ✅ Redis caching
    /// ✅ Swagger documentation
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ✅ Yêu cầu đăng nhập
    [Produces("application/json")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(
            IEmployeeService employeeService,
            ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region GET Endpoints

        /// <summary>
        /// Get paginated list of employees with filters
        /// </summary>
        /// <remarks>
        /// **Permissions Required:**
        /// - `hrm.view_all`: View all employees
        /// - `hrm.view_own`: View only own data
        /// 
        /// **Query Parameters:**
        /// - Page, PageSize: Pagination
        /// - SortBy, SortDescending: Sorting (name, email, code, department, position, hiredate, status, salary)
        /// - Search: Search in name, email, code, phone
        /// - Department: Filter by department
        /// - Status: Filter by status (1=Active, 2=Inactive, 3=OnLeave)
        /// - ManagerId: Filter by manager
        /// - StoreId: Filter by store
        /// - Position: Filter by position
        /// - HireDateFrom/To: Filter by hire date range
        /// 
        /// **Response:**
        /// - 200: Success with paginated data
        /// - 401: Unauthorized
        /// - 403: Forbidden (insufficient permissions)
        /// - 500: Internal server error
        /// </remarks>
        /// <param name="pagination">Pagination parameters</param>
        /// <param name="filters">Filter parameters</param>
        /// <returns>Paginated list of employees</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<EmployeeDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] PaginationParams pagination,
            [FromQuery] EmployeeFilters filters)
        {
            try
            {
                _logger.LogInformation("📋 GET /api/employees - Lấy danh sách employees");

                var result = await _employeeService.GetEmployeesAsync(pagination, filters);

                // Chuyển đổi từ PaginatedResult sang PaginatedResponse
                var response = new PaginatedResponse<EmployeeDto>(
                    result.Items,
                    result.PageNumber,
                    result.PageSize,
                    result.TotalItems
                );

                return Ok(ApiResponse<PaginatedResponse<EmployeeDto>>.SuccessResponse(
                    response,
                    $"Lấy danh sách {response.Items.Count()} employees thành công"
                ));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("⚠️ Unauthorized: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi lấy danh sách employees");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.FailureResponse("Có lỗi xảy ra khi lấy danh sách nhân viên"));
            }
        }

        /// <summary>
        /// Get employee details by UserId
        /// </summary>
        /// <remarks>
        /// **Permissions Required:**
        /// - `hrm.view_all`: View any employee
        /// - `hrm.view_own`: View only own data (must be your UserId)
        /// 
        /// **Response:**
        /// - 200: Success with employee details
        /// - 401: Unauthorized
        /// - 403: Forbidden (insufficient permissions or not your data)
        /// - 404: Employee not found
        /// - 500: Internal server error
        /// </remarks>
        /// <param name="userId">Employee UserId (Primary Key)</param>
        /// <returns>Employee details</returns>
        [HttpGet("{userId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEmployeeById(Guid userId)
        {
            try
            {
                _logger.LogInformation("🔍 GET /api/employees/{UserId} - Lấy thông tin employee", userId);

                var result = await _employeeService.GetEmployeeByIdAsync(userId);

                return Ok(ApiResponse<EmployeeDto>.SuccessResponse(
                    result,
                    $"Lấy thông tin employee '{result.Name}' thành công"
                ));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("⚠️ Not Found: {Message}", ex.Message);
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("⚠️ Unauthorized: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi lấy thông tin employee {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.FailureResponse("Có lỗi xảy ra khi lấy thông tin nhân viên"));
            }
        }

        #endregion

        #region POST Endpoints

        /// <summary>
        /// Create a new employee profile for an existing User
        /// </summary>
        /// <remarks>
        /// **Permission Required:** `hrm.manage`
        /// 
        /// **Notes:**
        /// - User must exist before creating employee profile
        /// - User can only have ONE employee profile
        /// - Employee Code must be unique
        /// - Name, Email, Phone, Avatar are taken from User entity (DO NOT provide in request)
        /// 
        /// **Response:**
        /// - 201: Employee profile created successfully
        /// - 400: Bad request (validation failed)
        /// - 401: Unauthorized
        /// - 403: Forbidden (insufficient permissions)
        /// - 409: Conflict (User already has employee profile or code exists)
        /// - 500: Internal server error
        /// 
        /// **Example Request:**
        /// ```json
        /// {
        ///   "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///   "code": "EMP001",
        ///   "position": "Software Engineer",
        ///   "department": "IT",
        ///   "hireDate": "2024-01-01",
        ///   "salary": 50000000,
        ///   "managerId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
        ///   "storeId": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
        ///   "skills": "C#, .NET, SQL Server"
        /// }
        /// ```
        /// </remarks>
        /// <param name="request">Employee creation data</param>
        /// <returns>Created employee details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request)
        {
            try
            {
                _logger.LogInformation("➕ POST /api/employees - Tạo employee mới cho UserId: {UserId}",
                    request.UserId);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.FailureResponse(
                        "Dữ liệu không hợp lệ",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray()
                    ));
                }

                var result = await _employeeService.CreateEmployeeAsync(request);

                return CreatedAtAction(
                    nameof(GetEmployeeById),
                    new { userId = result.UserId },
                    ApiResponse<EmployeeDto>.SuccessResponse(
                        result,
                        $"Tạo employee profile cho '{result.Name}' thành công"
                    )
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("⚠️ Conflict: {Message}", ex.Message);
                return Conflict(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("⚠️ Unauthorized: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("⚠️ Bad Request: {Message}", ex.Message);
                return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi tạo employee");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.FailureResponse("Có lỗi xảy ra khi tạo employee profile"));
            }
        }

        #endregion

        #region PUT Endpoints

        /// <summary>
        /// Update employee information (HR fields only)
        /// </summary>
        /// <remarks>
        /// **Permission Required:** `hrm.manage`
        /// 
        /// **Notes:**
        /// - Only HR-related fields can be updated here
        /// - To update Name, Email, Phone, Avatar → Use User API
        /// - Partial update supported (only send fields to update)
        /// - Employee Code must be unique
        /// 
        /// **Response:**
        /// - 200: Employee updated successfully
        /// - 400: Bad request (validation failed)
        /// - 401: Unauthorized
        /// - 403: Forbidden (insufficient permissions)
        /// - 404: Employee not found
        /// - 409: Conflict (employee code already exists)
        /// - 500: Internal server error
        /// 
        /// **Example Request:**
        /// ```json
        /// {
        ///   "code": "EMP001",
        ///   "position": "Senior Software Engineer",
        ///   "department": "IT",
        ///   "salary": 60000000,
        ///   "status": 1,
        ///   "managerId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
        ///   "storeId": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
        ///   "skills": "C#, .NET, Azure, Microservices"
        /// }
        /// ```
        /// </remarks>
        /// <param name="userId">Employee UserId (Primary Key)</param>
        /// <param name="request">Update data (partial update supported)</param>
        /// <returns>Updated employee details</returns>
        [HttpPut("{userId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEmployee(
            Guid userId,
            [FromBody] UpdateEmployeeRequest request)
        {
            try
            {
                _logger.LogInformation("✏️ PUT /api/employees/{UserId} - Cập nhật employee", userId);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<object>.FailureResponse(
                        "Dữ liệu không hợp lệ",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray()
                    ));
                }

                var result = await _employeeService.UpdateEmployeeAsync(userId, request);

                return Ok(ApiResponse<EmployeeDto>.SuccessResponse(
                    result,
                    $"Cập nhật thông tin employee '{result.Name}' thành công"
                ));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("⚠️ Not Found: {Message}", ex.Message);
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("⚠️ Conflict: {Message}", ex.Message);
                return Conflict(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("⚠️ Unauthorized: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("⚠️ Bad Request: {Message}", ex.Message);
                return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi cập nhật employee {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.FailureResponse("Có lỗi xảy ra khi cập nhật thông tin nhân viên"));
            }
        }

        #endregion

        #region DELETE Endpoints

        /// <summary>
        /// Delete employee profile (does NOT delete User account)
        /// </summary>
        /// <remarks>
        /// **Permission Required:** `hrm.manage`
        /// 
        /// **Notes:**
        /// - Only deletes Employee profile, User account remains active
        /// - Cannot delete if employee has:
        ///   - Leave requests
        ///   - Subordinates (employees reporting to them)
        /// 
        /// **Response:**
        /// - 204: Employee profile deleted successfully
        /// - 401: Unauthorized
        /// - 403: Forbidden (insufficient permissions)
        /// - 404: Employee not found
        /// - 409: Conflict (has dependencies)
        /// - 500: Internal server error
        /// </remarks>
        /// <param name="userId">Employee UserId (Primary Key)</param>
        /// <returns>No content</returns>
        [HttpDelete("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmployee(Guid userId)
        {
            try
            {
                _logger.LogInformation("🗑️ DELETE /api/employees/{UserId} - Xóa employee profile", userId);

                await _employeeService.DeleteEmployeeAsync(userId);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("⚠️ Not Found: {Message}", ex.Message);
                return NotFound(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("⚠️ Conflict: {Message}", ex.Message);
                return Conflict(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("⚠️ Unauthorized: {Message}", ex.Message);
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi xóa employee {UserId}", userId);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.FailureResponse("Có lỗi xảy ra khi xóa employee profile"));
            }
        }

        #endregion
    }
}