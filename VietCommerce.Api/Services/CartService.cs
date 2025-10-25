using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionService _permissionService;
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;

        public CartService(
            IUnitOfWork unitOfWork,
            IPermissionService permissionService,
            IMapper mapper,
            ILogger<CartService> logger)
        {
            _unitOfWork = unitOfWork;
            _permissionService = permissionService;
            _mapper = mapper;
            _logger = logger;
        }

        private decimal GetCurrentProductPrice(ICollection<ProductPrice> prices)
        {
            if (prices == null || !prices.Any())
                return 0;

            var now = DateTime.UtcNow;

            var activePrice = prices
                .Where(p => p.IsActive
                    && p.EffectiveFrom <= now
                    && (p.EffectiveTo == null || p.EffectiveTo >= now))
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefault();

            return activePrice?.Price ?? 0;
        }

        public async Task<ApiResponse<GetCartResponseDto>> GetCartAsync(Guid userId)
        {
            try
            {
                if (!await _permissionService.CheckUserPermissionAsync(userId, "cart.view"))
                {
                    _logger.LogWarning("User {UserId} attempted to view cart without permission", userId);
                    return ApiResponse<GetCartResponseDto>.FailureResponse("Access denied: cart.view permission required");
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return ApiResponse<GetCartResponseDto>.FailureResponse("User not found");
                }

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetOrCreateCartByUserIdAsync(userId);

                if (cart == null)
                {
                    return ApiResponse<GetCartResponseDto>.FailureResponse("Failed to get or create cart");
                }

                var cartItems = await cartRepository.GetCartItemsAsync(cart.Id);

                var cartDto = new GetCartResponseDto
                {
                    CartId = cart.Id,
                    UserId = cart.UserId,
                    Items = cartItems.Select(ci => new CartItemDto
                    {
                        Id = ci.Id,
                        CartId = ci.CartId,
                        ProductId = ci.ProductId,
                        ProductName = ci.Product?.Name ?? "Unknown",
                        ProductCode = ci.Product?.Code ?? string.Empty,
                        ProductImage = ci.Product?.Images?.FirstOrDefault()?.Url,
                        Quantity = ci.Quantity,
                        UnitPrice = GetCurrentProductPrice(ci.Product?.Prices),
                        StockAvailable = ci.Product?.Stock ?? 0,
                        AddedAt = ci.CreatedAt
                    }).ToList(),
                    TotalItems = cartItems.Sum(ci => ci.Quantity),
                    SubTotal = cartItems.Sum(ci => ci.Quantity * GetCurrentProductPrice(ci.Product?.Prices)),
                    TaxAmount = 0,
                    ShippingFee = 0,
                    CreatedAt = cart.CreatedAt,
                    UpdatedAt = cart.UpdatedAt
                };

                cartDto.TotalAmount = cartDto.SubTotal + cartDto.TaxAmount + cartDto.ShippingFee;

                _logger.LogInformation("Cart retrieved for user {UserId}. Items: {ItemCount}, Total: {Total}",
                    userId, cartDto.TotalItems, cartDto.TotalAmount);

                return ApiResponse<GetCartResponseDto>.SuccessResponse(cartDto, "Cart retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart for user {UserId}", userId);
                return ApiResponse<GetCartResponseDto>.FailureResponse("Failed to get cart");
            }
        }

        public async Task<ApiResponse<CartSummaryDto>> GetCartSummaryAsync(Guid userId)
        {
            try
            {
                if (!await _permissionService.CheckUserPermissionAsync(userId, "cart.view"))
                {
                    return ApiResponse<CartSummaryDto>.FailureResponse("Access denied: cart.view permission required");
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    return ApiResponse<CartSummaryDto>.FailureResponse("User not found");

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetUserCartWithItemsAsync(userId);

                if (cart == null)
                {
                    return ApiResponse<CartSummaryDto>.SuccessResponse(
                        new CartSummaryDto { ItemCount = 0, TotalAmount = 0, Status = "ACTIVE" },
                        "Cart is empty"
                    );
                }

                var summary = new CartSummaryDto
                {
                    ItemCount = cart.CartItems.Sum(ci => ci.Quantity),
                    TotalAmount = cart.CartItems.Sum(ci =>
                         ci.Quantity * GetCurrentProductPrice(ci.Product?.Prices)),
                    Status = cart.IsActive ? "ACTIVE" : "INACTIVE"
                };

                return ApiResponse<CartSummaryDto>.SuccessResponse(summary, "Cart summary retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart summary for user {UserId}", userId);
                return ApiResponse<CartSummaryDto>.FailureResponse("Failed to get cart summary");
            }
        }

        public async Task<ApiResponse<AddToCartResponseDto>> AddToCartAsync(Guid userId, AddToCartDto dto)
        {
            try
            {
                if (!await _permissionService.CheckUserPermissionAsync(userId, "cart.add_item"))
                {
                    _logger.LogWarning("User {UserId} attempted to add item without permission", userId);
                    return ApiResponse<AddToCartResponseDto>.FailureResponse("Access denied: cart.add_item permission required");
                }

                if (dto.Quantity <= 0)
                    return ApiResponse<AddToCartResponseDto>.FailureResponse("Quantity must be greater than 0");

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    return ApiResponse<AddToCartResponseDto>.FailureResponse("User not found");

                var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
                if (product == null)
                    return ApiResponse<AddToCartResponseDto>.FailureResponse("Product not found");

                if (product.Stock < dto.Quantity)
                {
                    _logger.LogWarning("Product {ProductId} stock insufficient. Required: {Required}, Available: {Available}",
                        dto.ProductId, dto.Quantity, product.Stock);
                    return ApiResponse<AddToCartResponseDto>.FailureResponse(
                        $"Insufficient stock. Available: {product.Stock}");
                }

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetOrCreateCartByUserIdAsync(userId);
                var currentPrice = GetCurrentProductPrice(product.Prices);

                var cartItem = await cartRepository.AddCartItemAsync(
                        cart.Id,
                        dto.ProductId,
                        dto.Quantity,
                        currentPrice
                    );

                var updatedCart = await cartRepository.GetUserCartWithItemsAsync(userId);
                var totalAmount = updatedCart.CartItems.Sum(ci => ci.Quantity * GetCurrentProductPrice(ci.Product?.Prices));

                var response = new AddToCartResponseDto
                {
                    CartItemId = cartItem.Id,
                    ProductId = dto.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = currentPrice,
                    CartItemCount = updatedCart.CartItems.Sum(ci => ci.Quantity),
                    CartTotalAmount = totalAmount
                };

                _logger.LogInformation("Item {ProductId} added to cart for user {UserId}. Quantity: {Quantity}",
                    dto.ProductId, userId, dto.Quantity);

                return ApiResponse<AddToCartResponseDto>.SuccessResponse(response, "Item added to cart successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart for user {UserId}", userId);
                return ApiResponse<AddToCartResponseDto>.FailureResponse("Failed to add item to cart");
            }
        }

        public async Task<ApiResponse<UpdateCartItemResponseDto>> UpdateCartItemAsync(Guid userId, UpdateCartItemDto dto)
        {
            try
            {
                if (!await _permissionService.CheckUserPermissionAsync(userId, "cart.update_item"))
                {
                    _logger.LogWarning("User {UserId} attempted to update cart item without permission", userId);
                    return ApiResponse<UpdateCartItemResponseDto>.FailureResponse(
                        "Access denied: cart.update_item permission required");
                }

                if (dto.Quantity <= 0)
                    return ApiResponse<UpdateCartItemResponseDto>.FailureResponse("Quantity must be greater than 0");

                var cartItem = await _unitOfWork.CartItems.GetByIdAsync(dto.CartItemId);
                if (cartItem == null)
                    return ApiResponse<UpdateCartItemResponseDto>.FailureResponse("Cart item not found");

                var cart = await _unitOfWork.Carts.GetByIdAsync(cartItem.CartId);
                if (cart == null || cart.UserId != userId)
                    return ApiResponse<UpdateCartItemResponseDto>.FailureResponse("Unauthorized access to cart");

                var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                if (product == null)
                    return ApiResponse<UpdateCartItemResponseDto>.FailureResponse("Product not found");

                if (product.Stock < dto.Quantity)
                {
                    return ApiResponse<UpdateCartItemResponseDto>.FailureResponse(
                        $"Insufficient stock. Available: {product.Stock}");
                }

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var updatedItem = await cartRepository.UpdateCartItemQuantityAsync(dto.CartItemId, dto.Quantity);

                var updatedCart = await cartRepository.GetUserCartWithItemsAsync(userId);
                var newCartTotal = updatedCart.CartItems.Sum(ci => ci.Quantity * GetCurrentProductPrice(ci.Product?.Prices));

                var response = new UpdateCartItemResponseDto
                {
                    CartItemId = updatedItem.Id,
                    NewQuantity = updatedItem.Quantity,
                    NewLineTotal = updatedItem.Quantity * GetCurrentProductPrice(product.Prices),
                    CartTotalAmount = newCartTotal
                };

                _logger.LogInformation("Cart item {CartItemId} updated for user {UserId}. New quantity: {Quantity}",
                    dto.CartItemId, userId, dto.Quantity);

                return ApiResponse<UpdateCartItemResponseDto>.SuccessResponse(response, "Cart item updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item {CartItemId} for user {UserId}",
                    dto.CartItemId, userId);
                return ApiResponse<UpdateCartItemResponseDto>.FailureResponse("Failed to update cart item");
            }
        }

        public async Task<ApiResponse<bool>> RemoveFromCartAsync(Guid userId, Guid cartItemId)
        {
            try
            {
                if (!await _permissionService.CheckUserPermissionAsync(userId, "cart.remove_item"))
                {
                    _logger.LogWarning("User {UserId} attempted to remove item without permission", userId);
                    return ApiResponse<bool>.FailureResponse("Access denied: cart.remove_item permission required");
                }

                var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
                if (cartItem == null)
                    return ApiResponse<bool>.FailureResponse("Cart item not found");

                var cart = await _unitOfWork.Carts.GetByIdAsync(cartItem.CartId);
                if (cart == null || cart.UserId != userId)
                    return ApiResponse<bool>.FailureResponse("Unauthorized access to cart");

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var result = await cartRepository.RemoveCartItemAsync(cartItemId);

                _logger.LogInformation("Item {CartItemId} removed from cart for user {UserId}", cartItemId, userId);

                return ApiResponse<bool>.SuccessResponse(result, "Item removed from cart successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart for user {UserId}", userId);
                return ApiResponse<bool>.FailureResponse("Failed to remove item from cart");
            }
        }

        public async Task<ApiResponse<ClearCartResponseDto>> ClearCartAsync(Guid userId)
        {
            try
            {
                if (!await _permissionService.CheckUserPermissionAsync(userId, "cart.clear"))
                {
                    _logger.LogWarning("User {UserId} attempted to clear cart without permission", userId);
                    return ApiResponse<ClearCartResponseDto>.FailureResponse("Access denied: cart.clear permission required");
                }

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetUserCartWithItemsAsync(userId);

                if (cart == null)
                    return ApiResponse<ClearCartResponseDto>.FailureResponse("Cart not found");

                var result = await cartRepository.ClearCartItemsAsync(cart.Id);

                _logger.LogInformation("Cart cleared for user {UserId}", userId);

                return ApiResponse<ClearCartResponseDto>.SuccessResponse(
                    new ClearCartResponseDto
                    {
                        CartId = cart.Id,
                        IsCleared = result,
                        ClearedAt = DateTime.UtcNow
                    },
                    "Cart cleared successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user {UserId}", userId);
                return ApiResponse<ClearCartResponseDto>.FailureResponse("Failed to clear cart");
            }
        }

        public async Task<ApiResponse<bool>> ValidateCartForCheckoutAsync(Guid userId)
        {
            try
            {
                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetUserCartWithItemsAsync(userId);

                if (cart == null)
                    return ApiResponse<bool>.FailureResponse("Cart not found");

                if (!cart.CartItems.Any())
                    return ApiResponse<bool>.FailureResponse("Cart is empty");

                foreach (var cartItem in cart.CartItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);

                    if (product == null)
                        return ApiResponse<bool>.FailureResponse($"Product {cartItem.ProductId} not found");

                    if (!product.IsActive)
                        return ApiResponse<bool>.FailureResponse($"Product {product.Name} is not available");

                    if (product.Stock < cartItem.Quantity)
                        return ApiResponse<bool>.FailureResponse(
                            $"Insufficient stock for {product.Name}. Available: {product.Stock}, Required: {cartItem.Quantity}");
                }

                _logger.LogInformation("Cart validation passed for user {UserId}", userId);

                return ApiResponse<bool>.SuccessResponse(true, "Cart validated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating cart for user {UserId}", userId);
                return ApiResponse<bool>.FailureResponse("Failed to validate cart");
            }
        }
    }
}