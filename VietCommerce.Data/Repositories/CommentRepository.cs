using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Comment> Comments, int TotalCount)> GetCommentsByPostIdAsync(
            Guid postId,
            int startIndex,
            int pageSize)
        {
            var query = _context.Set<Comment>()
                .Include(c => c.Customer)
                .Include(c => c.Replies)
                .Where(c => c.PostId == postId && !c.IsDeleted && c.ParentCommentId == null)
                .OrderByDescending(c => c.AddedOn); // Mới nhất trước

            var totalCount = await query.CountAsync();

            var comments = await query
                .Skip(startIndex)
                .Take(pageSize)
                .ToListAsync();

            return (comments, totalCount);
        }

        public async Task<IEnumerable<Comment>> GetRepliesByCommentIdAsync(Guid commentId)
        {
            return await _context.Set<Comment>()
                .Include(c => c.Customer)
                .Where(c => c.ParentCommentId == commentId && !c.IsDeleted)
                .OrderBy(c => c.AddedOn) // Replies: cũ nhất trước
                .ToListAsync();
        }

        public async Task<int> CountCommentsByPostIdAsync(Guid postId)
        {
            return await _context.Set<Comment>()
                .Where(c => c.PostId == postId && !c.IsDeleted)
                .CountAsync();
        }

        public async Task<bool> IsCommentOwnerAsync(Guid commentId, Guid customerId)
        {
            return await _context.Set<Comment>()
                .AnyAsync(c => c.Id == commentId && c.CustomerId == customerId);
        }

        public async Task<Guid?> GetPostOwnerIdAsync(Guid postId)
        {
            return await _context.Set<Post>()
                .Where(p => p.Id == postId && !p.IsDeleted)
                .Select(p => p.CustomerId)
                .FirstOrDefaultAsync();
        }
        public Task<Comment?> GetCommentWithDetailsAsync(Guid id)
        {
            return _dbSet
                .Where(c => c.Id == id)
                .Include(c => c.Customer)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Customer)
                .FirstOrDefaultAsync();
        }

    }
}
