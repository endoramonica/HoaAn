// ============================================
// FILE: ProductDto.cs
// Mô tả: DTO cơ bản cho danh sách sản phẩm
// ============================================
using System.ComponentModel.DataAnnotations;

// ============================================
// FILE: ProductReviewDto.cs
// Mô tả: DTO cho đánh giá sản phẩm
// ============================================
namespace VietCommerce.Core.DTOs.Products
{
    public class ProductReviewDto
    {
        public Guid Id { get; set; }
        
        public Guid ProductId { get; set; }
        
        public Guid UserId { get; set; }
        
        public string UserName { get; set; } = string.Empty;
        
        public string? UserAvatar { get; set; }
        
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
        
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
        
        public bool IsVerifiedPurchase { get; set; }
        
        public int HelpfulCount { get; set; }
        
        public List<string>? Images { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}
