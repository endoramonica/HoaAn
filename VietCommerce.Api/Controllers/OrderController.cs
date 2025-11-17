// File: VietCommerce.Api/Controllers/V1/OrderController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Controllers.V1
{
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
        /// </summary>
        /// <param name="id">Order ID</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<OrderDetailDto>>> GetOrderById(Guid id)
        {
            _logger.LogInformation("GET order {OrderId}", id);
            var result = await _orderService.GetOrderByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get current user's orders (paginated)
        /// </summary>
        /// <param name="filter">Filter options</param>
        [HttpGet("my-orders")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<OrderDetailDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PaginatedResult<OrderDetailDto>>>> GetMyOrders(
            [FromQuery] OrderFilterDTO filter)
        {
            _logger.LogInformation("GET user's orders");

            var userId = GetCurrentUserId();
            var result = await _orderService.GetUserOrdersAsync(userId, filter);

            return Ok(result);
        }

        /// <summary>
        /// Get all orders (admin only)
        /// </summary>
        /// <param name="filter">Filter options</param>
        [HttpGet]
        [Authorize(Roles = "Admin, Seller")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<OrderDetailDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PaginatedResult<OrderDetailDto>>>> GetAllOrders(
            [FromQuery] OrderFilterDTO filter)
        {
            _logger.LogInformation("GET all orders (admin)");

            var result = await _orderService.GetAllOrdersAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Search orders by keyword
        /// </summary>
        /// <param name="keyword">Search keyword (order number, customer name, etc.)</param>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<OrderDetailDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<OrderDetailDto>>>> SearchOrders(
            [FromQuery(Name = "q")] string keyword)
        {
            _logger.LogInformation("Search orders with keyword: {Keyword}", keyword);

            var result = await _orderService.SearchOrdersAsync(keyword);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get orders by status
        /// </summary>
        /// <param name="status">Order status</param>
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin, Seller")]
        [ProducesResponseType(typeof(ApiResponse<List<OrderDetailDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<OrderDetailDto>>>> GetOrdersByStatus(OrderStatus status)
        {
            _logger.LogInformation("GET orders by status: {Status}", status);

            var result = await _orderService.GetOrdersByStatusAsync(status);
            return Ok(result);
        }

        /// <summary>
        /// Get recent orders (dashboard)
        /// </summary>
        /// <param name="count">Number of recent orders</param>
        /// <param name="storeId">Optional store filter</param>
        [HttpGet("recent")]
        [ProducesResponseType(typeof(ApiResponse<List<OrderDetailDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<OrderDetailDto>>>> GetRecentOrders(
            [FromQuery] int count = 10,
            [FromQuery] Guid? storeId = null)
        {
            _logger.LogInformation("GET {Count} recent orders", count);

            var result = await _orderService.GetRecentOrdersAsync(count, storeId);
            return Ok(result);
        }

        /// <summary>
        /// Get order status history
        /// </summary>
        /// <param name="orderId">Order ID</param>
        [HttpGet("{orderId}/status-history")]
        [ProducesResponseType(typeof(ApiResponse<List<OrderStatusHistoryDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<OrderStatusHistoryDTO>>>> GetStatusHistory(Guid orderId)
        {
            _logger.LogInformation("GET status history for order {OrderId}", orderId);

            var result = await _orderService.GetOrderStatusHistoryAsync(orderId);
            return Ok(result);
        }

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="request">Status update request</param>
        [HttpPut("{orderId}/status")]
        [Authorize(Roles = "Admin, Seller")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateOrderStatus(
            Guid orderId,
            [FromBody] OrderStatusUpdateDTO request)
        {
            _logger.LogInformation("Update order {OrderId} status to {Status}", orderId, request.Status);

            var userId = GetCurrentUserId();
            var result = await _orderService.UpdateOrderStatusAsync(
                orderId,
                request.Status,
                userId,
                request.Notes);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="request">Cancel request with reason</param>
        //[HttpPost("{orderId}/cancel")]
        //[ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<ApiResponse<bool>>> CancelOrder(
        //    Guid orderId,
        //    [FromBody] CancelOrderAsync request)
        //{
        //    _logger.LogInformation("Cancel order {OrderId} with reason: {Reason}", orderId, request.Reason);

        //    var userId = GetCurrentUserId();
        //    var result = await _orderService.CancelOrderAsync(orderId, request.Reason, userId);

        //    if (!result.Success)
        //    {
        //        return BadRequest(result);
        //    }

        //    return Ok(result);
        //}

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
        /// </summary>
        /// <param name="storeId">Optional store filter</param>
        /// <param name="fromDate">Optional start date</param>
        /// <param name="toDate">Optional end date</param>
        [HttpGet("stats/count")]
        [Authorize(Roles = "Admin, Seller")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<int>>> GetTotalOrdersCount(
            [FromQuery] Guid? storeId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            _logger.LogInformation("GET total orders count");

            var result = await _orderService.GetTotalOrdersCountAsync(storeId, fromDate, toDate);
            return Ok(result);
        }

        /// <summary>
        /// Get order statistics - total revenue
        /// </summary>
        /// <param name="storeId">Optional store filter</param>
        /// <param name="fromDate">Optional start date</param>
        /// <param name="toDate">Optional end date</param>
        [HttpGet("stats/revenue")]
        [Authorize(Roles = "Admin, Seller")]
        [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<decimal>>> GetTotalRevenue(
            [FromQuery] Guid? storeId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            _logger.LogInformation("GET total revenue");

            var result = await _orderService.GetTotalRevenueAsync(storeId, fromDate, toDate);
            return Ok(result);
        }

        /// <summary>
        /// Bulk update order statuses
        /// </summary>
        /// <param name="request">Bulk update request</param>
        /// <summary>
        /// Bulk update order statuses
        /// </summary>
        /// <param name="request">Bulk update request</param>
        [HttpPost("bulk-update-status")]
        [Authorize(Roles = "Admin, Seller")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<int>>> BulkUpdateStatus(
            [FromBody] BulkUpdateStatusRequest request)
        {
            _logger.LogInformation("Bulk update {Count} orders to status {Status}",
                request.OrderIds.Count, request.NewStatus);

            if (request.OrderIds == null || !request.OrderIds.Any())
            {
                return BadRequest(ApiResponse<int>.FailureResponse("Order IDs list cannot be empty EMPTY_LIST"));
            }

            var userId = GetCurrentUserId();
            var result = await _orderService.BulkUpdateStatusAsync(
                request.OrderIds,
                request.NewStatus,
                userId,
                request.Reason);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // ========================================
        // PRIVATE HELPERS
        // ========================================

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Unable to retrieve current user ID");
        }
    }

    // ========================================
    // REQUEST/RESPONSE DTOs (LOCAL)
    // ========================================

    

    public class BulkUpdateStatusRequest
    {
        public List<Guid> OrderIds { get; set; } = new();
        public OrderStatus NewStatus { get; set; }
        public string? Reason { get; set; }
    }
}