using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Entities.HRM;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    public interface ILeaveRequestService
    {
        /// <summary>
        /// Get paginated list of leave requests with filtering
        /// </summary>
        Task<PaginatedResult<LeaveRequestDto>> GetLeaveRequestsAsync(
            PaginationParams pagination,
            LeaveRequestFilters filters);

        /// <summary>
        /// Get leave request by ID
        /// </summary>
        Task<LeaveRequestDto> GetLeaveRequestByIdAsync(Guid id);

        /// <summary>
        /// Create new leave request
        /// </summary>
        Task<LeaveRequestDto> CreateLeaveRequestAsync(CreateLeaveRequestRequest request);

        /// <summary>
        /// Update leave request status
        /// </summary>
        Task<LeaveRequestDto> UpdateLeaveRequestStatusAsync(
            Guid id,
            LeaveRequestStatus status,
            string? comments = null);
    }
}
