
using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Products
{
    public class ProductImageCreateDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public Guid ProductId { get; set; }
        
        [Required(ErrorMessage = "Image URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;
        
        public int DisplayOrder { get; set; } = 0;
        
        public bool IsPrimary { get; set; } = false;
    }
}
