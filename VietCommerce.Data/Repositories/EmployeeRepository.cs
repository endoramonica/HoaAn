using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Entities.HRM;
using VietCommerce.Core.Models;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Get paginated employees with filters
    /// </summary>
    public async Task<(List<Employee> Items, int TotalCount)> GetEmployeesPagedAsync(
        PaginationParams pagination,
        EmployeeFilters filters)
    {
        var query = _context.Employees
            .Include(e => e.User)
            .Include(e => e.Manager)
                .ThenInclude(m => m!.User)
            .Include(e => e.Store)
            .AsQueryable();

        // ✅ Filter by UserId (for hrm.view_own permission)
        if (filters.UserId.HasValue)
        {
            query = query.Where(e => e.Id == filters.UserId.Value);
        }

        // Filter by Search (tìm trong User.Name, User.Email, Code, User.Phone)
        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.ToLower();
            query = query.Where(e =>
                e.User.Name!.ToLower().Contains(search) ||
                e.User.Email.ToLower().Contains(search) ||
                e.Code.ToLower().Contains(search) ||
                (e.User.Phone != null && e.User.Phone.ToLower().Contains(search))
            );
        }

        // Filter by Department
        if (!string.IsNullOrWhiteSpace(filters.Department))
        {
            query = query.Where(e => e.Department == filters.Department);
        }

        // Filter by Status
        if (filters.Status.HasValue)
        {
            query = query.Where(e => e.Status == filters.Status.Value);
        }

        // Filter by ManagerId
        if (filters.ManagerId.HasValue)
        {
            query = query.Where(e => e.ManagerId == filters.ManagerId.Value);
        }

        // Filter by StoreId
        if (filters.StoreId.HasValue)
        {
            query = query.Where(e => e.StoreId == filters.StoreId.Value);
        }

        // Filter by HireDate range
        if (filters.HireDateFrom.HasValue)
        {
            query = query.Where(e => e.HireDate >= filters.HireDateFrom.Value);
        }

        if (filters.HireDateTo.HasValue)
        {
            query = query.Where(e => e.HireDate <= filters.HireDateTo.Value);
        }

        // Filter by Position
        if (!string.IsNullOrWhiteSpace(filters.Position))
        {
            query = query.Where(e => e.Position == filters.Position);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = ApplySorting(query, pagination);

        // Apply pagination
        var items = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    /// <summary>
    /// Get employee with all related data
    /// </summary>
    public async Task<Employee?> GetEmployeeWithDetailsAsync(Guid userId)
    {
        return await _context.Employees
            .Include(e => e.User)
            .Include(e => e.Manager)
                .ThenInclude(m => m!.User)
            .Include(e => e.Store)
            .FirstOrDefaultAsync(e => e.Id == userId);
    }

    /// <summary>
    /// Check if employee code exists
    /// </summary>
    public async Task<bool> IsEmployeeCodeExistsAsync(string code, Guid? excludeUserId = null)
    {
        var query = _context.Employees.Where(e => e.Code == code);

        if (excludeUserId.HasValue)
        {
            query = query.Where(e => e.Id != excludeUserId.Value);
        }

        return await query.AnyAsync();
    }

    /// <summary>
    /// Check if employee has any leave requests
    /// </summary>
    public async Task<bool> HasLeaveRequestsAsync(Guid userId)
    {
        return await _context.Set<LeaveRequest>()
            .AnyAsync(lr => lr.EmployeeId == userId);
    }

    /// <summary>
    /// Check if employee has any subordinates
    /// </summary>
    public async Task<bool> HasSubordinatesAsync(Guid managerId)
    {
        return await _context.Employees
            .AnyAsync(e => e.ManagerId == managerId);
    }

    /// <summary>
    /// Get employees by department
    /// </summary>
    public async Task<List<Employee>> GetEmployeesByDepartmentAsync(string department)
    {
        return await _context.Employees
            .Include(e => e.User)
            .Include(e => e.Manager)
                .ThenInclude(m => m!.User)
            .Where(e => e.Department == department)
            .OrderBy(e => e.User.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Get employees by manager
    /// </summary>
    public async Task<List<Employee>> GetEmployeesByManagerAsync(Guid managerId)
    {
        return await _context.Employees
            .Include(e => e.User)
            .Include(e => e.Store)
            .Where(e => e.ManagerId == managerId)
            .OrderBy(e => e.User.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Get employees by store
    /// </summary>
    public async Task<List<Employee>> GetEmployeesByStoreAsync(Guid storeId)
    {
        return await _context.Employees
            .Include(e => e.User)
            .Include(e => e.Manager)
                .ThenInclude(m => m!.User)
            .Where(e => e.StoreId == storeId)
            .OrderBy(e => e.User.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Get employees by status
    /// </summary>
    public async Task<List<Employee>> GetEmployeesByStatusAsync(EmployeeStatus status)
    {
        return await _context.Employees
            .Include(e => e.User)
            .Include(e => e.Manager)
                .ThenInclude(m => m!.User)
            .Where(e => e.Status == status)
            .OrderBy(e => e.User.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Search employees by keyword
    /// </summary>
    public async Task<List<Employee>> SearchEmployeesAsync(string keyword)
    {
        var search = keyword.ToLower();

        return await _context.Employees
            .Include(e => e.User)
            .Include(e => e.Manager)
                .ThenInclude(m => m!.User)
            .Include(e => e.Store)
            .Where(e =>
                e.User.Name!.ToLower().Contains(search) ||
                e.User.Email.ToLower().Contains(search) ||
                e.Code.ToLower().Contains(search) ||
                (e.User.Phone != null && e.User.Phone.ToLower().Contains(search)) ||
                e.Department.ToLower().Contains(search) ||
                e.Position.ToLower().Contains(search)
            )
            .OrderBy(e => e.User.Name)
            .ToListAsync();
    }

    #region Private Helper Methods

    /// <summary>
    /// Apply sorting to query based on pagination parameters
    /// </summary>
    private IQueryable<Employee> ApplySorting(IQueryable<Employee> query, PaginationParams pagination)
    {
        return pagination.SortBy?.ToLower() switch
        {
            "name" => pagination.SortDescending
                ? query.OrderByDescending(e => e.User.Name)
                : query.OrderBy(e => e.User.Name),
            "email" => pagination.SortDescending
                ? query.OrderByDescending(e => e.User.Email)
                : query.OrderBy(e => e.User.Email),
            "code" => pagination.SortDescending
                ? query.OrderByDescending(e => e.Code)
                : query.OrderBy(e => e.Code),
            "department" => pagination.SortDescending
                ? query.OrderByDescending(e => e.Department)
                : query.OrderBy(e => e.Department),
            "position" => pagination.SortDescending
                ? query.OrderByDescending(e => e.Position)
                : query.OrderBy(e => e.Position),
            "hiredate" => pagination.SortDescending
                ? query.OrderByDescending(e => e.HireDate)
                : query.OrderBy(e => e.HireDate),
            "status" => pagination.SortDescending
                ? query.OrderByDescending(e => e.Status)
                : query.OrderBy(e => e.Status),
            "salary" => pagination.SortDescending
                ? query.OrderByDescending(e => e.Salary)
                : query.OrderBy(e => e.Salary),
            _ => query.OrderBy(e => e.User.Name) // Default sort by Name
        };
    }

    #endregion
}