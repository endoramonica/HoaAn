// FILE 7: Controller
// Path: VietCommerce.API/Controllers/WishlistController.cs
// ============================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VietCommerce.Application.Services.EndUser.EndUser_Interfaces;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.DTOs.Wishlist;
using VietCommerce.Core.Models;

namespace VietCommerce.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        private readonly ICurrentUser _currentUser;

        public WishlistController(
            IWishlistService wishlistService,
            ICurrentUser currentUser)
        {
            _wishlistService = wishlistService;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Lấy danh sách wishlist của user hiện tại
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<System.Collections.Generic.List<WishlistItemDto>>>> GetWishlist()
        {
            var userId = _currentUser.UserId;
            var result = await _wishlistService.GetWishlistAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Thêm sản phẩm vào wishlist
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<WishlistItemDto>>> AddToWishlist(
            [FromBody] AddToWishlistRequest request)
        {
            var userId = _currentUser.UserId;
            var result = await _wishlistService.AddToWishlistAsync(userId, request.ProductId);
            return Ok(result);
        }

        /// <summary>
        /// Xóa wishlist item theo ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveFromWishlist(Guid id)
        {
            var userId = _currentUser.UserId;
            var result = await _wishlistService.RemoveFromWishlistAsync(userId, id);
            return Ok(result);
        }

        /// <summary>
        /// Xóa sản phẩm khỏi wishlist theo ProductId
        /// </summary>
        [HttpDelete("product/{productId}")]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveByProductId(Guid productId)
        {
            var userId = _currentUser.UserId;
            var result = await _wishlistService.RemoveByProductIdAsync(userId, productId);
            return Ok(result);
        }

        /// <summary>
        /// Kiểm tra sản phẩm có trong wishlist không
        /// </summary>
        [HttpGet("check/{productId}")]
        public async Task<ActionResult<ApiResponse<IsInWishlistResponse>>> CheckIsInWishlist(Guid productId)
        {
            var userId = _currentUser.UserId;
            var result = await _wishlistService.IsInWishlistAsync(userId, productId);
            return Ok(result);
        }

        /// <summary>
        /// Toggle wishlist - thêm nếu chưa có, xóa nếu đã có
        /// </summary>
        [HttpPost("toggle")]
        public async Task<ActionResult<ApiResponse<ToggleWishlistResponse>>> ToggleWishlist(
            [FromBody] AddToWishlistRequest request)
        {
            var userId = _currentUser.UserId;
            var result = await _wishlistService.ToggleWishlistAsync(userId, request.ProductId);
            return Ok(result);
        }

        /// <summary>
        /// Xóa toàn bộ wishlist
        /// </summary>
        [HttpDelete("clear")]
        public async Task<ActionResult<ApiResponse<bool>>> ClearWishlist()
        {
            var userId = _currentUser.UserId;
            var result = await _wishlistService.ClearWishlistAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Di chuyển tất cả wishlist items vào giỏ hàng
        /// TODO: Implement khi cần
        /// </summary>
        [HttpPost("move-to-cart")]
        public async Task<ActionResult<ApiResponse<bool>>> MoveAllToCart()
        {
            var userId = _currentUser.UserId;
            var result = await _wishlistService.MoveAllToCartAsync(userId);
            return Ok(result);
        }
        /// Lấy số lượng wishlist items (cho badge count)
        /// GET /api/v1/wishlist/count
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult<ApiResponse<int>>> GetWishlistCount()
        {
            var userId = _currentUser.UserId;
            var result = await _wishlistService.GetWishlistCountAsync(userId);
            return Ok(result);
        }
    }
}