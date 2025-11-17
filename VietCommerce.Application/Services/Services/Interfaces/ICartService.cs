// ============================================
// 1. ICartService Interface
// ============================================
// VietCommerce.Api/Services/Interfaces/ICartService.cs

using System;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    public interface ICartService
    {
        
        /// Lấy giỏ hàng của user đang login
        
        Task<ApiResponse<GetCartResponseDto>> GetCartAsync(Guid userId);

        
        /// Lấy summary giỏ hàng (số items, tổng tiền)
        
        Task<ApiResponse<CartSummaryDto>> GetCartSummaryAsync(Guid userId);

        
        /// Thêm sản phẩm vào giỏ hàng
        
        Task<ApiResponse<AddToCartResponseDto>> AddToCartAsync(Guid userId, AddToCartDto dto);

        
        /// Cập nhật số lượng sản phẩm trong giỏ
        
        Task<ApiResponse<UpdateCartItemResponseDto>> UpdateCartItemAsync(Guid userId, UpdateCartItemDto dto);

        
        /// Xóa sản phẩm khỏi giỏ
        
        Task<ApiResponse<bool>> RemoveFromCartAsync(Guid userId, Guid cartItemId);

        
        /// Xóa tất cả sản phẩm trong giỏ
        
        Task<ApiResponse<ClearCartResponseDto>> ClearCartAsync(Guid userId);

        
        /// Validate giỏ hàng trước khi tạo order
        /// - Kiểm tra stock
        /// - Kiểm tra giá
        /// - Kiểm tra items không rỗng
        
        Task<ApiResponse<bool>> ValidateCartForCheckoutAsync(Guid userId);

        
        /// Lấy giỏ hàng của guest user theo sessionId
        
        Task<ApiResponse<GetCartResponseDto>> GetGuestCartAsync(string sessionId);

        
        /// Thêm sản phẩm vào giỏ của guest user
        
        Task<ApiResponse<AddToCartResponseDto>> AddToGuestCartAsync(string sessionId, AddToCartDto dto);

        
        /// Cập nhật số lượng sản phẩm trong giỏ guest
        
        Task<ApiResponse<UpdateCartItemResponseDto>> UpdateGuestCartItemAsync(string sessionId, UpdateCartItemDto dto);

        
        /// Xóa sản phẩm khỏi giỏ guest
        
        Task<ApiResponse<bool>> RemoveFromGuestCartAsync(string sessionId, Guid cartItemId);

        
        /// Xóa tất cả sản phẩm trong giỏ guest
        
        Task<ApiResponse<ClearCartResponseDto>> ClearGuestCartAsync(string sessionId);

        
        /// Lấy summary giỏ hàng của guest
        
        Task<ApiResponse<CartSummaryDto>> GetGuestCartSummaryAsync(string sessionId);

        
        /// Validate giỏ guest trước checkout
        
        Task<ApiResponse<bool>> ValidateGuestCartForCheckoutAsync(string sessionId);

        
        /// Merge giỏ hàng guest sang user khi đăng nhập
        
        Task<ApiResponse<GetCartResponseDto>> MergeGuestCartToUserAsync(string sessionId, Guid userId);

        
        /// Áp dụng coupon/promo code vào giỏ hàng
        
        Task<ApiResponse<CartSummaryDto>> ApplyCouponAsync(Guid userId, string couponCode);

        
        /// Xóa coupon/promo code khỏi giỏ hàng
        
        Task<ApiResponse<CartSummaryDto>> RemoveCouponAsync(Guid userId);

        
        /// Cập nhật thông tin shipping cho giỏ hàng
        
        Task<ApiResponse<CartSummaryDto>> UpdateShippingInfoAsync(Guid userId, OrderShippingDto dto);

        
        /// Kiểm tra số lượng items trong giỏ
        
        Task<ApiResponse<int>> GetCartItemCountAsync(Guid userId);

        
        /// Kiểm tra số lượng items trong giỏ guest
        
        Task<ApiResponse<int>> GetGuestCartItemCountAsync(string sessionId);

        
        /// Lấy chi tiết một item trong giỏ
        
        Task<ApiResponse<CartItemDetailDto>> GetCartItemDetailAsync(Guid userId, Guid cartItemId);
        public string GetOrCreateSessionId();

    }
}
