using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Entities.HRM;
using VietCommerce.Core.Models;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface ILeaveRequestRepository : IGenericRepository<LeaveRequest>
    {
        /// <summary>
        /// Get leave requests with pagination, filtering, and includes (Employee, ApprovedByEmployee)
        /// </summary>
        Task<(IEnumerable<LeaveRequest> Items, int TotalCount)> GetLeaveRequestsPagedAsync(
            PaginationParams pagination,
            LeaveRequestFilters filters);

        /// <summary>
        /// Get leave request by ID with related entities (Employee.User, ApprovedByEmployee.User)
        /// </summary>
        Task<LeaveRequest?> GetLeaveRequestWithDetailsAsync(Guid id);

        /// <summary>
        /// Check if employee has overlapping leave requests in the given date range
        /// </summary>
        Task<bool> HasOverlappingLeaveAsync(
            Guid employeeId,
            DateTime startDate,
            DateTime endDate,
            Guid? excludeId = null);

        /// <summary>
        /// Get leave requests by employee ID
        /// </summary>
        Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId);

        /// <summary>
        /// Get pending leave requests count for employee
        /// </summary>
        Task<int> GetPendingCountByEmployeeAsync(Guid employeeId);

        /// <summary>
        /// Get leave requests by status
        /// </summary>
        Task<IEnumerable<LeaveRequest>> GetByStatusAsync(LeaveRequestStatus status);
    }
}
