using System.ComponentModel.DataAnnotations.Schema;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Products;
namespace VietCommerce.Core.Entities.Marketing
{
    [Table("PromotionProducts")]
    public class PromotionProduct : BaseEntity
    {
        public Guid PromotionId { get; set; }
        public Guid ProductId { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? DiscountOverride { get; set; }
        // Navigation
        public virtual Promotion Promotion { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
