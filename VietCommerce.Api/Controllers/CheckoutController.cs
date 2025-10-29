// File: VietCommerce.Api/Controllers/V1/CheckoutController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Controllers.V1
{
    /// <summary>
    /// Checkout Controller - Handles order creation and management
    /// Base URL: /api/v1/checkout
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            ICheckoutService checkoutService,
            ILogger<CheckoutController> logger)
        {
            _checkoutService = checkoutService;
            _logger = logger;
        }

        // ========================================
        // CHECKOUT ENDPOINTS
        // ========================================

        /// <summary>
        /// Process checkout and create order from cart
        /// </summary>
        /// <param name="dto">Checkout request containing cart ID and shipping info</param>
        /// <remarks>
        /// Flow:
        /// 1. Validate cart exists and belongs to user
        /// 2. Validate cart has items and products are available
        /// 3. Create order with items, shipping, and calculate totals
        /// 4. Clear cart after successful order creation
        /// 
        /// Example Request:
        /// POST /api/v1/checkout/process
        /// {
        ///   "cartId": "guid",
        ///   "shippingInfo": {
        ///     "recipientName": "Nguyễn Văn A",
        ///     "phoneNumber": "0123456789",
        ///     "address": "123 Đường ABC",
        ///     "ward": "Phường 1",
        ///     "district": "Quận 1",
        ///     "city": "TP. Hồ Chí Minh",
        ///     "postalCode": "70000",
        ///     "shippingMethod": "STANDARD"
        ///   },
        ///   "notes": "Please ring doorbell twice"
        /// }
        /// </remarks>
        /// <returns>Order created with OrderId and OrderNumber</returns>
        [HttpPost("process")]
        [ProducesResponseType(typeof(ApiResponse<CheckoutResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CheckoutResponseDto>>> ProcessCheckout(
            [FromBody] CheckoutDto dto)
        {
            _logger.LogInformation("Processing checkout for cart {CartId}", dto.CartId);

            var userId = GetCurrentUserId();
            var result = await _checkoutService.CheckoutAsync(userId, dto);

            if (!result.Success)
            {
                // Return appropriate status code based on error
                return result.Message switch
                {
                    "CART_NOT_FOUND" or "CUSTOMER_NOT_FOUND" or "STORE_NOT_FOUND" => NotFound(result),
                    "UNAUTHORIZED_CART" => Forbid(),
                    "INSUFFICIENT_STOCK" => BadRequest(result),
                    _ => BadRequest(result)
                };
            }

            return Ok(result);
        }

        // ========================================
        // ORDER RETRIEVAL ENDPOINTS
        // ========================================

        /// <summary>
        /// Get order by ID (customer can only see their own orders)
        /// </summary>
        /// <param name="orderId">Order ID</param>
        [HttpGet("{orderId}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<OrderDetailDto>>> GetOrderById(Guid orderId)
        {
            _logger.LogInformation("Getting order {OrderId}", orderId);

            var userId = GetCurrentUserId();
            var result = await _checkoutService.GetOrderByIdAsync(userId, orderId);

            if (!result.Success)
            {
                return result.Message switch
                {
                    "ORDER_NOT_FOUND" => NotFound(result),
                    "UNAUTHORIZED" => Forbid(result.Message),
                    _ => BadRequest(result)
                };
            }

            return Ok(result);
        }

        /// <summary>
        /// Get user's order history (paginated with filtering and sorting)
        /// </summary>
        /// <remarks>
        /// Supports:
        /// - Keyword search (order number, customer name)
        /// - Status filtering (Pending, Confirmed, Shipped, Completed, Cancelled)
        /// - Date range filtering (FromDate, ToDate)
        /// - Amount range filtering (MinAmount, MaxAmount)
        /// - Sorting by: CreatedAt, TotalAmount, OrderNumber, Status, CustomerName
        /// - Pagination (Page, PageSize)
        /// 
        /// Example: GET /api/v1/checkout/my-orders?page=1&pageSize=10&status=2&sortBy=CreatedAt&sortDescending=true
        /// </remarks>
        [HttpGet("my-orders")]
        [ProducesResponseType(typeof(ApiResponse<List<OrderDetailDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<OrderDetailDto>>>> GetMyOrders(
            [FromQuery] OrderFilterDTO filter)
        {
            _logger.LogInformation("Getting orders for user with filter: Page={Page}, PageSize={PageSize}",
                filter.Page, filter.PageSize);

            // Validate filter
            if (!filter.IsValid(out var validationError))
            {
                return BadRequest(ApiResponse<List<OrderDetailDto>>.FailureResponse($"Invalid filter: {validationError} INVALID_FILTER"));
            }

            var userId = GetCurrentUserId();
            var result = await _checkoutService.GetOrdersAsync(userId, filter);

            return Ok(result);
        }

        // ========================================
        // ORDER CANCELLATION ENDPOINTS
        // ========================================

        /// <summary>
        /// Cancel an order (customer can only cancel their own orders)
        /// </summary>
        /// <remarks>
        /// Business Rules:
        /// - Only orders with status Pending or Confirmed can be cancelled
        /// - User must own the order
        /// - Provides reason for cancellation (tracked in status history)
        /// - Automatically restores product stock (if implemented)
        /// 
        /// Example Request:
        /// POST /api/v1/checkout/{orderId}/cancel
        /// {
        ///   "reason": "Changed my mind about this order"
        /// }
        /// </remarks>
        /// <param name="orderId">Order ID to cancel</param>
        /// <param name="request">Cancellation request with reason</param>
        [HttpPost("{orderId}/cancel")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> CancelOrder(
            Guid orderId,
            [FromBody] CancelOrderRequest request)
        {
            _logger.LogInformation("Cancelling order {OrderId} with reason: {Reason}", orderId, request.Reason);

            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return BadRequest(ApiResponse<bool>.FailureResponse("Cancellation reason is required MISSING_REASON"));
            }

            var userId = GetCurrentUserId();
            var result = await _checkoutService.CancelOrderAsync(userId, orderId, request.Reason);

            if (!result.Success)
            {
                return result.Message switch
                {
                    "ORDER_NOT_FOUND" => NotFound(result),
                    "UNAUTHORIZED" => Forbid(result.Message),
                    "CANNOT_CANCEL" => BadRequest(result),
                    _ => BadRequest(result)
                };
            }

            return Ok(result);
        }

        // ========================================
        // HELPER METHODS
        // ========================================

        /// <summary>
        /// Extract current user ID from JWT token
        /// </summary>
        private Guid GetCurrentUserId()
        {
            // Try standard claims
            var userIdClaim = User.FindFirst("sub")
                              ?? User.FindFirst("nameid")  // your JWT đang dùng
                              ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                return userId;

            throw new UnauthorizedAccessException("Unable to retrieve current user ID from token");
        }


        // ========================================
        // REQUEST DTOs (Local to Controller)
        // ========================================

        /// <summary>
        /// Request model for cancelling order
        /// </summary>
        public class CancelOrderRequest
        {
            /// <summary>
            /// Reason for cancellation (required, max 500 chars)
            /// </summary>
            [Required(ErrorMessage = "Reason is required")]
            [MaxLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
            public string Reason { get; set; } = string.Empty;
        }
    }
}