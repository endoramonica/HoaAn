// VietCommerce.Api/Services/Interfaces/ICartService.cs
using System;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Services.Interfaces
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
    }
}