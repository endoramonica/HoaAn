using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Logistics;
namespace VietCommerce.Core.Entities.Logistics
{
    [Table("Suppliers")]
    [Index(nameof(Name))]
    [Index(nameof(Status))]
    public class Supplier : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;
        [Required]
        public SupplierStatus Status { get; set; } = SupplierStatus.Active;
        [Required]
        [MaxLength(100)]
        public string PaymentTerms { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string DeliverySchedule { get; set; } = string.Empty;
        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();
        [InverseProperty(nameof(StockTransfer.Supplier))]
        public ICollection<StockTransfer> StockTransfers { get; set; } = new List<StockTransfer>();
    }
    public enum SupplierStatus
    {
        Active = 1,
        Inactive = 2
    }
}
