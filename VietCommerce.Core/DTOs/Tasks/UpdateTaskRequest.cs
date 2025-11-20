using System;
using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Entities.Tasks;
using TaskStatus = VietCommerce.Core.Entities.Tasks.TaskStatus;

namespace VietCommerce.Core.DTOs.Tasks
{
    /// <summary>
    /// Request DTO for updating an existing task
    /// </summary>
    public class UpdateTaskRequest
    {
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public Guid? AssignedTo { get; set; }

        [EnumDataType(typeof(TaskPriority), ErrorMessage = "Invalid priority value")]
        public TaskPriority? Priority { get; set; }

        [EnumDataType(typeof(TaskStatus), ErrorMessage = "Invalid status value")]
        public TaskStatus? Status { get; set; }

        public DateTime? DueDate { get; set; }

        // Validation: DueDate must be in the future if provided
        public bool IsValid()
        {
            if (DueDate.HasValue && DueDate.Value < DateTime.UtcNow)
                return false;
            return true;
        }
    }
}