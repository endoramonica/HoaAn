using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Common;
namespace VietCommerce.Core.Entities.HRM
{
    [Table("WorkSchedules")]
    [Index(nameof(EmployeeId), nameof(Date))]
    [Index(nameof(Status))]
    public class WorkSchedule : BaseEntity
    {
        [Required]
        [MaxLength(36)]
        public Guid EmployeeId { get; set; } 
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [MaxLength(8)]
        public TimeSpan StartTime { get; set; }
        [Required]
        [MaxLength(8)]
        public TimeSpan EndTime { get; set; } 
        [Required]
        public WorkScheduleType Type { get; set; } = WorkScheduleType.Regular;
        [Required]
        public WorkScheduleStatus Status { get; set; } = WorkScheduleStatus.Scheduled;
        [MaxLength(1000)]
        public string? Notes { get; set; }
        // Navigation properties
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; } = null!;
        public string? ShiftName { get; set; }
    }
    public enum WorkScheduleType
    {
        Regular = 1,
        Overtime = 2,
        Holiday = 3
    }
    public enum WorkScheduleStatus
    {
        Scheduled = 1,
        Confirmed = 2,
        Completed = 3,
        Missed = 4
    }
}
