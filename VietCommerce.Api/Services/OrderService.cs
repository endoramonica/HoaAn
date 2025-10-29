// File: VietCommerce.Api/Services/OrderService.cs
using AutoMapper;
using Microsoft.Extensions.Logging;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        
        /// Get order by ID with full details
        
        public async Task<ApiResponse<OrderDetailDto>> GetOrderByIdAsync(Guid orderId)
        {
            try
            {
                _logger.LogInformation("Fetching order with ID: {OrderId}", orderId);

                var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

                if (order == null)
                {
                    _logger.LogWarning("Order not found: {OrderId}", orderId);
                    return ApiResponse<OrderDetailDto>.FailureResponse( "ORDER_NOT_FOUND");
                }

                var orderDto = MapOrderToDetailDto(order);
                return ApiResponse<OrderDetailDto>.SuccessResponse(orderDto, "Order retrieved SuccessResponsefully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId}", orderId);
                return ApiResponse<OrderDetailDto>.FailureResponse("ORDER_RETRIEVAL_ERROR");
            }
        }

        
        /// Get paginated orders for current user
        
        public async Task<ApiResponse<PaginatedResult<OrderDetailDto>>> GetUserOrdersAsync(
            Guid userId,
            OrderFilterDTO filter)
        {
            try
            {
                _logger.LogInformation("Fetching orders for user: {UserId}", userId);

                var result = await _orderRepository.GetUserOrdersAsync(userId, filter);

                var dtoResult = new PaginatedResult<OrderDetailDto>
                {
                    Items = result.Items.Select(MapOrderToDetailDto).ToList(),
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize,
                    TotalItems = result.TotalItems,
                    TotalPages = result.TotalPages
                };

                return ApiResponse<PaginatedResult<OrderDetailDto>>.SuccessResponse(dtoResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user orders for {UserId}", userId);
                    return ApiResponse<PaginatedResult<OrderDetailDto>>.FailureResponse("ORDER_LIST_ERROR");
            }
        }

        
        /// Get all orders (admin view)
        
        public async Task<ApiResponse<PaginatedResult<OrderDetailDto>>> GetAllOrdersAsync(OrderFilterDTO filter)
        {
            try
            {
                _logger.LogInformation("Fetching all orders with filter");

                var result = await _orderRepository.GetAllOrdersAsync(filter);

                var dtoResult = new PaginatedResult<OrderDetailDto>
                {
                    Items = result.Items.Select(MapOrderToDetailDto).ToList(),
                    PageNumber = result.PageNumber,
                    PageSize = result.PageSize,
                    TotalItems = result.TotalItems,
                    TotalPages = result.TotalPages
                };

                return ApiResponse<PaginatedResult<OrderDetailDto>>.SuccessResponse(dtoResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all orders");
                return ApiResponse<PaginatedResult<OrderDetailDto>>.FailureResponse("ORDER_LIST_ERROR");
            }
        }

        
        /// Get orders by status
        
        public async Task<ApiResponse<List<OrderDetailDto>>> GetOrdersByStatusAsync(OrderStatus status)
        {
            try
            {
                _logger.LogInformation("Fetching orders with status: {Status}", status);

                var orders = await _orderRepository.GetOrdersByStatusAsync(status);
                var dtoList = orders.Select(MapOrderToDetailDto).ToList();

                return ApiResponse<List<OrderDetailDto>>.SuccessResponse(dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders by status {Status}", status);
                return ApiResponse<List<OrderDetailDto>>.FailureResponse( "ORDER_LIST_ERROR");
            }
        }

        
        /// Get recent orders
        
        public async Task<ApiResponse<List<OrderDetailDto>>> GetRecentOrdersAsync(int count = 10, Guid? storeId = null)
        {
            try
            {
                _logger.LogInformation("Fetching {Count} recent orders", count);

                var orders = await _orderRepository.GetRecentOrdersAsync(count, storeId);
                var dtoList = orders.Select(MapOrderToDetailDto).ToList();

                return ApiResponse<List<OrderDetailDto>>.SuccessResponse(dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching recent orders");
                return ApiResponse<List<OrderDetailDto>>.FailureResponse( "ORDER_LIST_ERROR");
            }
        }

        
        /// Get order status history
        
        public async Task<ApiResponse<List<OrderStatusHistoryDTO>>> GetOrderStatusHistoryAsync(Guid orderId)
        {
            try
            {
                _logger.LogInformation("Fetching status history for order: {OrderId}", orderId);

                var histories = await _orderRepository.GetStatusHistoryAsync(orderId);
                var dtoList = histories.Select(h => new OrderStatusHistoryDTO
                {
                    Status = h.NewStatus,
                    ChangedAt = h.ChangedAt,
                    ChangedByName = h.ChangedByUser?.Name,
                    Notes = h.Notes
                }).ToList();

                return ApiResponse<List<OrderStatusHistoryDTO>>.SuccessResponse(dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching status history for order {OrderId}", orderId);
                return ApiResponse<List<OrderStatusHistoryDTO>>.FailureResponse("HISTORY_RETRIEVAL_ERROR");
            }
        }

        
        /// Search orders
        
        public async Task<ApiResponse<List<OrderDetailDto>>> SearchOrdersAsync(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return ApiResponse<List<OrderDetailDto>>.FailureResponse("Search keyword is required INVALID_SEARCH");
                }

                _logger.LogInformation("Searching orders with keyword: {Keyword}", keyword);

                var orders = await _orderRepository.SearchOrdersAsync(keyword);
                var dtoList = orders.Select(MapOrderToDetailDto).ToList();

                return ApiResponse<List<OrderDetailDto>>.SuccessResponse(dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching orders with keyword: {Keyword}", keyword);
                return ApiResponse<List<OrderDetailDto>>.FailureResponse("SEARCH_ERROR");
            }
        }

        
        /// Update order status
        
        public async Task<ApiResponse<bool>> UpdateOrderStatusAsync(
            Guid orderId,
            OrderStatus newStatus,
            Guid changedBy,
            string? notes = null)
        {
            try
            {
                _logger.LogInformation("Updating order {OrderId} status to {NewStatus}", orderId, newStatus);

                // Validate status transition
                var canChange = await _orderRepository.CanChangeStatusAsync(orderId, newStatus);
                if (!canChange)
                {
                    _logger.LogWarning("Invalid status transition for order {OrderId}", orderId);
                    return ApiResponse<bool>.FailureResponse(
                        "Cannot transition to this status INVALID_STATUS_TRANSITION");
                }

                // Update status
                var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, changedBy, notes);

                if (!result)
                {
                    return ApiResponse<bool>.FailureResponse("Order not found ORDER_NOT_FOUND");
                }

                _logger.LogInformation("Order {OrderId} status updated SuccessResponsefully", orderId);
                return ApiResponse<bool>.SuccessResponse(true, "Order status updated SuccessResponsefully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId} status", orderId);
                return ApiResponse<bool>.FailureResponse("Failed to update order status STATUS_UPDATE_ERROR");
            }
        }

        
        /// Cancel order
        
        public async Task<ApiResponse<bool>> CancelOrderAsync(
            Guid orderId,
            string reason,
            Guid changedBy)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reason))
                {
                    return ApiResponse<bool>.FailureResponse("Cancel reason is required INVALID_REASON");
                }

                _logger.LogInformation("Cancelling order {OrderId} with reason: {Reason}", orderId, reason);

                var result = await _orderRepository.CancelOrderAsync(orderId, reason, changedBy);

                if (!result)
                {
                    _logger.LogWarning("Cannot cancel order {OrderId}", orderId);
                    return ApiResponse<bool>.FailureResponse(
                        "Cannot cancel this order at current status CANNOT_CANCEL_ORDER");
                }

                _logger.LogInformation("Order {OrderId} cancelled SuccessResponsefully", orderId);
                return ApiResponse<bool>.SuccessResponse(true, "Order cancelled SuccessResponsefully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", orderId);
                return ApiResponse<bool>.FailureResponse("Failed to cancel order CANCEL_ERROR");
            }
        }

        
        /// Check if status change is valid
        
        public async Task<ApiResponse<bool>> CanChangeStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            try
            {
                var canChange = await _orderRepository.CanChangeStatusAsync(orderId, newStatus);
                return ApiResponse<bool>.SuccessResponse(canChange);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking status change validity");
                return ApiResponse<bool>.FailureResponse("Failed to check status validity VALIDATION_ERROR");
            }
        }

        
        /// Get total orders count
        
        public async Task<ApiResponse<int>> GetTotalOrdersCountAsync(
            Guid? storeId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                var count = await _orderRepository.GetTotalOrdersCountAsync(storeId, fromDate, toDate);
                return ApiResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total orders count");
                return ApiResponse<int>.FailureResponse("Failed to get count COUNT_ERROR");
            }
        }

        
        /// Get total revenue
        
        public async Task<ApiResponse<decimal>> GetTotalRevenueAsync(
            Guid? storeId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                var revenue = await _orderRepository.GetTotalRevenueAsync(storeId, fromDate, toDate);
                return ApiResponse<decimal>.SuccessResponse(revenue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue");
                return ApiResponse<decimal>.FailureResponse("Failed to get revenue REVENUE_ERROR");
            }
        }

        
        /// Bulk update order statuses
        
        public async Task<ApiResponse<int>> BulkUpdateStatusAsync(
            List<Guid> orderIds,
            OrderStatus newStatus,
            Guid changedBy,
            string? reason = null)
        {
            try
            {
                if (!orderIds.Any())
                {
                    return ApiResponse<int>.FailureResponse("Order IDs list is empty EMPTY_LIST");
                }

                _logger.LogInformation("Bulk updating {Count} orders to status {Status}", orderIds.Count, newStatus);

                var updatedCount = await _orderRepository.BulkUpdateStatusAsync(orderIds, newStatus, changedBy, reason);

                _logger.LogInformation("Bulk updated {Count} orders", updatedCount);
                return ApiResponse<int>.SuccessResponse(updatedCount, $"{updatedCount} orders updated SuccessResponsefully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk update");
                return ApiResponse<int>.FailureResponse("Failed to bulk update BULK_UPDATE_ERROR");
            }
        }

        // ========================================
        // PRIVATE HELPERS
        // ========================================

        private OrderDetailDto MapOrderToDetailDto(Order order)
        {
            var dto = _mapper.Map<OrderDetailDto>(order);

            // Additional manual mapping if needed
            dto.StoreName = order.Store?.Name;
            dto.CustomerName = order.Customer?.Name;
            dto.CustomerEmail = order.Customer?.Email;
            dto.CustomerPhone = order.Customer?.Phone;
            dto.CreatedByName = order.CreatedByUser?.Name;

            // Map items
            dto.Items = order.OrderItems?
                .Select(oi => new OrderItemDTO
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    ProductSKU = oi.ProductCode,
                    ProductImageUrl = oi.Product?.Images?.FirstOrDefault()?.Url,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.TotalPrice
                })
                .ToList() ?? new List<OrderItemDTO>();

            // Map shipping
            if (order.OrderShipping != null)
            {
                dto.Shipping = _mapper.Map<OrderShippingDto>(order.OrderShipping);
            }

            // Map status history
            dto.StatusHistories = order.OrderStatusHistories?
                .OrderByDescending(h => h.ChangedAt)
                .Select(h => new OrderStatusHistoryDTO
                {
                    Status = h.NewStatus,
                    ChangedAt = h.ChangedAt,
                    ChangedByName = h.ChangedByUser?.Name,
                    Notes = h.Notes
                })
                .ToList() ?? new List<OrderStatusHistoryDTO>();

            return dto;
        }
    }
}