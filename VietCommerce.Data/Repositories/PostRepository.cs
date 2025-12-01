// VietCommerce.Data/Repositories/PostRepository.cs
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Models;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

/// <summary>
/// Implementation của IPostRepository
/// Sử dụng GenericRepository làm base, custom các query phức tạp
/// </summary>
public class PostRepository : GenericRepository<Post>, IPostRepository
{
    public PostRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Post?> GetPostWithDetailsAsync(Guid postId)
    {
        return await _dbSet
            .Include(p => p.Customer)
            .Include(p => p.Likes)
            .Include(p => p.Comments.Where(c => !c.IsDeleted))
                .ThenInclude(c => c.Customer)
            .Include(p => p.Bookmarks)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);
    }

    public async Task<PaginatedResult<Post>> GetPostFeedAsync(
        int pageNumber,
        int pageSize,
        Guid? currentCustomerId = null)
    {
        var query = _dbSet
            .Where(p => p.IsActive && !p.IsDeleted)
            .Include(p => p.Customer)
            .Include(p => p.Likes)
            .Include(p => p.Comments.Where(c => !c.IsDeleted))
            .Include(p => p.Bookmarks)
            .OrderByDescending(p => p.PostedOn)
            .AsNoTracking();

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Post>(items, pageNumber, pageSize, totalItems);
    }

    public async Task<PaginatedResult<Post>> GetPostsByCustomerIdAsync(
        Guid customerId,
        int pageNumber,
        int pageSize)
    {
        var query = _dbSet
            .Where(p => p.CustomerId == customerId && p.IsActive && !p.IsDeleted)
            .Include(p => p.Customer)
            .Include(p => p.Likes)
            .Include(p => p.Comments.Where(c => !c.IsDeleted))
            .Include(p => p.Bookmarks)
            .OrderByDescending(p => p.PostedOn)
            .AsNoTracking();

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Post>(items, pageNumber, pageSize, totalItems);
    }

    public async Task<PaginatedResult<Post>> SearchPostsAsync(
        string keyword,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        var query = _dbSet
            .Where(p => p.IsActive && !p.IsDeleted)
            .Include(p => p.Customer)
            .AsNoTracking();

        // Filter by keyword
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim().ToLower();
            query = query.Where(p =>
                (p.Content != null && p.Content.ToLower().Contains(keyword)) ||
                (p.Customer.Name != null && p.Customer.Name.ToLower().Contains(keyword)));
        }

        // Filter by date range
        if (fromDate.HasValue)
        {
            query = query.Where(p => p.PostedOn >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(p => p.PostedOn <= toDate.Value);
        }

        query = query.OrderByDescending(p => p.PostedOn);

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<Post>(items, pageNumber, pageSize, totalItems);
    }

    public async Task<bool> IsDuplicateImageAsync(string photoHash)
    {
        if (string.IsNullOrWhiteSpace(photoHash))
            return false;

        return await _dbSet
            .AnyAsync(p => p.PhotoHash == photoHash && !p.IsDeleted);
    }

    public async Task<int> GetLikesCountAsync(Guid postId)
    {
        return await _context.Set<Like>()
            .CountAsync(l => l.PostId == postId);
    }

    public async Task<int> GetCommentsCountAsync(Guid postId)
    {
        return await _context.Set<Comment>()
            .CountAsync(c => c.PostId == postId && !c.IsDeleted);
    }

    public async Task<int> GetBookmarksCountAsync(Guid postId)
    {
        return await _context.Set<Bookmark>()
            .CountAsync(b => b.PostId == postId);
    }

    public async Task<bool> IsLikedByCustomerAsync(Guid postId, Guid customerId)
    {
        return await _context.Set<Like>()
            .AnyAsync(l => l.PostId == postId && l.CustomerId == customerId);
    }

    public async Task<bool> IsBookmarkedByCustomerAsync(Guid postId, Guid customerId)
    {
        return await _context.Set<Bookmark>()
            .AnyAsync(b => b.PostId == postId && b.CustomerId == customerId);
    }

    public async Task<List<Guid>> GetPostLikersAsync(Guid postId, int top = 10)
    {
        return await _context.Set<Like>()
            .Where(l => l.PostId == postId)
            .OrderByDescending(l => l.LikedOn)
            .Take(top)
            .Select(l => l.CustomerId)
            .ToListAsync();
    }

    public async Task<Post?> GetPostByPhotoHashAsync(string photoHash)
    {
        if (string.IsNullOrWhiteSpace(photoHash))
            return null;

        return await _dbSet
            .FirstOrDefaultAsync(p => p.PhotoHash == photoHash && !p.IsDeleted);
    }
    // ✅ Implementation ĐÚNG
    public async Task<HashSet<Guid>> GetLikedPostIdsByCustomerAsync(List<Guid> postIds, Guid customerId)
    {
        return await _context.Set<Like>()  // ← Sửa từ _dbSet thành _context.Set<Like>()
            .Where(l => postIds.Contains(l.PostId) && l.CustomerId == customerId)
            .Select(l => l.PostId)
            .ToHashSetAsync();
    }

    public async Task<HashSet<Guid>> GetBookmarkedPostIdsByCustomerAsync(List<Guid> postIds, Guid customerId)
    {
        return await _context.Set<Bookmark>()  // ← Sửa từ _context.PostBookmarks thành _context.Set<Bookmark>()
            .Where(b => postIds.Contains(b.PostId) && b.CustomerId == customerId)
            .Select(b => b.PostId)
            .ToHashSetAsync();
    }
}