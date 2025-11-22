using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.DTOs.Payments;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Payments;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Enums.Payments;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Payments
{
    /// <summary>
    ///     Concrete payment service implementation that coordinates order validation,
    ///     payment lifecycle, transaction history, caching and DTO mapping.
    /// </summary>
    public class PaymentService : BaseService, IPaymentService
    {
        private const string CachePrefixPayment = "payments";
        private const string CachePrefixOrderPayments = "orders:payments";
        private const string CachePrefixHistory = "payments:history";

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public PaymentService(
            ILogger<PaymentService> logger,
            ICacheService cacheService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUser currentUser)
            : base(logger, cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        #region Processing

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
        {
            ValidateNotNull(request, nameof(request));
            ValidateId(request.OrderId, nameof(request.OrderId));
            ValidateNotEmpty(request.Amount.ToString(), nameof(request.Amount));

            return request.PaymentType switch
            {
                PaymentMethodType.CASH => throw new ArgumentException(
                    "Use ProcessCashPaymentAsync for cash payments"),
                PaymentMethodType.CREDIT_CARD => throw new ArgumentException(
                    "Use ProcessCardPaymentAsync for card payments"),
                _ => await ExecuteAsync(async () =>
                {
                    var order = await EnsureOrderAsync(request.OrderId);
                    var paymentMethod = await EnsurePaymentMethodAsync(request.PaymentType);

                    var payment = await CreatePaymentAsync(order, paymentMethod, request.Amount);
                    payment.Status = PaymentMethodType.CONFIRMED;
                    payment.PaidAt = DateTime.UtcNow;

                    payment.PaymentTransactions.Add(new PaymentTransaction
                    {
                        TransactionId = GenerateTransactionCode(request.PaymentType.ToString()),
                        Status =PaymentMethodType.CONFIRMED,
                        GatewayResponse = "Manual payment success",
                        Amount = request.Amount,
                        TransactionDate = DateTime.UtcNow
                    });

                    _unitOfWork.Payments.Update(payment);
                    await _unitOfWork.SaveChangesAsync();

                    await InvalidatePaymentCachesAsync(order.Id);
                    return _mapper.Map<PaymentResponse>(payment);
                }, nameof(ProcessPaymentAsync))
            };
        }

        public async Task<PaymentResponse> ProcessCashPaymentAsync(CashPaymentRequest request)
        {
            ValidateNotNull(request, nameof(request));
            ValidateId(request.OrderId, nameof(request.OrderId));
            ThrowIf(request.AmountDue <= 0, "AmountDue phải lớn hơn 0");
            ThrowIf(request.AmountReceived < request.AmountDue, "Khách trả chưa đủ tiền mặt");

            return await ExecuteAsync(async () =>
            {
                var order = await EnsureOrderAsync(request.OrderId);
                EnsureOrderIsPayable(order);

                var paymentMethod = await EnsurePaymentMethodAsync(PaymentMethodType.CASH);
                var payment = await CreatePaymentAsync(order, paymentMethod, request.AmountDue);

                payment.Status = PaymentMethodType.CONFIRMED;
                payment.PaidAt = DateTime.UtcNow;

                payment.PaymentTransactions.Add(new PaymentTransaction
                {
                    TransactionId = GenerateTransactionCode(nameof(PaymentMethodType.CASH)),
                    Status = PaymentMethodType.CONFIRMED,
                    Amount = request.AmountDue,
                    TransactionDate = DateTime.UtcNow,
                    GatewayResponse = $"Cash received, change: {request.Change}"
                });

                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.SaveChangesAsync();

                await InvalidatePaymentCachesAsync(order.Id);

                return _mapper.Map<PaymentResponse>(payment);
            }, nameof(ProcessCashPaymentAsync));
        }

        public async Task<PaymentResponse> ProcessCardPaymentAsync(CardPaymentRequest request)
        {
            ValidateNotNull(request, nameof(request));
            ValidateId(request.OrderId, nameof(request.OrderId));
            ValidateNotEmpty(request.CardNumber, nameof(request.CardNumber));
            ValidateNotEmpty(request.CardHolderName, nameof(request.CardHolderName));
            ValidateNotEmpty(request.ExpiryDate, nameof(request.ExpiryDate));
            ValidateNotEmpty(request.CVV, nameof(request.CVV));

            return await ExecuteAsync(async () =>
            {
                var order = await EnsureOrderAsync(request.OrderId);
                EnsureOrderIsPayable(order);

                var paymentMethod = await EnsurePaymentMethodAsync(PaymentMethodType.CREDIT_CARD);

                // TODO: integrate real gateway (Stripe, Omise, etc.)
                var gatewayTransactionId = GenerateTransactionCode(nameof(PaymentMethodType.CREDIT_CARD));

                var amount = order.TotalAmount - GetPaidAmount(order);
                var payment = await CreatePaymentAsync(order, paymentMethod, amount);

                payment.Status = PaymentMethodType.CONFIRMED;
                payment.PaidAt = DateTime.UtcNow;

                payment.PaymentTransactions.Add(new PaymentTransaction
                {
                    TransactionId = gatewayTransactionId,
                    Status = PaymentMethodType.CONFIRMED,
                    Amount = amount,
                    TransactionDate = DateTime.UtcNow,
                    GatewayResponse = $"Approved for card ending {request.CardNumber[^4..]}"
                });

                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.SaveChangesAsync();

                await InvalidatePaymentCachesAsync(order.Id);

                return _mapper.Map<PaymentResponse>(payment);
            }, nameof(ProcessCardPaymentAsync));
        }

        public async Task<RefundResponse> ProcessRefundAsync(Guid paymentId, RefundRequest request)
        {
            ValidateId(paymentId, nameof(paymentId));
            ValidateNotNull(request, nameof(request));
            ThrowIf(request.Amount <= 0, "Refund amount phải lớn hơn 0");

            return await ExecuteAsync(async () =>
            {
                var payment = await EnsurePaymentAsync(paymentId);
                ThrowIf(payment.Amount < request.Amount, "Refund amount vượt quá số tiền thanh toán");
                ThrowIf(payment.Status == PaymentMethodType.REFUNDED, "Payment này đã được hoàn tiền");

                payment.Status = PaymentMethodType.REFUNDED;

                payment.PaymentTransactions.Add(new PaymentTransaction
                {
                    TransactionId = GenerateTransactionCode(nameof(PaymentMethodType.REFUNDED)),
                    Status = PaymentMethodType.REFUNDED,
                    Amount = -request.Amount,
                    TransactionDate = DateTime.UtcNow,
                    GatewayResponse = request.Reason
                });

                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.SaveChangesAsync();

                await InvalidatePaymentCachesAsync(payment.OrderId);

                return new RefundResponse
                {
                    RefundId = Guid.NewGuid(),
                    IsSuccess = true,
                    Message = "Refund processed successfully",
                    RefundedAmount = request.Amount,
                    RefundedAt = DateTime.UtcNow
                };
            }, nameof(ProcessRefundAsync));
        }

        public async Task VoidPaymentAsync(Guid paymentId, string reason)
        {
            ValidateId(paymentId, nameof(paymentId));
            ValidateNotEmpty(reason, nameof(reason));

            await ExecuteAsync(async () =>
            {
                var payment = await EnsurePaymentAsync(paymentId);
                ThrowIf(payment.Status == PaymentMethodType.VOID, "Payment đã void");

                payment.Status = PaymentMethodType.VOID;
                payment.PaymentTransactions.Add(new PaymentTransaction
                {
                    TransactionId = GenerateTransactionCode(nameof(PaymentMethodType.VOID)),
                    Status = PaymentMethodType.VOID,
                    Amount = 0,
                    GatewayResponse = $"Voided: {reason}",
                    TransactionDate = DateTime.UtcNow
                });

                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.SaveChangesAsync();

                await InvalidatePaymentCachesAsync(payment.OrderId);
            }, nameof(VoidPaymentAsync));
        }

        #endregion

        #region Calculation

        public async Task<PaymentCalculationDto> CalculateOrderTotalsAsync(CalculateOrderRequest request)
        {
            ValidateNotNull(request, nameof(request));
            ValidateNotEmpty(request.Items, nameof(request.Items));

            return await ExecuteAsync(async () =>
            {
                decimal subtotal = 0;
                foreach (var item in request.Items)
                {
                    ValidateId(item.ProductId, nameof(item.ProductId));
                    ThrowIf(item.Quantity <= 0, "Số lượng phải lớn hơn 0");

                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId)
                                  ?? throw new KeyNotFoundException("Không tìm thấy sản phẩm");

                    var price = product.Prices?.OrderByDescending(p => p.CreatedAt).FirstOrDefault()?.Price ?? 0;
                    subtotal += price * item.Quantity;
                }

                var taxAmount = subtotal * 0.08m;
                var shippingFee = subtotal >= 1_000_000m ? 0 : 30_000m;
                var discount = string.IsNullOrEmpty(request.VoucherCode) ? 0 : subtotal * 0.05m;
                var total = subtotal + taxAmount + shippingFee - discount;

                return new PaymentCalculationDto
                {
                    SubTotal = subtotal,
                    TaxAmount = taxAmount,
                    ShippingFee = shippingFee,
                    DiscountAmount = discount,
                    TotalAmount = total,
                    VoucherApplied = request.VoucherCode,
                    VoucherDiscount = discount,
                    FreeShipping = shippingFee == 0,
                    ShippingMethod = shippingFee == 0 ? "Express" : "Standard"
                };
            }, nameof(CalculateOrderTotalsAsync));
        }

        #endregion

        #region Query

        public async Task<PaginatedResult<PaymentDto>> GetPaymentHistoryAsync(
            PaginationParams pagination,
            PaymentFilters filters)
        {
            ValidateNotNull(pagination, nameof(pagination));
            filters ??= new PaymentFilters();

            return await ExecuteAsync(async () =>
            {
                var cacheKey = CreateCacheKey(
                    CachePrefixHistory,
                    pagination.Page,
                    pagination.PageSize,
                    filters.OrderId,
                    filters.Status,
                    filters.PaymentType,
                    filters.FromDate,
                    filters.ToDate);

                return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
                {
                    var (items, total) = await _unitOfWork.Payments.GetPagedAsync(
                            pagination.Page,
                            pagination.PageSize,
                            BuildPaymentFilter(filters),
                            p => p.CreatedAt,
                            ascending: false);

                    var dtos = _mapper.Map<IEnumerable<PaymentDto>>(items);
                    return new PaginatedResult<PaymentDto>(
                        dtos,
                        pagination.Page,
                        pagination.PageSize,
                        total);
                });
            }, nameof(GetPaymentHistoryAsync));
        }

        #endregion

        #region Helpers

        private async Task<Order> EnsureOrderAsync(Guid orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
                        ?? throw new KeyNotFoundException("Không tìm thấy đơn hàng");

            if (!_currentUser.IsAdmin && _currentUser.StoreId.HasValue && order.StoreId != _currentUser.StoreId)
            {
                throw new InvalidOperationException("Bạn không có quyền thao tác đơn hàng này");
            }

            return order;
        }

        private async Task<Payment> CreatePaymentAsync(
            Order order,
            PaymentMethod method,
            decimal amount)
        {
            var payment = new Payment
            {
                OrderId = order.Id,
                MethodId = method.Id,
                Amount = amount,
                Status = PaymentMethodType.PENDING,
                PaymentMethod = method,
                Order = order
            };

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return payment;
        }

        private async Task<Payment> EnsurePaymentAsync(Guid paymentId)
        {
            return await _unitOfWork.Payments.GetByIdAsync(paymentId)
                   ?? throw new KeyNotFoundException("Không tìm thấy payment");
        }

        private async Task<PaymentMethod> EnsurePaymentMethodAsync(PaymentMethodType type)
        {
            var method = await _unitOfWork.PaymentMethods.GetFirstOrDefaultAsync(
                m => m.Code == type.ToString() && m.IsActive)
                         ?? throw new KeyNotFoundException($"Không tìm thấy phương thức {type}");
            return method;
        }

        private void EnsureOrderIsPayable(Order order)
        {
            if (order.Status is OrderStatus.Cancelled or OrderStatus.Completed)
            {
                throw new InvalidOperationException("Đơn hàng không còn khả dụng để thanh toán");
            }
        }

        private decimal GetPaidAmount(Order order)
        {
            return order.Payments?
                       .Where(p => p.Status == PaymentMethodType.CONFIRMED)
                       .Sum(p => p.Amount) ?? 0m;
        }

        private string GenerateTransactionCode(string suffix)
        {
            return $"{suffix}-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}";
        }

        private async Task InvalidatePaymentCachesAsync(Guid orderId)
        {
            await InvalidateMultipleCachesAsync(
                CreateCacheKey(CachePrefixOrderPayments, orderId),
                CreateCacheKey(CachePrefixPayment, orderId),
                CachePrefixHistory);
        }

        private Expression<Func<Payment, bool>>? BuildPaymentFilter(PaymentFilters filters)
        {
            return payment =>
                (!filters.OrderId.HasValue || payment.OrderId == filters.OrderId.Value) &&
                (!filters.Status.HasValue || payment.Status == filters.Status.Value) &&
                (!filters.PaymentType.HasValue || (payment.PaymentMethod != null &&
                                                   payment.PaymentMethod.Code == filters.PaymentType.Value.ToString())) &&
                (!filters.FromDate.HasValue || payment.CreatedAt >= filters.FromDate.Value) &&
                (!filters.ToDate.HasValue || payment.CreatedAt <= filters.ToDate.Value);
        }

        #endregion
    }
}

