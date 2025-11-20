using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    public interface IShiftService
    {
        /// <summary>
        /// Get paginated list of shifts with filtering
        /// </summary>
        Task<PaginatedResult<ShiftDto>> GetShiftsAsync(
            PaginationParams pagination,
            ShiftFilters filters);

        /// <summary>
        /// Get shift by ID
        /// </summary>
        Task<ShiftDto> GetShiftByIdAsync(Guid id);

        /// <summary>
        /// Get current active shift for user
        /// </summary>
        Task<ShiftDto?> GetCurrentShiftAsync(Guid userId);

        /// <summary>
        /// Open a new shift
        /// </summary>
        Task<ShiftDto> OpenShiftAsync(OpenShiftRequest request);

        /// <summary>
        /// Close an active shift
        /// </summary>
        Task<ShiftDto> CloseShiftAsync(Guid id, CloseShiftRequest request);

        /// <summary>
        /// Update shift information
        /// </summary>
        Task<ShiftDto> UpdateShiftAsync(Guid id, UpdateShiftRequest request);
    }
}
