
using VietCommerce.Core.Common;
using VietCommerce.Core.DTOs.Category;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Services.Interfaces;

public interface ICategoryService
{
    // CRUD cơ bản
    Task<ApiResponse<CategoryDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllAsync();
    Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryDto dto);
    Task<ApiResponse<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryDto dto);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
    Task<ApiResponse<bool>> SoftDeleteAsync(Guid id);

    // Lọc theo Store và Tenant
    Task<ApiResponse<IEnumerable<CategoryDto>>> GetByStoreIdAsync(Guid storeId);
    Task<ApiResponse<IEnumerable<CategoryDto>>> GetActiveCategoriesAsync();

    // Quản lý Category cha-con
    Task<ApiResponse<IEnumerable<CategoryDto>>> GetSubCategoriesAsync(Guid parentId);
    Task<ApiResponse<CategoryDetailDto>> GetWithDetailsAsync(Guid id);

    // Phân trang
    Task<ApiResponse<PaginatedResult<CategoryDto>>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? storeId = null,
        bool? isActive = null,
        string? searchTerm = null);

    // Thống kê
    Task<ApiResponse<int>> CountByStoreAsync(Guid storeId);
    Task<ApiResponse<bool>> ExistsByNameAsync(Guid storeId, string name);

    // Bulk operations
    Task<ApiResponse<bool>> UpdateCategoryStatusAsync(Guid id, bool isActive);
    Task<ApiResponse<IEnumerable<CategoryDto>>> GetCategoryHierarchyAsync(Guid storeId);
}