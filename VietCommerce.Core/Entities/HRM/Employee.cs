using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Users;
namespace VietCommerce.Core.Entities.HRM
{
    [Table("Employees")]
    [Index(nameof(UserId), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Department))]
    [Index(nameof(Status))]
    public class Employee : BaseEntity
    {
        // 1-1 relationship with User
        [Required]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
        // Employee info
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Position { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;
        [Required]
        public DateTime HireDate { get; set; }
        [Required]
        [Precision(10, 2)]
        public decimal Salary { get; set; }
        [Required]
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
        public Guid? StoreId { get; set; }
        [MaxLength(500)]
        public string? Avatar { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string Skills { get; set; }= string.Empty;
        // Self-referencing relationship for Manager
        public Guid? ManagerId { get; set; }
        [ForeignKey(nameof(ManagerId))]
        public virtual Employee? Manager { get; set; }
        [InverseProperty(nameof(Manager))]
        public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
        // Navigation properties
        [ForeignKey(nameof(StoreId))]
        public virtual Store? Store { get; set; }
        [InverseProperty(nameof(PerformanceMetric.Employee))]
        public virtual ICollection<PerformanceMetric> Performance { get; set; } = new List<PerformanceMetric>();
        [InverseProperty(nameof(LeaveRequest.Employee))]
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        [InverseProperty(nameof(WorkSchedule.Employee))]
        public virtual ICollection<WorkSchedule> WorkSchedules { get; set; } = new List<WorkSchedule>();
    }
    public enum EmployeeStatus
    {
        Active = 1,
        Inactive = 2,
        OnLeave = 3
    }
}
