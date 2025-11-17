using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Wishlist;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.EndUser.EndUser_Interfaces
{
    public interface IWishlistService
    {
        /// <summary>
        /// Lấy danh sách wishlist của user
        /// </summary>
        Task<ApiResponse<List<WishlistItemDto>>> GetWishlistAsync(Guid userId);

        /// <summary>
        /// Thêm sản phẩm vào wishlist
        /// </summary>
        Task<ApiResponse<WishlistItemDto>> AddToWishlistAsync(Guid userId, Guid productId);

        /// <summary>
        /// Xóa wishlist item theo ID
        /// </summary>
        Task<ApiResponse<bool>> RemoveFromWishlistAsync(Guid userId, Guid wishlistItemId);

        /// <summary>
        /// Xóa sản phẩm khỏi wishlist theo ProductId
        /// </summary>
        Task<ApiResponse<bool>> RemoveByProductIdAsync(Guid userId, Guid productId);

        /// <summary>
        /// Kiểm tra sản phẩm có trong wishlist không
        /// </summary>
        Task<ApiResponse<IsInWishlistResponse>> IsInWishlistAsync(Guid userId, Guid productId);

        /// <summary>
        /// Toggle wishlist - thêm nếu chưa có, xóa nếu đã có
        /// </summary>
        Task<ApiResponse<ToggleWishlistResponse>> ToggleWishlistAsync(Guid userId, Guid productId);

        /// <summary>
        /// Xóa toàn bộ wishlist
        /// </summary>
        Task<ApiResponse<bool>> ClearWishlistAsync(Guid userId);

        /// <summary>
        /// Di chuyển tất cả wishlist items vào giỏ hàng
        /// TODO: Implement khi cần
        /// </summary>
        Task<ApiResponse<bool>> MoveAllToCartAsync(Guid userId);

        Task<ApiResponse<int>> GetWishlistCountAsync(Guid userId);
    }
}
