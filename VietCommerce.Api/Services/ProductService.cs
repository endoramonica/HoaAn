using AutoMapper;
using Microsoft.IdentityModel.Logging;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Helpers;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<ProductService> _logger;
    private readonly IMapper _mapper;

    public ProductService(
        IUnitOfWork unitOfWork,
        IPermissionService permissionService,
        ILogger<ProductService> logger,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _logger = logger;
        _mapper = mapper;
    }

    // ============================================
    // CREATE
    // ============================================

    public async Task<ApiResponse<ProductDetailDto>> CreateProductAsync(Guid actorUserId, ProductCreateDto dto)
    {
        try
        {
            // ✅ PERMISSION CHECK (defense-in-depth)
            if (!await _permissionService.CheckUserPermissionAsync(actorUserId, "product.create"))
            {
                _logger.LogWarning("User {UserId} attempted to create product without permission", actorUserId);
                return ApiResponse<ProductDetailDto>.FailureResponse("Access denied: product.create permission required");
            }

            // ✅ VALIDATE: Code uniqueness
            if (await _unitOfWork.Products.ExistsByCodeAsync(dto.Code))
            {
                return ApiResponse<ProductDetailDto>.FailureResponse($"Product code '{dto.Code}' already exists");
            }

            // ✅ MAP DTO → Entity
            var product = _mapper.Map<Product>(dto);
            product.Id = Guid.NewGuid();
            product.Slug = SlugHelper.GenerateSlug(dto.Name);
            product.CreatedBy = actorUserId;
            product.UpdatedBy = actorUserId;
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            // ✅ SET CỨNG STORE ID (test/demo)
            product.StoreId = Guid.Parse("47AA5519-C503-4CFA-8101-2EDB36FD9D8C");
            // ✅ SAVE
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // ✅ FETCH FULL ENTITY (with relations)
            var createdProduct = await _unitOfWork.Products.GetByIdAsync(product.Id);
            var result = _mapper.Map<ProductDetailDto>(createdProduct);

            _logger.LogInformation("Product created: {ProductId} by user {UserId}", product.Id, actorUserId);
            return ApiResponse<ProductDetailDto>.SuccessResponse(result, "Product created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product by user {UserId}", actorUserId);
            return ApiResponse<ProductDetailDto>.FailureResponse("Failed to create product");
        }
    }

    // ============================================
    // UPDATE
    // ============================================

    public async Task<ApiResponse<ProductDetailDto>> UpdateProductAsync(
        Guid actorUserId,
        Guid productId,
        ProductUpdateDto dto)
    {
        try
        {
            // ✅ PERMISSION CHECK
            if (!await _permissionService.CheckUserPermissionAsync(actorUserId, "product.update"))
            {
                _logger.LogWarning("User {UserId} attempted to update product {ProductId} without permission",
                    actorUserId, productId);
                return ApiResponse<ProductDetailDto>.FailureResponse("Access denied: product.update permission required");
            }

            // ✅ FETCH EXISTING
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                return ApiResponse<ProductDetailDto>.FailureResponse("Product not found");
            }

            // ✅ UPDATE ONLY PROVIDED FIELDS
            if (dto.Name != null)
            {
                product.Name = dto.Name;
                product.Slug = SlugHelper.GenerateSlug(dto.Name); // Cập nhật slug khi đổi tên
            }

            if (dto.CategoryId.HasValue) product.CategoryId = dto.CategoryId;
            if (dto.Sku != null) product.SKU = dto.Sku; // ✅ Map đúng: Sku → SKU
            if (dto.StockQuantity.HasValue) product.Stock = dto.StockQuantity.Value; // ✅ Map đúng: StockQuantity → Stock
            if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;

            product.UpdatedBy = actorUserId;
            product.UpdatedAt = DateTime.UtcNow;

            // ✅ SAVE
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            // ✅ RETURN UPDATED
            var updated = await _unitOfWork.Products.GetByIdAsync(productId);
            var result = _mapper.Map<ProductDetailDto>(updated);

            _logger.LogInformation("Product {ProductId} updated by user {UserId}", productId, actorUserId);
            return ApiResponse<ProductDetailDto>.SuccessResponse(result, "Product updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId} by user {UserId}", productId, actorUserId);
            return ApiResponse<ProductDetailDto>.FailureResponse("Failed to update product");
        }
    }

    // ============================================
    // DELETE (SOFT DELETE)
    // ============================================

    public async Task<ApiResponse<bool>> DeleteProductAsync(Guid actorUserId, Guid productId)
    {
        try
        {
            // ✅ PERMISSION CHECK
            if (!await _permissionService.CheckUserPermissionAsync(actorUserId, "product.delete"))
            {
                _logger.LogWarning("User {UserId} attempted to delete product {ProductId} without permission",
                    actorUserId, productId);
                return ApiResponse<bool>.FailureResponse("Access denied: product.delete permission required");
            }

            // ✅ SOFT DELETE
            var success = await _unitOfWork.Products.SoftDeleteAsync(productId, actorUserId);
            if (!success)
            {
                return ApiResponse<bool>.FailureResponse("Product not found or already deleted");
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product {ProductId} deleted by user {UserId}", productId, actorUserId);
            return ApiResponse<bool>.SuccessResponse(true, "Product deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId} by user {UserId}", productId, actorUserId);
            return ApiResponse<bool>.FailureResponse("Failed to delete product");
        }
    }

    // ============================================
    // GET BY ID
    // ============================================

    public async Task<ApiResponse<ProductDetailDto>> GetProductByIdAsync(Guid productId, Guid? actorUserId = null)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                return ApiResponse<ProductDetailDto>.FailureResponse("Product not found");
            }

            // ✅ PERMISSION CHECK: Inactive products require product.view permission
            if (!product.IsActive && actorUserId.HasValue)
            {
                if (!await _permissionService.CheckUserPermissionAsync(actorUserId.Value, "product.view"))
                {
                    return ApiResponse<ProductDetailDto>.FailureResponse("Product not available");
                }
            }
            else if (!product.IsActive)
            {
                return ApiResponse<ProductDetailDto>.FailureResponse("Product not available");
            }

            var result = _mapper.Map<ProductDetailDto>(product);
            return ApiResponse<ProductDetailDto>.SuccessResponse(result, "Product retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {ProductId}", productId);
            return ApiResponse<ProductDetailDto>.FailureResponse("Failed to retrieve product");
        }
    }

    // ============================================
    // GET BY SLUG (PUBLIC)
    // ============================================

    public async Task<ApiResponse<ProductDetailDto>> GetProductBySlugAsync(string slug, Guid? actorUserId = null)
    {
        try
        {
            var product = await _unitOfWork.Products.GetBySlugAsync(slug);
            if (product == null)
            {
                return ApiResponse<ProductDetailDto>.FailureResponse("Product not found");
            }

            // Check active status (same as GetById)
            if (!product.IsActive && actorUserId.HasValue)
            {
                if (!await _permissionService.CheckUserPermissionAsync(actorUserId.Value, "product.view"))
                {
                    return ApiResponse<ProductDetailDto>.FailureResponse("Product not available");
                }
            }
            else if (!product.IsActive)
            {
                return ApiResponse<ProductDetailDto>.FailureResponse("Product not available");
            }

            var result = _mapper.Map<ProductDetailDto>(product);
            return ApiResponse<ProductDetailDto>.SuccessResponse(result, "Product retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product by slug {Slug}", slug);
            return ApiResponse<ProductDetailDto>.FailureResponse("Failed to retrieve product");
        }
    }

    // ============================================
    // LIST (PAGINATED)
    // ============================================

    public async Task<ApiResponse<PaginatedResult<ProductListDto>>> GetProductsAsync(
        ProductFilterDto filter,
        Guid? actorUserId = null)
    {
        try
        {
            // ✅ CHECK PERMISSION: If user has product.list, show all (including inactive)
            bool showAll = false;
            if (actorUserId.HasValue)
            {
                showAll = await _permissionService.CheckUserPermissionAsync(actorUserId.Value, "product.list");
            }

            // Override IsActive filter if not admin
            if (!showAll)
            {
                filter.IsActive = true; // Force show only active products for public
            }

            // ✅ FETCH FROM REPOSITORY
            var paginatedResult = await _unitOfWork.Products.GetPaginatedAsync(
                filter.Page,
                filter.PageSize,
                filter.SearchTerm,
                filter.CategoryId,
                filter.StoreId,
                filter.IsActive,
                filter.MinPrice,
                filter.MaxPrice,
                filter.SortBy,
                filter.IsDescending
            );

            // ✅ MAP TO DTOs
            // ✅ MAP TO DTOs
            var dtos = _mapper.Map<List<ProductListDto>>(paginatedResult.Items);

            var result = new PaginatedResult<ProductListDto>
            {
                Items = dtos,
                PageNumber = paginatedResult.PageNumber,
                PageSize = paginatedResult.PageSize,
                TotalItems = paginatedResult.TotalItems,
                TotalPages = paginatedResult.TotalPages
            };

            return ApiResponse<PaginatedResult<ProductListDto>>.SuccessResponse(
                result,
                $"Retrieved {dtos.Count} products");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product list");
            return ApiResponse<PaginatedResult<ProductListDto>>.FailureResponse("Failed to retrieve products");
        }
    }

    // ============================================
    // STOCK MANAGEMENT
    // ============================================

    public async Task<ApiResponse<bool>> UpdateStockAsync(Guid actorUserId, Guid productId, int quantity)
    {
        try
        {
            // ✅ PERMISSION CHECK
            if (!await _permissionService.CheckUserPermissionAsync(actorUserId, "product.update_stock"))
            {
                return ApiResponse<bool>.FailureResponse("Access denied: product.update_stock permission required");
            }

            var success = await _unitOfWork.Products.UpdateStockAsync(productId, quantity);
            if (!success)
            {
                return ApiResponse<bool>.FailureResponse("Product not found");
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Stock updated for product {ProductId}: {Quantity} by user {UserId}",
                productId, quantity, actorUserId);

            return ApiResponse<bool>.SuccessResponse(true, "Stock updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating stock for product {ProductId}", productId);
            return ApiResponse<bool>.FailureResponse("Failed to update stock");
        }
    }

    public async Task<ApiResponse<int>> GetStockQuantityAsync(Guid productId)
    {
        try
        {
            var stock = await _unitOfWork.Products.GetStockQuantityAsync(productId);
            return ApiResponse<int>.SuccessResponse(stock, "Stock retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting stock for product {ProductId}", productId);
            return ApiResponse<int>.FailureResponse("Failed to retrieve stock");
        }
    }

    // ============================================
    // STATUS MANAGEMENT
    // ============================================

    public async Task<ApiResponse<bool>> SetActiveStatusAsync(Guid actorUserId, Guid productId, bool isActive)
    {
        try
        {
            if (!await _permissionService.CheckUserPermissionAsync(actorUserId, "product.update"))
            {
                return ApiResponse<bool>.FailureResponse("Access denied: product.update permission required");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                return ApiResponse<bool>.FailureResponse("Product not found");
            }

            product.IsActive = isActive;
            product.UpdatedBy = actorUserId;
            product.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product {ProductId} active status set to {IsActive} by user {UserId}",
                productId, isActive, actorUserId);

            return ApiResponse<bool>.SuccessResponse(true, $"Product {(isActive ? "activated" : "deactivated")} successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting active status for product {ProductId}", productId);
            return ApiResponse<bool>.FailureResponse("Failed to update product status");
        }
    }

    public async Task<ApiResponse<bool>> SetFeaturedStatusAsync(Guid actorUserId, Guid productId, bool isFeatured)
    {
        try
        {
            if (!await _permissionService.CheckUserPermissionAsync(actorUserId, "product.update"))
            {
                return ApiResponse<bool>.FailureResponse("Access denied: product.update permission required");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                return ApiResponse<bool>.FailureResponse("Product not found");
            }

            //product.IsFeatured = isFeatured;
            product.UpdatedBy = actorUserId;
            product.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product {ProductId} featured status set to {IsFeatured} by user {UserId}",
                productId, isFeatured, actorUserId);

            return ApiResponse<bool>.SuccessResponse(true, $"Product {(isFeatured ? "featured" : "unfeatured")} successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting featured status for product {ProductId}", productId);
            return ApiResponse<bool>.FailureResponse("Failed to update featured status");
        }
    }

    // ============================================
    // USER INTERACTIONS (PUBLIC)
    // ============================================

    public async Task<ApiResponse<bool>> IncrementViewCountAsync(Guid productId)
    {
        try
        {
            await _unitOfWork.Products.IncrementViewCountAsync(productId);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "View count incremented");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing view count for product {ProductId}", productId);
            return ApiResponse<bool>.FailureResponse("Failed to increment view count");
        }
    }

    public async Task<ApiResponse<bool>> ToggleFavoriteAsync(Guid userId, Guid productId)
    {
        try
        {
            // Check if product exists
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                return ApiResponse<bool>.FailureResponse("Product not found");
            }

            // Toggle favorite (repository handles the logic)
            var isFavorite = await _unitOfWork.Products.UpdateFavoriteStatusAsync(userId, productId, true);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User {UserId} toggled favorite for product {ProductId}", userId, productId);

            return ApiResponse<bool>.SuccessResponse(isFavorite,
                isFavorite ? "Product added to favorites" : "Product removed from favorites");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling favorite for product {ProductId} by user {UserId}", productId, userId);
            return ApiResponse<bool>.FailureResponse("Failed to update favorite status");
        }
    }

    public async Task<ApiResponse<List<ProductListDto>>> GetFavoriteProductsAsync(Guid userId)
    {
        try
        {
            var products = await _unitOfWork.Products.GetFavoriteProductsAsync(userId);
            var dtos = _mapper.Map<List<ProductListDto>>(products);

            return ApiResponse<List<ProductListDto>>.SuccessResponse(dtos,
                $"Retrieved {dtos.Count} favorite products");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting favorite products for user {UserId}", userId);
            return ApiResponse<List<ProductListDto>>.FailureResponse("Failed to retrieve favorite products");
        }
    }

    // ============================================
    // GET BY STORE/CATEGORY
    // ============================================

    public async Task<ApiResponse<List<ProductListDto>>> GetProductsByStoreAsync(Guid storeId, Guid? actorUserId = null)
    {
        try
        {
            var products = await _unitOfWork.Products.GetByStoreIdAsync(storeId);

            // Filter inactive if not admin
            if (!actorUserId.HasValue ||
                !await _permissionService.CheckUserPermissionAsync(actorUserId.Value, "product.list"))
            {
                products = products.Where(p => p.IsActive);
            }

            var dtos = _mapper.Map<List<ProductListDto>>(products);

            return ApiResponse<List<ProductListDto>>.SuccessResponse(dtos,
                $"Retrieved {dtos.Count} products for store");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products for store {StoreId}", storeId);
            return ApiResponse<List<ProductListDto>>.FailureResponse("Failed to retrieve store products");
        }
    }

    public async Task<ApiResponse<List<ProductListDto>>> GetProductsByCategoryAsync(Guid categoryId, Guid? actorUserId = null)
    {
        try
        {
            var products = await _unitOfWork.Products.GetByCategoryIdAsync(categoryId);

            // Filter inactive if not admin
            if (!actorUserId.HasValue ||
                !await _permissionService.CheckUserPermissionAsync(actorUserId.Value, "product.list"))
            {
                products = products.Where(p => p.IsActive);
            }

            var dtos = _mapper.Map<List<ProductListDto>>(products);

            return ApiResponse<List<ProductListDto>>.SuccessResponse(dtos,
                $"Retrieved {dtos.Count} products for category");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products for category {CategoryId}", categoryId);
            return ApiResponse<List<ProductListDto>>.FailureResponse("Failed to retrieve category products");
        }
    }
}