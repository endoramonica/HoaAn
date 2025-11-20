using VietCommerce.Core.Entities.HRM;

namespace VietCommerce.Core.DTOs.HRM
{
    public class ShiftFilters
    {
        public Guid? UserId { get; set; }
        public Guid? StoreId { get; set; }
        public ShiftStatus? Status { get; set; }
        public DateTime? StartTimeFrom { get; set; }
        public DateTime? StartTimeTo { get; set; }
    }
}
