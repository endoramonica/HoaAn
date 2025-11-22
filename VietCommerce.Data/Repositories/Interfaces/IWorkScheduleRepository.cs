using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Entities.HRM;
using VietCommerce.Core.Models;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface IWorkScheduleRepository : IGenericRepository<WorkSchedule>
    {
        Task<(List<WorkSchedule> items, int totalCount)> GetWorkSchedulesPagedAsync(
        PaginationParams pagination,
        WorkScheduleFilters filters);

        Task<WorkSchedule?> GetWorkScheduleWithDetailsAsync(Guid id);

        Task<bool> HasScheduleConflictAsync(
            Guid employeeId,
            DateTime date,
            TimeSpan startTime,
            TimeSpan endTime,
            Guid? excludeScheduleId = null);
    }
}
