using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VietCommerce.Data.Repositories.Interfaces
{
    /// <summary>
    /// Custom repository cho Like entity (không kế thừa BaseEntity)
    /// </summary>
    public interface ILikeRepository
    {
        /// <summary>
        /// Thêm like mới
        /// </summary>
        Task<Like> AddAsync(Like like);

        /// <summary>
        /// Xóa like
        /// </summary>
        Task<bool> DeleteAsync(Guid postId, Guid customerId);

        /// <summary>
        /// Kiểm tra user đã like post chưa
        /// </summary>
        Task<bool> ExistsAsync(Guid postId, Guid customerId);

        /// <summary>
        /// Lấy like entity
        /// </summary>
        Task<Like?> GetAsync(Guid postId, Guid customerId);

        /// <summary>
        /// Đếm số likes của post
        /// </summary>
        Task<int> CountByPostIdAsync(Guid postId);

        /// <summary>
        /// Lấy danh sách customers đã like post
        /// </summary>
        Task<List<Guid>> GetLikersByPostIdAsync(Guid postId, int top = 10);
    }

}
