using System.ComponentModel.DataAnnotations;
namespace VietCommerce.Core.DTOs.Payments;

public class RefundRequest
    {
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Reason { get; set; } = string.Empty;
    }
