using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VietCommerce.Core.Models
{
    /// <summary>
    /// Pagination parameters
    /// </summary>
    public class PaginationParams
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 20;

        [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        /// <summary>
        /// Sort by field: "Name", "Email", "CreatedAt", "LoyaltyPoints", "Tier"
        /// </summary>
        [MaxLength(50)]
        public string SortBy { get; set; } = "CreatedAt";

        /// <summary>
        /// Sort descending (true) or ascending (false)
        /// </summary>
        public bool SortDescending { get; set; } = true;

        /// <summary>
        /// Calculate skip count for pagination
        /// </summary>
        public int Skip => (Page - 1) * PageSize;
    }
}
