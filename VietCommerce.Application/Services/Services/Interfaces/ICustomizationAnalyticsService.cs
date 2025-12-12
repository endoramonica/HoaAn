using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Service for analyzing customization patterns in orders.
    /// Provides analytics on which customizable options are most popular across all orders.
    /// </summary>
    public interface ICustomizationAnalyticsService
    {
        /// <summary>
        /// Gets the most popular customizable options across all orders.
        /// Aggregates option usage and calculates total quantity ordered for each option.
        /// </summary>
        /// <returns>
        /// Dictionary mapping option ID to total quantity ordered.
        /// Example: {"opt-xoi": 1250, "opt-che": 890, "opt-banh": 450}
        /// </returns>
        /// <remarks>
        /// Validates: Requirements 9.1, 9.2
        /// Queries OrderItem.CustomizationsJson and aggregates option usage across all orders.
        /// Skips null customizations and handles JSON deserialization errors gracefully.
        /// </remarks>
        Task<Dictionary<string, int>> GetMostPopularOptionsAsync();

        /// <summary>
        /// Gets customization analytics with option names and percentages.
        /// </summary>
        /// <returns>
        /// List of analytics objects containing option ID, name, total quantity, and percentage of orders.
        /// </returns>
        /// <remarks>
        /// Validates: Requirements 9.1, 9.2, 9.3
        /// Provides detailed analytics including percentage calculations.
        /// Handles division by zero gracefully.
        /// </remarks>
        Task<List<CustomizationAnalyticsDto>> GetCustomizationAnalyticsAsync();
    }

    /// <summary>
    ///    /// DTO for zation analytics data.
    /// </summary>
    public class CustomizationAnalyticsDto
    {
        /// <summary>
        /// Unique identifier of the customizable option.
        /// </summary>
        public string OptionId { get; set; } = string.Empty;

        /// <summary>
        /// Name of the customizable option.
        /// </summary>
        public string OptionName { get; set; } = string.Empty;

        /// <summary>
        /// Total quantity of this option ordered across all orders.
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// Percentage of orders containing this option.
        /// </summary>
        public decimal PercentageOfOrders { get; set; }
    }
}
