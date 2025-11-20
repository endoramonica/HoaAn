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
/// Service implementation for managing work schedules
/// Implements business logic with validation, permission checks, and scheduling logic
/// 
/// FEATURES:
/// ✅ CRUD operations with validation
/// ✅ Permission Checks (RBAC) - hrm.assign_shifts, hrm.view_all
/// ✅ Schedule conflict detection
/// ✅ Self-service support (employees can view their own schedules)
/// ✅ Redis caching with TTL
/// </summary>
public class WorkScheduleService : BaseService, IWorkScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUser _currentUser;

    // Cache settings
    private const string CACHE_PREFIX_SINGLE = "work_schedule";
    private const string CACHE_PREFIX_LIST = "work_schedules:list";
    private static readonly TimeSpan CACHE_DURATION_SINGLE = TimeSpan.FromMinutes(20);
    private static readonly TimeSpan CACHE_DURATION_LIST = TimeSpan.FromMinutes(10);

    // Permission constants
    private const string PERMISSION_ASSIGN_SHIFTS = "hrm.assign_shifts";
    private const string PERMISSION_VIEW_ALL = "hrm.view_all";
    private const string PERMISSION_VIEW_OWN = "hrm.view_own";

    public WorkScheduleService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<WorkScheduleService> logger,
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
    /// Get paginated list of work schedules with filtering
    /// Permission: hrm.view_all (all schedules) or hrm.view_own (own schedules only)
    /// </summary>
    public async Task<PaginatedResult<WorkScheduleDto>> GetWorkSchedulesAsync(
        PaginationParams pagination,
        WorkScheduleFilters filters)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateNotNull(pagination, nameof(pagination));
            ValidateNotNull(filters, nameof(filters));

            var userId = _currentUser.UserId;
            ThrowIf(userId == Guid.Empty, "Không xác định được người dùng hiện tại (Unauthorized)");

            LogInfo("📋 Lấy danh sách work schedules - Page: {Page}, User: {UserId}",
                pagination.Page, userId);

            // 🔒 Check Permission
            var canViewAll = await _permissionService.CheckUserPermissionAsync(userId, PERMISSION_VIEW_ALL);
            var canViewOwn = await _permissionService.CheckUserPermissionAsync(userId, PERMISSION_VIEW_OWN);

            ThrowIf(!canViewAll && !canViewOwn,
                "Bạn không có quyền xem lịch làm việc");

            // If user can only view own data, filter by their employee ID
            if (!canViewAll && canViewOwn)
            {
                var employee = await _unitOfWork.Employees.GetByIdAsync(userId);
                ThrowIf(employee == null, "Không tìm thấy thông tin nhân viên của bạn");
                filters.EmployeeId = employee!.Id;
                LogDebug("🔒 User chỉ xem được lịch của mình - EmployeeId: {EmployeeId}", employee.Id);
            }

            var cacheKey = GenerateListCacheKey(pagination, filters,
                canViewAll ? "all" : userId.ToString());

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var (items, totalCount) = await _unitOfWork.WorkSchedules
                    .GetWorkSchedulesPagedAsync(pagination, filters);

                var dtos = _mapper.Map<List<WorkScheduleDto>>(items);

                var result = new PaginatedResult<WorkScheduleDto>(
                    dtos,
                    pagination.Page,
                    pagination.PageSize,
                    totalCount
                );

                LogInfo("✅ Tìm thấy {Count} work schedules (Total: {Total})", dtos.Count, totalCount);
                return result;

            }, CACHE_DURATION_LIST);

        }, "GetWorkSchedulesAsync");
    }

    /// <summary>
    /// Get work schedule details by ID
    /// Permission: hrm.view_all (any schedule) or own schedule
    /// </summary>
    public async Task<WorkScheduleDto> GetWorkScheduleByIdAsync(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(id, nameof(id));

            var userId = _currentUser.UserId;
            ThrowIf(userId == Guid.Empty, "Không xác định được người dùng hiện tại (Unauthorized)");

            LogInfo("🔍 Lấy thông tin work schedule - ID: {WorkScheduleId}", id);

            var cacheKey = CreateCacheKey(CACHE_PREFIX_SINGLE, id);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var schedule = await _unitOfWork.WorkSchedules.GetWorkScheduleWithDetailsAsync(id);
                ThrowIf(schedule == null, $"Không tìm thấy lịch làm việc với ID: {id}");

                // 🔒 Check Permission
                var canViewAll = await _permissionService.CheckUserPermissionAsync(userId, PERMISSION_VIEW_ALL);

                if (!canViewAll)
                {
                    var employee = await _unitOfWork.Employees.GetByIdAsync(userId);
                    ThrowIf(employee == null || schedule!.EmployeeId != employee.Id,
                        "Bạn chỉ có quyền xem lịch làm việc của chính mình");
                }

                var dto = _mapper.Map<WorkScheduleDto>(schedule);

                LogInfo("✅ Tìm thấy work schedule: Employee={EmployeeId}, Date={Date}",
                    dto.EmployeeId, dto.Date);
                return dto;

            }, CACHE_DURATION_SINGLE);

        }, "GetWorkScheduleByIdAsync");
    }

    /// <summary>
    /// Create a new work schedule
    /// Permission: hrm.assign_shifts
    /// </summary>
    public async Task<WorkScheduleDto> CreateWorkScheduleAsync(CreateWorkScheduleRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_ASSIGN_SHIFTS);

            ValidateNotNull(request, nameof(request));

            LogInfo("➕ Tạo work schedule mới - Employee: {EmployeeId}, Date: {Date}",
                request.EmployeeId, request.Date);

            // Validate employee exists
            var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId);
            ThrowIf(employee == null, $"Không tìm thấy nhân viên với ID: {request.EmployeeId}");

            // Validate times
            ThrowIf(request.EndTime <= request.StartTime,
                "Giờ kết thúc phải sau giờ bắt đầu");

            // Check for schedule conflicts
            var hasConflict = await _unitOfWork.WorkSchedules
                .HasScheduleConflictAsync(request.EmployeeId, request.Date, request.StartTime, request.EndTime);
            ThrowIf(hasConflict,
                $"Nhân viên đã có lịch làm việc trùng thời gian trong ngày {request.Date:dd/MM/yyyy}");

            // Map and create
            var schedule = _mapper.Map<WorkSchedule>(request);
            schedule.CreatedAt = DateTime.UtcNow;
            schedule.UpdatedAt = DateTime.UtcNow;
            //schedule.CreatedBy = _currentUser.UserId;

            await _unitOfWork.WorkSchedules.AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<WorkScheduleDto>(schedule);

            // Invalidate list cache
            await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX_LIST}:*");

            LogInfo("✅ Tạo work schedule thành công - ID: {WorkScheduleId}, Employee: {EmployeeId}",
                dto.Id, dto.EmployeeId);

            return dto;

        }, "CreateWorkScheduleAsync");
    }

    /// <summary>
    /// Update an existing work schedule
    /// Permission: hrm.assign_shifts
    /// </summary>
    public async Task<WorkScheduleDto> UpdateWorkScheduleAsync(Guid id, UpdateWorkScheduleRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_ASSIGN_SHIFTS);

            ValidateId(id, nameof(id));
            ValidateNotNull(request, nameof(request));

            LogInfo("✏️ Cập nhật work schedule - ID: {WorkScheduleId}", id);

            var schedule = await _unitOfWork.WorkSchedules.GetByIdAsync(id);
            ThrowIf(schedule == null, $"Không tìm thấy lịch làm việc với ID: {id}");

            // Validate times if changed
            var newStartTime = request.StartTime ?? schedule!.StartTime;
            var newEndTime = request.EndTime ?? schedule.EndTime;

            ThrowIf(newEndTime <= newStartTime,
                "Giờ kết thúc phải sau giờ bắt đầu");

            // Check for conflicts if times changed
            if (request.StartTime.HasValue || request.EndTime.HasValue)
            {
                var hasConflict = await _unitOfWork.WorkSchedules
                    .HasScheduleConflictAsync(
                        schedule.EmployeeId,
                        schedule.Date,
                        newStartTime,
                        newEndTime,
                        id);

                ThrowIf(hasConflict,
                    $"Nhân viên đã có lịch làm việc trùng thời gian trong ngày {schedule.Date:dd/MM/yyyy}");
            }

            // Apply partial updates
            if (request.StartTime.HasValue)
                schedule!.StartTime = request.StartTime.Value;

            if (request.EndTime.HasValue)
                schedule!.EndTime = request.EndTime.Value;

            if (!string.IsNullOrWhiteSpace(request.ShiftName))
                schedule!.ShiftName = request.ShiftName;

            if (!string.IsNullOrWhiteSpace(request.Notes))
                schedule!.Notes = request.Notes;

            schedule!.UpdatedAt = DateTime.UtcNow;
            //schedule.UpdatedBy = _currentUser.UserId;

            _unitOfWork.WorkSchedules.Update(schedule);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<WorkScheduleDto>(schedule);

            // Invalidate caches
            await InvalidateMultipleCachesAsync(
                CreateCacheKey(CACHE_PREFIX_SINGLE, id)
            );
            await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX_LIST}:*");

            LogInfo("✅ Cập nhật work schedule thành công - ID: {WorkScheduleId}", id);

            return dto;

        }, "UpdateWorkScheduleAsync");
    }

    /// <summary>
    /// Delete a work schedule
    /// Permission: hrm.assign_shifts
    /// </summary>
    public async Task DeleteWorkScheduleAsync(Guid id)
    {
        await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_ASSIGN_SHIFTS);

            ValidateId(id, nameof(id));

            LogInfo("🗑️ Xóa work schedule - ID: {WorkScheduleId}", id);

            var schedule = await _unitOfWork.WorkSchedules.GetByIdAsync(id);
            ThrowIf(schedule == null, $"Không tìm thấy lịch làm việc với ID: {id}");

            // Prevent deleting past schedules
            ThrowIf(schedule! < DateTime.UtcNow.Date,
                "Không thể xóa lịch làm việc trong quá khứ");

            _unitOfWork.WorkSchedules.Delete(schedule);
            await _unitOfWork.SaveChangesAsync();

            // Invalidate caches
            await InvalidateMultipleCachesAsync(
                CreateCacheKey(CACHE_PREFIX_SINGLE, id)
            );
            await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX_LIST}:*");

            LogInfo("✅ Xóa work schedule thành công - ID: {WorkScheduleId}", id);

        }, "DeleteWorkScheduleAsync");
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
    /// Generate cache key for work schedule list
    /// </summary>
    private string GenerateListCacheKey(PaginationParams pagination, WorkScheduleFilters filters, string scope)
    {
        var parts = new List<string>
        {
            scope,
            pagination.Page.ToString(),
            pagination.PageSize.ToString(),
            pagination.SortBy,
            pagination.SortDescending.ToString()
        };

        if (filters.EmployeeId.HasValue)
            parts.Add($"emp:{filters.EmployeeId.Value}");

        if (filters.DateFrom.HasValue)
            parts.Add($"from:{filters.DateFrom.Value:yyyyMMdd}");

        if (filters.DateTo.HasValue)
            parts.Add($"to:{filters.DateTo.Value:yyyyMMdd}");

        if (!string.IsNullOrWhiteSpace(filters.ShiftName))
            parts.Add($"shift:{filters.ShiftName}");

        return CreateCacheKey(CACHE_PREFIX_LIST, string.Join(":", parts));
    }

    #endregion
}