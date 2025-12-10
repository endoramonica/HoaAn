namespace VietCommerce.Core.DTOs.Products
{


    /// Filter/search parameters for product listing

    public class ProductFilterDto
    {
        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        // Search
        public string? SearchTerm { get; set; }

        // Filters
        public Guid? CategoryId { get; set; }
        public Guid? StoreId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsFeatured { get; set; }

        // Price range
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Sorting
        public string? SortBy { get; set; } // "name", "price", "created", "views"
        public bool IsDescending { get; set; } = false;

        // ========================================
        // 🔧 SERVICES-PRODUCT UNIFICATION - NEW FILTERS
        // ========================================

        /// <summary>
        /// Filter by type: "product" or "service"
        /// If null, returns all types (backward compatible)
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Filter by service category (only applies when type='service')
        /// Values: ancestor-worship, opening-ceremony, wedding, buddha-worship, new-house, feng-shui-consultation
        /// </summary>
        public string? ServiceCategory { get; set; }
    }
}