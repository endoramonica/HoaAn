using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Entities.HRM;
using VietCommerce.Core.Models;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface IShiftRepository : IGenericRepository<Shift> 
    {
        /// <summary>
        /// Lấy ca làm việc hiện tại (Status = Open) của 1 user
        /// </summary>
        Task<Shift?> GetCurrentShiftAsync(Guid userId);

        /// <summary>
        /// Lấy đầy đủ thông tin shift, bao gồm navigation properties
        /// </summary>
        Task<Shift?> GetShiftWithDetailsAsync(Guid shiftId);

        /// <summary>
        /// Lấy danh sách shifts có phân trang + filter
        /// </summary>
        Task<(List<Shift> Items, int TotalCount)> GetShiftsPagedAsync(
            PaginationParams pagination,
            ShiftFilters filters);
    }
}
