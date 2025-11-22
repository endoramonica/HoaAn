using VietCommerce.Core.Entities.HRM;

namespace VietCommerce.Core.DTOs.HRM
{
    public class WorkScheduleFilters
    {
        public string? ShiftName { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public WorkScheduleType? Type { get; set; }
        public WorkScheduleStatus? Status { get; set; }
        public string? Search { get; set; }
    }
}
