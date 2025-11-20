using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.HRM;
using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Models;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

public class ShiftRepository : GenericRepository<Shift>, IShiftRepository
{
    private readonly AppDbContext _context;

    public ShiftRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy ca làm việc hiện tại của nhân viên (Status = Open)
    /// </summary>
    public async Task<Shift?> GetCurrentShiftAsync(Guid userId)
    {
        return await _context.Shifts
            .Where(x =>
                x.UserId == userId &&
                x.Status == ShiftStatus.Open &&
                x.EndTime == null 
               // !x.IsDeleted
                )
            .OrderByDescending(x => x.StartTime)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Lấy shift theo ID kèm navigation Staff & Store
    /// </summary>
    public async Task<Shift?> GetShiftWithDetailsAsync(Guid shiftId)
    {
        return await _context.Shifts
            .Include(x => x.Staff)
            .Include(x => x.Store)
            .Where(x => x.Id == shiftId
            //&& !x.IsDeleted
            )
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Danh sách shifts có phân trang + filter
    /// </summary>
    public async Task<(List<Shift> Items, int TotalCount)> GetShiftsPagedAsync(
        PaginationParams pagination,
        ShiftFilters filters)
    {
        var query = _context.Shifts
            .Include(x => x.Staff)
            .Include(x => x.Store)
            //.Where(x => !x.IsDeleted)
            .AsQueryable();

        // -----------------------
        // 🔍 Apply Filters
        // -----------------------

        if (filters.UserId.HasValue && filters.UserId.Value != Guid.Empty)
            query = query.Where(x => x.UserId == filters.UserId.Value);

        if (filters.StoreId.HasValue && filters.StoreId.Value != Guid.Empty)
            query = query.Where(x => x.StoreId == filters.StoreId.Value);

        if (filters.Status.HasValue)
            query = query.Where(x => x.Status == filters.Status.Value);

        if (filters.StartTimeFrom.HasValue)
            query = query.Where(x => x.StartTime >= filters.StartTimeFrom.Value);

        if (filters.StartTimeTo.HasValue)
            query = query.Where(x => x.StartTime <= filters.StartTimeTo.Value);

        // -----------------------
        // 🔽 Sorting
        // -----------------------

        if (!string.IsNullOrWhiteSpace(pagination.SortBy))
        {
            query = pagination.SortDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, pagination.SortBy))
                : query.OrderBy(e => EF.Property<object>(e, pagination.SortBy));
        }
        else
        {
            // Default sort: newest by StartTime
            query = query.OrderByDescending(x => x.StartTime);
        }

        // -----------------------
        // 📊 Pagination
        // -----------------------
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
