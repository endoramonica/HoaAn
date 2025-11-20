using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Application.Services.Payments;
using VietCommerce.Core.Entities.Payments;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Enums.Payments;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Controllers
{
    [Route("api/v1/payment")]
    [ApiController]
    public class PaymentWebhookController : ControllerBase
    {
        private readonly IVnpayService _vnpayService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentWebhookController> _logger;

        public PaymentWebhookController(IVnpayService vnpayService, IUnitOfWork unitOfWork, ILogger<PaymentWebhookController> logger)
        {
            _vnpayService = vnpayService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet("vnpay/ipn")]
        [AllowAnonymous]
        public async Task<IActionResult> VnpayIpn()
        {
            try
            {
                var data = Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());
                var secureHash = data.GetValueOrDefault("vnp_SecureHash");
                if (string.IsNullOrEmpty(secureHash) || !_vnpayService.ValidateSignature(data, secureHash))
                {
                    _logger.LogWarning("Invalid VNPay signature");
                    return Ok(new { RspCode = "97", Message = "Invalid signature" });
                }

                var responseCode = _vnpayService.GetResponseCode(data);
                var orderNumber = _vnpayService.GetTransactionRef(data);
                var transactionNo = data.GetValueOrDefault("vnp_TransactionNo");

                // Sử dụng phương thức mới lấy order theo orderNumber (string)
                var order = await _unitOfWork.Orders.GetByOrderNumberAsync(orderNumber);
                if (order == null)
                    return Ok(new { RspCode = "01", Message = "Order not found" });

                var payment = await _unitOfWork.Payments.GetByOrderIdAsync(order.Id);
                if (payment == null || payment.Status == PaymentMethodType.CONFIRMED)
                    return Ok(new { RspCode = "02", Message = "Order already confirmed" });

                if (responseCode == "00")
                {
                    payment.Status = PaymentMethodType.CONFIRMED;
                    payment.PaidAt = DateTime.UtcNow;

                    var transaction = new PaymentTransaction
                    {
                        PaymentId = payment.Id,
                        TransactionId = transactionNo,
                        Status = PaymentMethodType.CONFIRMED,
                        Amount = payment.Amount,
                        TransactionDate = DateTime.UtcNow,
                        GatewayResponse = Newtonsoft.Json.JsonConvert.SerializeObject(data)
                    };
                    payment.PaymentTransactions.Add(transaction);

                    order.Status = OrderStatus.Confirmed;
                    //order.IsPaid = true;
                    //order.PaidAt = DateTime.UtcNow;
                    //order.PaymentMethod = "VNPay";
                    //order.TransactionId = transactionNo;

                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Payment confirmed for order {OrderNumber}", orderNumber);
                    return Ok(new { RspCode = "00", Message = "Confirm Success" });
                }
                else
                {
                    payment.Status = PaymentMethodType.FAILED;
                    _logger.LogWarning("Payment failed for order {OrderNumber}, code: {Code}", orderNumber, responseCode);
                    return Ok(new { RspCode = "99", Message = "Payment failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing VNPay IPN");
                return Ok(new { RspCode = "99", Message = "Unknown error" });
            }
        }
    }
}
