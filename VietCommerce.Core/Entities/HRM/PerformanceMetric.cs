using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Common;


namespace VietCommerce.Core.Entities.HRM
{
    [Table("PerformanceMetrics")]
    [Index(nameof(EmployeeId), nameof(Period))]
    public class PerformanceMetric : BaseEntity
    {
        
        
        [Required]
        [MaxLength(36)]
        public Guid EmployeeId { get; set; } 
        
        [Required]
        [MaxLength(50)]
        public string Period { get; set; } = string.Empty;
        
        public int OrdersProcessed { get; set; } = 0;
        
        public int TasksCompleted { get; set; } = 0;
        
        [Precision(3, 2)]
        public decimal CustomerRating { get; set; } = 0;
        
        [Precision(10, 2)]
        public decimal Sales { get; set; } = 0;
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        // Navigation properties
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; } = null!;
    }
}
