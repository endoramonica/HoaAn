using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.DTOs.Tasks;
using VietCommerce.Core.Entities.Tasks;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;
using TaskStatus = VietCommerce.Core.Entities.Tasks.TaskStatus;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Service quản lý Tasks (công việc)
    /// ✅ Permission-based access control
    /// ✅ Redis caching với TTL
    /// ✅ Ownership validation (creator/assignee)
    /// ✅ ApiResponse pattern cho error handling
    /// 
    /// VERSION: 1.0
    /// CREATED: 2025-01-30
    /// </summary>
    public class TaskService : BaseService, ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPermissionService _permissionService;
        private readonly ICurrentUser _currentUser;
        private readonly IRealtimeService? _realtime;

        // Cache TTL constants
        private const int CACHE_MINUTES_LIST = 10;
        private const int CACHE_MINUTES_DETAIL = 15;

        public TaskService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPermissionService permissionService,
            ICurrentUser currentUser,
            ILogger<TaskService> logger,
            ICacheService cacheService,
            IRealtimeService? realtime = null)
            : base(logger, cacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _realtime = realtime;
        }

        #region GetTasksAsync - Lấy danh sách tasks với phân trang
        public async Task<PaginatedResult<TaskDto>> GetTasksAsync(
            PaginationParams pagination,
            TaskFilters filters)
        {
            return await ExecuteAsync(async () =>
            {
                LogInfo("🔹 GetTasksAsync - UserId: {UserId}, Page: {Page}, Filters: {@Filters}",
                    _currentUser.UserId, pagination.Page, filters);

                // ✅ 1. Permission check
                bool hasViewAll = await _permissionService.CheckUserPermissionAsync(
                    _currentUser.UserId, "tasks.view_all");
                bool hasViewOwn = await _permissionService.CheckUserPermissionAsync(
                    _currentUser.UserId, "tasks.view_own");

                if (!hasViewAll && !hasViewOwn)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền xem tasks");
                }

                // ✅ 2. Apply ownership filter nếu user không có view_all
                if (!hasViewAll)
                {
                    // User chỉ xem tasks của mình (assigned to hoặc created by)
                    filters.UserId = _currentUser.UserId;
                    LogDebug("🔒 User chỉ xem tasks của mình: {UserId}", _currentUser.UserId);
                }

                // ✅ 3. Cache key
                var cacheKey = CreateCacheKey("tasks", "list", _currentUser.UserId,
                    pagination.Page, pagination.PageSize, filters.GetHashCode());

                // ✅ 4. Get from cache or DB
                var result = await GetFromCacheOrExecuteAsync(cacheKey, async () =>
                {
                    var (items, totalCount) = await _unitOfWork.Tasks.GetTasksAsync(pagination, filters);
                    var taskDtos = _mapper.Map<List<TaskDto>>(items);

                    return new PaginatedResult<TaskDto>(
                        taskDtos,
                        pagination.Page,
                        pagination.PageSize,
                        totalCount);
                }, TimeSpan.FromMinutes(CACHE_MINUTES_LIST));

                LogInfo("✅ GetTasksAsync thành công - TotalItems: {Count}", result.TotalItems);
                return result;

            }, "GetTasksAsync");
        }
        #endregion

        #region GetTaskByIdAsync - Lấy task theo ID
        public async Task<TaskDto> GetTaskByIdAsync(string id)
        {
            return await ExecuteAsync(async () =>
            {
                // ✅ 1. Validate ID
                if (!Guid.TryParse(id, out var taskId))
                {
                    throw new ArgumentException("Task ID không hợp lệ", nameof(id));
                }

                LogInfo("🔹 GetTaskByIdAsync - TaskId: {TaskId}, UserId: {UserId}", taskId, _currentUser.UserId);

                // ✅ 2. Permission check
                bool hasViewAll = await _permissionService.CheckUserPermissionAsync(
                    _currentUser.UserId, "tasks.view_all");
                bool hasViewOwn = await _permissionService.CheckUserPermissionAsync(
                    _currentUser.UserId, "tasks.view_own");

                if (!hasViewAll && !hasViewOwn)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền xem tasks");
                }

                // ✅ 3. Cache key
                var cacheKey = CreateCacheKey("tasks", "detail", taskId);

                // ✅ 4. Get from cache or DB
                var taskDto = await GetFromCacheOrExecuteAsync(cacheKey, async () =>
                {
                    var task = await _unitOfWork.Tasks.GetTaskByIdWithDetailsAsync(taskId);
                    if (task == null)
                    {
                        throw new KeyNotFoundException($"Không tìm thấy task với ID: {taskId}");
                    }

                    return _mapper.Map<TaskDto>(task);
                }, TimeSpan.FromMinutes(CACHE_MINUTES_DETAIL));

                // ✅ 5. Ownership check nếu user không có view_all
                if (!hasViewAll)
                {
                    bool isOwner = taskDto.AssignedTo == _currentUser.UserId ||
                                   taskDto.AssignedBy == _currentUser.UserId;

                    if (!isOwner)
                    {
                        throw new UnauthorizedAccessException("Bạn không có quyền xem task này");
                    }
                }

                LogInfo("✅ GetTaskByIdAsync thành công - TaskId: {TaskId}", taskId);
                return taskDto;

            }, "GetTaskByIdAsync");
        }
        #endregion

        #region CreateTaskAsync - Tạo task mới
        public async Task<TaskDto> CreateTaskAsync(CreateTaskRequest request)
        {
            return await ExecuteAsync(async () =>
            {
                LogInfo("🔹 CreateTaskAsync - UserId: {UserId}, Request: {@Request}",
                    _currentUser.UserId, request);

                // ✅ 1. Permission check
                bool hasPermission = await _permissionService.CheckUserPermissionAsync(
                    _currentUser.UserId, "tasks.create");

                if (!hasPermission)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền tạo task");
                }

                // ✅ 2. Validate request
                ValidateNotEmpty(request.Title, nameof(request.Title));
                ValidateId(request.AssignedTo, nameof(request.AssignedTo));

                // ✅ 3. Validate assignee exists
                var assigneeExists = await _unitOfWork.Users.AnyAsync(u => u.Id == request.AssignedTo);
                if (!assigneeExists)
                {
                    throw new ArgumentException($"Người được giao không tồn tại: {request.AssignedTo}");
                }

                // ✅ 4. Map và set default values
                var task = _mapper.Map<WorkTask>(request);
                task.AssignedBy = _currentUser.UserId;
                task.Status = TaskStatus.Pending;
                task.CreatedAt = DateTime.UtcNow;
                task.UpdatedAt = DateTime.UtcNow;

                // ✅ 5. Save to DB
                await _unitOfWork.Tasks.AddAsync(task);
                await _unitOfWork.SaveChangesAsync();

                // ✅ 6. Invalidate caches
                await InvalidateTaskCachesAsync(task.Id, request.AssignedTo);

                // ✅ 7. Load task with details và map to DTO
                var createdTask = await _unitOfWork.Tasks.GetTaskByIdWithDetailsAsync(task.Id);
                var taskDto = _mapper.Map<TaskDto>(createdTask);

                LogInfo("✅ CreateTaskAsync thành công - TaskId: {TaskId}", task.Id);
                if (_realtime != null)
                    await _realtime.BroadcastTaskUpdatedAsync(taskDto);
                return taskDto;

            }, "CreateTaskAsync");
        }
        #endregion

        #region UpdateTaskAsync - Cập nhật task
        public async Task<TaskDto> UpdateTaskAsync(string id, UpdateTaskRequest request)
        {
            return await ExecuteAsync(async () =>
            {
                // ✅ 1. Validate ID
                if (!Guid.TryParse(id, out var taskId))
                {
                    throw new ArgumentException("Task ID không hợp lệ", nameof(id));
                }

                LogInfo("🔹 UpdateTaskAsync - TaskId: {TaskId}, UserId: {UserId}, Request: {@Request}",
                    taskId, _currentUser.UserId, request);

                // ✅ 2. Get existing task
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy task với ID: {taskId}");
                }

                // ✅ 3. Permission check
                bool hasUpdateAll = await _permissionService.CheckUserPermissionAsync(
                    _currentUser.UserId, "tasks.update_all");
                bool hasUpdateOwn = await _permissionService.CheckUserPermissionAsync(
                    _currentUser.UserId, "tasks.update_own");

                if (!hasUpdateAll && !hasUpdateOwn)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền cập nhật task");
                }

                // ✅ 4. Ownership check nếu user không có update_all
                if (!hasUpdateAll)
                {
                    bool isCreator = task.AssignedBy == _currentUser.UserId;
                    if (!isCreator)
                    {
                        throw new UnauthorizedAccessException("Bạn chỉ có thể cập nhật task do mình tạo");
                    }
                }

                // ✅ 5. Validate assignee nếu có thay đổi
                if (request.AssignedTo.HasValue)
                {
                    var assigneeExists = await _unitOfWork.Users.AnyAsync(u => u.Id == request.AssignedTo.Value);
                    if (!assigneeExists)
                    {
                        throw new ArgumentException($"Người được giao không tồn tại: {request.AssignedTo}");
                    }
                }

                // ✅ 6. Map changes (chỉ update fields không null)
                _mapper.Map(request, task);
                task.UpdatedAt = DateTime.UtcNow;

                // ✅ 7. Special handling: Set CompletedAt khi status = Completed
                if (request.Status == TaskStatus.Completed && task.CompletedAt == null)
                {
                    task.CompletedAt = DateTime.UtcNow;
                    LogDebug("📝 Task completed at: {Time}", task.CompletedAt);
                }

                // ✅ 8. Update in DB
                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();

                // ✅ 9. Invalidate caches
                await InvalidateTaskCachesAsync(taskId, task.AssignedTo);

                // ✅ 10. Load updated task và map to DTO
                var updatedTask = await _unitOfWork.Tasks.GetTaskByIdWithDetailsAsync(taskId);
                var taskDto = _mapper.Map<TaskDto>(updatedTask);

                LogInfo("✅ UpdateTaskAsync thành công - TaskId: {TaskId}", taskId);
                if (_realtime != null)
                    await _realtime.BroadcastTaskUpdatedAsync(taskDto);
                return taskDto;

            }, "UpdateTaskAsync");
        }
        #endregion

        #region UpdateTaskStatusAsync - Cập nhật trạng thái task
        public async Task<TaskDto> UpdateTaskStatusAsync(string id, TaskStatus status)
        {
            return await ExecuteAsync(async () =>
            {
                // ✅ 1. Validate ID
                if (!Guid.TryParse(id, out var taskId))
                {
                    throw new ArgumentException("Task ID không hợp lệ", nameof(id));
                }

                LogInfo("🔹 UpdateTaskStatusAsync - TaskId: {TaskId}, Status: {Status}, UserId: {UserId}",
                    taskId, status, _currentUser.UserId);

                // ✅ 2. Get existing task
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy task với ID: {taskId}");
                }

                // ✅ 3. Permission check
                bool hasPermission = await _permissionService.CheckUserPermissionAsync(
                    _currentUser.UserId, "tasks.update_status");
                bool isAssignee = task.AssignedTo == _currentUser.UserId;

                if (!hasPermission && !isAssignee)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền cập nhật trạng thái task này");
                }

                // ✅ 4. Update status
                task.Status = status;
                task.UpdatedAt = DateTime.UtcNow;

                // ✅ 5. Set CompletedAt nếu status = Completed
                if (status == TaskStatus.Completed && task.CompletedAt == null)
                {
                    task.CompletedAt = DateTime.UtcNow;
                    LogDebug("📝 Task completed at: {Time}", task.CompletedAt);
                }

                // ✅ 6. Update in DB
                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();

                // ✅ 7. Invalidate caches
                await InvalidateTaskCachesAsync(taskId, task.AssignedTo);

                // ✅ 8. Load updated task và map to DTO
                var updatedTask = await _unitOfWork.Tasks.GetTaskByIdWithDetailsAsync(taskId);
                var taskDto = _mapper.Map<TaskDto>(updatedTask);

                LogInfo("✅ UpdateTaskStatusAsync thành công - TaskId: {TaskId}, Status: {Status}",
                    taskId, status);
                if (_realtime != null)
                    await _realtime.BroadcastTaskUpdatedAsync(taskDto);
                return taskDto;

            }, "UpdateTaskStatusAsync");
        }
        #endregion

        #region DeleteTaskAsync - Xóa task
        public async Task DeleteTaskAsync(string id)
        {
            await ExecuteAsync(async () =>
            {
                // ✅ 1. Validate ID
                if (!Guid.TryParse(id, out var taskId))
                {
                    throw new ArgumentException("Task ID không hợp lệ", nameof(id));
                }

                LogInfo("🔹 DeleteTaskAsync - TaskId: {TaskId}, UserId: {UserId}", taskId, _currentUser.UserId);

                // ✅ 2. Get existing task
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
                if (task == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy task với ID: {taskId}");
                }

                // ✅ 3. Permission check
                bool hasDeleteAll = await _permissionService.CheckUserPermissionAsync(
                    _currentUser.UserId, "tasks.delete_all");
                bool hasDeleteOwn = await _permissionService.CheckUserPermissionAsync(
                    _currentUser.UserId, "tasks.delete_own");

                if (!hasDeleteAll && !hasDeleteOwn)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền xóa task");
                }

                // ✅ 4. Ownership check nếu user không có delete_all
                if (!hasDeleteAll)
                {
                    bool isCreator = task.AssignedBy == _currentUser.UserId;
                    if (!isCreator)
                    {
                        throw new UnauthorizedAccessException("Bạn chỉ có thể xóa task do mình tạo");
                    }
                }

                // ✅ 5. Hard delete
                _unitOfWork.Tasks.Delete(task);
                await _unitOfWork.SaveChangesAsync();

                // ✅ 6. Invalidate caches
                await InvalidateTaskCachesAsync(taskId, task.AssignedTo);

                LogInfo("✅ DeleteTaskAsync thành công - TaskId: {TaskId}", taskId);

            }, "DeleteTaskAsync");
        }
        #endregion

        #region AssignTaskAsync - Gán task cho user khác
        public async Task AssignTaskAsync(string taskId, string assigneeId)
        {
            await ExecuteAsync(async () =>
            {
                // ✅ 1. Validate IDs
                if (!Guid.TryParse(taskId, out var taskGuid))
                {
                    throw new ArgumentException("Task ID không hợp lệ", nameof(taskId));
                }
                if (!Guid.TryParse(assigneeId, out var assigneeGuid))
                {
                    throw new ArgumentException("Assignee ID không hợp lệ", nameof(assigneeId));
                }

                LogInfo("🔹 AssignTaskAsync - TaskId: {TaskId}, AssigneeId: {AssigneeId}, UserId: {UserId}",
                    taskGuid, assigneeGuid, _currentUser.UserId);

                // ✅ 2. Permission check
                bool hasPermission = await _permissionService.CheckUserPermissionAsync(
                    _currentUser.UserId, "tasks.assign");

                if (!hasPermission)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền gán task");
                }

                // ✅ 3. Get existing task
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskGuid);
                if (task == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy task với ID: {taskGuid}");
                }

                // ✅ 4. Validate assignee exists
                var assigneeExists = await _unitOfWork.Users.AnyAsync(u => u.Id == assigneeGuid);
                if (!assigneeExists)
                {
                    throw new ArgumentException($"Người được giao không tồn tại: {assigneeGuid}");
                }

                // ✅ 5. Update assignee
                var oldAssigneeId = task.AssignedTo;
                task.AssignedTo = assigneeGuid;
                task.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();

                // ✅ 6. Invalidate caches (cả old và new assignee)
                await InvalidateTaskCachesAsync(taskGuid, oldAssigneeId);
                await InvalidateTaskCachesAsync(taskGuid, assigneeGuid);

                LogInfo("✅ AssignTaskAsync thành công - TaskId: {TaskId}, NewAssignee: {AssigneeId}",
                    taskGuid, assigneeGuid);

            }, "AssignTaskAsync");
        }
        #endregion

        #region CompleteTaskAsync - Hoàn thành task
        public async Task CompleteTaskAsync(string taskId, string completionNote)
        {
            await ExecuteAsync(async () =>
            {
                // ✅ 1. Validate ID
                if (!Guid.TryParse(taskId, out var taskGuid))
                {
                    throw new ArgumentException("Task ID không hợp lệ", nameof(taskId));
                }

                LogInfo("🔹 CompleteTaskAsync - TaskId: {TaskId}, UserId: {UserId}", taskGuid, _currentUser.UserId);

                // ✅ 2. Get existing task
                var task = await _unitOfWork.Tasks.GetByIdAsync(taskGuid);
                if (task == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy task với ID: {taskGuid}");
                }

                // ✅ 3. Permission check
                bool hasPermission = await _permissionService.CheckUserPermissionAsync(
                    _currentUser.UserId, "tasks.complete");
                bool isAssignee = task.AssignedTo == _currentUser.UserId;

                if (!hasPermission && !isAssignee)
                {
                    throw new UnauthorizedAccessException("Chỉ người được giao mới có thể hoàn thành task này");
                }

                // ✅ 4. Validate task chưa completed
                if (task.Status == TaskStatus.Completed)
                {
                    throw new InvalidOperationException("Task đã được hoàn thành trước đó");
                }

                // ✅ 5. Update task
                task.Status = TaskStatus.Completed;
                task.CompletedAt = DateTime.UtcNow;
                task.UpdatedAt = DateTime.UtcNow;
                // NOTE: completionNote parameter bị bỏ qua vì WorkTask không có field này

                _unitOfWork.Tasks.Update(task);
                await _unitOfWork.SaveChangesAsync();

                // ✅ 6. Invalidate caches
                await InvalidateTaskCachesAsync(taskGuid, task.AssignedTo);

                LogInfo("✅ CompleteTaskAsync thành công - TaskId: {TaskId}, CompletedAt: {Time}",
                    taskGuid, task.CompletedAt);

            }, "CompleteTaskAsync");
        }
        #endregion

        #region GetTasksByAssigneeAsync - Lấy tasks theo assignee
        public async Task<PaginatedResult<TaskDto>> GetTasksByAssigneeAsync(
            string assigneeId,
            PaginationParams pagination)
        {
            return await ExecuteAsync(async () =>
            {
                // ✅ 1. Validate ID
                if (!Guid.TryParse(assigneeId, out var assigneeGuid))
                {
                    throw new ArgumentException("Assignee ID không hợp lệ", nameof(assigneeId));
                }

                LogInfo("🔹 GetTasksByAssigneeAsync - AssigneeId: {AssigneeId}, UserId: {UserId}, Page: {Page}",
                    assigneeGuid, _currentUser.UserId, pagination.Page);

                // ✅ 2. Permission check
                bool hasViewAll = await _permissionService.CheckUserPermissionAsync(
                    _currentUser.UserId, "tasks.view_all");
                bool hasViewOwn = await _permissionService.CheckUserPermissionAsync(
                    _currentUser.UserId, "tasks.view_own");

                if (!hasViewAll && !hasViewOwn)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền xem tasks");
                }

                // ✅ 3. Ownership check nếu user không có view_all
                if (!hasViewAll && assigneeGuid != _currentUser.UserId)
                {
                    throw new UnauthorizedAccessException("Bạn chỉ có thể xem tasks của mình");
                }

                // ✅ 4. Cache key
                var cacheKey = CreateCacheKey("tasks", "assignee", assigneeGuid,
                    pagination.Page, pagination.PageSize);

                // ✅ 5. Get from cache or DB
                var result = await GetFromCacheOrExecuteAsync(cacheKey, async () =>
                {
                    var (items, totalCount) = await _unitOfWork.Tasks.GetTasksByAssigneeAsync(
                        assigneeGuid, pagination);
                    var taskDtos = _mapper.Map<List<TaskDto>>(items);

                    return new PaginatedResult<TaskDto>(
                        taskDtos,
                        pagination.Page,
                        pagination.PageSize,
                        totalCount);
                }, TimeSpan.FromMinutes(CACHE_MINUTES_LIST));

                LogInfo("✅ GetTasksByAssigneeAsync thành công - TotalItems: {Count}", result.TotalItems);
                return result;

            }, "GetTasksByAssigneeAsync");
        }
        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Invalidate tất cả cache liên quan đến task
        /// </summary>
        private async Task InvalidateTaskCachesAsync(Guid taskId, Guid assigneeId)
        {
            var keys = new List<string>
            {
                CreateCacheKey("tasks", "detail", taskId),
                CreateCacheKey("tasks", "assignee", assigneeId, "*")
            };

            await InvalidateMultipleCachesAsync(keys.ToArray());
            await InvalidateCacheByPrefixAsync("tasks:list:*");

            LogDebug("🗑️ Invalidated task caches for TaskId: {TaskId}, AssigneeId: {AssigneeId}",
                taskId, assigneeId);
        }

        #endregion
    }
}
