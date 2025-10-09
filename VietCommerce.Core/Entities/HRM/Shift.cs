using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Entities.Organization;

namespace VietCommerce.Core.Entities.HRM
{
    [Table("Shifts")]
    [Index(nameof(UserId), nameof(StartTime))]
    [Index(nameof(Status))]
    public class Shift : BaseEntity
    {
        
        
        [Required]
        [MaxLength(36)]
        public Guid UserId { get; set; } 
        
        [Required]
        [MaxLength(36)]
        public Guid StoreId { get; set; } 
        
        [Required]
        public DateTime StartTime { get; set; }
        
        public DateTime? EndTime { get; set; }
        
        [Required]
        [Precision(10, 2)]
        public decimal OpeningCash { get; set; } = 0;
        
        [Precision(10, 2)]
        public decimal? ClosingCash { get; set; }
        
        [Required]
        [Precision(10, 2)]
        public decimal TotalSales { get; set; } = 0;
        
        [Required]
        public int TotalTransactions { get; set; } = 0;
        
        [Required]
        public ShiftStatus Status { get; set; } = ShiftStatus.Open;
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public User Staff { get; set; } = null!;
        
        [ForeignKey(nameof(StoreId))]
        public Store Store { get; set; } = null!;
    }
    
    public enum ShiftStatus
    {
        Open = 1,
        Closed = 2
    }
}
