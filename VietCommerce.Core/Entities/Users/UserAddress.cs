using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Enums.Common;
namespace VietCommerce.Core.Entities.Users
{
    [Table("UserAddresses")]
    public class UserAddress : AuditableEntity
    {
        public Guid StoreId { get; set; }
        public Guid UserId { get; set; }
        [Required]
        public AddressType AddressType { get; set; } = AddressType.HOME;
        [Required, MaxLength(255)]
        public string AddressLine1 { get; set; } = string.Empty;
        [MaxLength(255)]
        public string AddressLine2 { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string? Province { get; set; }     
        [MaxLength(20)]
        public string? PostalCode { get; set; }
        [MaxLength(100)]
        public string Country { get; set; } = "Vietnam";
        public bool IsDefault { get; set; } = false;
        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Store Store { get; set; } = null!;
    }
}
