using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.DTOs.Payments;

public class CardPaymentRequest : PaymentRequest
    {
        [Required]
        public string CardToken { get; set; } = string.Empty; // Token from Gateway
        public string CardHolderName { get; set; } = string.Empty;
    }
