using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Core.Entities.CRM
{
    [Table("CRMInteractions")]
    [Index(nameof(CustomerId), nameof(CreatedAt))]
    [Index(nameof(Type))]
    [Index(nameof(Status))]
    public class CRMInteraction :  AuditableEntity
    {
        
        
        [Required]
        [MaxLength(36)]
        public Guid CustomerId { get; set; } 
        
        [Required]
        public CRMInteractionType Type { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;
        
        
        
        public DateTime? FollowUpDate { get; set; }
        
        [Required]
        public CRMInteractionStatus Status { get; set; } = CRMInteractionStatus.Pending;
        
        // Navigation properties
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; } = null!;
        
        [ForeignKey(nameof(CreatedBy))]
        public User CreatedByUser { get; set; } = null!;
    }
    
    public enum CRMInteractionType
    {
        Call = 1,
        Email = 2,
        Meeting = 3,
        Note = 4
    }
    
    public enum CRMInteractionStatus
    {
        Pending = 1,
        Completed = 2
    }
}
