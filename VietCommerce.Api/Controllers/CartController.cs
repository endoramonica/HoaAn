using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VietCommerce.Application.Helpers;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.DTOs.Orders;

namespace VietCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(
            ICartService cartService,
            ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        // 🔹 Get current authenticated user ID
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        // 🔹 Get or create guest session ID using SessionIdHelper
        private string GetOrCreateSessionId()
        {
            return SessionIdHelper.GetOrCreateSessionId(HttpContext, _logger);
        }

        // 🔹 Extract existing session ID (without creating new one)
        private string? GetSessionId()
        {
            return SessionIdHelper.ExtractSessionId(HttpContext, _logger);
        }

        // ============================================
        // USER CART ENDPOINTS (Authenticated)
        // ============================================

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _cartService.GetCartAsync(userId.Value);

            if (!result.Success)
            {
                if (result.Message.Contains("Access denied"))
                    return Forbid();
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("summary")]
        [Authorize]
        public async Task<IActionResult> GetCartSummary()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _cartService.GetCartSummaryAsync(userId.Value);

            if (!result.Success)
            {
                if (result.Message.Contains("Access denied"))
                    return Forbid();
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartService.AddToCartAsync(userId.Value, dto);

            if (!result.Success)
            {
                if (result.Message.Contains("Access denied"))
                    return Forbid();

                if (result.Message.Contains("not found") || result.Message.Contains("Insufficient"))
                    return BadRequest(result);

                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Add a package product to cart with customizations.
        /// Validates customization quantities against product constraints and calculates final price.
        /// </summary>
        /// <param name="dto">The add to cart request containing product ID and customizations</param>
        /// <returns>The newly added cart item with customization details and calculated prices</returns>
        /// <remarks>
        /// Requirements: 3.1, 3.3
        /// - Validates customizations using ProductService.ValidateCustomizationsAsync
        /// - Calculates final price: basePrice + sum(customization quantities × unit prices)
        /// - Stores customizations as JSON in CartItem
        /// - Returns CartItemDetailDto with BasePrice, CustomizationPrice, and FinalPrice breakdown
        /// </remarks>
        [HttpPost("add-with-customizations")]
        [Authorize]
        public async Task<IActionResult> AddToCartWithCustomizations([FromBody] AddToCartDto dto)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartService.AddToCartWithCustomizationsAsync(userId.Value, dto);

            if (!result.Success)
            {
                if (result.Message.Contains("Access denied"))
                    return Forbid();

                if (result.Message.Contains("not found") || result.Message.Contains("Insufficient") || result.Message.Contains("validation failed"))
                    return BadRequest(result);

                return StatusCode(500, result);
            }

            return Ok(result);
        }

        [HttpPut("update-item")]
        [Authorize]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto dto)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartService.UpdateCartItemAsync(userId.Value, dto);

            if (!result.Success)
            {
                if (result.Message.Contains("Access denied"))
                    return Forbid();

                if (result.Message.Contains("not found") || result.Message.Contains("Unauthorized"))
                    return BadRequest(result);

                if (result.Message.Contains("Insufficient"))
                    return BadRequest(result);

                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update the customizations for an existing cart item.
        /// Validates new customization quantities and recalculates the final price.
        /// </summary>
        /// <param name="cartItemId">The unique identifier of the cart item to update</param>
        /// <param name="customizations">The new list of customizations to apply</param>
        /// <returns>The updated cart item with new customization details and recalculated prices</returns>
        /// <remarks>
        /// Requirements: 4.1, 4.2
        /// - Validates new customizations against product constraints
        /// - Recalculates final price based on new customizations
        /// - Updates CartItem with new customizationsJson, customizationPrice, and finalPrice
        /// - Returns CartItemDetailDto with updated price breakdown
        /// </remarks>
        [HttpPut("items/{cartItemId}/customizations")]
        [Authorize]
        public async Task<IActionResult> UpdateCartItemCustomizations(
            Guid cartItemId,
            [FromBody] List<CartItemCustomizationDto> customizations)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartService.UpdateCartItemCustomizationsAsync(userId.Value, cartItemId, customizations);

            if (!result.Success)
            {
                if (result.Message.Contains("Access denied"))
                    return Forbid();

                if (result.Message.Contains("not found") || result.Message.Contains("Unauthorized") || result.Message.Contains("validation failed"))
                    return BadRequest(result);

                return StatusCode(500, result);
            }

            return Ok(result);
        }

        [HttpDelete("items/{cartItemId}")]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart(Guid cartItemId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _cartService.RemoveFromCartAsync(userId.Value, cartItemId);

            if (!result.Success)
            {
                if (result.Message.Contains("Access denied"))
                    return Forbid();

                if (result.Message.Contains("not found") || result.Message.Contains("Unauthorized"))
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("clear")]
        [Authorize]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _cartService.ClearCartAsync(userId.Value);

            if (!result.Success)
            {
                if (result.Message.Contains("Access denied"))
                    return Forbid();
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("validate")]
        [Authorize]
        public async Task<IActionResult> ValidateCart()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _cartService.ValidateCartForCheckoutAsync(userId.Value);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("item-count")]
        [Authorize]
        public async Task<IActionResult> GetCartItemCount()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _cartService.GetCartItemCountAsync(userId.Value);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("items/{cartItemId}")]
        [Authorize]
        public async Task<IActionResult> GetCartItemDetail(Guid cartItemId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _cartService.GetCartItemDetailAsync(userId.Value, cartItemId);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                    return NotFound(result);

                if (result.Message.Contains("Unauthorized"))
                    return Forbid();

                return BadRequest(result);
            }

            return Ok(result);
        }

        // ============================================
        // GUEST CART ENDPOINTS (Anonymous)
        // ============================================

        /// <summary>
        /// Get guest cart - session ID from cookie/header automatically
        /// </summary>
        [HttpGet("guest")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGuestCart()
        {
            // Automatically extract or create session ID
            var sessionId = GetOrCreateSessionId();

            _logger.LogDebug(
                "Getting guest cart for session: {SessionId}",
                SessionIdHelper.FormatSessionIdForLogging(sessionId));

            var result = await _cartService.GetGuestCartAsync(sessionId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get guest cart summary
        /// </summary>
        [HttpGet("guest/summary")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGuestCartSummary()
        {
            var sessionId = GetOrCreateSessionId();

            var result = await _cartService.GetGuestCartSummaryAsync(sessionId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Add item to guest cart
        /// </summary>
        [HttpPost("guest/add")]
        [AllowAnonymous]
        public async Task<IActionResult> AddToGuestCart([FromBody] AddToCartDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get or create session ID automatically
            var sessionId = GetOrCreateSessionId();

            _logger.LogInformation(
                "Adding item to guest cart | Product: {ProductId} | Quantity: {Quantity} | Session: {SessionId}",
                dto.ProductId,
                dto.Quantity,
                SessionIdHelper.FormatSessionIdForLogging(sessionId));

            var result = await _cartService.AddToGuestCartAsync(sessionId, dto);

            if (!result.Success)
            {
                if (result.Message.Contains("not found") || result.Message.Contains("Insufficient"))
                    return BadRequest(result);

                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update guest cart item
        /// </summary>
        [HttpPut("guest/items/{cartItemId}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateGuestCartItem(
            Guid cartItemId,
            [FromBody] UpdateCartItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sessionId = GetSessionId();
            if (string.IsNullOrWhiteSpace(sessionId))
                return BadRequest(new { message = "No active guest session" });

            var result = await _cartService.UpdateGuestCartItemAsync(sessionId, dto);

            if (!result.Success)
            {
                if (result.Message.Contains("not found") || result.Message.Contains("Unauthorized"))
                    return BadRequest(result);

                if (result.Message.Contains("Insufficient"))
                    return BadRequest(result);

                return StatusCode(500, result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Remove item from guest cart
        /// </summary>
        [HttpDelete("guest/items/{cartItemId}")]
        [AllowAnonymous]
        public async Task<IActionResult> RemoveFromGuestCart(Guid cartItemId)
        {
            var sessionId = GetSessionId();
            if (string.IsNullOrWhiteSpace(sessionId))
                return BadRequest(new { message = "No active guest session" });

            var result = await _cartService.RemoveFromGuestCartAsync(sessionId, cartItemId);

            if (!result.Success)
            {
                if (result.Message.Contains("not found") || result.Message.Contains("Unauthorized"))
                    return NotFound(result);

                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Clear all items from guest cart
        /// </summary>
        [HttpDelete("guest/clear")]
        [AllowAnonymous]
        public async Task<IActionResult> ClearGuestCart()
        {
            var sessionId = GetSessionId();
            if (string.IsNullOrWhiteSpace(sessionId))
                return BadRequest(new { message = "No active guest session" });

            var result = await _cartService.ClearGuestCartAsync(sessionId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Validate guest cart before checkout
        /// </summary>
        [HttpPost("guest/validate")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateGuestCart()
        {
            var sessionId = GetSessionId();
            if (string.IsNullOrWhiteSpace(sessionId))
                return BadRequest(new { message = "No active guest session" });

            var result = await _cartService.ValidateGuestCartForCheckoutAsync(sessionId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get guest cart item count
        /// </summary>
        [HttpGet("guest/item-count")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGuestCartItemCount()
        {
            var sessionId = GetSessionId();
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                // Return 0 if no session exists
                return Ok(new { success = true, data = 0 });
            }

            var result = await _cartService.GetGuestCartItemCountAsync(sessionId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // ============================================
        // MERGE & COUPON ENDPOINTS
        // ============================================

        /// <summary>
        /// Manually merge guest cart to user cart (usually done automatically on login)
        /// </summary>
        [HttpPost("merge")]
        [Authorize]
        public async Task<IActionResult> MergeGuestCart([FromBody] MergeCartDto dto)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            if (string.IsNullOrWhiteSpace(dto.SessionId))
                return BadRequest(new { message = "Session ID is required" });

            var result = await _cartService.MergeGuestCartToUserAsync(dto.SessionId, userId.Value);

            if (!result.Success)
                return BadRequest(result);

            // Clear session cookie after merge
            SessionIdHelper.ClearSessionId(Response, _logger);

            return Ok(result);
        }

        [HttpPost("coupon/apply")]
        [Authorize]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponDto dto)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            if (string.IsNullOrWhiteSpace(dto.CouponCode))
                return BadRequest(new { message = "Coupon code is required" });

            var result = await _cartService.ApplyCouponAsync(userId.Value, dto.CouponCode);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("coupon/remove")]
        [Authorize]
        public async Task<IActionResult> RemoveCoupon()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _cartService.RemoveCouponAsync(userId.Value);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("shipping")]
        [Authorize]
        public async Task<IActionResult> UpdateShippingInfo([FromBody] OrderShippingDto dto)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(new { message = "User not authenticated" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _cartService.UpdateShippingInfoAsync(userId.Value, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }

    public class MergeCartDto
    {
        public string SessionId { get; set; } = string.Empty;
    }

    public class ApplyCouponDto
    {
        public string CouponCode { get; set; } = string.Empty;
    }
}