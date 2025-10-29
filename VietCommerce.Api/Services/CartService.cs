using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Entities.Orders;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CartService> _logger;

        public CartService(
            IUnitOfWork unitOfWork,
            IPermissionService permissionService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CartService> logger)
        {
            _unitOfWork = unitOfWork;
            _permissionService = permissionService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
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
                    return ApiResponse<GetCartResponseDto>.FailureResponse("Access denied");

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    return ApiResponse<GetCartResponseDto>.FailureResponse("User not found");

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetUserCartWithItemsAsync(userId); // ✅ dùng method này

                if (cart == null)
                    return ApiResponse<GetCartResponseDto>.FailureResponse("Cart not found");

                var cartItems = cart.CartItems ?? new List<CartItem>();

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
                        ProductImage = ci.Product?.Images?
                            .OrderByDescending(img => img.CreatedAt)
                            .Select(img => img.Url)
                            .FirstOrDefault(),
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

                cartDto.TotalAmount = cartDto.SubTotal;

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

        public async Task<ApiResponse<GetCartResponseDto>> GetGuestCartAsync(string sessionId)
        {
            try
            {
                sessionId ??= GetOrCreateSessionId();
                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetBySessionIdAsync(sessionId);

                if (cart == null)
                {
                    return ApiResponse<GetCartResponseDto>.FailureResponse("Guest cart not found");
                }

                var cartItems = await cartRepository.GetCartItemsAsync(cart.Id);

                var cartDto = new GetCartResponseDto
                {
                    CartId = cart.Id,
                    Items = cartItems.Select(ci => new CartItemDto
                    {
                        Id = ci.Id,
                        CartId = ci.CartId,
                        ProductId = ci.ProductId,
                        ProductName = ci.Product?.Name ?? "Unknown",
                        ProductCode = ci.Product?.Code ?? string.Empty,
                        ProductImage = ci.Product?.Images?.FirstOrDefault()?.Url,
                        Quantity = ci.Quantity,
                        UnitPrice = GetCurrentProductPrice(ci.Product.Prices),
                        StockAvailable = ci.Product?.Stock ?? 0,
                        AddedAt = ci.CreatedAt
                    }).ToList(),
                    TotalItems = cartItems.Sum(ci => ci.Quantity),
                    SubTotal = cartItems.Sum(ci => ci.Quantity * GetCurrentProductPrice(ci.Product.Prices)),
                    TaxAmount = 0,
                    ShippingFee = 0,
                    CreatedAt = cart.CreatedAt,
                    UpdatedAt = cart.UpdatedAt
                };

                cartDto.TotalAmount = cartDto.SubTotal + cartDto.TaxAmount + cartDto.ShippingFee;

                _logger.LogInformation("Guest cart retrieved. SessionId: {SessionId}, Items: {ItemCount}, Total: {Total}",
                    sessionId, cartDto.TotalItems, cartDto.TotalAmount);

                return ApiResponse<GetCartResponseDto>.SuccessResponse(cartDto, "Guest cart retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting guest cart for session {SessionId}", sessionId);
                return ApiResponse<GetCartResponseDto>.FailureResponse("Failed to get guest cart");
            }
        }

        public async Task<ApiResponse<CartSummaryDto>> GetGuestCartSummaryAsync(string sessionId)
        {
            try
            {
                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetBySessionIdAsync(sessionId);

                if (cart == null)
                {
                    return ApiResponse<CartSummaryDto>.SuccessResponse(
                        new CartSummaryDto { ItemCount = 0, TotalAmount = 0, Status = "ACTIVE" },
                        "Guest cart is empty"
                    );
                }

                var summary = new CartSummaryDto
                {
                    ItemCount = cart.CartItems.Sum(ci => ci.Quantity),
                    TotalAmount = cart.CartItems.Sum(ci =>
                         ci.Quantity * GetCurrentProductPrice(ci.Product?.Prices)),
                    Status = cart.IsActive ? "ACTIVE" : "INACTIVE"
                };

                return ApiResponse<CartSummaryDto>.SuccessResponse(summary, "Guest cart summary retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting guest cart summary for session {SessionId}", sessionId);
                return ApiResponse<CartSummaryDto>.FailureResponse("Failed to get guest cart summary");
            }
        }

        public async Task<ApiResponse<AddToCartResponseDto>> AddToGuestCartAsync(string sessionId, AddToCartDto dto)
        {
            try
            {
                if (dto.Quantity <= 0)
                    return ApiResponse<AddToCartResponseDto>.FailureResponse("Quantity must be greater than 0");

                var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
                if (product == null)
                    return ApiResponse<AddToCartResponseDto>.FailureResponse("Product not found");

                if (product.Stock < dto.Quantity)
                {
                    _logger.LogWarning("Product {ProductId} stock insufficient for guest. Required: {Required}, Available: {Available}",
                        dto.ProductId, dto.Quantity, product.Stock);
                    return ApiResponse<AddToCartResponseDto>.FailureResponse(
                        $"Insufficient stock. Available: {product.Stock}");
                }

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetOrCreateGuestCartAsync(sessionId);
                var currentPrice = GetCurrentProductPrice(product.Prices);

                var cartItem = await cartRepository.AddCartItemAsync(
                        cart.Id,
                        dto.ProductId,
                        dto.Quantity,
                        currentPrice
                    );

                var updatedCart = await cartRepository.GetBySessionIdAsync(sessionId);
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

                _logger.LogInformation("Item {ProductId} added to guest cart. SessionId: {SessionId}, Quantity: {Quantity}",
                    dto.ProductId, sessionId, dto.Quantity);

                return ApiResponse<AddToCartResponseDto>.SuccessResponse(response, "Item added to guest cart successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to guest cart for session {SessionId}", sessionId);
                return ApiResponse<AddToCartResponseDto>.FailureResponse("Failed to add item to guest cart");
            }
        }

        public async Task<ApiResponse<UpdateCartItemResponseDto>> UpdateGuestCartItemAsync(string sessionId, UpdateCartItemDto dto)
        {
            try
            {
                if (dto.Quantity <= 0)
                    return ApiResponse<UpdateCartItemResponseDto>.FailureResponse("Quantity must be greater than 0");

                var cartItem = await _unitOfWork.CartItems.GetByIdAsync(dto.CartItemId);
                if (cartItem == null)
                    return ApiResponse<UpdateCartItemResponseDto>.FailureResponse("Cart item not found");

                var cart = await _unitOfWork.Carts.GetByIdAsync(cartItem.CartId);
                if (cart == null || cart.SessionId != sessionId)
                    return ApiResponse<UpdateCartItemResponseDto>.FailureResponse("Unauthorized access to guest cart");

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

                var updatedCart = await cartRepository.GetBySessionIdAsync(sessionId);
                var newCartTotal = updatedCart.CartItems.Sum(ci => ci.Quantity * GetCurrentProductPrice(ci.Product?.Prices));

                var response = new UpdateCartItemResponseDto
                {
                    CartItemId = updatedItem.Id,
                    NewQuantity = updatedItem.Quantity,
                    NewLineTotal = updatedItem.Quantity * GetCurrentProductPrice(product.Prices),
                    CartTotalAmount = newCartTotal
                };

                _logger.LogInformation("Guest cart item {CartItemId} updated. SessionId: {SessionId}, New quantity: {Quantity}",
                    dto.CartItemId, sessionId, dto.Quantity);

                return ApiResponse<UpdateCartItemResponseDto>.SuccessResponse(response, "Guest cart item updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating guest cart item {CartItemId} for session {SessionId}",
                    dto.CartItemId, sessionId);
                return ApiResponse<UpdateCartItemResponseDto>.FailureResponse("Failed to update guest cart item");
            }
        }

        public async Task<ApiResponse<bool>> RemoveFromGuestCartAsync(string sessionId, Guid cartItemId)
        {
            try
            {
                var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
                if (cartItem == null)
                    return ApiResponse<bool>.FailureResponse("Cart item not found");

                var cart = await _unitOfWork.Carts.GetByIdAsync(cartItem.CartId);
                if (cart == null || cart.SessionId != sessionId)
                    return ApiResponse<bool>.FailureResponse("Unauthorized access to guest cart");

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var result = await cartRepository.RemoveCartItemAsync(cartItemId);

                _logger.LogInformation("Item {CartItemId} removed from guest cart. SessionId: {SessionId}", cartItemId, sessionId);

                return ApiResponse<bool>.SuccessResponse(result, "Item removed from guest cart successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from guest cart for session {SessionId}", sessionId);
                return ApiResponse<bool>.FailureResponse("Failed to remove item from guest cart");
            }
        }

        public async Task<ApiResponse<ClearCartResponseDto>> ClearGuestCartAsync(string sessionId)
        {
            try
            {
                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetBySessionIdAsync(sessionId);

                if (cart == null)
                    return ApiResponse<ClearCartResponseDto>.FailureResponse("Guest cart not found");

                var result = await cartRepository.ClearCartItemsAsync(cart.Id);

                _logger.LogInformation("Guest cart cleared. SessionId: {SessionId}", sessionId);

                return ApiResponse<ClearCartResponseDto>.SuccessResponse(
                    new ClearCartResponseDto
                    {
                        CartId = cart.Id,
                        IsCleared = result,
                        ClearedAt = DateTime.UtcNow
                    },
                    "Guest cart cleared successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing guest cart for session {SessionId}", sessionId);
                return ApiResponse<ClearCartResponseDto>.FailureResponse("Failed to clear guest cart");
            }
        }

        public async Task<ApiResponse<bool>> ValidateGuestCartForCheckoutAsync(string sessionId)
        {
            try
            {
                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetBySessionIdAsync(sessionId);

                if (cart == null)
                    return ApiResponse<bool>.FailureResponse("Guest cart not found");

                if (!cart.CartItems.Any())
                    return ApiResponse<bool>.FailureResponse("Guest cart is empty");

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

                _logger.LogInformation("Guest cart validation passed. SessionId: {SessionId}", sessionId);

                return ApiResponse<bool>.SuccessResponse(true, "Guest cart validated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating guest cart for session {SessionId}", sessionId);
                return ApiResponse<bool>.FailureResponse("Failed to validate guest cart");
            }
        }

        public async Task<ApiResponse<GetCartResponseDto>> MergeGuestCartToUserAsync(string sessionId, Guid userId)
        {
            try
            {
                var cartRepository = _unitOfWork.Carts as ICartRepository;
                await cartRepository.MergeGuestCartToUserCartAsync(sessionId, userId);

                var cart = await cartRepository.GetUserCartWithItemsAsync(userId);

                if (cart == null)
                    return ApiResponse<GetCartResponseDto>.FailureResponse("Failed to merge carts");

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

                _logger.LogInformation("Guest cart merged to user cart. SessionId: {SessionId}, UserId: {UserId}, Items: {ItemCount}",
                    sessionId, userId, cartDto.TotalItems);

                return ApiResponse<GetCartResponseDto>.SuccessResponse(cartDto, "Guest cart merged successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error merging guest cart for session {SessionId} to user {UserId}", sessionId, userId);
                return ApiResponse<GetCartResponseDto>.FailureResponse("Failed to merge guest cart");
            }
        }

        public async Task<ApiResponse<CartSummaryDto>> ApplyCouponAsync(Guid userId, string couponCode)
        {
            try
            {
                return ApiResponse<CartSummaryDto>.FailureResponse("Coupon feature not implemented yet");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying coupon for user {UserId}", userId);
                return ApiResponse<CartSummaryDto>.FailureResponse("Failed to apply coupon");
            }
        }

        public async Task<ApiResponse<CartSummaryDto>> RemoveCouponAsync(Guid userId)
        {
            try
            {
                return ApiResponse<CartSummaryDto>.FailureResponse("Coupon feature not implemented yet");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing coupon for user {UserId}", userId);
                return ApiResponse<CartSummaryDto>.FailureResponse("Failed to remove coupon");
            }
        }

        public async Task<ApiResponse<CartSummaryDto>> UpdateShippingInfoAsync(Guid userId, OrderShippingDto dto)
        {
            try
            {
                return ApiResponse<CartSummaryDto>.FailureResponse("Shipping feature not implemented yet");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating shipping info for user {UserId}", userId);
                return ApiResponse<CartSummaryDto>.FailureResponse("Failed to update shipping info");
            }
        }

        public async Task<ApiResponse<int>> GetCartItemCountAsync(Guid userId)
        {
            try
            {
                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetUserCartWithItemsAsync(userId);

                if (cart == null)
                    return ApiResponse<int>.SuccessResponse(0, "Cart is empty");

                var count = cart.CartItems.Sum(ci => ci.Quantity);
                return ApiResponse<int>.SuccessResponse(count, "Cart item count retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart item count for user {UserId}", userId);
                return ApiResponse<int>.FailureResponse("Failed to get cart item count");
            }
        }

        public async Task<ApiResponse<int>> GetGuestCartItemCountAsync(string sessionId)
        {
            try
            {
                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetBySessionIdAsync(sessionId);

                if (cart == null)
                    return ApiResponse<int>.SuccessResponse(0, "Guest cart is empty");

                var count = cart.CartItems.Sum(ci => ci.Quantity);
                return ApiResponse<int>.SuccessResponse(count, "Guest cart item count retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting guest cart item count for session {SessionId}", sessionId);
                return ApiResponse<int>.FailureResponse("Failed to get guest cart item count");
            }
        }

        public async Task<ApiResponse<CartItemDetailDto>> GetCartItemDetailAsync(Guid userId, Guid cartItemId)
        {
            try
            {
                var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
                if (cartItem == null)
                    return ApiResponse<CartItemDetailDto>.FailureResponse("Cart item not found");

                var cart = await _unitOfWork.Carts.GetByIdAsync(cartItem.CartId);
                if (cart == null || cart.UserId != userId)
                    return ApiResponse<CartItemDetailDto>.FailureResponse("Unauthorized access to cart item");

                var detail = _mapper.Map<CartItemDetailDto>(cartItem);
                return ApiResponse<CartItemDetailDto>.SuccessResponse(detail, "Cart item detail retrieved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart item detail {CartItemId} for user {UserId}", cartItemId, userId);
                return ApiResponse<CartItemDetailDto>.FailureResponse("Failed to get cart item detail");
            }
        }
        public string GetOrCreateSessionId()
        {
            var context = _httpContextAccessor.HttpContext!;
            var request = context.Request;
            var response = context.Response;

            if (request.Cookies.TryGetValue("SessionId", out var sessionId) && !string.IsNullOrWhiteSpace(sessionId))
                return sessionId;

            // tạo mới nếu chưa có
            sessionId = Guid.NewGuid().ToString();
            response.Cookies.Append("SessionId", sessionId, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });

            return sessionId;
        }
    }
}