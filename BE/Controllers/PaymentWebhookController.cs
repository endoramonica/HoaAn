using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Payments;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Payments;
using VietCommerce.Core.Models;

namespace VietCommerce.AdminAPI.Controllers
{
    /// <summary>
    /// Payment Webhook Controller - Handles payment gateway callbacks (VNPay IPN, etc.)
    /// </summary>
    [ApiController]
    [Route("api/v1/payment")]
    [AllowAnonymous] // Payment webhooks don't require authentication
    public class PaymentWebhookController : ControllerBase
    {
        private readonly IVnpayService _vnpayService;
        private readonly VietCommerce.Application.Services.Payments.IPaymentService _paymentService;
        private readonly ILogger<PaymentWebhookController> _logger;

        public PaymentWebhookController(
            IVnpayService vnpayService,
            VietCommerce.Application.Services.Payments.IPaymentService paymentService,
            ILogger<PaymentWebhookController> logger)
        {
            _vnpayService = vnpayService;
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// VNPay IPN (Instant Payment Notification) Callback
        /// Receives payment result from VNPay gateway
        /// </summary>
        /// <returns>HTTP 200 OK if processed successfully</returns>
        [HttpGet("vnpay/ipn")]
        public async Task<IActionResult> VnpayIpn()
        {
            try
            {
                _logger.LogInformation("📥 VNPay IPN callback received");

                // Extract query parameters from request
                var callbackData = Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());

                if (!callbackData.Any())
                {
                    _logger.LogWarning("⚠️ VNPay IPN callback received with no parameters");
                    return BadRequest("No callback data");
                }

                _logger.LogInformation("🔍 Validating VNPay signature...");

                // Extract secure hash
                if (!callbackData.TryGetValue("vnp_SecureHash", out var secureHash))
                {
                    _logger.LogWarning("⚠️ VNPay callback missing vnp_SecureHash");
                    return BadRequest("Missing secure hash");
                }

                // Validate signature
                if (!_vnpayService.ValidateSignature(callbackData, secureHash))
                {
                    _logger.LogError("🚫 SECURITY ALERT: VNPay callback signature validation failed");
                    return BadRequest("Invalid signature");
                }

                _logger.LogInformation("✅ VNPay signature validated");

                // Process the callback
                var result = await _paymentService.ProcessVNPayCallbackAsync(callbackData);

                if (result.Success)
                {
                    _logger.LogInformation("✅ VNPay callback processed successfully. OrderId: {OrderId}, ResponseCode: {ResponseCode}",
                        result.OrderId, result.ResponseCode);
                    return Ok("OK");
                }
                else
                {
                    _logger.LogWarning("⚠️ VNPay callback processing failed: {Message}", result.Message);
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error processing VNPay IPN callback");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get payment status for an order
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>Payment status information</returns>
        [HttpGet("status/{orderId}")]
        public async Task<IActionResult> GetPaymentStatus(string orderId)
        {
            try
            {
                _logger.LogInformation("🔍 Getting payment status for order: {OrderId}", orderId);

                if (string.IsNullOrWhiteSpace(orderId))
                {
                    return BadRequest("Order ID is required");
                }

                var status = await _paymentService.GetPaymentStatusAsync(orderId);

                if (status == null)
                {
                    _logger.LogWarning("⚠️ Payment not found for order: {OrderId}", orderId);
                    return NotFound("Payment not found");
                }

                _logger.LogInformation("✅ Payment status retrieved for order: {OrderId}, Status: {Status}",
                    orderId, status.Status);

                return Ok(ApiResponse<PaymentStatusDto>.SuccessResponse(status, "Payment status retrieved"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving payment status for order: {OrderId}", orderId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
