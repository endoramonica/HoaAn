// File: VietCommerce.Application/Services/Payments/VnpayService.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
        decimal GetAmount(IDictionary<string, string> data);
        DateTime GetTransactionDate(IDictionary<string, string> data);
        decimal ConvertVndAmount(long vndAmountInHundreds);
    }

    public class VnpayService : IVnpayService
    {
        private readonly VnpayConfig _config;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogger<VnpayService> _logger;

        public VnpayService(
            IOptions<VnpayConfig> config,
            IHttpContextAccessor httpContext,
            ILogger<VnpayService> logger)
        {
            _config = config.Value;
            _httpContext = httpContext;
            _logger = logger;
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
            var isValid = calcHash.Equals(secureHash, StringComparison.OrdinalIgnoreCase);

            // Log signature validation attempt
            if (isValid)
            {
                _logger.LogInformation(
                    "VNPay signature validation successful. TxnRef: {TxnRef}, ResponseCode: {ResponseCode}",
                    GetTransactionRef(data),
                    GetResponseCode(data));
            }
            else
            {
                _logger.LogWarning(
                    "VNPay signature validation failed. Expected: {ExpectedHash}, Got: {ReceivedHash}, TxnRef: {TxnRef}",
                    calcHash,
                    secureHash,
                    GetTransactionRef(data));
            }

            return isValid;
        }

        public string GetResponseCode(IDictionary<string, string> data)
        {
            data.TryGetValue("vnp_ResponseCode", out var value);
            return value ?? string.Empty;
        }

        public string GetTransactionRef(IDictionary<string, string> data)
        {
            data.TryGetValue("vnp_TxnRef", out var value);
            return value ?? string.Empty;
        }

        /// <summary>
        /// Extracts the payment amount from VNPay callback data.
        /// VNPay returns amount in VND multiplied by 100, so we need to convert it to decimal.
        /// </summary>
        /// <param name="data">VNPay callback data dictionary</param>
        /// <returns>Amount in VND as decimal</returns>
        public decimal GetAmount(IDictionary<string, string> data)
        {
            if (data.TryGetValue("vnp_Amount", out var amountStr) && long.TryParse(amountStr, out var amountInHundreds))
            {
                var amount = ConvertVndAmount(amountInHundreds);
                _logger.LogInformation(
                    "Extracted amount from VNPay callback. Amount (in 100s): {AmountInHundreds}, Converted amount: {Amount} VND",
                    amountInHundreds,
                    amount);
                return amount;
            }

            _logger.LogWarning("Failed to extract amount from VNPay callback data");
            return 0m;
        }

        /// <summary>
        /// Extracts the transaction date from VNPay callback data.
        /// VNPay returns date in format: yyyyMMddHHmmss
        /// </summary>
        /// <param name="data">VNPay callback data dictionary</param>
        /// <returns>Transaction date as DateTime in UTC</returns>
        public DateTime GetTransactionDate(IDictionary<string, string> data)
        {
            if (data.TryGetValue("vnp_TransactionDate", out var dateStr) &&
                DateTime.TryParseExact(dateStr, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.AssumeUniversal, out var transactionDate))
            {
                _logger.LogInformation(
                    "Extracted transaction date from VNPay callback. Date: {TransactionDate}",
                    transactionDate);
                return transactionDate;
            }

            _logger.LogWarning("Failed to extract transaction date from VNPay callback data. DateStr: {DateStr}", dateStr ?? "null");
            return DateTime.UtcNow;
        }

        /// <summary>
        /// Converts VND amount from VNPay format (in 100s) to decimal.
        /// /// VNPay sends amount as: actual_amount * 100
        /// /// For example: 1,000,000 VND is sent as 100000000
        /// /// </summary>
        /// <param name="vndAmountInHundreds">Amount in VND multiplied by 100</param>
        /// /// <returns>Amount in VND as decimal</returns>
        public decimal ConvertVndAmount(long vndAmountInHundreds)
        {
            var amount = (decimal)vndAmountInHundreds / 100m;
            _logger.LogDebug(
                "Converted VND amount. Input (in 100s): {InputAmount}, Output: {OutputAmount} VND",
                vndAmountInHundreds,
                amount);
            return amount;
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