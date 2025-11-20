using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Tasks;
using VietCommerce.Core.Entities.Tasks;
using VietCommerce.Core.Models;
using TaskStatus = VietCommerce.Core.Entities.Tasks.TaskStatus;

namespace VietCommerce.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for WorkTask operations
    /// Extends generic repository with task-specific queries
    /// </summary>
    public interface ITaskRepository : IGenericRepository<WorkTask>
    {
        /// <summary>
        /// Get paginated tasks with filters
        /// </summary>
        Task<(IEnumerable<WorkTask> Items, int TotalCount)> GetTasksAsync(
            PaginationParams pagination,
            TaskFilters filters);

        /// <summary>
        /// Get task by ID with related entities (User navigation properties)
        /// </summary>
        Task<WorkTask?> GetTaskByIdWithDetailsAsync(Guid id);

        /// <summary>
        /// Get paginated tasks assigned to a specific user
        /// </summary>
        Task<(IEnumerable<WorkTask> Items, int TotalCount)> GetTasksByAssigneeAsync(
            Guid assigneeId,
            PaginationParams pagination);

        /// <summary>
        /// Get tasks created by a specific user
        /// </summary>
        Task<(IEnumerable<WorkTask> Items, int TotalCount)> GetTasksByCreatorAsync(
            Guid creatorId,
            PaginationParams pagination);

        /// <summary>
        /// Get tasks by status
        /// </summary>
        Task<IEnumerable<WorkTask>> GetTasksByStatusAsync(TaskStatus status);

        /// <summary>
        /// Get overdue tasks
        /// </summary>
        Task<IEnumerable<WorkTask>> GetOverdueTasksAsync();

        /// <summary>
        /// Get tasks due within a date range
        /// </summary>
        Task<IEnumerable<WorkTask>> GetTasksDueInRangeAsync(DateTime from, DateTime to);

        /// <summary>
        /// Check if user is assigned to a task
        /// </summary>
        Task<bool> IsUserAssignedToTaskAsync(Guid taskId, Guid userId);

        /// <summary>
        /// Count tasks by status for a specific user
        /// </summary>
        Task<Dictionary<TaskStatus, int>> GetTaskCountsByStatusAsync(Guid userId);

        /// <summary>
        /// Get tasks by store ID
        /// </summary>
        Task<IEnumerable<WorkTask>> GetTasksByStoreAsync(Guid storeId);
    }
}