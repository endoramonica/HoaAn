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
    [Index(nameof(Id), IsUnique = true)]
    [Index(nameof(Department))]
    [Index(nameof(Status))]
    public class Employee : BaseEntity
    {
        // 1-1 relationship with User
        [Key]
        [ForeignKey(nameof(User))]
        public Guid Id { get; set; }
        public virtual User User { get; set; } = null!;

        // Employee info - CHỈ GIỮ THÔNG TIN HR
        [Required]
        [MaxLength(100)]
        public string Code { get; set; } = string.Empty; // Mã nhân viên

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

        [Column(TypeName = "nvarchar(max)")]
        public string Skills { get; set; } = string.Empty;

        // Self-referencing relationship for Manager
        public Guid? ManagerId { get; set; }
        [ForeignKey(nameof(ManagerId))]
        public virtual Employee? Manager { get; set; }

        [InverseProperty(nameof(Manager))]
        public virtual ICollection<Employee> Subordinates { get; set; } = new List<Employee>();

        // ✅ THÊM LẠI Navigation property Store
        [ForeignKey(nameof(StoreId))]
        public virtual Store? Store { get; set; }

        // Navigation properties
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