using VietCommerce.Core.Enums.Payments;
namespace VietCommerce.Core.DTOs.Payments;

// 3. Filters
public class PaymentFilters
    {
        public Guid? OrderId { get; set; }
        public PaymentMethodType? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
    }
