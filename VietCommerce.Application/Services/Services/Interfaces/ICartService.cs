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

        /// <summary>
        /// Adds a product to the user's cart with customizations for package products.
        /// Validates customization quantities against product constraints and calculates final price.
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
        /// <param name="dto">The add to cart request containing product ID and optional customizations</param>
        /// <returns>The newly added cart item with customization details and calculated prices</returns>
        /// <remarks>
        /// Requirements: 3.1
        /// - Validates customizations using ProductService.ValidateCustomizationsAsync
        /// - Calculates final price: basePrice + sum(customization quantities × unit prices)
        /// - Stores customizations as JSON in CartItem
        /// - Returns CartItemDetailDto with BasePrice, CustomizationPrice, and FinalPrice breakdown
        /// </remarks>
        Task<ApiResponse<CartItemDetailDto>> AddToCartWithCustomizationsAsync(Guid userId, AddToCartDto dto);

        /// <summary>
        /// Updates the customizations for an existing cart item.
        /// Validates new customization quantities and recalculates the final price.
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
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
        Task<ApiResponse<CartItemDetailDto>> UpdateCartItemCustomizationsAsync(Guid userId, Guid cartItemId, List<CartItemCustomizationDto> customizations);

        public string GetOrCreateSessionId();

    }
}
