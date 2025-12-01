// VietCommerce.Data/Repositories/BookmarkRepository.cs
using Microsoft.EntityFrameworkCore;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

// VietCommerce.Data/Repositories/BookmarkRepository.cs


public class BookmarkRepository : IBookmarkRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<Bookmark> _dbSet;

    public BookmarkRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<Bookmark>();
    }

    public async Task<Bookmark> AddAsync(Bookmark bookmark)
    {
        var entry = await _dbSet.AddAsync(bookmark);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<bool> DeleteAsync(Guid postId, Guid customerId)
    {
        var bookmark = await GetAsync(postId, customerId);
        if (bookmark == null)
            return false;

        _dbSet.Remove(bookmark);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid postId, Guid customerId)
    {
        return await _dbSet.AnyAsync(b => b.PostId == postId && b.CustomerId == customerId);
    }

    public async Task<Bookmark?> GetAsync(Guid postId, Guid customerId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(b => b.PostId == postId && b.CustomerId == customerId);
    }

    public async Task<int> CountByPostIdAsync(Guid postId)
    {
        return await _dbSet.CountAsync(b => b.PostId == postId);
    }

    public async Task<List<Bookmark>> GetByCustomerIdAsync(Guid customerId, int pageNumber = 1, int pageSize = 20)
    {
        return await _dbSet
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.BookmarkedOn)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(b => b.Post)
                .ThenInclude(p => p.Customer)
            .ToListAsync();
    }
}