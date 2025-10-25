    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using VietCommerce.Core.Common;
    using VietCommerce.Core.Entities.Organization;
    using VietCommerce.Core.Enums.Common;
    namespace VietCommerce.Core.Entities.Customers
    {
        [Table("CustomerAddresses")]
        public class CustomerAddress : BaseEntity, ISoftDelete
        {
            // ⭐ THÔNG TIN CƠ BẢN
            [Required]
            [Column(Order = 1)]
            public Guid CustomerId { get; set; }
            [Required]
            [MaxLength(500)]
            public string StreetAddress { get; set; } = string.Empty;
            [MaxLength(200)]
            public string? City { get; set; }
            [MaxLength(50)]
            public string? PostalCode { get; set; }
            [MaxLength(200)]
            public string? State { get; set; }
            [MaxLength(100)]
            public string? Country { get; set; }
            [Required]
            public AddressType AddressType { get; set; } // HOME=1, OFFICE=2, OTHER=3
            // ⭐ THÔNG TIN NGƯỜI NHẬN
            [MaxLength(200)]
            public string? RecipientName { get; set; }
            [MaxLength(20)]
            public string? PhoneNumber { get; set; }
            [MaxLength(200)]
            public string? Email { get; set; }
            // ⭐ FLAGS
            [Column("IsDefault")]
            public bool IsDefault { get; set; }
            [Column("IsPrimary")]
            public bool IsPrimary { get; set; }
            [Column("IsActive")]
            public bool IsActive { get; set; } = true;
            // ⭐ SOFT DELETE FIELDS (từ ISoftDelete)
            public bool IsDeleted { get; set; } = false;
            public DateTime? DeletedAt { get; set; }
            public Guid? DeletedBy { get; set; }  // ID người xóa (Staff/User)
            // ⭐ MULTI-TENANT SUPPORT
            public Guid TenantId { get; set; }
            // ⭐ NAVIGATION PROPERTIES
            [ForeignKey("CustomerId")]
            public virtual Customer Customer { get; set; } = null!;
            [ForeignKey("TenantId")]
            public virtual Tenant Tenant { get; set; } = null!;
            // ⭐ CONSTRUCTOR
            public CustomerAddress()
            {
                CreatedAt = DateTime.UtcNow;
                IsActive = true;
            }
        }
    }
