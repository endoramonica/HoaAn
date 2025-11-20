using AutoMapper;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Entities.HRM;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services;

/// <summary>
/// Service implementation for managing employees
/// Implements business logic with validation, permission checks, and caching
/// 
/// FEATURES:
/// ✅ CRUD operations with validation
/// ✅ Permission Checks (RBAC) - hrm.view_own, hrm.view_all, hrm.manage
/// ✅ Self-service support (employees can view their own data)
/// ✅ Redis caching with TTL
/// ✅ Pagination and filtering
/// ✅ Audit logging
/// </summary>
public class EmployeeService : BaseService, IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUser _currentUser;

    // Cache settings
    private const string CACHE_PREFIX_SINGLE = "employee";
    private const string CACHE_PREFIX_LIST = "employees:list";
    private static readonly TimeSpan CACHE_DURATION_SINGLE = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan CACHE_DURATION_LIST = TimeSpan.FromMinutes(10);

    // Permission constants
    private const string PERMISSION_VIEW_OWN = "hrm.view_own";
    private const string PERMISSION_VIEW_ALL = "hrm.view_all";
    private const string PERMISSION_MANAGE = "hrm.manage";

    public EmployeeService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<EmployeeService> logger,
        ICacheService? cacheService,
        IPermissionService permissionService,
        ICurrentUser currentUser)
        : base(logger, cacheService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    #region Public Methods

    /// <summary>
    /// Get paginated list of employees with filtering
    /// Permission: hrm.view_all (all employees) or hrm.view_own (own data only)
    /// </summary>
    public async Task<PaginatedResult<EmployeeDto>> GetEmployeesAsync(
       PaginationParams pagination,
       EmployeeFilters filters)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateNotNull(pagination, nameof(pagination));
            ValidateNotNull(filters, nameof(filters));

            var userId = _currentUser.UserId;
            ThrowIf(userId == Guid.Empty, "Không xác định được người dùng hiện tại (Unauthorized)");

            LogInfo("📋 Lấy danh sách employees - Page: {Page}, PageSize: {PageSize}, User: {UserId}",
                pagination.Page, pagination.PageSize, userId);

            // 🔒 Check Permission
            var canViewAll = await _permissionService.CheckUserPermissionAsync(userId, PERMISSION_VIEW_ALL);
            var canViewOwn = await _permissionService.CheckUserPermissionAsync(userId, PERMISSION_VIEW_OWN);

            ThrowIf(!canViewAll && !canViewOwn,
                $"Bạn không có quyền xem danh sách nhân viên ({PERMISSION_VIEW_ALL} hoặc {PERMISSION_VIEW_OWN})");

            // If user can only view own data, filter by their UserId
            if (!canViewAll && canViewOwn)
            {
                // ✅ Lọc theo UserId của employee (vì UserId là PK của Employee)
                filters.UserId = userId;
                LogDebug("🔒 User chỉ có quyền xem dữ liệu của mình - UserId: {UserId}", userId);
            }

            var cacheKey = GenerateListCacheKey(pagination, filters, canViewAll ? "all" : userId.ToString());

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var (items, totalCount) = await _unitOfWork.Employees
                    .GetEmployeesPagedAsync(pagination, filters);

                var dtos = _mapper.Map<List<EmployeeDto>>(items);

                var result = new PaginatedResult<EmployeeDto>(
                    dtos,
                    pagination.Page,
                    pagination.PageSize,
                    totalCount
                );

                LogInfo("✅ Tìm thấy {Count} employees (Total: {Total})", dtos.Count, totalCount);
                return result;

            }, CACHE_DURATION_LIST);

        }, "GetEmployeesAsync");
    }

    /// <summary>
    /// Get employee details by UserId
    /// Permission: hrm.view_all (any employee) or hrm.view_own (own data only)
    /// </summary>
    public async Task<EmployeeDto> GetEmployeeByIdAsync(Guid userId)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(userId, nameof(userId));

            var currentUserId = _currentUser.UserId;
            ThrowIf(currentUserId == Guid.Empty, "Không xác định được người dùng hiện tại (Unauthorized)");

            LogInfo("🔍 Lấy thông tin employee - UserId: {UserId}, CurrentUser: {CurrentUserId}",
                userId, currentUserId);

            // 🔒 Check Permission
            var canViewAll = await _permissionService.CheckUserPermissionAsync(currentUserId, PERMISSION_VIEW_ALL);
            var canViewOwn = await _permissionService.CheckUserPermissionAsync(currentUserId, PERMISSION_VIEW_OWN);

            ThrowIf(!canViewAll && !canViewOwn,
                $"Bạn không có quyền xem thông tin nhân viên ({PERMISSION_VIEW_ALL} hoặc {PERMISSION_VIEW_OWN})");

            // If user can only view own data, verify it's their record
            if (!canViewAll && canViewOwn)
            {
                ThrowIf(userId != currentUserId,
                    "Bạn chỉ có quyền xem thông tin nhân viên của chính mình");
            }

            var cacheKey = CreateCacheKey(CACHE_PREFIX_SINGLE, userId);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var employee = await _unitOfWork.Employees.GetEmployeeWithDetailsAsync(userId);
                ThrowIf(employee == null, $"Không tìm thấy employee với UserId: {userId}");

                var dto = _mapper.Map<EmployeeDto>(employee);

                LogInfo("✅ Tìm thấy employee: {Name} ({Email})", dto.Name, dto.Email);
                return dto;

            }, CACHE_DURATION_SINGLE);

        }, "GetEmployeeByIdAsync");
    }

    /// <summary>
    /// Create a new employee profile for an existing User
    /// Permission: hrm.manage
    /// </summary>
    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_MANAGE);

            ValidateNotNull(request, nameof(request));
            ValidateId(request.UserId, nameof(request.UserId));

            LogInfo("➕ Tạo employee profile mới - UserId: {UserId}, Code: {Code}",
                request.UserId, request.Code);

            // ✅ Kiểm tra User có tồn tại không
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            ThrowIf(user == null, $"Không tìm thấy User với ID: {request.UserId}");

            // ✅ Kiểm tra User đã có Employee profile chưa
            var existingEmployee = await _unitOfWork.Employees.GetByIdAsync(request.UserId);
            ThrowIf(existingEmployee != null,
                $"User '{user!.Name}' đã có Employee profile");

            // ✅ Check duplicate employee code
            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var codeExists = await _unitOfWork.Employees.IsEmployeeCodeExistsAsync(request.Code);
                ThrowIf(codeExists, $"Mã nhân viên '{request.Code}' đã tồn tại");
            }

            // Map and create
            var employee = _mapper.Map<Employee>(request);
            employee.Status = EmployeeStatus.Active;

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            // Load related data for DTO
            var createdEmployee = await _unitOfWork.Employees.GetEmployeeWithDetailsAsync(employee.Id);
            var dto = _mapper.Map<EmployeeDto>(createdEmployee);

            // Invalidate list cache
            await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX_LIST}:*");

            LogInfo("✅ Tạo employee thành công - UserId: {UserId}, Name: {Name}, Code: {Code}",
                dto.UserId, dto.Name, dto.Code);

            return dto;

        }, "CreateEmployeeAsync");
    }

    /// <summary>
    /// Update an existing employee (partial update supported)
    /// Permission: hrm.manage
    /// Note: Name, Email, Phone, Avatar must be updated via User API
    /// </summary>
    public async Task<EmployeeDto> UpdateEmployeeAsync(Guid userId, UpdateEmployeeRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_MANAGE);

            ValidateId(userId, nameof(userId));
            ValidateNotNull(request, nameof(request));

            LogInfo("✏️ Cập nhật employee - UserId: {UserId}", userId);

            var employee = await _unitOfWork.Employees.GetByIdAsync(userId);
            ThrowIf(employee == null, $"Không tìm thấy employee với UserId: {userId}");

            // ✅ Validate employee code if changed
            if (!string.IsNullOrWhiteSpace(request.Code) &&
                request.Code != employee!.Code)
            {
                var codeExists = await _unitOfWork.Employees
                    .IsEmployeeCodeExistsAsync(request.Code, userId);
                ThrowIf(codeExists, $"Mã nhân viên '{request.Code}' đã tồn tại");
            }

            // Apply partial updates (chỉ update các field HR)
            _mapper.Map(request, employee);

            _unitOfWork.Employees.Update(employee!);
            await _unitOfWork.SaveChangesAsync();

            // Load updated data with relations
            var updatedEmployee = await _unitOfWork.Employees.GetEmployeeWithDetailsAsync(userId);
            var dto = _mapper.Map<EmployeeDto>(updatedEmployee);

            // Invalidate caches
            await InvalidateMultipleCachesAsync(
                CreateCacheKey(CACHE_PREFIX_SINGLE, userId)
            );
            await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX_LIST}:*");

            LogInfo("✅ Cập nhật employee thành công - UserId: {UserId}, Name: {Name}",
                userId, dto.Name);

            return dto;

        }, "UpdateEmployeeAsync");
    }

    /// <summary>
    /// Delete an employee profile (does NOT delete User account)
    /// Permission: hrm.manage
    /// </summary>
    public async Task DeleteEmployeeAsync(Guid userId)
    {
        await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_MANAGE);

            ValidateId(userId, nameof(userId));

            LogInfo("🗑️ Xóa employee profile - UserId: {UserId}", userId);

            var employee = await _unitOfWork.Employees.GetEmployeeWithDetailsAsync(userId);
            ThrowIf(employee == null, $"Không tìm thấy employee với UserId: {userId}");

            // Check for dependencies
            var hasLeaveRequests = await _unitOfWork.Employees.HasLeaveRequestsAsync(userId);
            ThrowIf(hasLeaveRequests,
                $"Không thể xóa Employee profile của '{employee!.User.Name}' vì có đơn nghỉ phép liên quan");

            var hasSubordinates = await _unitOfWork.Employees.HasSubordinatesAsync(userId);
            ThrowIf(hasSubordinates,
                $"Không thể xóa Employee profile của '{employee.User.Name}' vì có nhân viên cấp dưới");

            _unitOfWork.Employees.Delete(employee);
            await _unitOfWork.SaveChangesAsync();

            // Invalidate caches
            await InvalidateMultipleCachesAsync(
                CreateCacheKey(CACHE_PREFIX_SINGLE, userId)
            );
            await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX_LIST}:*");

            LogInfo("✅ Xóa employee profile thành công - UserId: {UserId}, Name: {Name}",
                userId, employee.User.Name);

        }, "DeleteEmployeeAsync");
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Helper to check user permissions
    /// </summary>
    private async Task EnsurePermissionAsync(string permissionName)
    {
        var userId = _currentUser.UserId;
        ThrowIf(userId == Guid.Empty, "Không xác định được người dùng hiện tại (Unauthorized)");

        var hasPermission = await _permissionService.CheckUserPermissionAsync(userId, permissionName);
        ThrowIf(!hasPermission, $"Bạn không có quyền thực hiện hành động này ({permissionName})");
    }

    /// <summary>
    /// Generate cache key for employee list
    /// </summary>
    private string GenerateListCacheKey(PaginationParams pagination, EmployeeFilters filters, string scope)
    {
        var parts = new List<string>
        {
            scope, // "all" or userId
            pagination.Page.ToString(),
            pagination.PageSize.ToString(),
            pagination.SortBy ?? "",
            pagination.SortDescending.ToString()
        };

        if (!string.IsNullOrWhiteSpace(filters.Search))
            parts.Add($"search:{filters.Search}");

        if (!string.IsNullOrWhiteSpace(filters.Department))
            parts.Add($"dept:{filters.Department}");

        if (filters.Status.HasValue)
            parts.Add($"status:{filters.Status.Value}");

        if (filters.ManagerId.HasValue)
            parts.Add($"manager:{filters.ManagerId.Value}");

        if (filters.StoreId.HasValue)
            parts.Add($"store:{filters.StoreId.Value}");

        if (filters.HireDateFrom.HasValue)
            parts.Add($"hireFrom:{filters.HireDateFrom.Value:yyyyMMdd}");

        if (filters.HireDateTo.HasValue)
            parts.Add($"hireTo:{filters.HireDateTo.Value:yyyyMMdd}");

        if (!string.IsNullOrWhiteSpace(filters.Position))
            parts.Add($"pos:{filters.Position}");


        return CreateCacheKey(CACHE_PREFIX_LIST, string.Join(":", parts));
    }

    #endregion
}