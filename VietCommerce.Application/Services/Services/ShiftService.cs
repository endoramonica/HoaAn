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
/// Service implementation for managing shifts (work sessions)
/// Implements business logic with validation, permission checks, and shift lifecycle management
/// 
/// FEATURES:
/// ✅ CRUD operations with validation
/// ✅ Permission Checks (RBAC) - hrm.view_own, hrm.view_all, hrm.manage
/// ✅ Shift lifecycle (Open → Close)
/// ✅ Current shift tracking
/// ✅ Self-service support (employees can manage their own shifts)
/// ✅ Redis caching with TTL
/// </summary>
public class ShiftService : BaseService, IShiftService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUser _currentUser;

    // Cache settings
    private const string CACHE_PREFIX_SINGLE = "shift";
    private const string CACHE_PREFIX_LIST = "shifts:list";
    private const string CACHE_PREFIX_CURRENT = "shift:current";
    private static readonly TimeSpan CACHE_DURATION_SINGLE = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan CACHE_DURATION_LIST = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan CACHE_DURATION_CURRENT = TimeSpan.FromMinutes(2);

    // Permission constants
    private const string PERMISSION_VIEW_OWN = "hrm.view_own";
    private const string PERMISSION_VIEW_ALL = "hrm.view_all";
    private const string PERMISSION_MANAGE = "hrm.manage";

    public ShiftService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ShiftService> logger,
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
    /// Get paginated list of shifts with filtering
    /// Permission: hrm.view_all (all shifts) or hrm.view_own (own shifts only)
    /// </summary>
    public async Task<PaginatedResult<ShiftDto>> GetShiftsAsync(
        PaginationParams pagination,
        ShiftFilters filters)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateNotNull(pagination, nameof(pagination));
            ValidateNotNull(filters, nameof(filters));

            var userId = _currentUser.UserId;
            ThrowIf(userId == Guid.Empty, "Không xác định được người dùng hiện tại (Unauthorized)");

            LogInfo("📋 Lấy danh sách shifts - Page: {Page}, User: {UserId}",
                pagination.Page, userId);

            // 🔒 Check Permission
            var canViewAll = await _permissionService.CheckUserPermissionAsync(userId, PERMISSION_VIEW_ALL);
            var canViewOwn = await _permissionService.CheckUserPermissionAsync(userId, PERMISSION_VIEW_OWN);

            ThrowIf(!canViewAll && !canViewOwn,
                "Bạn không có quyền xem danh sách ca làm việc");

            // If user can only view own data, filter by their UserId
            if (!canViewAll && canViewOwn)
            {
                filters.UserId = userId;
                LogDebug("🔒 User chỉ xem được ca của mình - UserId: {UserId}", userId);
            }

            var cacheKey = GenerateListCacheKey(pagination, filters,
                canViewAll ? "all" : userId.ToString());

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var (items, totalCount) = await _unitOfWork.Shifts
                    .GetShiftsPagedAsync(pagination, filters);

                var dtos = _mapper.Map<List<ShiftDto>>(items);

                var result = new PaginatedResult<ShiftDto>(
                    dtos,
                    pagination.Page,
                    pagination.PageSize,
                    totalCount
                );

                LogInfo("✅ Tìm thấy {Count} shifts (Total: {Total})", dtos.Count, totalCount);
                return result;

            }, CACHE_DURATION_LIST);

        }, "GetShiftsAsync");
    }

    /// <summary>
    /// Get shift details by ID
    /// Permission: hrm.view_all (any shift) or own shift
    /// </summary>
    public async Task<ShiftDto> GetShiftByIdAsync(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(id, nameof(id));

            var userId = _currentUser.UserId;
            ThrowIf(userId == Guid.Empty, "Không xác định được người dùng hiện tại (Unauthorized)");

            LogInfo("🔍 Lấy thông tin shift - ID: {ShiftId}", id);

            var cacheKey = CreateCacheKey(CACHE_PREFIX_SINGLE, id);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var shift = await _unitOfWork.Shifts.GetShiftWithDetailsAsync(id);
                ThrowIf(shift == null, $"Không tìm thấy ca làm việc với ID: {id}");

                // 🔒 Check Permission
                var canViewAll = await _permissionService.CheckUserPermissionAsync(userId, PERMISSION_VIEW_ALL);

                if (!canViewAll)
                {
                    ThrowIf(shift!.UserId != userId,
                        "Bạn chỉ có quyền xem ca làm việc của chính mình");
                }

                var dto = _mapper.Map<ShiftDto>(shift);

                LogInfo("✅ Tìm thấy shift: User={UserId}, Status={Status}",
                    dto.UserId, dto.Status);
                return dto;

            }, CACHE_DURATION_SINGLE);

        }, "GetShiftByIdAsync");
    }

    /// <summary>
    /// Get current active shift for user
    /// Permission: hrm.view_own (can view own shift)
    /// </summary>
    public async Task<ShiftDto?> GetCurrentShiftAsync(Guid userId)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(userId, nameof(userId));

            var currentUserId = _currentUser.UserId;
            ThrowIf(currentUserId == Guid.Empty, "Không xác định được người dùng hiện tại (Unauthorized)");

            LogInfo("🔍 Lấy ca làm việc hiện tại - UserId: {UserId}", userId);

            // 🔒 Check Permission
            var canViewAll = await _permissionService.CheckUserPermissionAsync(currentUserId, PERMISSION_VIEW_ALL);

            // If user cannot view all, they can only view their own shift
            if (!canViewAll)
            {
                ThrowIf(userId != currentUserId,
                    "Bạn chỉ có quyền xem ca làm việc của chính mình");
            }

            var cacheKey = CreateCacheKey(CACHE_PREFIX_CURRENT, userId);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var shift = await _unitOfWork.Shifts.GetCurrentShiftAsync(userId);

                if (shift == null)
                {
                    LogDebug("ℹ️ User không có ca làm việc hiện tại - UserId: {UserId}", userId);
                    return null;
                }

                var dto = _mapper.Map<ShiftDto>(shift);
                LogInfo("✅ Tìm thấy ca hiện tại: ShiftId={ShiftId}, StartTime={StartTime}",
                    dto.Id, dto.StartTime);
                return dto;

            }, CACHE_DURATION_CURRENT);

        }, "GetCurrentShiftAsync");
    }

    /// <summary>
    /// Open a new shift
    /// Permission: hrm.view_own (employees can open their own shift)
    /// </summary>
    public async Task<ShiftDto> OpenShiftAsync(OpenShiftRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission - any employee can open their own shift
            await EnsurePermissionAsync(PERMISSION_VIEW_OWN);

            ValidateNotNull(request, nameof(request));

            var userId = _currentUser.UserId;
            LogInfo("➕ Mở ca làm việc mới - User: {UserId}", userId);

            // Check if user already has an active shift
            var currentShift = await _unitOfWork.Shifts.GetCurrentShiftAsync(userId);
            ThrowIf(currentShift != null,
                $"Bạn đang có ca làm việc đang mở (ShiftId: {currentShift!.Id}). Vui lòng đóng ca trước khi mở ca mới.");

            // Map and create
            var shift = _mapper.Map<Shift>(request);
            shift.UserId = userId;
            shift.Status = ShiftStatus.Open;
            shift.StartTime = DateTime.UtcNow;
            shift.CreatedAt = DateTime.UtcNow;
            shift.UpdatedAt = DateTime.UtcNow;
            //shift.CreatedBy = userId;

            await _unitOfWork.Shifts.AddAsync(shift);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<ShiftDto>(shift);

            // Invalidate caches
            await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX_LIST}:*");
            await InvalidateCacheAsync(CreateCacheKey(CACHE_PREFIX_CURRENT, userId));

            LogInfo("✅ Mở ca thành công - ShiftId: {ShiftId}, User: {UserId}",
                dto.Id, userId);

            return dto;

        }, "OpenShiftAsync");
    }

    /// <summary>
    /// Close an active shift
    /// Permission: hrm.view_own (employees can close their own shift) or hrm.manage (managers can close any shift)
    /// </summary>
    public async Task<ShiftDto> CloseShiftAsync(Guid id, CloseShiftRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(id, nameof(id));
            ValidateNotNull(request, nameof(request));

            var userId = _currentUser.UserId;
            LogInfo("🔒 Đóng ca làm việc - ShiftId: {ShiftId}, User: {UserId}", id, userId);

            var shift = await _unitOfWork.Shifts.GetByIdAsync(id);
            ThrowIf(shift == null, $"Không tìm thấy ca làm việc với ID: {id}");

            // 🔒 Check Permission
            var canManage = await _permissionService.CheckUserPermissionAsync(userId, PERMISSION_MANAGE);

            // If user cannot manage, they can only close their own shift
            if (!canManage)
            {
                ThrowIf(shift!.UserId != userId,
                    "Bạn chỉ có quyền đóng ca làm việc của chính mình");
            }

            // Validate shift is open
            ThrowIf(shift!.Status != ShiftStatus.Open,
                $"Chỉ có thể đóng ca đang mở (trạng thái hiện tại: {shift.Status})");

            // Update shift
            shift.Status = ShiftStatus.Closed;
            shift.EndTime = DateTime.UtcNow;
            //shift.Notes = request.Notes;
            shift.UpdatedAt = DateTime.UtcNow;
            //shift.UpdatedBy = userId;

            _unitOfWork.Shifts.Update(shift);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<ShiftDto>(shift);

            // Invalidate caches
            await InvalidateMultipleCachesAsync(
                CreateCacheKey(CACHE_PREFIX_SINGLE, id),
                CreateCacheKey(CACHE_PREFIX_CURRENT, shift.UserId)
            );
            await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX_LIST}:*");

            LogInfo("✅ Đóng ca thành công - ShiftId: {ShiftId}, Duration: {Duration} minutes",
                id, (shift.EndTime.Value - shift.StartTime).TotalMinutes);

            return dto;

        }, "CloseShiftAsync");
    }

    /// <summary>
    /// Update shift information (for managers)
    /// Permission: hrm.manage
    /// </summary>
    public async Task<ShiftDto> UpdateShiftAsync(Guid id, UpdateShiftRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_MANAGE);

            ValidateId(id, nameof(id));
            ValidateNotNull(request, nameof(request));

            LogInfo("✏️ Cập nhật shift - ID: {ShiftId}", id);

            var shift = await _unitOfWork.Shifts.GetByIdAsync(id);
            ThrowIf(shift == null, $"Không tìm thấy ca làm việc với ID: {id}");

            // --- Update Notes ---
            if (!string.IsNullOrWhiteSpace(request.Notes))
            {
                shift.Notes = request.Notes;
            }

            // --- Validate STATUS transition ---
            ThrowIf(
                shift.Status == ShiftStatus.Closed && request.Status != ShiftStatus.Closed
                , "Không thể mở lại ca đã đóng" // Nguyên tắc: ca đã đóng không được mở lại
                );
            

            // Nếu hợp lệ thì update status
            shift.Status = request.Status;

            shift.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Shifts.Update(shift);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<ShiftDto>(shift);

            // 🗑 Invalidate caches
            await InvalidateMultipleCachesAsync(
                CreateCacheKey(CACHE_PREFIX_SINGLE, id),
                CreateCacheKey(CACHE_PREFIX_CURRENT, shift.UserId)
            );
            await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX_LIST}:*");

            LogInfo("✅ Cập nhật shift thành công - ID: {ShiftId}", id);

            return dto;

        }, "UpdateShiftAsync");
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
    /// Generate cache key for shift list
    /// </summary>
    private string GenerateListCacheKey(PaginationParams pagination, ShiftFilters filters, string scope)
    {
        var parts = new List<string>
        {
            scope,
            pagination.Page.ToString(),
            pagination.PageSize.ToString(),
            pagination.SortBy,
            pagination.SortDescending.ToString()
        };

        if (filters.UserId.HasValue)
            parts.Add($"user:{filters.UserId.Value}");

        if (filters.Status.HasValue)
            parts.Add($"status:{filters.Status.Value}");

        if (filters.StartTimeFrom.HasValue)
            parts.Add($"from:{filters.StartTimeFrom.Value:yyyyMMddHHmm}");

        if (filters.StartTimeTo.HasValue)
            parts.Add($"to:{filters.StartTimeTo.Value:yyyyMMddHHmm}");

        return CreateCacheKey(CACHE_PREFIX_LIST, string.Join(":", parts));
    }

    #endregion
}