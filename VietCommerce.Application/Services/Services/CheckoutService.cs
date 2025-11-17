// File: VietCommerce.Api/Services/CheckoutService.cs (FIXED VERSION)
using AutoMapper;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CheckoutService> _logger;

        public CheckoutService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CheckoutService> logger)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Process checkout and create order from cart
        /// </summary>
        public async Task<ApiResponse<CheckoutResponseDto>> CheckoutAsync(Guid userId, CheckoutDto dto)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("🛒 Checkout started for user {UserId}, cart {CartId}", userId, dto.CartId);

                // [1] Validate request
                var validation = ValidateCheckoutRequest(dto);
                if (!validation.Success)
                    return validation;

                // [2] Get or create customer
                var storeId = Guid.Parse("47AA5519-C503-4CFA-8101-2EDB36FD9D8C");
                var customer = await _unitOfWork.Customers.EnsureCustomerExistsAsync(userId, storeId);

                // [3] Validate cart
                var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(dto.CartId);
                if (cart == null)
                    return ApiResponse<CheckoutResponseDto>.FailureResponse("Cart not found CART_NOT_FOUND");
                if (cart.UserId != userId)
                    return ApiResponse<CheckoutResponseDto>.FailureResponse("Unauthorized cart access UNAUTHORIZED_CART");

                var cartItems = await _unitOfWork.Carts.GetCartItemsAsync(dto.CartId);
                if (cartItems == null || !cartItems.Any())
                    return ApiResponse<CheckoutResponseDto>.FailureResponse("Cart is empty EMPTY_CART");

                // [4] Create order
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = GenerateOrderNumber(),
                    StoreId = storeId,
                    CustomerId = customer.Id,
                    Status = OrderStatus.Pending,
                    ShippingFee = CalculateShippingFee(dto.ShippingInfo),
                    CreatedById = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // [5] Map order items
                var orderItems = new List<OrderItem>();
                foreach (var ci in cartItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(ci.ProductId);
                    if (product == null)
                    {
                        _logger.LogWarning("Product {ProductId} not found in cart", ci.ProductId);
                        continue;
                    }

                    var currentPrice = GetCurrentProductPrice(product);
                    if (currentPrice == null)
                    {
                        _logger.LogWarning("No active price found for product {ProductId}", ci.ProductId);
                        continue;
                    }

                    if (product.Stock < ci.Quantity)
                    {
                        return ApiResponse<CheckoutResponseDto>.FailureResponse(
                            $"Insufficient stock for {product.Name} INSUFFICIENT_STOCK");
                    }

                    orderItems.Add(new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ProductCode = product.Code ?? "",
                        UnitPrice = currentPrice.Price,
                        Quantity = ci.Quantity,
                        TotalPrice = currentPrice.Price * ci.Quantity,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                if (!orderItems.Any())
                    return ApiResponse<CheckoutResponseDto>.FailureResponse("No valid items in cart INVALID_ITEMS");

                // [6] Calculate totals
                order.SubTotal = orderItems.Sum(i => i.TotalPrice);
                order.TaxAmount = CalculateTax(order.SubTotal);
                order.DiscountAmount = await CalculateDiscountAsync(dto.CouponCode) ?? 0m;
                order.TotalAmount = order.SubTotal + order.ShippingFee + order.TaxAmount - order.DiscountAmount;
                order.Notes = dto.Notes ?? string.Empty;

                // [7] Create shipping
                var orderShipping = _mapper.Map<OrderShipping>(dto.ShippingInfo);
                orderShipping.Id = Guid.NewGuid();
                orderShipping.OrderId = order.Id;
                orderShipping.ShippingCost = order.ShippingFee;
                orderShipping.Status = ShippingStatus.PREPARING;
                orderShipping.CreatedAt = DateTime.UtcNow;
                orderShipping.UpdatedAt = DateTime.UtcNow;

                // [8] Create status history
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

                // [9] Save all
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.OrderItems.AddRangeAsync(orderItems);
                await _unitOfWork.OrderShipping.AddAsync(orderShipping);
                await _unitOfWork.OrderStatusHistories.AddAsync(statusHistory);
                await _unitOfWork.SaveChangesAsync();

                // [10] Clear cart
                await _unitOfWork.Carts.ClearCartItemsAsync(dto.CartId);
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("✅ Order {OrderNumber} created successfully for user {UserId}",
                    order.OrderNumber, userId);

                // [11] Return response
                var response = new CheckoutResponseDto
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    CreatedAt = order.CreatedAt,
                    StoreId = order.StoreId,
                    StoreName = order.Store?.Name,
                    CustomerId = customer.Id,
                    CustomerName = customer.Name,
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
                    Shipping = _mapper.Map<OrderShippingDto>(orderShipping)
                };

                return ApiResponse<CheckoutResponseDto>.SuccessResponse(response, "Order created successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "❌ Checkout failed for user {UserId}", userId);
                return ApiResponse<CheckoutResponseDto>.FailureResponse(
                    "Checkout failed. Please try again. CHECKOUT_ERROR");
            }
        }


        /// <summary>
        /// Get order by ID
        /// </summary>
        public async Task<ApiResponse<OrderDetailDto>> GetOrderByIdAsync(Guid userId, Guid orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

                if (order == null)
                    return ApiResponse<OrderDetailDto>.FailureResponse("Order not found ORDER_NOT_FOUND");

                // Security: Verify user owns this order
                if (order.CustomerId != userId)
                {
                    _logger.LogWarning("Unauthorized access to order {OrderId} by user {UserId}", orderId, userId);
                    return ApiResponse<OrderDetailDto>.FailureResponse("Unauthorized UNAUTHORIZED");
                }

                var orderDto = MapOrderToDetailDto(order);
                return ApiResponse<OrderDetailDto>.SuccessResponse(orderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId}", orderId);
                return ApiResponse<OrderDetailDto>.FailureResponse("Failed to retrieve order ORDER_ERROR");
            }
        }

        /// <summary>
        /// Get user's orders
        /// </summary>
        public async Task<ApiResponse<List<OrderDetailDto>>> GetOrdersAsync(Guid userId, OrderFilterDTO filter)
        {
            try
            {
                var result = await _orderRepository.GetUserOrdersAsync(userId, filter);
                var dtoList = result.Items.Select(MapOrderToDetailDto).ToList();

                return ApiResponse<List<OrderDetailDto>>.SuccessResponse(dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for user {UserId}", userId);
                return ApiResponse<List<OrderDetailDto>>.FailureResponse("Failed to retrieve orders ORDER_ERROR");
            }
        }

        /// <summary>
        /// Cancel order
        /// </summary>
        public async Task<ApiResponse<bool>> CancelOrderAsync(Guid userId, Guid orderId, string reason)
        {
            try
            {
                var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

                if (order == null)
                    return ApiResponse<bool>.FailureResponse("Order not found ORDER_NOT_FOUND");

                // Security check
                if (order.CustomerId != userId)
                    return ApiResponse<bool>.FailureResponse("Unauthorized UNAUTHORIZED");

                // Check if can cancel
                var canCancel = await _orderRepository.CanChangeStatusAsync(orderId, OrderStatus.Cancelled);
                if (!canCancel)
                    return ApiResponse<bool>.FailureResponse("Cannot cancel order at current status CANNOT_CANCEL");

                // Cancel order
                var result = await _orderRepository.CancelOrderAsync(orderId, reason, userId);

                if (!result)
                    return ApiResponse<bool>.FailureResponse("Failed to cancel order CANCEL_ERROR");

                _logger.LogInformation("Order {OrderId} cancelled by user {UserId}", orderId, userId);
                return ApiResponse<bool>.SuccessResponse(true, "Order cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", orderId);
                return ApiResponse<bool>.FailureResponse("Failed to cancel order CANCEL_ERROR");
            }
        }

        // ========================================
        // PRIVATE HELPERS
        // ========================================

        /// <summary>
        /// Get current active price for a product
        /// </summary>
        private ProductPrice? GetCurrentProductPrice(Product product)
        {
            if (product.Prices == null || !product.Prices.Any())
                return null;

            var now = DateTime.UtcNow;

            // Get active prices within date range
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
            // TODO: Implement shipping fee calculation based on method, location, etc.
            return 30000m; // Default: 30,000 VND
        }

        private decimal CalculateTax(decimal subTotal)
        {
            // TODO: Implement tax calculation (usually 10% in Vietnam)
            return subTotal * 0.1m;
        }

        private async Task<decimal?> CalculateDiscountAsync(string? couponCode)
        {
            if (string.IsNullOrWhiteSpace(couponCode))
                return null;

            // TODO: Implement coupon validation and discount calculation
            return null;
        }

        private OrderDetailDto MapOrderToDetailDto(Order order)
        {
            var dto = new OrderDetailDto
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

            return dto;
        }

        
         
    }
}