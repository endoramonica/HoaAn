// File: VietCommerce.Api/Services/OrderService.cs
using AutoMapper;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Helpers;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly ICurrentUser _currentUser; // ✅ THÊM dependency
        private readonly IRealtimeService? _realtime;

        public OrderService(
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<OrderService> logger,
            ICurrentUser currentUser,
            IRealtimeService? realtime = null) // ✅ THÊM vào constructor
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser; // ✅ INJECT service
            _realtime = realtime;
        }

        /// <summary>
        /// Get order by ID with full details
        /// ✅ TỰ ĐỘNG kiểm tra quyền xem order dựa trên user hiện tại
        /// </summary>
        public async Task<ApiResponse<OrderDetailDto>> GetOrderByIdAsync(Guid orderId)
        {
            try
            {
                _logger.LogInformation("Fetching order with ID: {OrderId}", orderId);

                var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

                if (order == null)
                {
                    _logger.LogWarning("Order not found: {OrderId}", orderId);
                    return ApiResponse<OrderDetailDto>.FailureResponse("ORDER_NOT_FOUND");
                }

                // ✅ KIỂM TRA QUYỀN: User chỉ xem được order của mình (trừ Admin)
                if (!_currentUser.IsAdmin && order.CustomerId != _currentUser.CustomerId)
                {
                    _logger.LogWarning(
                        "User {UserId} (Customer: {CustomerId}) attempted to access order {OrderId} belonging to customer {OrderCustomerId}",
                        _currentUser.UserId, _currentUser.CustomerId, orderId, order.CustomerId);

                    return ApiResponse<OrderDetailDto>.FailureResponse("ACCESS_DENIED");
                }

                var orderDto = MapOrderToDetailDto(order);

                _logger.LogInformation(
                    "✅ Order fetched: {OrderId}, CustomerId: {CustomerId}, Accessed by: {UserId}",
                    orderId, order.CustomerId, _currentUser.UserId);

                return ApiResponse<OrderDetailDto>.SuccessResponse(orderDto, "Order retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId}", orderId);
                return ApiResponse<OrderDetailDto>.FailureResponse("ORDER_RETRIEVAL_ERROR");
            }
        }

        /// <summary>
        /// Get paginated orders for current user
        /// ✅ TỰ ĐỘNG lấy userId/customerId từ ICurrentUser
        /// </summary>
        public async Task<ApiResponse<PaginatedResult<OrderDetailDto>>> GetUserOrdersAsync(OrderFilterDTO filter)
        {
            try
            {
                // ✅ TỰ ĐỘNG lấy CustomerId từ token
                var customerId = _currentUser.CustomerId;

                if (customerId == Guid.Empty)
                {
                    _logger.LogWarning("User {UserId} has no associated CustomerId", _currentUser.UserId);
                    return ApiResponse<PaginatedResult<OrderDetailDto>>.FailureResponse("CUSTOMER_NOT_FOUND");
                }

                _logger.LogInformation("Fetching orders for customer: {CustomerId}", customerId);

                var result = await _orderRepository.GetUserOrdersAsync(customerId, filter);

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
                _logger.LogError(ex, "Error fetching user orders for {UserId}", _currentUser.UserId);
                return ApiResponse<PaginatedResult<OrderDetailDto>>.FailureResponse("ORDER_LIST_ERROR");
            }
        }

        /// <summary>
        /// Get all orders (admin view)
        /// ✅ CHỈ ADMIN mới được gọi
        /// </summary>
        public async Task<ApiResponse<PaginatedResult<OrderDetailDto>>> GetAllOrdersAsync(OrderFilterDTO filter)
        {
            try
            {
                // ✅ KIỂM TRA QUYỀN ADMIN
                if (!_currentUser.IsAdmin)
                {
                    _logger.LogWarning("Non-admin user {UserId} attempted to access all orders", _currentUser.UserId);
                    return ApiResponse<PaginatedResult<OrderDetailDto>>.FailureResponse("ACCESS_DENIED");
                }

                _logger.LogInformation("Admin {UserId} fetching all orders with filter", _currentUser.UserId);

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

        /// <summary>
        /// Get orders by status
        /// ✅ Admin: xem tất cả, User: chỉ xem của mình
        /// </summary>
        public async Task<ApiResponse<List<OrderDetailDto>>> GetOrdersByStatusAsync(OrderStatus status)
        {
            try
            {
                _logger.LogInformation("Fetching orders with status: {Status}", status);

                List<Order> orders;

                if (_currentUser.IsAdmin)
                {
                    // Admin xem tất cả orders theo status
                    orders = await _orderRepository.GetOrdersByStatusAsync(status);
                }
                else
                {
                    // User chỉ xem orders của mình theo status
                    var customerId = _currentUser.CustomerId;
                    if (customerId == Guid.Empty)
                    {
                        return ApiResponse<List<OrderDetailDto>>.FailureResponse("CUSTOMER_NOT_FOUND");
                    }

                    orders = (await _orderRepository.GetOrdersByStatusAsync(status))
                        .Where(o => o.CustomerId == customerId)
                        .ToList();
                }

                var dtoList = orders.Select(MapOrderToDetailDto).ToList();

                return ApiResponse<List<OrderDetailDto>>.SuccessResponse(dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders by status {Status}", status);
                return ApiResponse<List<OrderDetailDto>>.FailureResponse("ORDER_LIST_ERROR");
            }
        }

        /// <summary>
        /// Get recent orders
        /// ✅ Admin: xem tất cả, User: chỉ xem của mình
        /// </summary>
        //public async Task<ApiResponse<List<OrderDetailDto>>> GetRecentOrdersAsync(int count = 10, Guid? storeId = null)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Fetching {Count} recent orders", count);

        //        List<Order> orders;

        //        if (_currentUser.IsAdmin)
        //        {
        //            orders = await _unitOfWork.Orders.GetRecentOrdersAsync
        //        }
        //        else
        //        {
        //            var customerId = _currentUser.CustomerId;
        //            if (customerId == Guid.Empty)
        //            {
        //                return ApiResponse<List<OrderDetailDto>>.FailureResponse("CUSTOMER_NOT_FOUND");
        //            }

        //            orders = (await _orderRepository.GetRecentOrdersAsync(count * 2, storeId))
        //                .Where(o => o.CustomerId == customerId)
        //                .Take(count)
        //                .ToList();
        //        }

        //        var dtoList = orders.Select(MapOrderToDetailDto).ToList();

        //        return ApiResponse<List<OrderDetailDto>>.SuccessResponse(dtoList);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error fetching recent orders");
        //        return ApiResponse<List<OrderDetailDto>>.FailureResponse("ORDER_LIST_ERROR");
        //    }
        //}

        /// <summary>
        /// Get order status history
        /// ✅ Kiểm tra quyền trước khi xem history
        /// </summary>
        public async Task<ApiResponse<List<OrderStatusHistoryDTO>>> GetOrderStatusHistoryAsync(Guid orderId)
        {
            try
            {
                _logger.LogInformation("Fetching status history for order: {OrderId}", orderId);

                // ✅ KIỂM TRA QUYỀN trước
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    return ApiResponse<List<OrderStatusHistoryDTO>>.FailureResponse("ORDER_NOT_FOUND");
                }

                if (!_currentUser.IsAdmin && order.CustomerId != _currentUser.CustomerId)
                {
                    _logger.LogWarning("User {UserId} attempted to access history of order {OrderId}",
                        _currentUser.UserId, orderId);
                    return ApiResponse<List<OrderStatusHistoryDTO>>.FailureResponse("ACCESS_DENIED");
                }

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

        /// <summary>
        /// Search orders
        /// ✅ Admin: search tất cả, User: search trong orders của mình
        /// </summary>
        //public async Task<ApiResponse<List<OrderDetailDto>>> SearchOrdersAsync(string keyword)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(keyword))
        //        {
        //            return ApiResponse<List<OrderDetailDto>>.FailureResponse("INVALID_SEARCH");
        //        }

        //        _logger.LogInformation("Searching orders with keyword: {Keyword}", keyword);

        //        List<Order> orders;

        //        if (_currentUser.IsAdmin)
        //        {
        //            orders = await _orderRepository.SearchOrdersAsync(keyword);
        //        }
        //        else
        //        {
        //            var customerId = _currentUser.CustomerId;
        //            if (customerId == Guid.Empty)
        //            {
        //                return ApiResponse<List<OrderDetailDto>>.FailureResponse("CUSTOMER_NOT_FOUND");
        //            }

        //            orders = (await _orderRepository.SearchOrdersAsync(keyword))
        //                .Where(o => o.CustomerId == customerId)
        //                .ToList();
        //        }

        //        var dtoList = orders.Select(MapOrderToDetailDto).ToList();

        //        return ApiResponse<List<OrderDetailDto>>.SuccessResponse(dtoList);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error searching orders with keyword: {Keyword}", keyword);
        //        return ApiResponse<List<OrderDetailDto>>.FailureResponse("SEARCH_ERROR");
        //    }
        //}

        /// <summary>
        /// Update order status
        /// ✅ TỰ ĐỘNG lấy changedBy từ current user
        /// </summary>
        public async Task<ApiResponse<bool>> UpdateOrderStatusAsync(
            Guid orderId,
            OrderStatus newStatus,
            string? notes = null)
        {
            try
            {
                // ✅ TỰ ĐỘNG lấy userId từ token
                var changedBy = _currentUser.UserId;

                _logger.LogInformation("User {UserId} updating order {OrderId} status to {NewStatus}",
                    changedBy, orderId, newStatus);

                // Validate status transition
                var canChange = await _orderRepository.CanChangeStatusAsync(orderId, newStatus);
                if (!canChange)
                {
                    _logger.LogWarning("Invalid status transition for order {OrderId}", orderId);
                    return ApiResponse<bool>.FailureResponse("INVALID_STATUS_TRANSITION");
                }

                // Update status
                var result = await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus, changedBy, notes);

                if (!result)
                {
                    return ApiResponse<bool>.FailureResponse("ORDER_NOT_FOUND");
                }

                _logger.LogInformation("Order {OrderId} status updated successfully", orderId);
                if (_realtime != null)
                    await _realtime.BroadcastOrderStatusUpdatedAsync(orderId, (int)newStatus);
                return ApiResponse<bool>.SuccessResponse(true, "Order status updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId} status", orderId);
                return ApiResponse<bool>.FailureResponse("STATUS_UPDATE_ERROR");
            }
        }

        /// <summary>
        /// Cancel order
        /// ✅ TỰ ĐỘNG lấy changedBy từ current user
        /// </summary>
        public async Task<ApiResponse<bool>> CancelOrderAsync(Guid orderId, string reason)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reason))
                {
                    return ApiResponse<bool>.FailureResponse("INVALID_REASON");
                }

                // ✅ TỰ ĐỘNG lấy userId từ token
                var changedBy = _currentUser.UserId;

                _logger.LogInformation("User {UserId} cancelling order {OrderId} with reason: {Reason}",
                    changedBy, orderId, reason);

                // ✅ KIỂM TRA QUYỀN: User chỉ cancel được order của mình
                if (!_currentUser.IsAdmin)
                {
                    var order = await _orderRepository.GetByIdAsync(orderId);
                    if (order == null)
                    {
                        return ApiResponse<bool>.FailureResponse("ORDER_NOT_FOUND");
                    }

                    if (order.CustomerId != _currentUser.CustomerId)
                    {
                        _logger.LogWarning("User {UserId} attempted to cancel order {OrderId} not owned by them",
                            changedBy, orderId);
                        return ApiResponse<bool>.FailureResponse("ACCESS_DENIED");
                    }
                }

                var result = await _orderRepository.CancelOrderAsync(orderId, reason, changedBy);

                if (!result)
                {
                    _logger.LogWarning("Cannot cancel order {OrderId}", orderId);
                    return ApiResponse<bool>.FailureResponse("CANNOT_CANCEL_ORDER");
                }

                _logger.LogInformation("Order {OrderId} cancelled successfully", orderId);
                return ApiResponse<bool>.SuccessResponse(true, "Order cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", orderId);
                return ApiResponse<bool>.FailureResponse("CANCEL_ERROR");
            }
        }

        /// <summary>
        /// Check if status change is valid
        /// </summary>
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
                return ApiResponse<bool>.FailureResponse("VALIDATION_ERROR");
            }
        }

        /// <summary>
        /// Get total orders count
        /// ✅ Admin: tất cả orders, User: chỉ orders của mình
        /// </summary>
        public async Task<ApiResponse<int>> GetTotalOrdersCountAsync(
            Guid? storeId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                int count;

                if (_currentUser.IsAdmin)
                {
                    count = await _orderRepository.GetTotalOrdersCountAsync(storeId, fromDate, toDate);
                }
                else
                {
                    // User chỉ đếm orders của mình
                    var customerId = _currentUser.CustomerId;
                    if (customerId == Guid.Empty)
                    {
                        return ApiResponse<int>.FailureResponse("CUSTOMER_NOT_FOUND");
                    }

                    var filter = new OrderFilterDTO
                    {
                        CustomerId = customerId,
                        FromDate = fromDate,
                        ToDate = toDate
                    };

                    var result = await _orderRepository.GetUserOrdersAsync(customerId, filter);
                    count = result.TotalItems;
                }

                return ApiResponse<int>.SuccessResponse(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total orders count");
                return ApiResponse<int>.FailureResponse("COUNT_ERROR");
            }
        }

        /// <summary>
        /// Get total revenue
        /// ✅ CHỈ ADMIN mới được xem
        /// </summary>
        public async Task<ApiResponse<decimal>> GetTotalRevenueAsync(
            Guid? storeId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                // ✅ KIỂM TRA QUYỀN ADMIN
                if (!_currentUser.IsAdmin)
                {
                    _logger.LogWarning("Non-admin user {UserId} attempted to access revenue", _currentUser.UserId);
                    return ApiResponse<decimal>.FailureResponse("ACCESS_DENIED");
                }

                var revenue = await _orderRepository.GetTotalRevenueAsync(storeId, fromDate, toDate);
                return ApiResponse<decimal>.SuccessResponse(revenue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue");
                return ApiResponse<decimal>.FailureResponse("REVENUE_ERROR");
            }
        }

        /// <summary>
        /// Bulk update order statuses
        /// ✅ CHỈ ADMIN mới được bulk update
        /// </summary>
        public async Task<ApiResponse<int>> BulkUpdateStatusAsync(
            List<Guid> orderIds,
            OrderStatus newStatus,
            string? reason = null)
        {
            try
            {
                // ✅ KIỂM TRA QUYỀN ADMIN
                if (!_currentUser.IsAdmin)
                {
                    _logger.LogWarning("Non-admin user {UserId} attempted bulk update", _currentUser.UserId);
                    return ApiResponse<int>.FailureResponse("ACCESS_DENIED");
                }

                if (!orderIds.Any())
                {
                    return ApiResponse<int>.FailureResponse("EMPTY_LIST");
                }

                // ✅ TỰ ĐỘNG lấy changedBy
                var changedBy = _currentUser.UserId;

                _logger.LogInformation("Admin {UserId} bulk updating {Count} orders to status {Status}",
                    changedBy, orderIds.Count, newStatus);

                var updatedCount = await _orderRepository.BulkUpdateStatusAsync(orderIds, newStatus, changedBy, reason);

                _logger.LogInformation("Bulk updated {Count} orders", updatedCount);
                return ApiResponse<int>.SuccessResponse(updatedCount, $"{updatedCount} orders updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bulk update");
                return ApiResponse<int>.FailureResponse("BULK_UPDATE_ERROR");
            }
        }

        /// <summary>
        /// Create an OrderItem from a CartItem by copying available snapshot data.
        /// ⚠️ Assumes CartItem pricing has been calculated beforehand.
        /// </summary>
        /// <param name="cartItem">CartItem to convert to OrderItem</param>
        /// <param name="orderId">Order ID to associate with the new OrderItem</param>
        /// <returns>
        /// An OrderItem created by copying snapshot data from the CartItem.
        /// Pricing values are copied as-is without recalculation.
        /// </returns>
        /// <remarks>
        /// IMPORTANT:
        /// - This method DOES NOT calculate, recalculate, or validate pricing.
        /// - BasePrice, CustomizationPrice, UnitPrice, and FinalPrice MUST already be
        ///   correctly populated on the CartItem before calling this method.
        /// - This method blindly copies pricing values from the CartItem.
        /// - If CartItem pricing data is missing, zero, or incorrect, the resulting
        ///   OrderItem will contain the same incorrect values.
        /// - Pricing responsibility belongs to the Cart domain / CartService, not here.
        /// </remarks>


        public async Task<OrderItem> CreateOrderItemFromCartItemAsync(
            CartItem cartItem,
            Guid orderId)
        {
            try
            {
                _logger.LogInformation(
                    "Creating OrderItem from CartItem {CartItemId} for Order {OrderId}",
                    cartItem.Id, orderId);

                // Create new OrderItem with snapshotted data
                var orderItem = new OrderItem
                {
                    OrderId = orderId,
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.Product?.Name ?? "Unknown Product",
                    ProductCode = cartItem.Product?.Code ?? "UNKNOWN",
                    UnitPrice = cartItem.FinalPrice,
                    Quantity = cartItem.Quantity,
                    TotalPrice = cartItem.FinalPrice,

                    // Snapshot customization data
                    CustomizationsJson = cartItem.CustomizationsJson,
                    BasePrice = cartItem.BasePrice,
                    CustomizationPrice = cartItem.CustomizationPrice,

                    // Audit fields
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = _currentUser.UserId,
                    IsActive = true,
                    IsDeleted = false
                };

                _logger.LogInformation(
                    "OrderItem created from CartItem {CartItemId}: " +
                    "BasePrice={BasePrice}, CustomizationPrice={CustomizationPrice}, FinalPrice={FinalPrice}",
                    cartItem.Id, cartItem.BasePrice, cartItem.CustomizationPrice, cartItem.FinalPrice);

                return await Task.FromResult(orderItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error creating OrderItem from CartItem {CartItemId} for Order {OrderId}",
                    cartItem.Id, orderId);
                throw;
            }
        }

        // ========================================
        // PRIVATE HELPERS
        // ========================================

        private OrderDetailDto MapOrderToDetailDto(Order order)
        {
            var dto = _mapper.Map<OrderDetailDto>(order);

            dto.StoreName = order.Store?.Name;
            dto.CustomerName = order.Customer?.Name;
            dto.CustomerEmail = order.Customer?.Email;
            dto.CustomerPhone = order.Customer?.Phone;
            dto.CreatedByName = order.CreatedByUser?.Name;

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

            if (order.OrderShipping != null)
            {
                dto.Shipping = _mapper.Map<OrderShippingDto>(order.OrderShipping);
            }

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
