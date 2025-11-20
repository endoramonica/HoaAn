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
/// Service implementation for managing leave requests
/// Implements business logic with validation, permission checks, and approval workflows
/// 
/// FEATURES:
/// ✅ CRUD operations with validation
/// ✅ Permission Checks (RBAC) - hrm.submit_leave_requests, hrm.approve_leave_requests
/// ✅ Self-service support (employees can submit their own requests)
/// ✅ Approval workflow
/// ✅ Status tracking
/// ✅ Redis caching with TTL
/// </summary>
public class LeaveRequestService : BaseService, ILeaveRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUser _currentUser;

    // Cache settings
    private const string CACHE_PREFIX_SINGLE = "leave_request";
    private const string CACHE_PREFIX_LIST = "leave_requests:list";
    private static readonly TimeSpan CACHE_DURATION_SINGLE = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan CACHE_DURATION_LIST = TimeSpan.FromMinutes(5);

    // Permission constants
    private const string PERMISSION_SUBMIT = "hrm.submit_leave_requests";
    private const string PERMISSION_APPROVE = "hrm.approve_leave_requests";
    private const string PERMISSION_VIEW_ALL = "hrm.view_all";

    public LeaveRequestService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<LeaveRequestService> logger,
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
    /// Get paginated list of leave requests with filtering
    /// Permission: hrm.approve_leave_requests (all requests) or hrm.submit_leave_requests (own requests only)
    /// </summary>
    public async Task<PaginatedResult<LeaveRequestDto>> GetLeaveRequestsAsync(
        PaginationParams pagination,
        LeaveRequestFilters filters)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateNotNull(pagination, nameof(pagination));
            ValidateNotNull(filters, nameof(filters));

            var userId = _currentUser.UserId;
            ThrowIf(userId == Guid.Empty, "Không xác định được người dùng hiện tại (Unauthorized)");

            LogInfo("📋 Lấy danh sách leave requests - Page: {Page}, User: {UserId}",
                pagination.Page, userId);

            // 🔒 Check Permission
            var canApprove = await _permissionService.CheckUserPermissionAsync(userId, PERMISSION_APPROVE);
            var canViewAll = await _permissionService.CheckUserPermissionAsync(userId, PERMISSION_VIEW_ALL);
            var canSubmit = await _permissionService.CheckUserPermissionAsync(userId, PERMISSION_SUBMIT);

            ThrowIf(!canApprove && !canViewAll && !canSubmit,
                "Bạn không có quyền xem danh sách đơn nghỉ phép");

            // If user can only submit (not approve/view all), filter by their employee ID
            if (!canApprove && !canViewAll && canSubmit)
            {
                var employee = await _unitOfWork.Employees.GetByIdAsync(userId);
                ThrowIf(employee == null, "Không tìm thấy thông tin nhân viên của bạn");
                filters.EmployeeId = userId;
                LogDebug("🔒 User chỉ xem được đơn của mình - EmployeeId: {EmployeeId}", employee.Id);
            }

            var cacheKey = GenerateListCacheKey(pagination, filters,
                (canApprove || canViewAll) ? "all" : userId.ToString());

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var (items, totalCount) = await _unitOfWork.LeaveRequests
                    .GetLeaveRequestsPagedAsync(pagination, filters);

                var dtos = _mapper.Map<List<LeaveRequestDto>>(items);

                var result = new PaginatedResult<LeaveRequestDto>(
                    dtos,
                    pagination.Page,
                    pagination.PageSize,
                    totalCount
                );

                LogInfo("✅ Tìm thấy {Count} leave requests (Total: {Total})", dtos.Count, totalCount);
                return result;

            }, CACHE_DURATION_LIST);

        }, "GetLeaveRequestsAsync");
    }

    /// <summary>
    /// Get leave request details by ID
    /// Permission: hrm.approve_leave_requests (any request) or own request
    /// </summary>
    public async Task<LeaveRequestDto> GetLeaveRequestByIdAsync(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(id, nameof(id));

            var userId = _currentUser.UserId;
            ThrowIf(userId == Guid.Empty, "Không xác định được người dùng hiện tại (Unauthorized)");

            LogInfo("🔍 Lấy thông tin leave request - ID: {LeaveRequestId}", id);

            var cacheKey = CreateCacheKey(CACHE_PREFIX_SINGLE, id);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var leaveRequest = await _unitOfWork.LeaveRequests.GetLeaveRequestWithDetailsAsync(id);
                ThrowIf(leaveRequest == null, $"Không tìm thấy đơn nghỉ phép với ID: {id}");

                // 🔒 Check Permission
                var canApprove = await _permissionService.CheckUserPermissionAsync(userId, PERMISSION_APPROVE);
                var canViewAll = await _permissionService.CheckUserPermissionAsync(userId, PERMISSION_VIEW_ALL);

                // If user cannot approve/view all, verify it's their own request
                if (!canApprove && !canViewAll)
                {
                    var employee = await _unitOfWork.Employees.GetByIdAsync(userId);
                    ThrowIf(employee == null || leaveRequest!.EmployeeId != employee.Id,
                        "Bạn chỉ có quyền xem đơn nghỉ phép của chính mình");
                }

                var dto = _mapper.Map<LeaveRequestDto>(leaveRequest);

                LogInfo("✅ Tìm thấy leave request: Employee={EmployeeId}, Type={LeaveType}",
                    dto.EmployeeId, dto.LeaveType);
                return dto;

            }, CACHE_DURATION_SINGLE);

        }, "GetLeaveRequestByIdAsync");
    }

    /// <summary>
    /// Create a new leave request
    /// Permission: hrm.submit_leave_requests
    /// </summary>
    public async Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_SUBMIT);

            ValidateNotNull(request, nameof(request));

            var userId = _currentUser.UserId;
            LogInfo("➕ Tạo leave request mới - User: {UserId}, LeaveType: {LeaveType}",
                userId, request.LeaveType);

            // Validate dates
            ThrowIf(request.EndDate < request.StartDate,
                "Ngày kết thúc phải sau ngày bắt đầu");

            ThrowIf(request.StartDate < DateTime.UtcNow.Date,
                "Không thể tạo đơn nghỉ phép cho ngày trong quá khứ");

            // Get employee of current user
            var employee = await _unitOfWork.Employees.GetByIdAsync(userId);
            ThrowIf(employee == null, "Không tìm thấy thông tin nhân viên của bạn");

            // Check for overlapping leave requests
            var hasOverlap = await _unitOfWork.LeaveRequests
                .HasOverlappingLeaveAsync(employee!.Id, request.StartDate, request.EndDate);
            ThrowIf(hasOverlap,
                "Bạn đã có đơn nghỉ phép trong khoảng thời gian này");

            // Map and create
            var leaveRequest = _mapper.Map<LeaveRequest>(request);
            leaveRequest.EmployeeId = employee.Id;
            leaveRequest.Status = LeaveRequestStatus.Pending;
            leaveRequest.CreatedAt = DateTime.UtcNow;
            leaveRequest.UpdatedAt = DateTime.UtcNow;
            //leaveRequest.CreatedBy = userId;

            await _unitOfWork.LeaveRequests.AddAsync(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<LeaveRequestDto>(leaveRequest);

            // Invalidate list cache
            await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX_LIST}:*");

            LogInfo("✅ Tạo leave request thành công - ID: {LeaveRequestId}, Employee: {EmployeeId}",
                dto.Id, dto.EmployeeId);

            return dto;

        }, "CreateLeaveRequestAsync");
    }

    /// <summary>
    /// Update leave request status (approve/reject)
    /// Permission: hrm.approve_leave_requests
    /// </summary>
    public async Task<LeaveRequestDto> UpdateLeaveRequestStatusAsync(
        Guid id,
        LeaveRequestStatus status,
        string? comments = null)
    {
        return await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_APPROVE);

            ValidateId(id, nameof(id));

            var userId = _currentUser.UserId;
            LogInfo("✏️ Cập nhật status leave request - ID: {LeaveRequestId}, NewStatus: {Status}",
                id, status);

            var leaveRequest = await _unitOfWork.LeaveRequests.GetByIdAsync(id);
            ThrowIf(leaveRequest == null, $"Không tìm thấy đơn nghỉ phép với ID: {id}");

            // Validate status transition
            ThrowIf(leaveRequest!.Status != LeaveRequestStatus.Pending,
                $"Chỉ có thể phê duyệt đơn đang ở trạng thái Pending (hiện tại: {leaveRequest.Status})");

            ThrowIf(status == LeaveRequestStatus.Pending,
                "Không thể chuyển về trạng thái Pending");

            var oldStatus = leaveRequest.Status;
            leaveRequest.Status = status;
            leaveRequest.ApprovedBy = userId;
            //leaveRequest.ApprovedAt = DateTime.UtcNow;
            leaveRequest.Comments = comments;
            leaveRequest.UpdatedAt = DateTime.UtcNow;
           //leaveRequest.UpdatedBy = userId;

            _unitOfWork.LeaveRequests.Update(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<LeaveRequestDto>(leaveRequest);

            // Invalidate caches
            await InvalidateMultipleCachesAsync(
                CreateCacheKey(CACHE_PREFIX_SINGLE, id)
            );
            await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX_LIST}:*");

            LogInfo("✅ Cập nhật status thành công - ID: {LeaveRequestId}, {OldStatus} → {NewStatus}",
                id, oldStatus, status);

            return dto;

        }, "UpdateLeaveRequestStatusAsync");
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
    /// Generate cache key for leave request list
    /// </summary>
    private string GenerateListCacheKey(PaginationParams pagination, LeaveRequestFilters filters, string scope)
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

        if (filters.Status.HasValue)
            parts.Add($"status:{filters.Status.Value}");

        if (filters.LeaveType.HasValue)
            parts.Add($"type:{filters.LeaveType.Value}");

        if (filters.StartDate.HasValue)
            parts.Add($"start:{filters.StartDate.Value:yyyyMMdd}");

        if (filters.EndDate.HasValue)
            parts.Add($"end:{filters.EndDate.Value:yyyyMMdd}");

        return CreateCacheKey(CACHE_PREFIX_LIST, string.Join(":", parts));
    }

    #endregion
}