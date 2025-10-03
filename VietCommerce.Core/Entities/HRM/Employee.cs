using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.HRM;

namespace VietCommerce.Core.Entities.HRM
{
    [Table("Employees")]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Department))]
    [Index(nameof(Status))]
    public class Employee : BaseEntity
    {
        
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        
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
        
        [MaxLength(36)]
        public string? ManagerId { get; set; }
        
        [MaxLength(36)]
        public string? StoreId { get; set; }
        
        [MaxLength(500)]
        public string? Avatar { get; set; }
        
        [Column(TypeName = "nvarchar(max)")]
        public string Skills { get; set; } = "[]";
        
        // Navigation properties
        [ForeignKey(nameof(ManagerId))]
        public Employee? Manager { get; set; }
        
        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; }
        
        [InverseProperty(nameof(Manager))]
        public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
        
        [InverseProperty(nameof(PerformanceMetric.Employee))]
        public ICollection<PerformanceMetric> Performance { get; set; } = new List<PerformanceMetric>();
        
        [InverseProperty(nameof(LeaveRequest.Employee))]
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        
        [InverseProperty(nameof(WorkSchedule.Employee))]
        public ICollection<WorkSchedule> WorkSchedules { get; set; } = new List<WorkSchedule>();
    }
    
    public enum EmployeeStatus
    {
        Active = 1,
        Inactive = 2,
        OnLeave = 3
    }
}
