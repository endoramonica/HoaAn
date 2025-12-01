namespace VietCommerce.Data.Repositories.Interfaces
{

    // VietCommerce.Data/Repositories/Interfaces/IBookmarkRepository.cs

    /// <summary>
    /// Custom repository cho Bookmark entity (không kế thừa BaseEntity)
    /// </summary>
    public interface IBookmarkRepository
    {
        /// <summary>
        /// Thêm bookmark mới
        /// </summary>
        Task<Bookmark> AddAsync(Bookmark bookmark);

        /// <summary>
        /// Xóa bookmark
        /// </summary>
        Task<bool> DeleteAsync(Guid postId, Guid customerId);

        /// <summary>
        /// Kiểm tra user đã bookmark post chưa
        /// </summary>
        Task<bool> ExistsAsync(Guid postId, Guid customerId);

        /// <summary>
        /// Lấy bookmark entity
        /// </summary>
        Task<Bookmark?> GetAsync(Guid postId, Guid customerId);

        /// <summary>
        /// Đếm số bookmarks của post
        /// </summary>
        Task<int> CountByPostIdAsync(Guid postId);

        /// <summary>
        /// Lấy bookmarks của customer
        /// </summary>
        Task<List<Bookmark>> GetByCustomerIdAsync(Guid customerId, int pageNumber = 1, int pageSize = 20);
    }
}