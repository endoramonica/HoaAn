using Microsoft.Extensions.Logging;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Category;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Products;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Services;

public class CategoryService : BaseService, ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private const string CACHE_PREFIX = "category";

    public CategoryService(
        IUnitOfWork unitOfWork,
        ILogger<CategoryService> logger,
        ICacheService? cacheService = null)
        : base(logger, cacheService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    #region CRUD Operations

    public async Task<ApiResponse<CategoryDto>> GetByIdAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(id, nameof(id));

            var cacheKey = CreateCacheKey(CACHE_PREFIX, "id", id);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                ThrowIf(category == null || category.IsDeleted, "Không tìm thấy danh mục");

                return MapToDto(category!);
            }, TimeSpan.FromMinutes(30));

        }, "GetByIdAsync", "Lấy category thành công");
    }

    public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllAsync()
    {
        return await ExecuteAsApiResponseAsync<IEnumerable<CategoryDto>>(async () =>
        {
            var cacheKey = CreateCacheKey(CACHE_PREFIX, "all");

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var categories = await _unitOfWork.Categories.GetAllAsync();
                return (IEnumerable<CategoryDto>)categories
                    .Where(c => !c.IsDeleted)
                    .Select(MapToDto)
                    .ToList();
            }, TimeSpan.FromMinutes(15));

        }, "GetAllAsync", "Lấy tất cả categories thành công");
    }

    public async Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryDto dto)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateNotNull(dto, nameof(dto));
            ValidateNotEmpty(dto.Name, nameof(dto.Name));
            ValidateId(dto.StoreId, nameof(dto.StoreId));

            // Kiểm tra tên trùng
            var exists = await _unitOfWork.Categories.ExistsByNameAsync(dto.StoreId, dto.Name);
            ThrowIf(exists, "Tên danh mục đã tồn tại trong cửa hàng");

            // Kiểm tra ParentId hợp lệ
            if (dto.ParentId.HasValue)
            {
                var parent = await _unitOfWork.Categories.GetByIdAsync(dto.ParentId.Value);
                ThrowIf(parent == null || parent.IsDeleted, "Danh mục cha không tồn tại");
            }

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                StoreId = dto.StoreId,
                ParentId = dto.ParentId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            // Invalidate cache
            await InvalidateCategoryCache(dto.StoreId);

            return MapToDto(category);

        }, "CreateAsync", "Tạo danh mục thành công");
    }

    public async Task<ApiResponse<CategoryDto>> UpdateAsync(Guid id, UpdateCategoryDto dto)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(id, nameof(id));
            ValidateNotNull(dto, nameof(dto));
            ValidateNotEmpty(dto.Name, nameof(dto.Name));

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            ThrowIf(category == null || category.IsDeleted, "Không tìm thấy danh mục");

            // Kiểm tra tên trùng (trừ chính nó)
            var exists = await _unitOfWork.Categories.AnyAsync(c =>
                c.StoreId == category!.StoreId
                && c.Name.ToLower() == dto.Name.ToLower()
                && c.Id != id
                && !c.IsDeleted);
            ThrowIf(exists, "Tên danh mục đã tồn tại");

            // Kiểm tra vòng lặp parent
            if (dto.ParentId.HasValue)
            {
                var isCircular = await IsCircularReference(id, dto.ParentId.Value);
                ThrowIf(isCircular, "Không thể đặt danh mục con làm danh mục cha");
            }

            category!.Name = dto.Name;
            category.ParentId = dto.ParentId;
            category.IsActive = dto.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            // Invalidate cache
            await InvalidateCategoryCache(category.StoreId, id);

            return MapToDto(category);

        }, "UpdateAsync", "Cập nhật danh mục thành công");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(id, nameof(id));

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            ThrowIf(category == null, "Không tìm thấy danh mục");

            // Kiểm tra có subcategory không
            var hasSubCategories = await _unitOfWork.Categories.AnyAsync(c =>
                c.ParentId == id && !c.IsDeleted);
            ThrowIf(hasSubCategories, "Không thể xóa danh mục có danh mục con");

            _unitOfWork.Categories.Delete(category!);
            await _unitOfWork.SaveChangesAsync();

            // Invalidate cache
            await InvalidateCategoryCache(category!.StoreId, id);

            return true;

        }, "DeleteAsync", "Xóa danh mục thành công");
    }

    public async Task<ApiResponse<bool>> SoftDeleteAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(id, nameof(id));

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            ThrowIf(category == null || category.IsDeleted, "Không tìm thấy danh mục");

            category!.IsDeleted = true;
            category.DeletedAt = DateTime.UtcNow;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            // Invalidate cache
            await InvalidateCategoryCache(category.StoreId, id);

            return true;

        }, "SoftDeleteAsync", "Xóa mềm danh mục thành công");
    }

    #endregion

    #region Query Operations

    public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetByStoreIdAsync(Guid storeId)
    {
        return await ExecuteAsApiResponseAsync<IEnumerable<CategoryDto>>(async () =>
        {
            ValidateId(storeId, nameof(storeId));

            var cacheKey = CreateCacheKey(CACHE_PREFIX, "store", storeId);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var categories = await _unitOfWork.Categories.GetByStoreIdAsync(storeId);
                return (IEnumerable<CategoryDto>)categories.Select(MapToDto).ToList();
            }, TimeSpan.FromMinutes(20));

        }, "GetByStoreIdAsync", "Lấy categories theo store thành công");
    }

    public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetActiveCategoriesAsync()
    {
        return await ExecuteAsApiResponseAsync<IEnumerable<CategoryDto>>(async () =>
        {
            var cacheKey = CreateCacheKey(CACHE_PREFIX, "active");

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
                return (IEnumerable<CategoryDto>)categories.Select(MapToDto).ToList();
            }, TimeSpan.FromMinutes(15));

        }, "GetActiveCategoriesAsync", "Lấy active categories thành công");
    }

    public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetSubCategoriesAsync(Guid parentId)
    {
        return await ExecuteAsApiResponseAsync<IEnumerable<CategoryDto>>(async () =>
        {
            ValidateId(parentId, nameof(parentId));

            var cacheKey = CreateCacheKey(CACHE_PREFIX, "sub", parentId);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var categories = await _unitOfWork.Categories.GetSubCategoriesAsync(parentId);
                return (IEnumerable<CategoryDto>)categories.Select(MapToDto).ToList();
            }, TimeSpan.FromMinutes(20));

        }, "GetSubCategoriesAsync", "Lấy subcategories thành công");
    }

    public async Task<ApiResponse<CategoryDetailDto>> GetWithDetailsAsync(Guid id)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(id, nameof(id));

            var cacheKey = CreateCacheKey(CACHE_PREFIX, "details", id);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var category = await _unitOfWork.Categories.GetWithDetailsAsync(id);
                ThrowIf(category == null, "Không tìm thấy danh mục");

                return new CategoryDetailDto
                {
                    Id = category!.Id,
                    StoreId = category.StoreId,
                    Name = category.Name,
                    ParentId = category.ParentId,
                    ParentName = category.ParentCategory?.Name,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt,
                    SubCategories = category.SubCategories.Select(MapToDto).ToList(),
                    Products = category.Products.Select(p => new ProductSummaryDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Prices
                            .Where(pr => pr.IsActive
                                && pr.PriceType == PriceType.REGULAR
                                && pr.EffectiveFrom <= DateTime.UtcNow
                                && (pr.EffectiveTo == null || pr.EffectiveTo >= DateTime.UtcNow))
                            .OrderByDescending(pr => pr.EffectiveFrom)
                            .Select(pr => pr.Price)
                            .FirstOrDefault(),
                        IsActive = p.IsActive
                    }).ToList(),
                    SubCategoriesCount = category.SubCategories.Count,
                    ProductsCount = category.Products.Count
                };
            }, TimeSpan.FromMinutes(15));

        }, "GetWithDetailsAsync", "Lấy category details thành công");
    }

    public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetCategoryHierarchyAsync(Guid storeId)
    {
        return await ExecuteAsApiResponseAsync<IEnumerable<CategoryDto>>(async () =>
        {
            ValidateId(storeId, nameof(storeId));

            var cacheKey = CreateCacheKey(CACHE_PREFIX, "hierarchy", storeId);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var allCategories = await _unitOfWork.Categories.GetByStoreIdAsync(storeId);
                var rootCategories = allCategories.Where(c => c.ParentId == null).ToList();
                return (IEnumerable<CategoryDto>)rootCategories.Select(MapToDto).ToList();
            }, TimeSpan.FromMinutes(20));

        }, "GetCategoryHierarchyAsync", "Lấy category hierarchy thành công");
    }

    #endregion

    #region Pagination

    public async Task<ApiResponse<PaginatedResult<CategoryDto>>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Guid? storeId = null,
        bool? isActive = null,
        string? searchTerm = null)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ThrowIf(pageNumber <= 0, "PageNumber phải > 0");
            ThrowIf(pageSize <= 0 || pageSize > 100, "PageSize phải từ 1-100");

            var query = await _unitOfWork.Categories.GetAllAsync();

            // Filters
            if (storeId.HasValue)
                query = query.Where(c => c.StoreId == storeId.Value);

            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            query = query.Where(c => !c.IsDeleted);

            var totalItems = query.Count();
            var items = query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapToDto)
                .ToList();

            return new PaginatedResult<CategoryDto>(items, pageNumber, pageSize, totalItems);

        }, "GetPagedAsync", "Phân trang categories thành công");
    }

    #endregion

    #region Statistics

    public async Task<ApiResponse<int>> CountByStoreAsync(Guid storeId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(storeId, nameof(storeId));

            var cacheKey = CreateCacheKey(CACHE_PREFIX, "count", storeId);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                return await _unitOfWork.Categories.CountByStoreAsync(storeId);
            }, TimeSpan.FromMinutes(10));

        }, "CountByStoreAsync", "Đếm categories thành công");
    }

    public async Task<ApiResponse<bool>> ExistsByNameAsync(Guid storeId, string name)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(storeId, nameof(storeId));
            ValidateNotEmpty(name, nameof(name));

            return await _unitOfWork.Categories.ExistsByNameAsync(storeId, name);

        }, "ExistsByNameAsync", "Kiểm tra tồn tại thành công");
    }

    #endregion

    #region Bulk Operations

    public async Task<ApiResponse<bool>> UpdateCategoryStatusAsync(Guid id, bool isActive)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(id, nameof(id));

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            ThrowIf(category == null || category.IsDeleted, "Không tìm thấy danh mục");

            category!.IsActive = isActive;
            category.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync();

            // Invalidate cache
            await InvalidateCategoryCache(category.StoreId, id);

            return true;

        }, "UpdateCategoryStatusAsync", "Cập nhật trạng thái thành công");
    }

    #endregion

    #region Helper Methods

    private CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            StoreId = category.StoreId,
            Name = category.Name,
            ParentId = category.ParentId,
            ParentName = category.ParentCategory?.Name,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
            SubCategoriesCount = category.SubCategories?.Count ?? 0,
            ProductsCount = category.Products?.Count ?? 0
        };
    }

    private async Task<bool> IsCircularReference(Guid categoryId, Guid newParentId)
    {
        var currentParentId = newParentId;
        while (currentParentId != Guid.Empty)
        {
            if (currentParentId == categoryId)
                return true;

            var parent = await _unitOfWork.Categories.GetByIdAsync(currentParentId);
            if (parent?.ParentId == null)
                break;

            currentParentId = parent.ParentId.Value;
        }
        return false;
    }

    private async Task InvalidateCategoryCache(Guid storeId, Guid? categoryId = null)
    {
        var keys = new List<string>
        {
            CreateCacheKey(CACHE_PREFIX, "all"),
            CreateCacheKey(CACHE_PREFIX, "active"),
            CreateCacheKey(CACHE_PREFIX, "store", storeId),
            CreateCacheKey(CACHE_PREFIX, "hierarchy", storeId)
        };

        if (categoryId.HasValue)
        {
            keys.Add(CreateCacheKey(CACHE_PREFIX, "id", categoryId.Value));
            keys.Add(CreateCacheKey(CACHE_PREFIX, "details", categoryId.Value));
        }

        await InvalidateMultipleCachesAsync(keys.ToArray());
    }

    #endregion
}