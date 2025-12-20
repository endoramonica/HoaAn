// File: VietCommerce.Application.Services/Services/CheckoutService.cs
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Payments;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Payments;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Enums.Payments;
using VietCommerce.Core.Models;
using VietCommerce.Core.Helpers;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services
{
    public class CheckoutService : BaseService, ICheckoutService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IVnpayService _vnpayService;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ICurrentUser _currentUser;
        private readonly IOrderService _orderService;

        public CheckoutService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork,
            IVnpayService vnpayService,
            IHttpContextAccessor httpContext,
            ICurrentUser currentUser,
            IOrderService orderService,
            IMapper mapper,
            ILogger<CheckoutService> logger,
            ICacheService cacheService)
            : base(logger, cacheService)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _vnpayService = vnpayService;
            _httpContext = httpContext;
            _currentUser = currentUser;
            _orderService = orderService;
        }

        // ========================================
        // HELPER METHOD: Unified CustomerId Retrieval
        // ========================================
        private async Task<Guid> GetCustomerIdAsync()
        {
            // Step 1: Check if CustomerId is available in JWT claims
            if (_currentUser.CustomerId != Guid.Empty)
            {
                LogInfo("✅ Using CustomerId from JWT: {CustomerId}", _currentUser.CustomerId);
                return _currentUser.CustomerId;
            }

            // Step 2: Retrieve CustomerId from database using UserId
            var userId = _currentUser.UserId;
            LogInfo("🔍 CustomerId not in JWT, looking up by UserId: {UserId}", userId);

            var customer = await _customerRepository.GetByUserIdAsync(userId);

            if (customer == null)
            {
                LogWarning("⚠️ Customer not found for UserId: {UserId}", userId);
                throw new UnauthorizedAccessException("Customer not found for user");
            }

            LogInfo("✅ Found CustomerId from database: {CustomerId}", customer.Id);
            return customer.Id;
        }

        // ========================================
        // GetOrderByIdAsync - Using Helper Method
        // ========================================
        public async Task<ApiResponse<OrderDetailDto>> GetOrderByIdAsync(Guid userId, Guid orderId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo("📦 Getting order {OrderId} for user {UserId}", orderId, userId);

                var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

                if (order == null)
                {
                    LogWarning("⚠️ Order {OrderId} not found", orderId);
                    throw new KeyNotFoundException("Order not found");
                }

                // ✅ Use helper method for consistent CustomerId retrieval
                var customerId = await GetCustomerIdAsync();

                if (order.CustomerId != customerId)
                {
                    LogWarning("🚫 Unauthorized access to order {OrderId} by user {UserId}. Order.CustomerId={OrderCustomerId}, CurrentCustomerId={CurrentCustomerId}",
                        orderId, userId, order.CustomerId, customerId);
                    throw new UnauthorizedAccessException("Unauthorized access to order");
                }

                var orderDto = MapOrderToDetailDto(order);
                LogInfo("✅ Order {OrderId} retrieved successfully", orderId);

                return orderDto;
            }, nameof(GetOrderByIdAsync), "Order retrieved successfully");
        }

        // ========================================
        // GetOrdersAsync - Complete Implementation
        // ========================================
        public async Task<ApiResponse<List<OrderDetailDto>>> GetOrdersAsync(Guid userId, OrderFilterDTO filter)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo("📋 Getting orders for user {UserId}", userId);

                // ✅ Use helper method for consistent CustomerId retrieval
                var customerId = await GetCustomerIdAsync();

                // Query orders by CustomerId with filters
                var result = await _orderRepository.GetOrdersByCustomerIdAsync(customerId, filter);

                // Map to DTOs
                var dtoList = result.Items.Select(MapOrderToDetailDto).ToList();

                LogInfo("✅ Retrieved {Count} orders for customer {CustomerId}",
                    dtoList.Count, customerId);

                return dtoList;
            }, nameof(GetOrdersAsync), "Orders retrieved successfully");
        }

        // ========================================
        // CancelOrderAsync - Using Helper Method
        // ========================================
        public async Task<ApiResponse<bool>> CancelOrderAsync(Guid userId, Guid orderId, string reason)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                LogInfo("🚫 Cancelling order {OrderId} for user {UserId}", orderId, userId);

                var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

                if (order == null)
                    throw new KeyNotFoundException("Order not found");

                // ✅ Use helper method for consistent CustomerId retrieval
                var customerId = await GetCustomerIdAsync();

                if (order.CustomerId != customerId)
                {
                    LogWarning("🚫 Unauthorized cancel attempt for order {OrderId} by user {UserId}. Order.CustomerId={OrderCustomerId}, CurrentCustomerId={CurrentCustomerId}",
                        orderId, userId, order.CustomerId, customerId);
                    throw new UnauthorizedAccessException("Unauthorized access to order");
                }

                var canCancel = await _orderRepository.CanChangeStatusAsync(orderId, OrderStatus.Cancelled);
                if (!canCancel)
                    throw new InvalidOperationException("Cannot cancel order at current status");

                var result = await _orderRepository.CancelOrderAsync(orderId, reason, userId);

                if (!result)
                    throw new InvalidOperationException("Failed to cancel order");

                LogInfo("✅ Order {OrderId} cancelled by user {UserId}", orderId, userId);
                return true;
            }, nameof(CancelOrderAsync), "Order cancelled successfully");
        }

        // ========================================
        // CheckoutAsync - Using Helper Method
        // ========================================
        public async Task<ApiResponse<CheckoutResponseDto>> CheckoutAsync(Guid userId, CheckoutDto dto)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                LogInfo("🛒 Checkout started for user {UserId}, cart {CartId}", userId, dto.CartId);

                var validation = ValidateCheckoutRequest(dto);
                if (!validation.Success)
                    return validation;

                var storeId = Guid.Parse("47AA5519-C503-4CFA-8101-2EDB36FD9D8C");

                // ✅ Use helper method - it will handle both JWT CustomerId and database lookup
                Guid customerId;
                try
                {
                    customerId = await GetCustomerIdAsync();
                    LogInfo("✅ Using CustomerId: {CustomerId}", customerId);
                }
                catch (UnauthorizedAccessException)
                {
                    // If customer doesn't exist, create one
                    var customer = await _unitOfWork.Customers.EnsureCustomerExistsAsync(userId, storeId);
                    customerId = customer.Id;
                    LogInfo("✅ Created new Customer: {CustomerId}", customerId);
                }

                var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(dto.CartId);
                if (cart == null)
                    return ApiResponse<CheckoutResponseDto>.FailureResponse("Cart not found CART_NOT_FOUND");
                if (cart.UserId != userId)
                    return ApiResponse<CheckoutResponseDto>.FailureResponse("Unauthorized cart access UNAUTHORIZED_CART");

                var cartItems = await _unitOfWork.Carts.GetCartItemsAsync(dto.CartId);
                if (cartItems == null || !cartItems.Any())
                    return ApiResponse<CheckoutResponseDto>.FailureResponse("Cart is empty EMPTY_CART");

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = GenerateOrderNumber(),
                    StoreId = storeId,
                    CustomerId = customerId, // ✅ Using customerId from helper
                    Status = OrderStatus.Pending,
                    ShippingFee = CalculateShippingFee(dto.ShippingInfo),
                    CreatedById = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var orderItems = new List<OrderItem>();
                foreach (var ci in cartItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(ci.ProductId);
                    if (product == null)
                    {
                        LogWarning("Product {ProductId} not found in cart", ci.ProductId);
                        continue;
                    }

                    if (product.Stock < ci.Quantity)
                    {
                        return ApiResponse<CheckoutResponseDto>.FailureResponse(
                            $"Insufficient stock for {product.Name} INSUFFICIENT_STOCK");
                    }

                    // Use OrderService to create OrderItem from CartItem
                    // This ensures customization data is properly snapshotted
                    var orderItem = await _orderService.CreateOrderItemFromCartItemAsync(ci, order.Id);

                    // Enforce Cart as source of truth for pricing
                    // If CartItem has a computed FinalPrice (>0), use it; otherwise fallback to current product price
                    var fallbackUnitPrice = PriceCalculationHelper.GetCurrentPrice(product);
                    var unitPrice = ci.FinalPrice > 0 ? ci.FinalPrice : fallbackUnitPrice;
                    var totalPrice = ci.FinalPrice > 0 ? ci.FinalPrice : ci.Quantity * fallbackUnitPrice;
                    orderItem.UnitPrice = unitPrice;
                    orderItem.TotalPrice = totalPrice;

                    orderItems.Add(orderItem);
                }

                if (!orderItems.Any())
                    return ApiResponse<CheckoutResponseDto>.FailureResponse("No valid items in cart INVALID_ITEMS");

                // Calculate order subtotal based on Cart pricing rules
                order.SubTotal = orderItems.Sum(i => i.TotalPrice);
                order.TaxAmount = CalculateTax(order.SubTotal);
                order.DiscountAmount = await CalculateDiscountAsync(dto.CouponCode) ?? 0m;
                order.TotalAmount = order.SubTotal + order.ShippingFee + order.TaxAmount - order.DiscountAmount;
                order.Notes = dto.Notes ?? string.Empty;

                var orderShipping = _mapper.Map<OrderShipping>(dto.ShippingInfo);
                orderShipping.Id = Guid.NewGuid();
                orderShipping.OrderId = order.Id;
                orderShipping.ShippingCost = order.ShippingFee;
                orderShipping.Status = ShippingStatus.PREPARING;
                orderShipping.CreatedAt = DateTime.UtcNow;
                orderShipping.UpdatedAt = DateTime.UtcNow;

                var statusHistory = new OrderStatusHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    OldStatus = OrderStatus.Pending,
                    NewStatus = OrderStatus.Pending,
                    ChangedBy = userId,
                    Notes = "Order created",
                    ChangedAt = DateTime.UtcNow
                };

                var paymentMethodId = await GetPaymentMethodIdAsync(dto.PaymentMethod);
                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    MethodId = paymentMethodId,
                    Amount = order.TotalAmount,
                    Status = PaymentMethodType.PENDING,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.OrderItems.AddRangeAsync(orderItems);
                await _unitOfWork.OrderShipping.AddAsync(orderShipping);
                await _unitOfWork.OrderStatusHistories.AddAsync(statusHistory);
                await _unitOfWork.Payments.AddAsync(payment);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.Carts.ClearCartItemsAsync(dto.CartId);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                LogInfo("✅ Transaction committed. Order {OrderNumber} created for user {UserId}, CustomerId={CustomerId}",
                    order.OrderNumber, userId, customerId);

                var responseDto = new CheckoutResponseDto
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    CreatedAt = order.CreatedAt,
                    StoreId = order.StoreId,
                    CustomerId = customerId,
                    CustomerName = _currentUser.UserName,
                    Items = orderItems.Select(oi => new OrderItemDTO
                    {
                        Id = oi.Id,
                        OrderId = oi.OrderId,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        ProductSKU = oi.ProductCode,
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.TotalPrice
                    }).ToList(),
                    Shipping = _mapper.Map<OrderShippingDto>(orderShipping),
                    PaymentMethodUsed = dto.PaymentMethod.ToString()
                };

                if (dto.PaymentMethod != PaymentMethodType.COD)
                {
                    var ip = _httpContext.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                    responseDto.PaymentUrl = _vnpayService.CreatePaymentUrl(
                        order.OrderNumber,
                        order.TotalAmount,
                        ip
                    );

                    LogInfo("💳 Payment URL generated for order {OrderNumber}", order.OrderNumber);
                    return ApiResponse<CheckoutResponseDto>.SuccessResponse(
                        responseDto,
                        "Order created successfully. Redirect to payment."
                    );
                }

                LogInfo("💵 COD payment selected for order {OrderNumber}", order.OrderNumber);
                return ApiResponse<CheckoutResponseDto>.SuccessResponse(
                    responseDto,
                    "Order created successfully. Payment on delivery."
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                LogError("❌ Checkout failed for user {UserId}, cart {CartId}", ex, userId, dto.CartId);
                return ApiResponse<CheckoutResponseDto>.FailureResponse(
                    "Checkout failed. Please try again. CHECKOUT_ERROR");
            }
        }

        // ========================================
        // PRIVATE HELPERS
        // ========================================

        private ProductPrice? GetCurrentProductPrice(Product product)
        {
            if (product.Prices == null || !product.Prices.Any())
                return null;

            var now = DateTime.UtcNow;

            return product.Prices
                .Where(p => p.IsActive && p.EffectiveFrom <= now &&
                           (p.EffectiveTo == null || p.EffectiveTo >= now))
                .OrderByDescending(p => p.EffectiveFrom)
                .FirstOrDefault();
        }

        private ApiResponse<CheckoutResponseDto> ValidateCheckoutRequest(CheckoutDto dto)
        {
            if (dto.ShippingInfo == null)
                return ApiResponse<CheckoutResponseDto>.FailureResponse(
                    "Shipping information is required MISSING_SHIPPING");

            if (string.IsNullOrWhiteSpace(dto.ShippingInfo.RecipientName))
                return ApiResponse<CheckoutResponseDto>.FailureResponse(
                    "Recipient name is required INVALID_RECIPIENT");

            if (string.IsNullOrWhiteSpace(dto.ShippingInfo.Address))
                return ApiResponse<CheckoutResponseDto>.FailureResponse(
                    "Address is required INVALID_ADDRESS");

            return ApiResponse<CheckoutResponseDto>.SuccessResponse(null!);
        }

        private string GenerateOrderNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyMMddHHmmss");
            var randomSuffix = GenerateRandomSuffix(6);
            return $"{timestamp}{randomSuffix}";
        }

        private string GenerateRandomSuffix(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Range(0, length)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }

        private decimal CalculateShippingFee(OrderShippingInputDto shippingInfo)
        {
            return 30000m;
        }

        private decimal CalculateTax(decimal subTotal)
        {
            return subTotal * 0.1m;
        }

        private async Task<decimal?> CalculateDiscountAsync(string? couponCode)
        {
            if (string.IsNullOrWhiteSpace(couponCode))
                return null;
            return null;
        }

        private OrderDetailDto MapOrderToDetailDto(Order order)
        {
            return new OrderDetailDto
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                StoreId = order.StoreId,
                StoreName = order.Store?.Name,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.Name,
                CustomerEmail = order.Customer?.Email,
                CustomerPhone = order.Customer?.Phone,
                Status = order.Status,
                SubTotal = order.SubTotal,
                ShippingFee = order.ShippingFee,
                TaxAmount = order.TaxAmount,
                DiscountAmount = order.DiscountAmount,
                TotalAmount = order.TotalAmount,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                CompletedAt = order.CompletedAt,
                CreatedById = order.CreatedById,
                CreatedByName = order.CreatedByUser?.Name,
                Items = order.OrderItems?.Select(oi => new OrderItemDTO
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    ProductSKU = oi.ProductCode,
                    ProductImageUrl = oi.Product?.Images?.FirstOrDefault()?.Url,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                    TotalPrice = oi.TotalPrice
                }).ToList() ?? new List<OrderItemDTO>(),
                Shipping = _mapper.Map<OrderShippingDto>(order.OrderShipping),
                StatusHistories = order.OrderStatusHistories?
                    .OrderByDescending(h => h.ChangedAt)
                    .Select(h => new OrderStatusHistoryDTO
                    {
                        Status = h.NewStatus,
                        ChangedAt = h.ChangedAt,
                        ChangedByName = h.ChangedByUser?.Name,
                        Notes = h.Notes
                    })
                    .ToList() ?? new List<OrderStatusHistoryDTO>()
            };
        }

        private async Task<Guid?> GetPaymentMethodIdAsync(PaymentMethodType type)
        {
            var method = await _unitOfWork.PaymentMethods.GetByCodeAsync(type.ToString());
            return method?.Id;
        }
    }
}
