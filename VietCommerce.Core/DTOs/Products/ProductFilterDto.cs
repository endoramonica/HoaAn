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
    }
}