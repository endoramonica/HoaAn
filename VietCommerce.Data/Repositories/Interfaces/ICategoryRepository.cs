using VietCommerce.Core.Entities.Products;
namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        /// <summary>
        /// Lấy danh sách category theo cửa hàng (StoreId).
        /// </summary>
        Task<IEnumerable<Category>> GetByStoreIdAsync(Guid storeId);
        /// <summary>
        /// Lấy danh sách các category con của một category cha.
        /// </summary>
        Task<IEnumerable<Category>> GetSubCategoriesAsync(Guid parentId);
        /// <summary>
        /// Lấy danh sách category đang hoạt động.
        /// </summary>
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        /// <summary>
        /// Kiểm tra xem một category có trùng tên trong cùng cửa hàng không.
        /// </summary>
        Task<bool> ExistsByNameAsync(Guid storeId, string name);
        /// <summary>
        /// Lấy category bao gồm danh sách SubCategories và Products (Include Navigation).
        /// </summary>
        Task<Category?> GetWithDetailsAsync(Guid id);
        /// <summary>
        /// Đếm số lượng category theo cửa hàng.
        /// </summary>
        Task<int> CountByStoreAsync(Guid storeId);
    }
}