using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Payments;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.DTOs.Payments;
using VietCommerce.Core.Models;

namespace VietCommerce.AdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly VietCommerce.Application.Services.Payments.IPaymentService _paymentService;

        public PaymentController(VietCommerce.Application.Services.Payments.IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPaymentAsync([FromBody] PaymentRequest request)
        {
            var result = await _paymentService.ProcessPaymentAsync(request);
            return Ok(result);
        }

        [HttpPost("process-cash")]
        public async Task<IActionResult> ProcessCashPaymentAsync([FromBody] CashPaymentRequest request)
        {
            var result = await _paymentService.ProcessCashPaymentAsync(request);
            return Ok(result);
        }

        [HttpPost("process-card")]
        public async Task<IActionResult> ProcessCardPaymentAsync([FromBody] CardPaymentRequest request)
        {
            var result = await _paymentService.ProcessCardPaymentAsync(request);
            return Ok(result);
        }

        [HttpPost("refund/{paymentId}")]
        public async Task<IActionResult> ProcessRefundAsync(Guid paymentId, [FromBody] RefundRequest request)
        {
            var result = await _paymentService.ProcessRefundAsync(paymentId, request);
            return Ok(result);
        }

        [HttpPost("void/{paymentId}")]
        public async Task<IActionResult> VoidPaymentAsync(Guid paymentId, [FromBody] string reason)
        {
            await _paymentService.VoidPaymentAsync(paymentId, reason);
            return NoContent();
        }

        [HttpPost("calculate-totals")]
        public async Task<IActionResult> CalculateOrderTotalsAsync([FromBody] CalculateOrderRequest request)
        {
            var result = await _paymentService.CalculateOrderTotalsAsync(request);
            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetPaymentHistoryAsync([FromQuery] PaginationParams pagination, [FromQuery] PaymentFilters filters)
        {
            var result = await _paymentService.GetPaymentHistoryAsync(pagination, filters);
            return Ok(result);
        }
    }
}
