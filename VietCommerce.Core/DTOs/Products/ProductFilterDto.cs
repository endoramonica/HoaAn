

// ============================================
// FILE: ProductCreateDto.cs
// Mô tả: DTO để tạo sản phẩm mới
// ============================================

// ============================================
// FILE: ProductUpdateDto.cs
// Mô tả: DTO để cập nhật sản phẩm
// ============================================

// ============================================
// FILE: ProductFilterDto.cs
// Mô tả: DTO cho filtering và pagination
// ============================================
using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Products
{
    public class ProductFilterDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        [StringLength(200, ErrorMessage = "Search term cannot exceed 200 characters")]
        public string? SearchTerm { get; set; }
        
        public Guid? CategoryId { get; set; }
        
        public Guid? StoreId { get; set; }
        
        public bool? IsActive { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Min price must be non-negative")]
        public decimal? MinPrice { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Max price must be non-negative")]
        public decimal? MaxPrice { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Min stock must be non-negative")]
        public int? MinStock { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Max stock must be non-negative")]
        public int? MaxStock { get; set; }
        
        /// <summary>
        /// Sort by: name, price, stock, createdat, updatedat
        /// </summary>
        [StringLength(50)]
        public string? SortBy { get; set; }
        
        public bool IsDescending { get; set; } = false;
        
        public bool? HasPromotion { get; set; }
        
        public bool? InStock { get; set; }
    }
}
