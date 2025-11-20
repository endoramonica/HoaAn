using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.DTOs.Payments;

// 2. Request DTOs
public class PaymentRequest
    {
        [Required]
        public Guid OrderId { get; set; }
        [Required]
        public Guid PaymentMethodId { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }
