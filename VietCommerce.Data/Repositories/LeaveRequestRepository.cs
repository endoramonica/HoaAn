using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Entities.HRM;
using VietCommerce.Core.Models;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

// LeaveRequestRepository.cs
public class LeaveRequestRepository : GenericRepository<LeaveRequest>, ILeaveRequestRepository
{
    public LeaveRequestRepository(AppDbContext context) : base(context) { }
    
        /// <summary>
        /// Get leave requests with pagination, filtering, and eager loading
        /// </summary>
    public async Task<(IEnumerable<LeaveRequest> Items, int TotalCount)> GetLeaveRequestsPagedAsync(
        PaginationParams pagination,
        LeaveRequestFilters filters)
    {
        var query = _dbSet
            .Include(lr => lr.Employee)
                .ThenInclude(e => e.User) // For EmployeeName
            .Include(lr => lr.ApprovedByEmployee)
                .ThenInclude(e => e.User) // For ApprovedByName
            .AsNoTracking()
            .AsQueryable();

        // Apply filters
        if (filters.EmployeeId.HasValue)
        {
            query = query.Where(lr => lr.EmployeeId == filters.EmployeeId.Value);
        }

        if (filters.Status.HasValue)
        {
            query = query.Where(lr => lr.Status == filters.Status.Value);
        }

        if (filters.LeaveType.HasValue)
        {
            query = query.Where(lr => lr.Type == filters.LeaveType.Value);
        }

        if (filters.StartDate.HasValue)
        {
            query = query.Where(lr => lr.StartDate >= filters.StartDate.Value);
        }

        if (filters.EndDate.HasValue)
        {
            query = query.Where(lr => lr.EndDate <= filters.EndDate.Value);
        }

        if (filters.SubmittedFrom.HasValue)
        {
            query = query.Where(lr => lr.SubmittedAt >= filters.SubmittedFrom.Value);
        }

        if (filters.SubmittedTo.HasValue)
        {
            query = query.Where(lr => lr.SubmittedAt <= filters.SubmittedTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            var searchLower = filters.SearchTerm.ToLower();
            query = query.Where(lr =>
                lr.Reason.ToLower().Contains(searchLower) ||
                lr.Employee.User.Name.ToLower().Contains(searchLower) ||
                (lr.Comments != null && lr.Comments.ToLower().Contains(searchLower)));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = ApplySorting(query, pagination.SortBy, pagination.SortDescending);

        // Apply pagination
        var items = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    /// <summary>
    /// Get leave request by ID with related entities
    /// </summary>
    public async Task<LeaveRequest?> GetLeaveRequestWithDetailsAsync(Guid id)
    {
        return await _dbSet
            .Include(lr => lr.Employee)
                .ThenInclude(e => e.User)
            .Include(lr => lr.ApprovedByEmployee)
                .ThenInclude(e => e.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(lr => lr.Id == id);
    }

    /// <summary>
    /// Check if employee has overlapping leave requests
    /// Excludes cancelled/rejected and optionally a specific request ID
    /// </summary>
    public async Task<bool> HasOverlappingLeaveAsync(
        Guid employeeId,
        DateTime startDate,
        DateTime endDate,
        Guid? excludeId = null)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(lr => lr.EmployeeId == employeeId)
            .Where(lr => lr.Status != LeaveRequestStatus.Rejected) // Only consider pending/approved
            .Where(lr =>
                (lr.StartDate <= endDate && lr.EndDate >= startDate)); // Overlap condition

        if (excludeId.HasValue)
        {
            query = query.Where(lr => lr.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// Get leave requests by employee ID
    /// </summary>
    public async Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId)
    {
        return await _dbSet
            .Include(lr => lr.Employee)
                .ThenInclude(e => e.User)
            .Include(lr => lr.ApprovedByEmployee)
                .ThenInclude(e => e.User)
            .Where(lr => lr.EmployeeId == employeeId)
            .OrderByDescending(lr => lr.SubmittedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Get pending leave requests count for employee
    /// </summary>
    public async Task<int> GetPendingCountByEmployeeAsync(Guid employeeId)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(lr => lr.EmployeeId == employeeId && lr.Status == LeaveRequestStatus.Pending);
    }

    /// <summary>
    /// Get leave requests by status
    /// </summary>
    public async Task<IEnumerable<LeaveRequest>> GetByStatusAsync(LeaveRequestStatus status)
    {
        return await _dbSet
            .Include(lr => lr.Employee)
                .ThenInclude(e => e.User)
            .Include(lr => lr.ApprovedByEmployee)
                .ThenInclude(e => e.User)
            .Where(lr => lr.Status == status)
            .OrderByDescending(lr => lr.SubmittedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Apply dynamic sorting based on field name
    /// </summary>
    private IQueryable<LeaveRequest> ApplySorting(
        IQueryable<LeaveRequest> query,
        string sortBy,
        bool descending)
    {
        return sortBy.ToLower() switch
        {
            "type" => descending
                ? query.OrderByDescending(lr => lr.Type)
                : query.OrderBy(lr => lr.Type),
            "startdate" => descending
                ? query.OrderByDescending(lr => lr.StartDate)
                : query.OrderBy(lr => lr.StartDate),
            "enddate" => descending
                ? query.OrderByDescending(lr => lr.EndDate)
                : query.OrderBy(lr => lr.EndDate),
            "status" => descending
                ? query.OrderByDescending(lr => lr.Status)
                : query.OrderBy(lr => lr.Status),
            "submittedat" => descending
                ? query.OrderByDescending(lr => lr.SubmittedAt)
                : query.OrderBy(lr => lr.SubmittedAt),
            "days" => descending
                ? query.OrderByDescending(lr => lr.Days)
                : query.OrderBy(lr => lr.Days),
            "employeename" => descending
                ? query.OrderByDescending(lr => lr.Employee.User.Name)
                : query.OrderBy(lr => lr.Employee.User.Name),
            _ => descending
                ? query.OrderByDescending(lr => lr.SubmittedAt) // Default sort
                : query.OrderBy(lr => lr.SubmittedAt)
        };
    }
}

