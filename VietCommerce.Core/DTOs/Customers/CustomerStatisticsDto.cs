using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VietCommerce.Core.DTOs.Customers
{
    // ========================================
    // ADDITIONAL DTOs
    // ========================================

    /// <summary>
    /// Customer statistics summary
    /// </summary>
    public class CustomerStatisticsDto
    {
        public Guid CustomerId { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalInteractions { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public DateTime? LastInteractionDate { get; set; }
        public int LoyaltyPoints { get; set; }
        public string? Tier { get; set; }
    }

    /// <summary>
    /// Customer order summary
    /// </summary>
    public class CustomerOrderSummaryDto
    {
        public Guid CustomerId { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageOrderValue { get; set; }
        public DateTime? FirstOrderDate { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }

    /// <summary>
    /// Loyalty points history
    /// </summary>
    public class LoyaltyHistoryDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public int PointsChanged { get; set; }
        public int BalanceAfter { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
    }
}
