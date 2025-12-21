using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Application.Services.Payments;
using VietCommerce.Core.DTOs.Payments;
using VietCommerce.Core.Entities.Payments;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Enums.Payments;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Controllers
{
    [Route("api/v1/payment")]
    [ApiController]
    public class PaymentWebhookController : ControllerBase
    {
        private readonly IVnpayService _vnpayService;
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentWebhookController> _logger;

        public PaymentWebhookController(
            IVnpayService vnpayService,
            IPaymentService paymentService,
            IUnitOfWork unitOfWork,
            ILogger<PaymentWebhookController> logger)
        {
            _vnpayService = vnpayService;
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Handles VNPay IPN (Instant Payment Notification) callback.
        /// Validates secure hash signature and processes payment callback.
        /// Requirements: 1.3, 1.4, 1.5, 5.1, 5.5
        /// </summary>
        [HttpGet("vnpay/ipn")]
        [AllowAnonymous]
        public async Task<IActionResult> VnpayIpn()
        {
            try
            {
                // Extract query parameters from request
                var data = Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());

                _logger.LogInformation(
                    "Received VNPay IPN callback. TxnRef: {TxnRef}, ResponseCode: {ResponseCode}",
                    _vnpayService.GetTransactionRef(data),
                    _vnpayService.GetResponseCode(data));

                // Call VnpayService.ValidateSignature()
                var secureHash = data.GetValueOrDefault("vnp_SecureHash");
                if (string.IsNullOrEmpty(secureHash))
                {
                    _logger.LogWarning("VNPay IPN callback missing secure hash");
                    return BadRequest(new { RspCode = "97", Message = "Invalid signature - missing secure hash" });
                }

                if (!_vnpayService.ValidateSignature(data, secureHash))
                {
                    // If invalid: log security alert, return HTTP 400
                    _logger.LogWarning(
                        "SECURITY ALERT: Invalid VNPay signature detected. TxnRef: {TxnRef}, ResponseCode: {ResponseCode}",
                        _vnpayService.GetTransactionRef(data),
                        _vnpayService.GetResponseCode(data));
                    return BadRequest(new { RspCode = "97", Message = "Invalid signature" });
                }

                // If valid: call PaymentService.ProcessVNPayCallbackAsync()
                var result = await _paymentService.ProcessVNPayCallbackAsync(data);

                if (!result.Success)
                {
                    // Handle edge cases (non-existent order, database errors)
                    _logger.LogWarning(
                        "VNPay callback processing failed. OrderId: {OrderId}, Message: {Message}",
                        result.OrderId,
                        result.Message);
                    return BadRequest(new { RspCode = "99", Message = result.Message });
                }

                // Return HTTP 200 "OK" for successful processing
                _logger.LogInformation(
                    "VNPay callback processed successfully. OrderId: {OrderId}, ResponseCode: {ResponseCode}",
                    result.OrderId,
                    result.ResponseCode);
                return Ok(new { RspCode = "00", Message = "Confirm Success" });
            }
            catch (Exception ex)
            {
                // Handle edge cases (database errors)
                _logger.LogError(ex, "Error processing VNPay IPN callback");
                return BadRequest(new { RspCode = "99", Message = "Unknown error" });
            }
        }

        /// <summary>
        /// Retrieves the current payment status for an order.
        /// Extracts orderId from route parameter and returns PaymentStatusDto with current status.
        /// Handles non-existent orders gracefully.
        /// Requirements: 4.4, 4.5
        /// </summary>
        [HttpGet("status/{orderId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaymentStatus(string orderId)
        {
            try
            {
                _logger.LogInformation("Retrieving payment status for order: {OrderId}", orderId);

                // Extract orderId from route parameter
                if (string.IsNullOrWhiteSpace(orderId))
                {
                    _logger.LogWarning("Payment status request with empty order ID");
                    return BadRequest(new { Message = "Order ID is required" });
                }

                // Call PaymentService.GetPaymentStatusAsync()
                var status = await _paymentService.GetPaymentStatusAsync(orderId);

                // Handle non-existent orders gracefully
                if (status == null)
                {
                    _logger.LogWarning("Payment not found for order: {OrderId}", orderId);
                    return NotFound(new { Message = "Payment not found" });
                }

                // Return PaymentStatusDto with current status
                _logger.LogInformation(
                    "Payment status retrieved successfully. OrderId: {OrderId}, Status: {Status}",
                    orderId, status.Status);
                return Ok(new { Success = true, Data = status, Message = "Payment status retrieved" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment status for order: {OrderId}", orderId);
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }
    }
}
