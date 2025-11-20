using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Tasks;
using VietCommerce.Core.Entities.Tasks;
using VietCommerce.Core.Models;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;
using TaskStatus = VietCommerce.Core.Entities.Tasks.TaskStatus;

namespace VietCommerce.Data.Repositories
{
    /// <summary>
    /// Repository implementation for WorkTask
    /// Provides optimized queries with Include for navigation properties
    /// </summary>
    public class TaskRepository : GenericRepository<WorkTask>, ITaskRepository
    {
        public TaskRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Get paginated tasks with filters and includes
        /// </summary>
        public async Task<(IEnumerable<WorkTask> Items, int TotalCount)> GetTasksAsync(
            PaginationParams pagination,
            TaskFilters filters)
        {
            var query = _dbSet
                .Include(t => t.AssignedToUser)
                .Include(t => t.AssignedByUser)
                .AsQueryable();

            // Apply filters
            if (filters.AssignedTo.HasValue)
                query = query.Where(t => t.AssignedTo == filters.AssignedTo.Value);

            if (filters.AssignedBy.HasValue)
                query = query.Where(t => t.AssignedBy == filters.AssignedBy.Value);

            if (filters.Status.HasValue)
                query = query.Where(t => t.Status == filters.Status.Value);

            if (filters.Priority.HasValue)
                query = query.Where(t => t.Priority == filters.Priority.Value);

            if (filters.DueDateFrom.HasValue)
                query = query.Where(t => t.DueDate >= filters.DueDateFrom.Value);

            if (filters.DueDateTo.HasValue)
                query = query.Where(t => t.DueDate <= filters.DueDateTo.Value);

            if (filters.IsOverdue.HasValue && filters.IsOverdue.Value)
                query = query.Where(t => t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed);

            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            {
                var searchLower = filters.SearchTerm.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(searchLower) ||
                    t.Description.ToLower().Contains(searchLower));
            }

            if (filters.StoreId.HasValue)
            {
                query = query.Where(t =>
                    t.AssignedToUser.StoreId == filters.StoreId.Value ||
                    t.AssignedByUser.StoreId == filters.StoreId.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, pagination);

            // Apply pagination
            var items = await query
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        /// <summary>
        /// Get task by ID with navigation properties loaded
        /// </summary>
        public async Task<WorkTask?> GetTaskByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(t => t.AssignedToUser)
                .Include(t => t.AssignedByUser)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        /// <summary>
        /// Get tasks assigned to a user with pagination
        /// </summary>
        public async Task<(IEnumerable<WorkTask> Items, int TotalCount)> GetTasksByAssigneeAsync(
            Guid assigneeId,
            PaginationParams pagination)
        {
            var query = _dbSet
                .Include(t => t.AssignedToUser)
                .Include(t => t.AssignedByUser)
                .Where(t => t.AssignedTo == assigneeId);

            var totalCount = await query.CountAsync();

            query = ApplySorting(query, pagination);

            var items = await query
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        /// <summary>
        /// Get tasks created by a user with pagination
        /// </summary>
        public async Task<(IEnumerable<WorkTask> Items, int TotalCount)> GetTasksByCreatorAsync(
            Guid creatorId,
            PaginationParams pagination)
        {
            var query = _dbSet
                .Include(t => t.AssignedToUser)
                .Include(t => t.AssignedByUser)
                .Where(t => t.AssignedBy == creatorId);

            var totalCount = await query.CountAsync();

            query = ApplySorting(query, pagination);

            var items = await query
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        /// <summary>
        /// Get tasks by status
        /// </summary>
        public async Task<IEnumerable<WorkTask>> GetTasksByStatusAsync(TaskStatus status)
        {
            return await _dbSet
                .Include(t => t.AssignedToUser)
                .Include(t => t.AssignedByUser)
                .Where(t => t.Status == status)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        /// <summary>
        /// Get overdue tasks (DueDate passed + not completed)
        /// </summary>
        public async Task<IEnumerable<WorkTask>> GetOverdueTasksAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Include(t => t.AssignedToUser)
                .Include(t => t.AssignedByUser)
                .Where(t => t.DueDate < now && t.Status != TaskStatus.Completed)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        /// <summary>
        /// Get tasks due within date range
        /// </summary>
        public async Task<IEnumerable<WorkTask>> GetTasksDueInRangeAsync(DateTime from, DateTime to)
        {
            return await _dbSet
                .Include(t => t.AssignedToUser)
                .Include(t => t.AssignedByUser)
                .Where(t => t.DueDate >= from && t.DueDate <= to)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        /// <summary>
        /// Check if user is assigned to a task
        /// </summary>
        public async Task<bool> IsUserAssignedToTaskAsync(Guid taskId, Guid userId)
        {
            return await _dbSet.AnyAsync(t => t.Id == taskId && t.AssignedTo == userId);
        }

        /// <summary>
        /// Get task counts grouped by status for a user
        /// </summary>
        public async Task<Dictionary<TaskStatus, int>> GetTaskCountsByStatusAsync(Guid userId)
        {
            return await _dbSet
                .Where(t => t.AssignedTo == userId)
                .GroupBy(t => t.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        /// <summary>
        /// Get tasks by store ID
        /// </summary>
        public async Task<IEnumerable<WorkTask>> GetTasksByStoreAsync(Guid storeId)
        {
            return await _dbSet
                .Include(t => t.AssignedToUser)
                .Include(t => t.AssignedByUser)
                .Where(t => t.AssignedToUser.StoreId == storeId || t.AssignedByUser.StoreId == storeId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        #region Helper Methods

        /// <summary>
        /// Apply sorting based on pagination params
        /// </summary>
        private IQueryable<WorkTask> ApplySorting(IQueryable<WorkTask> query, PaginationParams pagination)
        {
            return pagination.SortBy?.ToLower() switch
            {
                "title" => pagination.SortDescending
                    ? query.OrderByDescending(t => t.Title)
                    : query.OrderBy(t => t.Title),

                "priority" => pagination.SortDescending
                    ? query.OrderByDescending(t => t.Priority)
                    : query.OrderBy(t => t.Priority),

                "status" => pagination.SortDescending
                    ? query.OrderByDescending(t => t.Status)
                    : query.OrderBy(t => t.Status),

                "duedate" => pagination.SortDescending
                    ? query.OrderByDescending(t => t.DueDate)
                    : query.OrderBy(t => t.DueDate),

                "createdat" => pagination.SortDescending
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt),

                _ => pagination.SortDescending
                    ? query.OrderByDescending(t => t.CreatedAt)
                    : query.OrderBy(t => t.CreatedAt)
            };
        }

        #endregion
    }
}