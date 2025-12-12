using System.ComponentModel.DataAnnotations;

namespace VietCommerce.Core.DTOs.Products
{


    /// DTO for updating an existing product
    /// All fields optional - only update provided fields

    public class ProductUpdateDto
    {
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Short description cannot exceed 500 characters")]
        public string? ShortDescription { get; set; }

        [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        public decimal? Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Compare price must be greater than or equal to 0")]
        public decimal? CompareAtPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Cost must be greater than or equal to 0")]
        public decimal? Cost { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? BrandId { get; set; }

        [StringLength(200, ErrorMessage = "SKU cannot exceed 200 characters")]
        public string? Sku { get; set; }

        [StringLength(200, ErrorMessage = "Barcode cannot exceed 200 characters")]
        public string? Barcode { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be greater than or equal to 0")]
        public int? StockQuantity { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsFeatured { get; set; }

        public List<string>? Images { get; set; }

        public List<string>? Tags { get; set; }

        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        /// <summary>
        /// List of fixed items included in the package product (e.g., "Cá chép (3 con)", "Mũ giấy (3 cái)").
        /// Only applicable for package products. Null for regular products.
        /// </summary>
        public List<string>? Details { get; set; }

        /// <summary>
        /// List of customizable options for the package product.
        /// Allows customers to modify quantities of specific items within the package.
        /// Only applicable for package products. Null for regular products.
        /// </summary>
        public List<CustomizableOptionDto>? CustomizableOptions { get; set; }
    }
}