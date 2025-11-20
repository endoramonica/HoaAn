using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    public interface IWorkScheduleService
    {
        /// <summary>
        /// Get paginated list of work schedules with filtering
        /// </summary>
        Task<PaginatedResult<WorkScheduleDto>> GetWorkSchedulesAsync(
            PaginationParams pagination,
            WorkScheduleFilters filters);

        /// <summary>
        /// Get work schedule by ID
        /// </summary>
        Task<WorkScheduleDto> GetWorkScheduleByIdAsync(Guid id);

        /// <summary>
        /// Create new work schedule
        /// </summary>
        Task<WorkScheduleDto> CreateWorkScheduleAsync(CreateWorkScheduleRequest request);

        /// <summary>
        /// Update existing work schedule
        /// </summary>
        Task<WorkScheduleDto> UpdateWorkScheduleAsync(Guid id, UpdateWorkScheduleRequest request);

        /// <summary>
        /// Delete work schedule
        /// </summary>
        Task DeleteWorkScheduleAsync(Guid id);
    }
}
