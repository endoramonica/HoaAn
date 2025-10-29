// File: VietCommerce.Api/Services/Interfaces/IOrderService.cs
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Services.Interfaces
{
    public interface IOrderService
    {
        
        /// Get order by ID with full details
        
        Task<ApiResponse<OrderDetailDto>> GetOrderByIdAsync(Guid orderId);

        
        /// Get paginated orders for current user (customer view - only their orders)
        
        Task<ApiResponse<PaginatedResult<OrderDetailDto>>> GetUserOrdersAsync(
            Guid userId,
            OrderFilterDTO filter);

        
        /// Get all orders (admin/seller view)
        
        Task<ApiResponse<PaginatedResult<OrderDetailDto>>> GetAllOrdersAsync(OrderFilterDTO filter);

        
        /// Get orders by specific status
        
        Task<ApiResponse<List<OrderDetailDto>>> GetOrdersByStatusAsync(OrderStatus status);

        
        /// Get recent orders (dashboard)
        
        Task<ApiResponse<List<OrderDetailDto>>> GetRecentOrdersAsync(int count = 10, Guid? storeId = null);

        
        /// Get status history for an order
        
        Task<ApiResponse<List<OrderStatusHistoryDTO>>> GetOrderStatusHistoryAsync(Guid orderId);

        
        /// Search orders by keyword
        
        Task<ApiResponse<List<OrderDetailDto>>> SearchOrdersAsync(string keyword);

        
        /// Update order status with validation
        
        Task<ApiResponse<bool>> UpdateOrderStatusAsync(
            Guid orderId,
            OrderStatus newStatus,
            Guid changedBy,
            string? notes = null);

        
        /// Cancel an order
        
        Task<ApiResponse<bool>> CancelOrderAsync(
            Guid orderId,
            string reason,
            Guid changedBy);

        
        /// Check if status transition is valid
        
        Task<ApiResponse<bool>> CanChangeStatusAsync(Guid orderId, OrderStatus newStatus);

        
        /// Get order count statistics
        
        Task<ApiResponse<int>> GetTotalOrdersCountAsync(
            Guid? storeId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        
        /// Get total revenue statistics
        
        Task<ApiResponse<decimal>> GetTotalRevenueAsync(
            Guid? storeId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        
        /// Bulk update order statuses
        
        Task<ApiResponse<int>> BulkUpdateStatusAsync(
            List<Guid> orderIds,
            OrderStatus newStatus,
            Guid changedBy,
            string? reason = null);
    }
}