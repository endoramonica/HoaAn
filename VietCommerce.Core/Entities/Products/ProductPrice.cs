using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Products;

namespace VietCommerce.Core.Entities.Products
{
    [Table("ProductPrices")]
    public class ProductPrice : AuditableEntity
    {
       
        public Guid ProductId { get; set; }

        [Required]
        public PriceType PriceType { get; set; } = PriceType.REGULAR;

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal Price { get; set; }

        [Required]
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public bool IsActive { get; set; } = true;

       

        // Navigation
        public virtual Product Product { get; set; } = null!;
        public virtual User Creator { get; set; } = null!;
    }
}
