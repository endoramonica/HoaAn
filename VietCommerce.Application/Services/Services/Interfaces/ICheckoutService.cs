using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Service for handling checkout and order management operations
    /// </summary>
    public interface ICheckoutService
    {
        /// <summary>
        /// Process checkout and create order from cart
        /// </summary>
        /// <param name="userId">Current user ID (from JWT)</param>
        /// <param name="dto">Checkout request data including cart, shipping, and payment info</param>
        /// <returns>
        /// Success: CheckoutResponseDto with OrderId, OrderNumber, and order details
        /// Failure: Error message (cart not found, insufficient stock, etc.)
        /// </returns>
        /// <remarks>
        /// Flow:
        /// 1. Validate user authentication
        /// 2. Get or create Customer from UserId
        /// 3. Validate cart exists and has items
        /// 4. Check product availability and stock
        /// 5. Create Order + OrderItems + OrderShipping
        /// 6. Calculate totals (SubTotal, ShippingFee, Tax, Total)
        /// 7. Clear cart after successful order
        /// 8. Create initial OrderStatusHistory
        /// 9. Return order details
        /// </remarks>
        Task<ApiResponse<CheckoutResponseDto>> CheckoutAsync(Guid userId, CheckoutDto dto);

        /// <summary>
        /// Get order details by OrderId
        /// </summary>
        /// <param name="userId">Current user ID (for authorization check)</param>
        /// <param name="orderId">Order unique identifier</param>
        /// <returns>
        /// Success: Full order details including items, shipping, status history
        /// Failure: Order not found or unauthorized access
        /// </returns>
        /// <remarks>
        /// Security: Verify user owns this order (Order.CustomerId matches User's CustomerId)
        /// </remarks>
        Task<ApiResponse<OrderDetailDto>> GetOrderByIdAsync(Guid userId, Guid orderId);

        /// <summary>
        /// Get user's order history with filtering and pagination
        /// </summary>
        /// <param name="userId">Current user ID</param>
        /// <param name="filter">Filter parameters (status, date range, pagination, etc.)</param>
        /// <returns>
        /// Success: Paginated list of orders
        /// Failure: Validation error or database error
        /// </returns>
        /// <remarks>
        /// Automatically filters by user's CustomerId
        /// Supports:
        /// - Keyword search (OrderNumber, CustomerName)
        /// - Status filter (Pending, Confirmed, Shipped, etc.)
        /// - Date range filter
        /// - Amount range filter
        /// - Sorting (by date, amount, status)
        /// - Pagination
        /// </remarks>
        Task<ApiResponse<List<OrderDetailDto>>> GetOrdersAsync(Guid userId, OrderFilterDTO filter);

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <param name="userId">Current user ID (for authorization)</param>
        /// <param name="orderId">Order ID to cancel</param>
        /// <param name="reason">Cancellation reason (required)</param>
        /// <returns>
        /// Success: true
        /// Failure: Cannot cancel (already shipped/completed), unauthorized, or not found
        /// </returns>
        /// <remarks>
        /// Business Rules:
        /// - Only orders with status Pending or Confirmed can be cancelled
        /// - User must own the order (security check)
        /// - Creates OrderStatusHistory record
        /// - May restore product inventory (if implemented)
        /// </remarks>
        Task<ApiResponse<bool>> CancelOrderAsync(Guid userId, Guid orderId, string reason);

        // ============================================
        // OPTIONAL METHODS (Implement if needed)
        // ============================================

        /// <summary>
        /// Get checkout summary before placing order (optional)
        /// </summary>
        /// <param name="userId">Current user ID</param>
        /// <param name="cartId">Cart ID to preview</param>
        /// <returns>Calculated totals without creating order</returns>
        // Task<ApiResponse<CheckoutSummaryDto>> GetCheckoutSummaryAsync(Guid userId, Guid cartId);

        /// <summary>
        /// Validate cart before checkout (optional - can be part of CheckoutAsync)
        /// </summary>
        /// <param name="userId">Current user ID</param>
        /// <param name="cartId">Cart to validate</param>
        /// <returns>Validation result with list of issues</returns>
        // Task<ApiResponse<CartValidationResultDto>> ValidateCartAsync(Guid userId, Guid cartId);

        /// <summary>
        /// Apply coupon code (optional - if implementing discounts)
        /// </summary>
        /// <param name="userId">Current user ID</param>
        /// <param name="orderId">Order ID</param>
        /// <param name="couponCode">Coupon code to apply</param>
        /// <returns>Updated order with discount applied</returns>
        // Task<ApiResponse<OrderDetailDto>> ApplyCouponAsync(Guid userId, Guid orderId, string couponCode);
    }
}