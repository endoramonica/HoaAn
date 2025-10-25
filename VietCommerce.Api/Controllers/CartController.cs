using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Cart;

namespace VietCommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        [HttpGet]
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

        [HttpPut("update-item")]
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

        [HttpDelete("items/{cartItemId}")]
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
    }
}