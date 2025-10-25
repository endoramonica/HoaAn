using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.HRM;
namespace VietCommerce.Core.Entities.HRM
{
    [Table("LeaveRequests")]
    [Index(nameof(EmployeeId), nameof(Status))]
    [Index(nameof(StartDate))]
    public class LeaveRequest : BaseEntity
    {
        [Required]
        [MaxLength(36)]
        public Guid EmployeeId { get; set; } 
        [Required]
        public LeaveRequestType Type { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public int Days { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Reason { get; set; } = string.Empty;
        [Required]
        public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;
        [MaxLength(36)]
        public Guid? ApprovedBy { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        [MaxLength(1000)]
        public string? Comments { get; set; }
        // Navigation properties
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employee { get; set; } = null!;
        [ForeignKey(nameof(ApprovedBy))]
        public Employee? ApprovedByEmployee { get; set; }
    }
    public enum LeaveRequestType
    {
        Vacation = 1,
        Sick = 2,
        Personal = 3,
        Emergency = 4
    }
    public enum LeaveRequestStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
}
