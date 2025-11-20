using System;
using System.ComponentModel.DataAnnotations;
using VietCommerce.Core.Entities.Tasks;

namespace VietCommerce.Core.DTOs.Tasks
{
    /// <summary>
    /// Request DTO for creating a new task
    /// </summary>
    public class CreateTaskRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "AssignedTo is required")]
        public Guid AssignedTo { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        [EnumDataType(typeof(TaskPriority), ErrorMessage = "Invalid priority value")]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public DateTime? DueDate { get; set; }

        // Validation: DueDate must be in the future
        public bool IsValid()
        {
            if (DueDate.HasValue && DueDate.Value < DateTime.UtcNow)
                return false;
            return true;
        }
    }
}