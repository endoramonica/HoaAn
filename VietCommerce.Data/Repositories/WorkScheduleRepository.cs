using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Entities.HRM;
using VietCommerce.Core.Models;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

/// <summary>
/// Repository implementation for WorkSchedule entity
/// Handles data access with filtering, pagination, conflict detection, and eager loading
/// </summary>
public class WorkScheduleRepository : GenericRepository<WorkSchedule>, IWorkScheduleRepository
{
    public WorkScheduleRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get paginated work schedules with advanced filtering and sorting
    /// Includes Employee navigation property for detailed information
    /// </summary>
    /// <param name="pagination">Pagination parameters (page, pageSize, sorting)</param>
    /// <param name="filters">Filter criteria (employee, date range, type, status, search)</param>
    /// <returns>Tuple of filtered items and total count</returns>
    public async Task<(List<WorkSchedule> items, int totalCount)> GetWorkSchedulesPagedAsync(
        PaginationParams pagination,
        WorkScheduleFilters filters)
    {
        // Start with base query including Employee navigation
        var query = _context.WorkSchedules
            .Include(ws => ws.Employee)
                .ThenInclude(e => e.User) // Include User for full employee details
            .AsQueryable();

        // ═══════════════════════════════════════════════════════════════
        // APPLY FILTERS
        // ═══════════════════════════════════════════════════════════════

        // Filter by EmployeeId
        if (filters.EmployeeId.HasValue)
        {
            query = query.Where(ws => ws.EmployeeId == filters.EmployeeId.Value);
        }

        // Filter by StoreId (through Employee relationship)
        if (filters.StoreId.HasValue)
        {
            query = query.Where(ws => ws.Employee.StoreId == filters.StoreId.Value);
        }

        // Filter by Date Range
        if (filters.DateFrom.HasValue)
        {
            query = query.Where(ws => ws.Date >= filters.DateFrom.Value);
        }

        if (filters.DateTo.HasValue)
        {
            query = query.Where(ws => ws.Date <= filters.DateTo.Value);
        }

        // Filter by WorkScheduleType
        if (filters.Type.HasValue)
        {
            query = query.Where(ws => ws.Type == filters.Type.Value);
        }

        // Filter by WorkScheduleStatus
        if (filters.Status.HasValue)
        {
            query = query.Where(ws => ws.Status == filters.Status.Value);
        }

        // Filter by ShiftName
        if (!string.IsNullOrWhiteSpace(filters.ShiftName))
        {
            query = query.Where(ws => ws.ShiftName != null &&
                                     ws.ShiftName.Contains(filters.ShiftName));
        }

        // Search across multiple fields
        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var searchTerm = filters.Search.ToLower().Trim();
            query = query.Where(ws =>
                (ws.ShiftName != null && ws.ShiftName.ToLower().Contains(searchTerm)) ||
                (ws.Notes != null && ws.Notes.ToLower().Contains(searchTerm)) ||
                ws.Employee.Code.ToLower().Contains(searchTerm) ||
                ws.Employee.User.Name.ToLower().Contains(searchTerm) ||
                ws.Employee.User.Email.ToLower().Contains(searchTerm)
            );
        }

        // ═══════════════════════════════════════════════════════════════
        // GET TOTAL COUNT (before pagination)
        // ═══════════════════════════════════════════════════════════════
        var totalCount = await query.CountAsync();

        // ═══════════════════════════════════════════════════════════════
        // APPLY SORTING
        // ═══════════════════════════════════════════════════════════════
        query = ApplySorting(query, pagination.SortBy, pagination.SortDescending);

        // ═══════════════════════════════════════════════════════════════
        // APPLY PAGINATION
        // ═══════════════════════════════════════════════════════════════
        var items = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .AsNoTracking() // Read-only for better performance
            .ToListAsync();

        return (items, totalCount);
    }

    /// <summary>
    /// Get single work schedule with full details (Employee, User)
    /// </summary>
    /// <param name="id">WorkSchedule ID</param>
    /// <returns>WorkSchedule with navigation properties or null</returns>
    public async Task<WorkSchedule?> GetWorkScheduleWithDetailsAsync(Guid id)
    {
        return await _context.WorkSchedules
            .Include(ws => ws.Employee)
                .ThenInclude(e => e.User)
            .Include(ws => ws.Employee)
                .ThenInclude(e => e.Store) // Include Store if needed
            .AsNoTracking()
            .FirstOrDefaultAsync(ws => ws.Id == id);
    }

    /// <summary>
    /// Check if a schedule conflicts with existing schedules for an employee
    /// Conflict occurs when time ranges overlap on the same date
    /// 
    /// CONFLICT LOGIC:
    /// - Same employee, same date
    /// - Time ranges overlap (partial or full)
    /// - Excludes the schedule being updated (via excludeScheduleId)
    /// - Only checks active schedules (Status != Missed)
    /// </summary>
    /// <param name="employeeId">Employee to check</param>
    /// <param name="date">Date to check</param>
    /// <param name="startTime">Start time of new/updated schedule</param>
    /// <param name="endTime">End time of new/updated schedule</param>
    /// <param name="excludeScheduleId">Schedule ID to exclude (for updates)</param>
    /// <returns>True if conflict exists, false otherwise</returns>
    public async Task<bool> HasScheduleConflictAsync(
        Guid employeeId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeScheduleId = null)
    {
        // Query existing schedules for the employee on the same date
        var query = _context.WorkSchedules
            .Where(ws => ws.EmployeeId == employeeId)
            .Where(ws => ws.Date.Date == date.Date)
            .Where(ws => ws.Status != WorkScheduleStatus.Missed); // Ignore missed schedules

        // Exclude the schedule being updated
        if (excludeScheduleId.HasValue)
        {
            query = query.Where(ws => ws.Id != excludeScheduleId.Value);
        }

        // Check for time overlap
        // Overlap occurs if:
        // (New.Start < Existing.End) AND (New.End > Existing.Start)
        var hasConflict = await query
            .AnyAsync(ws =>
                startTime < ws.EndTime &&
                endTime > ws.StartTime
            );

        return hasConflict;
    }

    // ═══════════════════════════════════════════════════════════════
    // PRIVATE HELPER METHODS
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Apply dynamic sorting based on field name
    /// Supports sorting by Date, StartTime, EndTime, Status, Type, ShiftName, EmployeeCode, EmployeeName
    /// </summary>
    private IQueryable<WorkSchedule> ApplySorting(
        IQueryable<WorkSchedule> query,
        string sortBy,
        bool descending)
    {
        // Normalize sort field
        var sortField = sortBy?.Trim().ToLower() ?? "date";

        // Apply sorting based on field
        query = sortField switch
        {
            "date" => descending
                ? query.OrderByDescending(ws => ws.Date)
                : query.OrderBy(ws => ws.Date),

            "starttime" => descending
                ? query.OrderByDescending(ws => ws.StartTime)
                : query.OrderBy(ws => ws.StartTime),

            "endtime" => descending
                ? query.OrderByDescending(ws => ws.EndTime)
                : query.OrderBy(ws => ws.EndTime),

            "status" => descending
                ? query.OrderByDescending(ws => ws.Status)
                : query.OrderBy(ws => ws.Status),

            "type" => descending
                ? query.OrderByDescending(ws => ws.Type)
                : query.OrderBy(ws => ws.Type),

            "shiftname" => descending
                ? query.OrderByDescending(ws => ws.ShiftName)
                : query.OrderBy(ws => ws.ShiftName),

            "employeecode" => descending
                ? query.OrderByDescending(ws => ws.Employee.Code)
                : query.OrderBy(ws => ws.Employee.Code),

            "employeename" => descending
                ? query.OrderByDescending(ws => ws.Employee.User.Name)
                : query.OrderBy(ws => ws.Employee.User.Name),

            "createdat" => descending
                ? query.OrderByDescending(ws => ws.CreatedAt)
                : query.OrderBy(ws => ws.CreatedAt),

            // Default: sort by Date descending
            _ => query.OrderByDescending(ws => ws.Date)
        };

        return query;
    }
}