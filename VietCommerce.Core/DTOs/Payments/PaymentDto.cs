using VietCommerce.Core.Enums.Payments;
namespace VietCommerce.Core.DTOs.Payments;

// 1. Common DTOs
public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid? MethodId { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public PaymentMethodType Status { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
