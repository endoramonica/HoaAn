using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Core.Entities.Tasks
{
    [Table("Tasks")]
    [Index(nameof(AssignedTo), nameof(Status))]
    [Index(nameof(Priority))]
    [Index(nameof(DueDate))]
    public class WorkTask : BaseEntity
    {
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(36)]
        public string AssignedTo { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(36)]
        public string AssignedBy { get; set; } = string.Empty;
        
        [Required]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        
        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        
        public DateTime? DueDate { get; set; }
        
        public DateTime? CompletedAt { get; set; }
        
        // Navigation properties
        [ForeignKey(nameof(AssignedTo))]
        public User AssignedToUser { get; set; } = null!;
        
        [ForeignKey(nameof(AssignedBy))]
        public User AssignedByUser { get; set; } = null!;
    }
    
    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }
    
    public enum TaskStatus
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3
    }
}
