using AutoMapper;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Helpers;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services;

/// <summary>
/// Service quản lý sản phẩm - kế thừa BaseService
/// Tích hợp: Redis Cache, Permission, Logging, Exception Handling
/// VERSION: 2.0
/// LAST UPDATED: 2025-10-31
/// </summary>
public class ProductService : BaseService, IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;
    private readonly CustomizableOptionValidator _customizableOptionValidator;

    // Cache keys prefix
    private const string CACHE_KEY_PRODUCT = "product";
    private const string CACHE_KEY_PRODUCT_LIST = "product:list";
    private const string CACHE_KEY_PRODUCT_STORE = "product:store";
    private const string CACHE_KEY_PRODUCT_CATEGORY = "product:category";
    private const string CACHE_KEY_FAVORITES = "product:favorites";

    // Cache duration
    private static readonly TimeSpan CACHE_DURATION_DETAIL = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan CACHE_DURATION_LIST = TimeSpan.FromMinutes(15);

    public ProductService(
        IUnitOfWork unitOfWork,
        IPermissionService permissionService,
        ILogger<ProductService> logger,
        IMapper mapper,
        ICacheService cacheService,
        CustomizableOptionValidator customizableOptionValidator)
        : base(logger, cacheService)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _mapper = mapper;
        _customizableOptionValidator = customizableOptionValidator ?? throw new ArgumentNullException(nameof(customizableOptionValidator));
    }

    // ============================================
    // CREATE
    // ============================================
    public async Task<ApiResponse<ProductDetailDto>> CreateProductAsync(Guid actorUserId, ProductCreateDto dto)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            // ✅ PERMISSION CHECK
            ValidateId(actorUserId, nameof(actorUserId));
            ValidateNotNull(dto, nameof(dto));

            if (!await _permissionService.CheckUserPermissionAsync(actorUserId, "product.create"))
            {
                LogWarning("User {UserId} attempted to create product without permission", actorUserId);
                throw new UnauthorizedAccessException("Access denied: product.create permission required");
            }

            // ✅ VALIDATE: Code uniqueness
            if (await _unitOfWork.Products.ExistsByCodeAsync(dto.Code))
            {
                throw new InvalidOperationException($"Product code '{dto.Code}' already exists");
            }

            // ✅ VALIDATE: Package product fields (if present)
            if (dto.CustomizableOptions != null && dto.CustomizableOptions.Count > 0)
            {
                var validationResult = await ValidateCustomizableOptionsAsync(dto.CustomizableOptions);
                if (!validationResult.IsValid)
                {
                    throw new InvalidOperationException($"Customizable options validation failed: {string.Join("; ", validationResult.Errors)}");
                }
            }

            // ✅ MAP & CREATE
            var product = _mapper.Map<Product>(dto);
            product.Id = Guid.NewGuid();
            product.Slug = SlugHelper.GenerateSlug(dto.Name);
            product.CreatedBy = actorUserId;
            product.UpdatedBy = actorUserId;
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            product.StoreId = Guid.Parse("47AA5519-C503-4CFA-8101-2EDB36FD9D8C");

            // ✅ SERIALIZE: Package product fields
            if (dto.Details != null && dto.Details.Count > 0)
            {
                product.DetailsJson = JsonSerializationHelper.SerializeDetails(dto.Details);
                LogInfo("✅ Serialized {Count} product details", dto.Details.Count);
            }

            if (dto.CustomizableOptions != null && dto.CustomizableOptions.Count > 0)
            {
                product.CustomizableOptionsJson = JsonSerializationHelper.SerializeCustomizableOptions(dto.CustomizableOptions);
                LogInfo("✅ Serialized {Count} customizable options", dto.CustomizableOptions.Count);
            }

            // ✅ SAVE
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            // ✅ INVALIDATE CACHE
            await InvalidateProductCachesAsync(product.StoreId, product.CategoryId);

            // ✅ FETCH & RETURN
            var createdProduct = await _unitOfWork.Products.GetByIdAsync(product.Id);
            var result = _mapper.Map<ProductDetailDto>(createdProduct);

            LogInfo("✅ Product created: {ProductId} by user {UserId}", product.Id, actorUserId);
            return result;
        },
        "CreateProduct",
        "Product created successfully");
    }

    // ============================================
    // UPDATE
    // ============================================
    public async Task<ApiResponse<ProductDetailDto>> UpdateProductAsync(
        Guid actorUserId,
        Guid productId,
        ProductUpdateDto dto)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            // ✅ VALIDATE
            ValidateId(actorUserId, nameof(actorUserId));
            ValidateId(productId, nameof(productId));
            ValidateNotNull(dto, nameof(dto));

            // ✅ PERMISSION CHECK
            if (!await _permissionService.CheckUserPermissionAsync(actorUserId, "product.update"))
            {
                LogWarning("User {UserId} attempted to update product {ProductId} without permission",
                    actorUserId, productId);
                throw new UnauthorizedAccessException("Access denied: product.update permission required");
            }

            // ✅ FETCH EXISTING
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // ✅ UPDATE FIELDS
            if (dto.Name != null)
            {
                product.Name = dto.Name;
                product.Slug = SlugHelper.GenerateSlug(dto.Name);
            }
            if (dto.CategoryId.HasValue) product.CategoryId = dto.CategoryId;
            if (dto.Sku != null) product.SKU = dto.Sku;
            if (dto.StockQuantity.HasValue) product.Stock = dto.StockQuantity.Value;
            if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;

            product.UpdatedBy = actorUserId;
            product.UpdatedAt = DateTime.UtcNow;

            // ✅ SAVE
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            // ✅ INVALIDATE CACHE
            await InvalidateProductCachesAsync(product.StoreId, product.CategoryId, productId);

            // ✅ RETURN UPDATED
            var updated = await _unitOfWork.Products.GetByIdAsync(productId);
            var result = _mapper.Map<ProductDetailDto>(updated);

            LogInfo("✅ Product {ProductId} updated by user {UserId}", productId, actorUserId);
            return result;
        },
        "UpdateProduct",
        "Product updated successfully");
    }

    /// <summary>
    /// Updates a package product's customizable options and details.
    /// Validates new options structure and updates CustomizableOptionsJson.
    /// Ensures existing orders are not affected (they have snapshots).
    /// Requirements: 6.1, 6.2
    /// </summary>
    public async Task<ApiResponse<ProductDetailDto>> UpdatePackageProductAsync(
        Guid actorUserId,
        Guid productId,
        ProductUpdateDto dto)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            // ✅ VALIDATE
            ValidateId(actorUserId, nameof(actorUserId));
            ValidateId(productId, nameof(productId));
            ValidateNotNull(dto, nameof(dto));

            // ✅ PERMISSION CHECK
            if (!await _permissionService.CheckUserPermissionAsync(actorUserId, "product.update"))
            {
                LogWarning("User {UserId} attempted to update package product {ProductId} without permission",
                    actorUserId, productId);
                throw new UnauthorizedAccessException("Access denied: product.update permission required");
            }

            // ✅ FETCH EXISTING PRODUCT
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // ✅ VALIDATE NEW OPTIONS STRUCTURE (if provided)
            if (dto.CustomizableOptions != null && dto.CustomizableOptions.Count > 0)
            {
                var validationResult = await ValidateCustomizableOptionsAsync(dto.CustomizableOptions);
                if (!validationResult.IsValid)
                {
                    throw new InvalidOperationException($"Customizable options validation failed: {string.Join("; ", validationResult.Errors)}");
                }
            }

            // ✅ UPDATE PACKAGE FIELDS
            // Update Details if provided
            if (dto.Details != null)
            {
                if (dto.Details.Count > 0)
                {
                    product.DetailsJson = JsonSerializationHelper.SerializeDetails(dto.Details);
                    LogInfo("✅ Updated {Count} product details", dto.Details.Count);
                }
                else
                {
                    product.DetailsJson = null;
                    LogInfo("✅ Cleared product details");
                }
            }

            // Update CustomizableOptions if provided
            if (dto.CustomizableOptions != null)
            {
                if (dto.CustomizableOptions.Count > 0)
                {
                    product.CustomizableOptionsJson = JsonSerializationHelper.SerializeCustomizableOptions(dto.CustomizableOptions);
                    LogInfo("✅ Updated {Count} customizable options", dto.CustomizableOptions.Count);
                }
                else
                {
                    product.CustomizableOptionsJson = null;
                    LogInfo("✅ Cleared customizable options");
                }
            }

            // ✅ UPDATE STANDARD FIELDS (from base UpdateProductAsync logic)
            if (dto.Name != null)
            {
                product.Name = dto.Name;
                product.Slug = SlugHelper.GenerateSlug(dto.Name);
            }
            if (dto.CategoryId.HasValue) product.CategoryId = dto.CategoryId;
            if (dto.Sku != null) product.SKU = dto.Sku;
            if (dto.StockQuantity.HasValue) product.Stock = dto.StockQuantity.Value;
            if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;

            product.UpdatedBy = actorUserId;
            product.UpdatedAt = DateTime.UtcNow;

            // ✅ SAVE
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            // ✅ INVALIDATE CACHE
            // Note: Existing orders are NOT affected because they have snapshots of customizations
            // in OrderItem.CustomizationsJson, BasePrice, and CustomizationPrice
            await InvalidateProductCachesAsync(product.StoreId, product.CategoryId, productId);

            // ✅ FETCH & RETURN UPDATED
            var updated = await _unitOfWork.Products.GetByIdAsync(productId);
            var result = _mapper.Map<ProductDetailDto>(updated);

            LogInfo("✅ Package product {ProductId} updated by user {UserId}. Existing orders unaffected (snapshots preserved).",
                productId, actorUserId);
            return result;
        },
        "UpdatePackageProduct",
        "Package product updated successfully. Existing orders are not affected.");
    }

    // ============================================
    // DELETE (SOFT DELETE)
    // ============================================
    public async Task<ApiResponse<bool>> DeleteProductAsync(Guid actorUserId, Guid productId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            // ✅ VALIDATE
            ValidateId(actorUserId, nameof(actorUserId));
            ValidateId(productId, nameof(productId));

            // ✅ PERMISSION CHECK
            if (!await _permissionService.CheckUserPermissionAsync(actorUserId, "product.delete"))
            {
                LogWarning("User {UserId} attempted to delete product {ProductId} without permission",
                    actorUserId, productId);
                throw new UnauthorizedAccessException("Access denied: product.delete permission required");
            }

            // ✅ SOFT DELETE
            var success = await _unitOfWork.Products.SoftDeleteAsync(productId, actorUserId);
            if (!success)
            {
                throw new InvalidOperationException("Product not found or already deleted");
            }

            await _unitOfWork.SaveChangesAsync();

            // ✅ INVALIDATE CACHE
            await InvalidateProductCachesAsync(null, null, productId);

            LogInfo("✅ Product {ProductId} deleted by user {UserId}", productId, actorUserId);
            return true;
        },
        "DeleteProduct",
        "Product deleted successfully");
    }

    // ============================================
    // GET BY ID (WITH CACHE)
    // ============================================
    public async Task<ApiResponse<ProductDetailDto>> GetProductByIdAsync(Guid productId, Guid? actorUserId = null)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(productId, nameof(productId));

            var cacheKey = CreateCacheKey(CACHE_KEY_PRODUCT, productId);

            var product = await GetFromCacheOrExecuteAsync(
                cacheKey,
                async () => await _unitOfWork.Products.GetByIdAsync(productId),
                CACHE_DURATION_DETAIL
            );

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // ✅ PERMISSION CHECK: Inactive products require permission
            if (!product.IsActive)
            {
                if (!actorUserId.HasValue ||
                    !await _permissionService.CheckUserPermissionAsync(actorUserId.Value, "product.view"))
                {
                    throw new UnauthorizedAccessException("Product not available");
                }
            }

            var result = _mapper.Map<ProductDetailDto>(product);
            return result;
        },
        "GetProductById",
        "Product retrieved successfully");
    }

    // ============================================
    // GET BY SLUG (WITH CACHE)
    // ============================================
    public async Task<ApiResponse<ProductDetailDto>> GetProductBySlugAsync(string slug, Guid? actorUserId = null)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateNotEmpty(slug, nameof(slug));

            var cacheKey = CreateCacheKey(CACHE_KEY_PRODUCT, "slug", slug);

            var product = await GetFromCacheOrExecuteAsync(
                cacheKey,
                async () => await _unitOfWork.Products.GetBySlugAsync(slug),
                CACHE_DURATION_DETAIL
            );

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // Check active status
            if (!product.IsActive)
            {
                if (!actorUserId.HasValue ||
                    !await _permissionService.CheckUserPermissionAsync(actorUserId.Value, "product.view"))
                {
                    throw new UnauthorizedAccessException("Product not available");
                }
            }

            var result = _mapper.Map<ProductDetailDto>(product);
            return result;
        },
        "GetProductBySlug",
        "Product retrieved successfully");
    }
    public async Task<ApiResponse<ProductDetailDto>> GetProductByBarcodeAsync(
    string code,
    Guid? actorUserId = null)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            // Validate
            ValidateNotEmpty(code, nameof(code));

            // Cache key
            var cacheKey = CreateCacheKey(CACHE_KEY_PRODUCT, "barcode", code);

            // Fetch from cache or DB
            var product = await GetFromCacheOrExecuteAsync(
                cacheKey,
                async () => await _unitOfWork.Products.GetByCodeAsync(code),
                CACHE_DURATION_DETAIL
            );

            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // Check active status
            if (!product.IsActive)
            {
                // User must have permission to view inactive product
                if (!actorUserId.HasValue ||
                    !await _permissionService.CheckUserPermissionAsync(actorUserId.Value, "product.view"))
                {
                    throw new UnauthorizedAccessException("Product not available");
                }
            }

            // Map to DTO
            var result = _mapper.Map<ProductDetailDto>(product);

            return result;
        },
        "GetProductByBarcode",
        "Product retrieved successfully");
    }

    // ============================================
    // LIST (PAGINATED) - WITH CACHE
    // ============================================
    public async Task<ApiResponse<PaginatedResult<ProductListDto>>> GetProductsAsync(
        ProductFilterDto filter,
        Guid? actorUserId = null)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateNotNull(filter, nameof(filter));

            // ✅ CHECK PERMISSION
            bool showAll = false;
            if (actorUserId.HasValue)
            {
                showAll = await _permissionService.CheckUserPermissionAsync(actorUserId.Value, "product.list");
            }

            // Override IsActive filter if not admin
            if (!showAll)
            {
                filter.IsActive = true;
            }

            // ✅ CREATE CACHE KEY (🔧 SERVICES-PRODUCT UNIFICATION - Added type and serviceCategory)
            var cacheKey = CreateCacheKey(
                CACHE_KEY_PRODUCT_LIST,
                filter.Page,
                filter.PageSize,
                filter.SearchTerm ?? "all",
                filter.CategoryId?.ToString() ?? "all",
                filter.StoreId?.ToString() ?? "all",
                filter.IsActive?.ToString() ?? "all",
                filter.MinPrice?.ToString() ?? "0",
                filter.MaxPrice?.ToString() ?? "max",
                filter.SortBy ?? "default",
                filter.IsDescending,
                filter.Type ?? "all",
                filter.ServiceCategory ?? "all"
            );

            // ✅ GET FROM CACHE OR EXECUTE (🔧 SERVICES-PRODUCT UNIFICATION - Added type and serviceCategory)
            var paginatedResult = await GetFromCacheOrExecuteAsync(
                cacheKey,
                async () => await _unitOfWork.Products.GetPaginatedDtoAsync(
                    filter.Page,
                    filter.PageSize,
                    filter.SearchTerm,
                    filter.CategoryId,
                    filter.StoreId,
                    filter.IsActive,
                    filter.MinPrice,
                    filter.MaxPrice,
                    filter.SortBy,
                    filter.IsDescending,
                    filter.Type,
                    filter.ServiceCategory
                ),
                CACHE_DURATION_LIST
            );
            // ✅ Trả trực tiếp DTO
            var result = paginatedResult;

            return result;

        },
        "GetProducts",
        $"Products retrieved successfully");
    }

    // ============================================
    // STOCK MANAGEMENT
    // ============================================
    public async Task<ApiResponse<bool>> UpdateStockAsync(Guid actorUserId, Guid productId, int quantity)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(actorUserId, nameof(actorUserId));
            ValidateId(productId, nameof(productId));

            if (!await _permissionService.CheckUserPermissionAsync(actorUserId, "product.update_stock"))
            {
                throw new UnauthorizedAccessException("Access denied: product.update_stock permission required");
            }

            var success = await _unitOfWork.Products.UpdateStockAsync(productId, quantity);
            if (!success)
            {
                throw new KeyNotFoundException("Product not found");
            }

            await _unitOfWork.SaveChangesAsync();

            // ✅ INVALIDATE CACHE
            await InvalidateCacheAsync(CreateCacheKey(CACHE_KEY_PRODUCT, productId));

            LogInfo("✅ Stock updated for product {ProductId}: {Quantity} by user {UserId}",
                productId, quantity, actorUserId);

            return true;
        },
        "UpdateStock",
        "Stock updated successfully");
    }

    public async Task<ApiResponse<int>> GetStockQuantityAsync(Guid productId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(productId, nameof(productId));

            var stock = await _unitOfWork.Products.GetStockQuantityAsync(productId);
            return stock;
        },
        "GetStockQuantity",
        "Stock retrieved successfully");
    }

    // ============================================
    // STATUS MANAGEMENT
    // ============================================
    public async Task<ApiResponse<bool>> SetActiveStatusAsync(Guid actorUserId, Guid productId, bool isActive)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(actorUserId, nameof(actorUserId));
            ValidateId(productId, nameof(productId));

            if (!await _permissionService.CheckUserPermissionAsync(actorUserId, "product.update"))
            {
                throw new UnauthorizedAccessException("Access denied: product.update permission required");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            product.IsActive = isActive;
            product.UpdatedBy = actorUserId;
            product.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            // ✅ INVALIDATE CACHE
            await InvalidateProductCachesAsync(product.StoreId, product.CategoryId, productId);

            LogInfo("✅ Product {ProductId} active status set to {IsActive} by user {UserId}",
                productId, isActive, actorUserId);

            return true;
        },
        "SetActiveStatus",
        $"Product {(isActive ? "activated" : "deactivated")} successfully");
    }

    public async Task<ApiResponse<bool>> SetFeaturedStatusAsync(Guid actorUserId, Guid productId, bool isFeatured)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(actorUserId, nameof(actorUserId));
            ValidateId(productId, nameof(productId));

            if (!await _permissionService.CheckUserPermissionAsync(actorUserId, "product.update"))
            {
                throw new UnauthorizedAccessException("Access denied: product.update permission required");
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            // Note: IsFeatured property commented out in original code
            product.UpdatedBy = actorUserId;
            product.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            // ✅ INVALIDATE CACHE
            await InvalidateCacheAsync(CreateCacheKey(CACHE_KEY_PRODUCT, productId));

            LogInfo("✅ Product {ProductId} featured status set to {IsFeatured} by user {UserId}",
                productId, isFeatured, actorUserId);

            return true;
        },
        "SetFeaturedStatus",
        $"Product {(isFeatured ? "featured" : "unfeatured")} successfully");
    }

    // ============================================
    // USER INTERACTIONS
    // ============================================
    public async Task<ApiResponse<bool>> IncrementViewCountAsync(Guid productId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(productId, nameof(productId));

            await _unitOfWork.Products.IncrementViewCountAsync(productId);
            await _unitOfWork.SaveChangesAsync();

            // ✅ INVALIDATE CACHE (view count changed)
            await InvalidateCacheAsync(CreateCacheKey(CACHE_KEY_PRODUCT, productId));

            return true;
        },
        "IncrementViewCount",
        "View count incremented");
    }

    public async Task<ApiResponse<bool>> ToggleFavoriteAsync(Guid userId, Guid productId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(userId, nameof(userId));
            ValidateId(productId, nameof(productId));

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new KeyNotFoundException("Product not found");
            }

            var isFavorite = await _unitOfWork.Products.UpdateFavoriteStatusAsync(userId, productId, true);
            await _unitOfWork.SaveChangesAsync();

            // ✅ INVALIDATE FAVORITES CACHE
            await InvalidateCacheAsync(CreateCacheKey(CACHE_KEY_FAVORITES, userId));

            LogInfo("✅ User {UserId} toggled favorite for product {ProductId}", userId, productId);

            return isFavorite;
        },
        "ToggleFavorite",
        "Favorite status updated");
    }

    public async Task<ApiResponse<List<ProductListDto>>> GetFavoriteProductsAsync(Guid userId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(userId, nameof(userId));

            var cacheKey = CreateCacheKey(CACHE_KEY_FAVORITES, userId);

            var products = await GetFromCacheOrExecuteAsync(
                cacheKey,
                async () => await _unitOfWork.Products.GetFavoriteProductsAsync(userId),
                CACHE_DURATION_LIST
            );

            var dtos = _mapper.Map<List<ProductListDto>>(products);
            return dtos;
        },
        "GetFavoriteProducts",
        "Favorite products retrieved successfully");
    }

    // ============================================
    // GET BY STORE/CATEGORY (WITH CACHE)
    // ============================================
    public async Task<ApiResponse<List<ProductListDto>>> GetProductsByStoreAsync(Guid storeId, Guid? actorUserId = null)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(storeId, nameof(storeId));

            var cacheKey = CreateCacheKey(CACHE_KEY_PRODUCT_STORE, storeId);

            var products = await GetFromCacheOrExecuteAsync(
                cacheKey,
                async () => await _unitOfWork.Products.GetByStoreIdAsync(storeId),
                CACHE_DURATION_LIST
            );

            // Filter inactive if not admin
            if (!actorUserId.HasValue ||
                !await _permissionService.CheckUserPermissionAsync(actorUserId.Value, "product.list"))
            {
                products = products.Where(p => p.IsActive);
            }

            var dtos = _mapper.Map<List<ProductListDto>>(products);
            return dtos;
        },
        "GetProductsByStore",
        "Store products retrieved successfully");
    }

    public async Task<ApiResponse<List<ProductListDto>>> GetProductsByCategoryAsync(Guid categoryId, Guid? actorUserId = null)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            ValidateId(categoryId, nameof(categoryId));

            var cacheKey = CreateCacheKey(CACHE_KEY_PRODUCT_CATEGORY, categoryId);

            var products = await GetFromCacheOrExecuteAsync(
                cacheKey,
                async () => await _unitOfWork.Products.GetByCategoryIdAsync(categoryId),
                CACHE_DURATION_LIST
            );

            // Filter inactive if not admin
            if (!actorUserId.HasValue ||
                !await _permissionService.CheckUserPermissionAsync(actorUserId.Value, "product.list"))
            {
                products = products.Where(p => p.IsActive);
            }

            var dtos = _mapper.Map<List<ProductListDto>>(products);
            return dtos;
        },
        "GetProductsByCategory",
        "Category products retrieved successfully");
    }

    // ============================================
    // PACKAGE PRODUCT VALIDATION
    // ============================================
    public async Task<ValidationResult> ValidateCustomizableOptionsAsync(List<CustomizableOptionDto> options)
    {
        return await _customizableOptionValidator.ValidateCustomizableOptionsAsync(options);
    }

    public async Task<ValidationResult> ValidateCustomizationsAsync(
        Guid productId,
        List<CartItemCustomizationDto> customizations)
    {
        return await ExecuteAsync(async () =>
        {
            // Validate input
            if (customizations == null || customizations.Count == 0)
            {
                LogWarning("❌ Customizations list is null or empty");
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new[] { "Customizations list cannot be null or empty" }
                };
            }

            // Fetch product
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                LogWarning("❌ Product {ProductId} not found", productId);
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new[] { "Product not found" }
                };
            }

            // Deserialize customizable options from product
            var options = JsonSerializationHelper.DeserializeCustomizableOptions(product.CustomizableOptionsJson);
            if (options.Count == 0)
            {
                LogWarning("❌ Product {ProductId} has no customizable options", productId);
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = new[] { "Product does not have customizable options" }
                };
            }

            // Validate each customization
            var allErrors = new List<string>();
            var customizationIndex = 0;

            foreach (var customization in customizations)
            {
                // Find matching option
                var option = options.FirstOrDefault(o => o.Id == customization.OptionId);
                if (option == null)
                {
                    allErrors.Add($"Customization {customizationIndex} - Option ID '{customization.OptionId}' not found in product");
                    customizationIndex++;
                    continue;
                }

                // Validate quantity within bounds
                if (customization.Quantity < option.MinQuantity)
                {
                    allErrors.Add($"Customization {customizationIndex} - Quantity {customization.Quantity} is below minimum {option.MinQuantity}");
                }

                if (option.MaxQuantity.HasValue && customization.Quantity > option.MaxQuantity.Value)
                {
                    allErrors.Add($"Customization {customizationIndex} - Quantity {customization.Quantity} exceeds maximum {option.MaxQuantity.Value}");
                }

                customizationIndex++;
            }

            // Return result
            if (allErrors.Any())
            {
                LogWarning($"❌ Customizations validation failed with {allErrors.Count} error(s)");
                return new ValidationResult
                {
                    IsValid = false,
                    Errors = allErrors.ToArray()
                };
            }

            LogInfo("✅ Customizations validation passed for product {ProductId}", productId);
            return new ValidationResult
            {
                IsValid = true,
                Errors = Array.Empty<string>()
            };
        },
        "ValidateCustomizations");
    }

    // ============================================
    // PRIVATE HELPER METHODS
    // ============================================
    private async Task InvalidateProductCachesAsync(
    Guid? storeId,
    Guid? categoryId,
    Guid? productId = null)
    {
        var tasks = new List<Task>
    {
        // Invalidate list caches
        InvalidateCacheByPrefixAsync($"{CACHE_KEY_PRODUCT_LIST}:*")
    };

        if (productId.HasValue)
        {
            tasks.Add(InvalidateCacheAsync(CreateCacheKey(CACHE_KEY_PRODUCT, productId.Value)));
        }

        if (storeId.HasValue)
        {
            tasks.Add(InvalidateCacheAsync(CreateCacheKey(CACHE_KEY_PRODUCT_STORE, storeId.Value)));
        }

        if (categoryId.HasValue)
        {
            tasks.Add(InvalidateCacheAsync(CreateCacheKey(CACHE_KEY_PRODUCT_CATEGORY, categoryId.Value)));
        }

        await Task.WhenAll(tasks);
    }

}