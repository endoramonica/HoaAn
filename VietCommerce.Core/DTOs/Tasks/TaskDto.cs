using System;
using VietCommerce.Core.Entities.Tasks;
using TaskStatus = VietCommerce.Core.Entities.Tasks.TaskStatus;

namespace VietCommerce.Core.DTOs.Tasks
{
    /// <summary>
    /// DTO for WorkTask response
    /// </summary>
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Assignee info
        public Guid AssignedTo { get; set; }
        public string AssignedToName { get; set; } = string.Empty;
        public string? AssignedToEmail { get; set; }

        // Creator info
        public Guid AssignedBy { get; set; }
        public string AssignedByName { get; set; } = string.Empty;

        // Task details
        public TaskPriority Priority { get; set; }
        public string PriorityName { get; set; } = string.Empty;

        public TaskStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Computed properties
        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.UtcNow && Status != TaskStatus.Completed;
        public int? DaysUntilDue => DueDate.HasValue ? (int?)(DueDate.Value - DateTime.UtcNow).TotalDays : null;
    }
}