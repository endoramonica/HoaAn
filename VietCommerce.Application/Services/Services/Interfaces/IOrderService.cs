// File: VietCommerce.Api/Services/Interfaces/IOrderService.cs
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Order service interface
    /// ✅ Methods automatically use ICurrentUser to get user context
    /// ✅ No need to pass userId/customerId from controllers
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Get order by ID with full details
        /// ✅ Automatically checks if user has permission to view this order
        /// - Admin: can view any order
        /// - Customer: can only view their own orders
        /// </summary>
        /// <param name="orderId">Order ID to retrieve</param>
        /// <returns>Order details with items, shipping, and status history</returns>
        Task<ApiResponse<OrderDetailDto>> GetOrderByIdAsync(Guid orderId);

        /// <summary>
        /// Get paginated orders for current user (customer view - only their orders)
        /// ✅ Automatically uses CustomerId from JWT token
        /// </summary>
        /// <param name="filter">Filter criteria (date range, status, etc.)</param>
        /// <returns>Paginated list of user's orders</returns>
        Task<ApiResponse<PaginatedResult<OrderDetailDto>>> GetUserOrdersAsync(OrderFilterDTO filter);

        /// <summary>
        /// Get all orders (admin/seller view)
        /// ✅ ADMIN ONLY - automatically enforced
        /// </summary>
        /// <param name="filter">Filter criteria for all orders</param>
        /// <returns>Paginated list of all orders in system</returns>
        Task<ApiResponse<PaginatedResult<OrderDetailDto>>> GetAllOrdersAsync(OrderFilterDTO filter);

        /// <summary>
        /// Get orders by specific status
        /// ✅ Admin: all orders with this status
        /// ✅ Customer: only their orders with this status
        /// </summary>
        /// <param name="status">Order status to filter by</param>
        /// <returns>List of orders with specified status</returns>
        Task<ApiResponse<List<OrderDetailDto>>> GetOrdersByStatusAsync(OrderStatus status);

        /// <summary>
        /// Get recent orders (dashboard)
        /// ✅ Admin: recent orders from all customers
        /// ✅ Customer: their recent orders only
        /// </summary>
        /// <param name="count">Number of orders to retrieve (default: 10)</param>
        /// <param name="storeId">Optional store filter</param>
        /// <returns>List of recent orders</returns>
        // Task<ApiResponse<List<OrderDetailDto>>> GetRecentOrdersAsync(int count = 10, Guid? storeId = null);

        /// <summary>
        /// Get status history for an order
        /// ✅ Automatically checks permission before returning history
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>List of status changes with timestamps and who made the change</returns>
        Task<ApiResponse<List<OrderStatusHistoryDTO>>> GetOrderStatusHistoryAsync(Guid orderId);

        /// <summary>
        /// Search orders by keyword
        /// ✅ Admin: search all orders
        /// ✅ Customer: search only their orders
        /// </summary>
        /// <param name="keyword">Search term (order code, customer name, etc.)</param>
        /// <returns>List of matching orders</returns>
       // Task<ApiResponse<List<OrderDetailDto>>> SearchOrdersAsync(string keyword);

        /// <summary>
        /// Update order status with validation
        /// ✅ Automatically uses current UserId as changedBy
        /// ✅ Validates status transition rules
        /// </summary>
        /// <param name="orderId">Order ID to update</param>
        /// <param name="newStatus">New status to set</param>
        /// <param name="notes">Optional notes for status change</param>
        /// <returns>Success/failure result</returns>
        Task<ApiResponse<bool>> UpdateOrderStatusAsync(
            Guid orderId,
            OrderStatus newStatus,
            string? notes = null);

        /// <summary>
        /// Cancel an order
        /// ✅ Automatically uses current UserId as changedBy
        /// ✅ Customer: can only cancel their own orders
        /// ✅ Admin: can cancel any order
        /// </summary>
        /// <param name="orderId">Order ID to cancel</param>
        /// <param name="reason">Cancellation reason (required)</param>
        /// <returns>Success/failure result</returns>
        Task<ApiResponse<bool>> CancelOrderAsync(
            Guid orderId,
            string reason);

        /// <summary>
        /// Check if status transition is valid
        /// Validates business rules for order status changes
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="newStatus">Status to transition to</param>
        /// <returns>True if transition is allowed</returns>
        Task<ApiResponse<bool>> CanChangeStatusAsync(Guid orderId, OrderStatus newStatus);

        /// <summary>
        /// Get order count statistics
        /// ✅ Admin: count of all orders
        /// ✅ Customer: count of their orders only
        /// </summary>
        /// <param name="storeId">Optional store filter</param>
        /// <param name="fromDate">Start date for date range filter</param>
        /// <param name="toDate">End date for date range filter</param>
        /// <returns>Total order count</returns>
        Task<ApiResponse<int>> GetTotalOrdersCountAsync(
            Guid? storeId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        /// <summary>
        /// Get total revenue statistics
        /// ✅ ADMIN ONLY - automatically enforced
        /// </summary>
        /// <param name="storeId">Optional store filter</param>
        /// <param name="fromDate">Start date for date range filter</param>
        /// <param name="toDate">End date for date range filter</param>
        /// <returns>Total revenue amount</returns>
        Task<ApiResponse<decimal>> GetTotalRevenueAsync(
            Guid? storeId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        /// <summary>
        /// Bulk update order statuses
        /// ✅ ADMIN ONLY - automatically enforced
        /// ✅ Automatically uses current UserId as changedBy
        /// </summary>
        /// <param name="orderIds">List of order IDs to update</param>
        /// <param name="newStatus">New status to set for all orders</param>
        /// <param name="reason">Optional reason for bulk update</param>
        /// <returns>Number of successfully updated orders</returns>
        Task<ApiResponse<int>> BulkUpdateStatusAsync(
            List<Guid> orderIds,
            OrderStatus newStatus,
            string? reason = null);
    }
}