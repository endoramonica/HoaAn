using System;
using VietCommerce.Core.Entities.Tasks;
using TaskStatus = VietCommerce.Core.Entities.Tasks.TaskStatus;

namespace VietCommerce.Core.DTOs.Tasks
{
    /// <summary>
    /// Filter parameters for task queries
    /// </summary>
    public class TaskFilters
    {
        public Guid? UserId { get; set; }

        /// <summary>
        /// Filter by assignee ID
        /// </summary>
        public Guid? AssignedTo { get; set; }

        /// <summary>
        /// Filter by creator ID
        /// </summary>
        public Guid? AssignedBy { get; set; }

        /// <summary>
        /// Filter by task status
        /// </summary>
        public TaskStatus? Status { get; set; }

        /// <summary>
        /// Filter by priority
        /// </summary>
        public TaskPriority? Priority { get; set; }

        /// <summary>
        /// Filter by due date range - start
        /// </summary>
        public DateTime? DueDateFrom { get; set; }

        /// <summary>
        /// Filter by due date range - end
        /// </summary>
        public DateTime? DueDateTo { get; set; }

        /// <summary>
        /// Search by title or description
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Show only overdue tasks
        /// </summary>
        public bool? IsOverdue { get; set; }

        /// <summary>
        /// Filter by StoreId (if applicable)
        /// </summary>
        public Guid? StoreId { get; set; }
    }
}