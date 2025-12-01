using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        /// <summary>
        /// Lấy comments của một post với pagination
        /// Sắp xếp theo AddedOn giảm dần (mới nhất trước)
        /// </summary>
        Task<(IEnumerable<Comment> Comments, int TotalCount)> GetCommentsByPostIdAsync(
            Guid postId,
            int startIndex,
            int pageSize);

        /// <summary>
        /// Lấy replies của một comment
        /// </summary>
        Task<IEnumerable<Comment>> GetRepliesByCommentIdAsync(Guid commentId);

        /// <summary>
        /// Đếm số comments của một post (không tính deleted)
        /// </summary>
        Task<int> CountCommentsByPostIdAsync(Guid postId);

        /// <summary>
        /// Check xem comment có thuộc về customer không
        /// </summary>
        Task<bool> IsCommentOwnerAsync(Guid commentId, Guid customerId);

        /// <summary>
        /// Lấy PostOwnerId từ PostId (để gửi notification)
        /// </summary>
        Task<Guid?> GetPostOwnerIdAsync(Guid postId);

        Task<Comment?> GetCommentWithDetailsAsync(Guid id);

    }
}
