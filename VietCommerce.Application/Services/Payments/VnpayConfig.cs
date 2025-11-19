// File: VietCommerce.Application/Services/Payments/VnpayService.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace VietCommerce.Application.Services.Payments
{
    public class VnpayConfig
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
    }

    public interface IVnpayService
    {
        string CreatePaymentUrl(string orderId, decimal amount, string ipAddress);
        bool ValidateSignature(IDictionary<string, string> data, string secureHash);
        string GetResponseCode(IDictionary<string, string> data);
        string GetTransactionRef(IDictionary<string, string> data);
    }

    public class VnpayService : IVnpayService
    {
        private readonly VnpayConfig _config;
        private readonly IHttpContextAccessor _httpContext;

        public VnpayService(IOptions<VnpayConfig> config, IHttpContextAccessor httpContext)
        {
            _config = config.Value;
            _httpContext = httpContext;
        }

        public string CreatePaymentUrl(string orderId, decimal amount, string ipAddress)
        {
            var vnpay = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", _config.TmnCode },
                { "vnp_Amount", ((long)(amount * 100)).ToString() },
                { "vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", ipAddress ?? "127.0.0.1" },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", $"Thanh toan don hang {orderId}" },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", _config.ReturnUrl },
                { "vnp_TxnRef", orderId }
            };

            var signData = string.Join("&", vnpay.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            var secureHash = HmacSha512(_config.HashSecret, signData);
            vnpay.Add("vnp_SecureHash", secureHash);

            var paymentUrl = _config.PaymentUrl + "?" + string.Join("&", vnpay.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
            return paymentUrl;
        }

        public bool ValidateSignature(IDictionary<string, string> data, string secureHash)
        {
            var signData = string.Join("&", data
                .Where(kvp => kvp.Key != "vnp_SecureHash")
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

            var calcHash = HmacSha512(_config.HashSecret, signData);
            return calcHash.Equals(secureHash, StringComparison.OrdinalIgnoreCase);
        }

        public string GetResponseCode(IDictionary<string, string> data)
        {
            data.TryGetValue("vnp_ResponseCode", out var value);
            return value;
        }

        public string GetTransactionRef(IDictionary<string, string> data)
        {
            data.TryGetValue("vnp_TxnRef", out var value);
            return value;
        }


        private string HmacSha512(string key, string input)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(input);
            using var hmac = new HMACSHA512(keyBytes);
            var hash = hmac.ComputeHash(inputBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}