// VietCommerce.Data/Repositories/LikeRepository.cs
using Microsoft.EntityFrameworkCore;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

public class LikeRepository : ILikeRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<Like> _dbSet;

    public LikeRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<Like>();
    }

    public async Task<Like> AddAsync(Like like)
    {
        var entry = await _dbSet.AddAsync(like);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<bool> DeleteAsync(Guid postId, Guid customerId)
    {
        var like = await GetAsync(postId, customerId);
        if (like == null)
            return false;

        _dbSet.Remove(like);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid postId, Guid customerId)
    {
        return await _dbSet.AnyAsync(l => l.PostId == postId && l.CustomerId == customerId);
    }

    public async Task<Like?> GetAsync(Guid postId, Guid customerId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(l => l.PostId == postId && l.CustomerId == customerId);
    }

    public async Task<int> CountByPostIdAsync(Guid postId)
    {
        return await _dbSet.CountAsync(l => l.PostId == postId);
    }

    public async Task<List<Guid>> GetLikersByPostIdAsync(Guid postId, int top = 10)
    {
        return await _dbSet
            .Where(l => l.PostId == postId)
            .OrderByDescending(l => l.LikedOn)
            .Take(top)
            .Select(l => l.CustomerId)
            .ToListAsync();
    }
}


