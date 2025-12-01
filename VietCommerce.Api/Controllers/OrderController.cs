// File: VietCommerce.Api/Controllers/V1/OrderController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Models;
using static VietCommerce.Api.Controllers.V1.CheckoutController;

namespace VietCommerce.Api.Controllers.V1
{
    /// <summary>
    /// Order management endpoints
    /// ✅ Service layer handles all authorization via ICurrentUser
    /// ✅ Controller focuses on HTTP concerns only
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Get order by ID
        /// ✅ Service automatically checks if user has permission to view this order
        /// </summary>
        /// <param name="id">Order ID</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<OrderDetailDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<OrderDetailDto>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<OrderDetailDto>>> GetOrderById(Guid id)
        {
            _logger.LogInformation("GET order {OrderId}", id);
            var result = await _orderService.GetOrderByIdAsync(id);

            if (!result.Success)
            {
                // Check error message for appropriate HTTP status code
                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get current user's orders (paginated)
        /// ✅ Service automatically uses CustomerId from JWT token
        /// </summary>
        /// <param name="filter">Filter options (status, date range, etc.)</param>
        [HttpGet("my-orders")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<OrderDetailDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<OrderDetailDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PaginatedResult<OrderDetailDto>>>> GetMyOrders(
            [FromQuery] OrderFilterDTO filter)
        {
            _logger.LogInformation("GET user's orders");

            var result = await _orderService.GetUserOrdersAsync(filter);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get all orders (admin/seller only)
        /// ✅ Service enforces admin check internally
        /// </summary>
        /// <param name="filter">Filter options</param>
        [HttpGet]
        [Authorize(Roles = "Admin, Seller")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<OrderDetailDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<OrderDetailDto>>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<PaginatedResult<OrderDetailDto>>>> GetAllOrders(
            [FromQuery] OrderFilterDTO filter)
        {
            _logger.LogInformation("GET all orders (admin)");

            var result = await _orderService.GetAllOrdersAsync(filter);

            if (!result.Success && result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true)
                return StatusCode(StatusCodes.Status403Forbidden, result);

            return Ok(result);
        }

        /// <summary>
        /// Search orders by keyword
        /// ✅ Service automatically filters results based on user role
        /// </summary>
        /// <param name="keyword">Search keyword (order number, customer name, etc.)</param>
        //[HttpGet("search")]
        //[ProducesResponseType(typeof(ApiResponse<List<OrderDetailDto>>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponse<List<OrderDetailDto>>), StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<ApiResponse<List<OrderDetailDto>>>> SearchOrders(
        //    [FromQuery(Name = "q")] string keyword)
        //{
        //    _logger.LogInformation("Search orders with keyword: {Keyword}", keyword);

        //    var result = await _orderService.SearchOrdersAsync(keyword);

        //    if (!result.Success)
        //        return BadRequest(result);

        //    return Ok(result);
        //}

        /// <summary>
        /// Get orders by status
        /// ✅ Service automatically filters based on user role
        /// </summary>
        /// <param name="status">Order status</param>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(ApiResponse<List<OrderDetailDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<OrderDetailDto>>>> GetOrdersByStatus(OrderStatus status)
        {
            _logger.LogInformation("GET orders by status: {Status}", status);

            var result = await _orderService.GetOrdersByStatusAsync(status);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get recent orders (dashboard)
        /// ✅ Service automatically filters based on user role
        /// </summary>
        /// <param name="count">Number of recent orders</param>
        /// <param name="storeId">Optional store filter</param>
        //[HttpGet("recent")]
        //[ProducesResponseType(typeof(ApiResponse<List<OrderDetailDto>>), StatusCodes.Status200OK)]
        //public async Task<ActionResult<ApiResponse<List<OrderDetailDto>>>> GetRecentOrders(
        //    [FromQuery] int count = 10,
        //    [FromQuery] Guid? storeId = null)
        //{
        //    _logger.LogInformation("GET {Count} recent orders", count);

        //    var result = await _orderService.GetRecentOrdersAsync(count, storeId);

        //    if (!result.Success)
        //        return BadRequest(result);

        //    return Ok(result);
        //}

        /// <summary>
        /// Get order status history
        /// ✅ Service checks permission before returning history
        /// </summary>
        /// <param name="orderId">Order ID</param>
        [HttpGet("{orderId}/status-history")]
        [ProducesResponseType(typeof(ApiResponse<List<OrderStatusHistoryDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<OrderStatusHistoryDTO>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<List<OrderStatusHistoryDTO>>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<List<OrderStatusHistoryDTO>>>> GetStatusHistory(Guid orderId)
        {
            _logger.LogInformation("GET status history for order {OrderId}", orderId);

            var result = await _orderService.GetOrderStatusHistoryAsync(orderId);

            if (!result.Success)
            {
                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update order status
        /// ✅ Service automatically uses current UserId as changedBy
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="request">Status update request</param>
        [HttpPut("{orderId}/status")]
        [Authorize(Roles = "Admin, Seller")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateOrderStatus(
            Guid orderId,
            [FromBody] OrderStatusUpdateDTO request)
        {
            _logger.LogInformation("Update order {OrderId} status to {Status}", orderId, request.Status);

            var result = await _orderService.UpdateOrderStatusAsync(
                orderId,
                request.Status,
                request.Notes);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Cancel an order
        /// ✅ Service automatically uses current UserId as changedBy
        /// ✅ Service checks if user has permission to cancel
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="request">Cancel request with reason</param>
        [HttpPost("{orderId}/cancel")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> CancelOrder(
            Guid orderId,
            [FromBody] CancelOrderRequest request)
        {
            _logger.LogInformation("Cancel order {OrderId} with reason: {Reason}", orderId, request.Reason);

            var result = await _orderService.CancelOrderAsync(orderId, request.Reason);

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Check if order status can be changed
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="newStatus">Desired new status</param>
        [HttpGet("{orderId}/can-change-status")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> CanChangeStatus(
            Guid orderId,
            [FromQuery] OrderStatus newStatus)
        {
            _logger.LogInformation("Check if order {OrderId} can change to {Status}", orderId, newStatus);

            var result = await _orderService.CanChangeStatusAsync(orderId, newStatus);
            return Ok(result);
        }

        /// <summary>
        /// Get order statistics - total count
        /// ✅ Service automatically filters based on user role
        /// </summary>
        /// <param name="storeId">Optional store filter</param>
        /// <param name="fromDate">Optional start date</param>
        /// <param name="toDate">Optional end date</param>
        [HttpGet("stats/count")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<int>>> GetTotalOrdersCount(
            [FromQuery] Guid? storeId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            _logger.LogInformation("GET total orders count");

            var result = await _orderService.GetTotalOrdersCountAsync(storeId, fromDate, toDate);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get order statistics - total revenue
        /// ✅ ADMIN ONLY - Service enforces this
        /// </summary>
        /// <param name="storeId">Optional store filter</param>
        /// <param name="fromDate">Optional start date</param>
        /// <param name="toDate">Optional end date</param>
        [HttpGet("stats/revenue")]
        [Authorize(Roles = "Admin, Seller")]
        [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<decimal>>> GetTotalRevenue(
            [FromQuery] Guid? storeId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            _logger.LogInformation("GET total revenue");

            var result = await _orderService.GetTotalRevenueAsync(storeId, fromDate, toDate);

            if (!result.Success && result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true)
                return StatusCode(StatusCodes.Status403Forbidden, result);

            return Ok(result);
        }

        /// <summary>
        /// Bulk update order statuses
        /// ✅ ADMIN ONLY - Service enforces this
        /// ✅ Service automatically uses current UserId as changedBy
        /// </summary>
        /// <param name="request">Bulk update request</param>
        [HttpPost("bulk-update-status")]
        [Authorize(Roles = "Admin, Seller")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<int>>> BulkUpdateStatus(
            [FromBody] BulkUpdateStatusRequest request)
        {
            _logger.LogInformation("Bulk update {Count} orders to status {Status}",
                request.OrderIds?.Count ?? 0, request.NewStatus);

            if (request.OrderIds == null || !request.OrderIds.Any())
            {
                return BadRequest(ApiResponse<int>.FailureResponse("Order IDs list cannot be empty"));
            }

            var result = await _orderService.BulkUpdateStatusAsync(
                request.OrderIds,
                request.NewStatus,
                request.Reason);

            if (!result.Success)
            {
                if (result.Message?.Contains("denied", StringComparison.OrdinalIgnoreCase) == true)
                    return StatusCode(StatusCodes.Status403Forbidden, result);

                return BadRequest(result);
            }

            return Ok(result);
        }
    }

    // ========================================
    // REQUEST DTOs
    // ========================================


    /// <summary>
    /// Request to bulk update order statuses
    /// </summary>
    public class BulkUpdateStatusRequest
    {
        /// <summary>
        /// List of order IDs to update
        /// </summary>
        public List<Guid> OrderIds { get; set; } = new();

        /// <summary>
        /// New status to apply
        /// </summary>
        public OrderStatus NewStatus { get; set; }

        /// <summary>
        /// Optional reason for the update
        /// </summary>
        public string? Reason { get; set; }
    }
}