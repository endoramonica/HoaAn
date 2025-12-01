// ================================================================
// FILE: VietCommerce.Api/Services/CartService.cs
// ENHANCED: BaseService integration + Caching + Better error handling
// ================================================================

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Cart Service - Handles shopping cart operations for both authenticated users and guests
    /// Features:
    /// - User cart management (CRUD operations)
    /// - Guest cart management with session IDs
    /// - Cart validation for checkout
    /// - Guest to user cart merge on login
    /// - Permission-based access control
    /// - Redis caching for performance
    /// </summary>
    public class CartService : BaseService, ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionService _permissionService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Cache key prefixes
        private const string USER_CART_CACHE_PREFIX = "cart:user";
        private const string GUEST_CART_CACHE_PREFIX = "cart:guest";
        private const string CART_SUMMARY_CACHE_PREFIX = "cart:summary";

        
        private const int CACHE_DURATION_MINUTES = 60;

        public CartService(
            IUnitOfWork unitOfWork,
            IPermissionService permissionService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CartService> logger,
            ICacheService cacheService)
            : base(logger, cacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        #region Helpers

        /// <summary>
        /// Get current active product price from price collection
        /// Filters by: IsActive + EffectiveFrom <= now + EffectiveTo == null or >= now
        /// </summary>
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

        /// <summary>
        /// Get or create session ID for guest user
        /// Stored in secure httponly cookie with 7-day expiration
        /// </summary>
        public string GetOrCreateSessionId()
        {
            var context = _httpContextAccessor.HttpContext;
            ThrowIf(context == null, "HttpContext not available");

            var request = context.Request;
            var response = context.Response;

            // Try to get existing session ID
            if (request.Cookies.TryGetValue("SessionId", out var sessionId) && !string.IsNullOrWhiteSpace(sessionId))
            {
                LogDebug($"📍 Existing session ID found: {sessionId.Substring(0, 8)}...");
                return sessionId;
            }

            // Create new session ID
            sessionId = Guid.NewGuid().ToString();
            response.Cookies.Append("SessionId", sessionId, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });

            LogInfo($"✅ New session ID created: {sessionId.Substring(0, 8)}...");
            return sessionId;
        }

        #endregion

        #region User Cart Methods

        /// <summary>
        /// Get user's shopping cart with all items
        /// Returns detailed cart DTO with calculated totals
        /// </summary>
        public async Task<ApiResponse<GetCartResponseDto>> GetCartAsync(Guid userId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId);

                // Check permission
                ThrowIfNot(
                    await _permissionService.CheckUserPermissionAsync(userId, "cart.view"),
                    "Access denied: cart.view permission required");

                // Get user
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                ThrowIf(user == null, "User not found");

                // Get cart from cache or DB
                var cacheKey = CreateCacheKey(USER_CART_CACHE_PREFIX, userId);
                var cart = await GetFromCacheOrExecuteAsync(
                    cacheKey,
                    async () =>
                    {
                        var cartRepository = _unitOfWork.Carts as ICartRepository;
                        return await cartRepository.GetUserCartWithItemsAsync(userId);
                    },
                    TimeSpan.FromMinutes(CACHE_DURATION_MINUTES)
                );

                ThrowIf(cart == null, "Cart not found");

                var cartItems = cart.CartItems ?? new List<CartItem>();

                var cartDto = MapCartToDto(cart, cartItems);

                LogInfo($"📦 Cart retrieved for user {userId}: {cartItems.Count} items");

                return cartDto;

            }, "GetCart", "Cart retrieved successfully");
        }

        /// <summary>
        /// Get cart summary (item count, total amount, status)
        /// Lightweight response for quick lookups
        /// </summary>
        public async Task<ApiResponse<CartSummaryDto>> GetCartSummaryAsync(Guid userId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId);

                ThrowIfNot(
                    await _permissionService.CheckUserPermissionAsync(userId, "cart.view"),
                    "Access denied: cart.view permission required");

                var cacheKey = CreateCacheKey(CART_SUMMARY_CACHE_PREFIX, userId);

                var summary = await GetFromCacheOrExecuteAsync(
                    cacheKey,
                    async () =>
                    {
                        var cartRepository = _unitOfWork.Carts as ICartRepository;
                        var cart = await cartRepository.GetUserCartWithItemsAsync(userId);

                        if (cart == null)
                        {
                            return new CartSummaryDto
                            {
                                ItemCount = 0,
                                TotalAmount = 0,
                                Status = "ACTIVE"
                            };
                        }

                        return new CartSummaryDto
                        {
                            ItemCount = cart.CartItems.Sum(ci => ci.Quantity),
                            TotalAmount = cart.CartItems.Sum(ci =>
                                ci.Quantity * GetCurrentProductPrice(ci.Product?.Prices)),
                            Status = cart.IsActive ? "ACTIVE" : "INACTIVE"
                        };
                    },
                    TimeSpan.FromMinutes(CACHE_DURATION_MINUTES)
                );

                LogDebug($"📊 Cart summary for user {userId}: {summary.ItemCount} items, Total: {summary.TotalAmount}");

                return summary;

            }, "GetCartSummary", "Cart summary retrieved successfully");
        }

        /// <summary>
        /// Add product to user's cart
        /// Validates stock availability and permission
        /// </summary>
        public async Task<ApiResponse<AddToCartResponseDto>> AddToCartAsync(Guid userId, AddToCartDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId);
                ValidateNotNull(dto, nameof(dto));
                ValidateId(dto.ProductId, nameof(dto.ProductId));
                ThrowIf(dto.Quantity <= 0, "Quantity must be greater than 0");

                ThrowIfNot(
                    await _permissionService.CheckUserPermissionAsync(userId, "cart.add_item"),
                    "Access denied: cart.add_item permission required");

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                ThrowIf(user == null, "User not found");

                var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
                ThrowIf(product == null, "Product not found");

                ThrowIf(product.Stock < dto.Quantity,
                    $"Insufficient stock. Available: {product.Stock}, Required: {dto.Quantity}");

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
                var totalAmount = updatedCart.CartItems.Sum(ci =>
                    ci.Quantity * GetCurrentProductPrice(ci.Product?.Prices));

                // Invalidate cache
                await InvalidateMultipleCachesAsync(
                    CreateCacheKey(USER_CART_CACHE_PREFIX, userId),
                    CreateCacheKey(CART_SUMMARY_CACHE_PREFIX, userId)
                );

                var response = new AddToCartResponseDto
                {
                    CartItemId = cartItem.Id,
                    ProductId = dto.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = currentPrice,
                    CartItemCount = updatedCart.CartItems.Sum(ci => ci.Quantity),
                    CartTotalAmount = totalAmount
                };

                LogInfo($"✅ Item {dto.ProductId} added to cart for user {userId}. Qty: {dto.Quantity}");

                return response;

            }, "AddToCart", "Item added to cart successfully");
        }

        /// <summary>
        /// Update quantity of item in user's cart
        /// Validates stock and permission
        /// </summary>
        public async Task<ApiResponse<UpdateCartItemResponseDto>> UpdateCartItemAsync(Guid userId, UpdateCartItemDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId);
                ValidateNotNull(dto, nameof(dto));
                ValidateId(dto.CartItemId, nameof(dto.CartItemId));
                ThrowIf(dto.Quantity <= 0, "Quantity must be greater than 0");

                ThrowIfNot(
                    await _permissionService.CheckUserPermissionAsync(userId, "cart.update_item"),
                    "Access denied: cart.update_item permission required");

                var cartItem = await _unitOfWork.CartItems.GetByIdAsync(dto.CartItemId);
                ThrowIf(cartItem == null, "Cart item not found");

                var cart = await _unitOfWork.Carts.GetByIdAsync(cartItem.CartId);
                ThrowIf(cart == null || cart.UserId != userId, "Unauthorized access to cart");

                var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                ThrowIf(product == null, "Product not found");

                ThrowIf(product.Stock < dto.Quantity,
                    $"Insufficient stock. Available: {product.Stock}");

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var updatedItem = await cartRepository.UpdateCartItemQuantityAsync(dto.CartItemId, dto.Quantity);

                var updatedCart = await cartRepository.GetUserCartWithItemsAsync(userId);
                var newCartTotal = updatedCart.CartItems.Sum(ci =>
                    ci.Quantity * GetCurrentProductPrice(ci.Product?.Prices));

                // Invalidate cache
                await InvalidateMultipleCachesAsync(
                    CreateCacheKey(USER_CART_CACHE_PREFIX, userId),
                    CreateCacheKey(CART_SUMMARY_CACHE_PREFIX, userId)
                );

                var response = new UpdateCartItemResponseDto
                {
                    CartItemId = updatedItem.Id,
                    NewQuantity = updatedItem.Quantity,
                    NewLineTotal = updatedItem.Quantity * GetCurrentProductPrice(product.Prices),
                    CartTotalAmount = newCartTotal
                };

                LogInfo($"✏️ Cart item {dto.CartItemId} updated. New qty: {dto.Quantity}");

                return response;

            }, "UpdateCartItem", "Cart item updated successfully");
        }

        /// <summary>
        /// Remove item from user's cart
        /// </summary>
        public async Task<ApiResponse<bool>> RemoveFromCartAsync(Guid userId, Guid cartItemId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId);
                ValidateId(cartItemId);

                ThrowIfNot(
                    await _permissionService.CheckUserPermissionAsync(userId, "cart.remove_item"),
                    "Access denied: cart.remove_item permission required");

                var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
                ThrowIf(cartItem == null, "Cart item not found");

                var cart = await _unitOfWork.Carts.GetByIdAsync(cartItem.CartId);
                ThrowIf(cart == null || cart.UserId != userId, "Unauthorized access to cart");

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                await cartRepository.RemoveCartItemAsync(cartItemId);

                // Invalidate cache
                await InvalidateMultipleCachesAsync(
                    CreateCacheKey(USER_CART_CACHE_PREFIX, userId),
                    CreateCacheKey(CART_SUMMARY_CACHE_PREFIX, userId)
                );

                LogInfo($"🗑️ Item {cartItemId} removed from cart for user {userId}");

                return true;

            }, "RemoveFromCart", "Item removed from cart successfully");
        }

        /// <summary>
        /// Clear all items from user's cart
        /// </summary>
        public async Task<ApiResponse<ClearCartResponseDto>> ClearCartAsync(Guid userId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId);

                ThrowIfNot(
                    await _permissionService.CheckUserPermissionAsync(userId, "cart.clear"),
                    "Access denied: cart.clear permission required");

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetUserCartWithItemsAsync(userId);
                ThrowIf(cart == null, "Cart not found");

                await cartRepository.ClearCartItemsAsync(cart.Id);

                // Invalidate cache
                await InvalidateMultipleCachesAsync(
                    CreateCacheKey(USER_CART_CACHE_PREFIX, userId),
                    CreateCacheKey(CART_SUMMARY_CACHE_PREFIX, userId)
                );

                LogInfo($"🧹 Cart cleared for user {userId}");

                return new ClearCartResponseDto
                {
                    CartId = cart.Id,
                    IsCleared = true,
                    ClearedAt = DateTime.UtcNow
                };

            }, "ClearCart", "Cart cleared successfully");
        }

        /// <summary>
        /// Validate cart before checkout
        /// Checks: cart not empty, products active, sufficient stock
        /// </summary>
        public async Task<ApiResponse<bool>> ValidateCartForCheckoutAsync(Guid userId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId);

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetUserCartWithItemsAsync(userId);
                ThrowIf(cart == null, "Cart not found");
                ThrowIf(!cart.CartItems.Any(), "Cart is empty");

                foreach (var cartItem in cart.CartItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                    ThrowIf(product == null, $"Product {cartItem.ProductId} not found");
                    ThrowIf(!product.IsActive, $"Product {product.Name} is not available");
                    ThrowIf(product.Stock < cartItem.Quantity,
                        $"Insufficient stock for {product.Name}. Available: {product.Stock}, Required: {cartItem.Quantity}");
                }

                LogInfo($"✅ Cart validation passed for user {userId}");
                return true;

            }, "ValidateCartForCheckout", "Cart validated successfully");
        }

        #endregion

        #region Guest Cart Methods

        /// <summary>
        /// Get guest cart by session ID
        /// Creates new empty cart if doesn't exist
        /// </summary>
        public async Task<ApiResponse<GetCartResponseDto>> GetGuestCartAsync(string sessionId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(sessionId, nameof(sessionId));

                var cacheKey = CreateCacheKey(GUEST_CART_CACHE_PREFIX, sessionId);
                var cart = await GetFromCacheOrExecuteAsync(
                    cacheKey,
                    async () =>
                    {
                        var cartRepository = _unitOfWork.Carts as ICartRepository;
                        var existingCart = await cartRepository.GetBySessionIdAsync(sessionId);

                        // ✅ If cart doesn't exist, create a new empty one
                        if (existingCart == null)
                        {
                            LogInfo($"📦 Creating new guest cart for session: {sessionId.Substring(0, 8)}...");
                            existingCart = await cartRepository.GetOrCreateGuestCartAsync(sessionId);
                        }

                        return existingCart;
                    },
                    TimeSpan.FromMinutes(CACHE_DURATION_MINUTES)
                );

                var cartItems = cart.CartItems ?? new List<CartItem>();
                var cartDto = MapCartToDto(cart, cartItems);

                LogInfo($"👤 Guest cart retrieved: {cartItems.Count} items");

                return cartDto;

            }, "GetGuestCart", "Guest cart retrieved successfully");
        }

        /// <summary>
        /// Get guest cart summary
        /// </summary>
        public async Task<ApiResponse<CartSummaryDto>> GetGuestCartSummaryAsync(string sessionId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(sessionId, nameof(sessionId));

                var cacheKey = CreateCacheKey(CART_SUMMARY_CACHE_PREFIX, $"guest:{sessionId}");

                var summary = await GetFromCacheOrExecuteAsync(
                    cacheKey,
                    async () =>
                    {
                        var cartRepository = _unitOfWork.Carts as ICartRepository;

                        // ✅ Get or create cart instead of just getting
                        var cart = await cartRepository.GetOrCreateGuestCartAsync(sessionId);

                        return new CartSummaryDto
                        {
                            ItemCount = cart.CartItems?.Sum(ci => ci.Quantity) ?? 0,
                            TotalAmount = cart.CartItems?.Sum(ci =>
                                ci.Quantity * GetCurrentProductPrice(ci.Product?.Prices)) ?? 0,
                            Status = cart.IsActive ? "ACTIVE" : "INACTIVE"
                        };
                    },
                    TimeSpan.FromMinutes(CACHE_DURATION_MINUTES)
                );

                return summary;

            }, "GetGuestCartSummary", "Guest cart summary retrieved successfully");
        }

        /// <summary>
        /// Add item to guest cart
        /// </summary>
        public async Task<ApiResponse<AddToCartResponseDto>> AddToGuestCartAsync(string sessionId, AddToCartDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(sessionId, nameof(sessionId));
                ValidateNotNull(dto, nameof(dto));
                ValidateId(dto.ProductId, nameof(dto.ProductId));
                ThrowIf(dto.Quantity <= 0, "Quantity must be greater than 0");

                var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
                ThrowIf(product == null, "Product not found");

                ThrowIf(product.Stock < dto.Quantity,
                    $"Insufficient stock. Available: {product.Stock}");

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
                var totalAmount = updatedCart.CartItems.Sum(ci =>
                    ci.Quantity * GetCurrentProductPrice(ci.Product?.Prices));

                // Invalidate cache
                await InvalidateMultipleCachesAsync(
                    CreateCacheKey(GUEST_CART_CACHE_PREFIX, sessionId),
                    CreateCacheKey(CART_SUMMARY_CACHE_PREFIX, $"guest:{sessionId}")
                );

                var response = new AddToCartResponseDto
                {
                    CartItemId = cartItem.Id,
                    ProductId = dto.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = currentPrice,
                    CartItemCount = updatedCart.CartItems.Sum(ci => ci.Quantity),
                    CartTotalAmount = totalAmount
                };

                LogInfo($"➕ Item {dto.ProductId} added to guest cart. Qty: {dto.Quantity}");

                return response;

            }, "AddToGuestCart", "Item added to guest cart successfully");
        }

        /// <summary>
        /// Update item quantity in guest cart
        /// </summary>
        public async Task<ApiResponse<UpdateCartItemResponseDto>> UpdateGuestCartItemAsync(string sessionId, UpdateCartItemDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(sessionId, nameof(sessionId));
                ValidateNotNull(dto, nameof(dto));
                ThrowIf(dto.Quantity <= 0, "Quantity must be greater than 0");

                var cartItem = await _unitOfWork.CartItems.GetByIdAsync(dto.CartItemId);
                ThrowIf(cartItem == null, "Cart item not found");

                var cart = await _unitOfWork.Carts.GetByIdAsync(cartItem.CartId);
                ThrowIf(cart == null || cart.SessionId != sessionId, "Unauthorized access to guest cart");

                var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                ThrowIf(product == null, "Product not found");

                ThrowIf(product.Stock < dto.Quantity,
                    $"Insufficient stock. Available: {product.Stock}");

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var updatedItem = await cartRepository.UpdateCartItemQuantityAsync(dto.CartItemId, dto.Quantity);

                var updatedCart = await cartRepository.GetBySessionIdAsync(sessionId);
                var newCartTotal = updatedCart.CartItems.Sum(ci =>
                    ci.Quantity * GetCurrentProductPrice(ci.Product?.Prices));

                // Invalidate cache
                await InvalidateMultipleCachesAsync(
                    CreateCacheKey(GUEST_CART_CACHE_PREFIX, sessionId),
                    CreateCacheKey(CART_SUMMARY_CACHE_PREFIX, $"guest:{sessionId}")
                );

                var response = new UpdateCartItemResponseDto
                {
                    CartItemId = updatedItem.Id,
                    NewQuantity = updatedItem.Quantity,
                    NewLineTotal = updatedItem.Quantity * GetCurrentProductPrice(product.Prices),
                    CartTotalAmount = newCartTotal
                };

                return response;

            }, "UpdateGuestCartItem", "Guest cart item updated successfully");
        }

        /// <summary>
        /// Remove item from guest cart
        /// </summary>
        public async Task<ApiResponse<bool>> RemoveFromGuestCartAsync(string sessionId, Guid cartItemId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(sessionId, nameof(sessionId));
                ValidateId(cartItemId);

                var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
                ThrowIf(cartItem == null, "Cart item not found");

                var cart = await _unitOfWork.Carts.GetByIdAsync(cartItem.CartId);
                ThrowIf(cart == null || cart.SessionId != sessionId, "Unauthorized access to guest cart");

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                await cartRepository.RemoveCartItemAsync(cartItemId);

                // Invalidate cache
                await InvalidateMultipleCachesAsync(
                    CreateCacheKey(GUEST_CART_CACHE_PREFIX, sessionId),
                    CreateCacheKey(CART_SUMMARY_CACHE_PREFIX, $"guest:{sessionId}")
                );

                return true;

            }, "RemoveFromGuestCart", "Item removed from guest cart successfully");
        }

        /// <summary>
        /// Clear all items from guest cart
        /// </summary>
        public async Task<ApiResponse<ClearCartResponseDto>> ClearGuestCartAsync(string sessionId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(sessionId, nameof(sessionId));

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetBySessionIdAsync(sessionId);
                ThrowIf(cart == null, "Guest cart not found");

                await cartRepository.ClearCartItemsAsync(cart.Id);

                // Invalidate cache
                await InvalidateMultipleCachesAsync(
                    CreateCacheKey(GUEST_CART_CACHE_PREFIX, sessionId),
                    CreateCacheKey(CART_SUMMARY_CACHE_PREFIX, $"guest:{sessionId}")
                );

                return new ClearCartResponseDto
                {
                    CartId = cart.Id,
                    IsCleared = true,
                    ClearedAt = DateTime.UtcNow
                };

            }, "ClearGuestCart", "Guest cart cleared successfully");
        }

        /// <summary>
        /// Validate guest cart before checkout
        /// </summary>
        public async Task<ApiResponse<bool>> ValidateGuestCartForCheckoutAsync(string sessionId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(sessionId, nameof(sessionId));

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetBySessionIdAsync(sessionId);
                ThrowIf(cart == null, "Guest cart not found");
                ThrowIf(!cart.CartItems.Any(), "Guest cart is empty");

                foreach (var cartItem in cart.CartItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                    ThrowIf(product == null, $"Product {cartItem.ProductId} not found");
                    ThrowIf(!product.IsActive, $"Product {product.Name} is not available");
                    ThrowIf(product.Stock < cartItem.Quantity,
                        $"Insufficient stock for {product.Name}. Available: {product.Stock}, Required: {cartItem.Quantity}");
                }

                return true;

            }, "ValidateGuestCartForCheckout", "Guest cart validated successfully");
        }

        /// <summary>
        /// Merge guest cart to user cart on login
        /// Combines items from both carts
        /// </summary>
        public async Task<ApiResponse<GetCartResponseDto>> MergeGuestCartToUserAsync(string sessionId, Guid userId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(sessionId, nameof(sessionId));
                ValidateId(userId);

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                await cartRepository.MergeGuestCartToUserCartAsync(sessionId, userId);

                var cart = await cartRepository.GetUserCartWithItemsAsync(userId);
                ThrowIf(cart == null, "Failed to merge carts");

                var cartItems = cart.CartItems ?? new List<CartItem>();
                var cartDto = MapCartToDto(cart, cartItems);

                // Invalidate all cart caches
                await InvalidateMultipleCachesAsync(
                    CreateCacheKey(USER_CART_CACHE_PREFIX, userId),
                    CreateCacheKey(CART_SUMMARY_CACHE_PREFIX, userId),
                    CreateCacheKey(GUEST_CART_CACHE_PREFIX, sessionId),
                    CreateCacheKey(CART_SUMMARY_CACHE_PREFIX, $"guest:{sessionId}")
                );

                LogInfo($"✅ Guest cart merged to user {userId}. Items: {cartItems.Count}");

                return cartDto;

            }, "MergeGuestCartToUser", "Guest cart merged successfully");
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Get item count in user's cart
        /// </summary>
        public async Task<ApiResponse<int>> GetCartItemCountAsync(Guid userId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId);

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetUserCartWithItemsAsync(userId);

                if (cart == null)
                    return 0;

                return cart.CartItems.Sum(ci => ci.Quantity);

            }, "GetCartItemCount", "Cart item count retrieved");
        }

        /// <summary>
        /// Get item count in guest cart
        /// </summary>
        public async Task<ApiResponse<int>> GetGuestCartItemCountAsync(string sessionId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(sessionId, nameof(sessionId));

                var cartRepository = _unitOfWork.Carts as ICartRepository;
                var cart = await cartRepository.GetBySessionIdAsync(sessionId);

                if (cart == null)
                    return 0;

                return cart.CartItems.Sum(ci => ci.Quantity);

            }, "GetGuestCartItemCount", "Guest cart item count retrieved");
        }

        /// <summary>
        /// Get detailed information about a specific cart item
        /// </summary>
        public async Task<ApiResponse<CartItemDetailDto>> GetCartItemDetailAsync(Guid userId, Guid cartItemId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId);
                ValidateId(cartItemId);

                var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
                ThrowIf(cartItem == null, "Cart item not found");

                var cart = await _unitOfWork.Carts.GetByIdAsync(cartItem.CartId);
                ThrowIf(cart == null || cart.UserId != userId, "Unauthorized access to cart item");

                var detail = _mapper.Map<CartItemDetailDto>(cartItem);
                return detail;

            }, "GetCartItemDetail", "Cart item detail retrieved successfully");
        }

        /// <summary>
        /// Apply coupon code to cart (NOT IMPLEMENTED YET)
        /// </summary>
        public async Task<ApiResponse<CartSummaryDto>> ApplyCouponAsync(Guid userId, string couponCode)
        {
            return await ExecuteAsApiResponseAsync<CartSummaryDto>(async () =>
            {
                ValidateId(userId);
                ValidateNotEmpty(couponCode, nameof(couponCode));

                throw new NotImplementedException("Coupon feature not implemented yet");

            }, "ApplyCoupon", "Coupon applied successfully");
        }

        /// <summary>
        /// Remove coupon from cart (NOT IMPLEMENTED YET)
        /// </summary>
        public async Task<ApiResponse<CartSummaryDto>> RemoveCouponAsync(Guid userId)
        {
            return await ExecuteAsApiResponseAsync<CartSummaryDto>(async () =>
            {
                ValidateId(userId);

                throw new NotImplementedException("Coupon feature not implemented yet");

            }, "RemoveCoupon", "Coupon removed successfully");
        }

        /// <summary>
        /// Update shipping information for cart (NOT IMPLEMENTED YET)
        /// </summary>
        public async Task<ApiResponse<CartSummaryDto>> UpdateShippingInfoAsync(Guid userId, OrderShippingDto dto)
        {
            return await ExecuteAsApiResponseAsync<CartSummaryDto>(async () =>
            {
                ValidateId(userId);
                ValidateNotNull(dto, nameof(dto));

                throw new NotImplementedException("Shipping feature not implemented yet");

            }, "UpdateShippingInfo", "Shipping info updated successfully");
        }

        /// <summary>
        /// Map Cart entity to GetCartResponseDto
        /// Calculates totals and formats for API response
        /// </summary>
        private GetCartResponseDto MapCartToDto(Cart cart, IEnumerable<CartItem> cartItems)
        {
            var subTotal = cartItems.Sum(ci => ci.Quantity * GetCurrentProductPrice(ci.Product?.Prices));

            return new GetCartResponseDto
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
                SubTotal = subTotal,
                TaxAmount = 0,
                ShippingFee = 0,
                TotalAmount = subTotal,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt
            };
        }

        #endregion
    }
}