using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities;
using VietCommerce.Core.Entities.Common;
using VietCommerce.Core.Models;
using VietCommerce.Data;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Generic;

namespace VietCommerce.Data.Repositories.Generic;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<PaginatedResult<T>> GetPaginatedAsync(int pageNumber, int pageSize)
    {
        var skip = (pageNumber - 1) * pageSize;
        var items = await _dbSet.Skip(skip).Take(pageSize).ToListAsync();
        var total = await _dbSet.CountAsync();
        return new PaginatedResult<T>(items, pageNumber, pageSize, total);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}