using VietCommerce.Application.Services.Services;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Models;


namespace VietCommerce.Application.Services.Services.Interfaces;


/// Product service with permission-based authorization
/// All methods accept actorUserId for permission checking

public interface IProductService
{

    // CRUD OPERATIONS (Permission-protected)



    /// Create new product
    /// Permission: product.create

    Task<ApiResponse<ProductDetailDto>> CreateProductAsync(Guid actorUserId, ProductCreateDto dto);


    /// Update existing product
    /// Permission: product.update

    Task<ApiResponse<ProductDetailDto>> UpdateProductAsync(Guid actorUserId, Guid productId, ProductUpdateDto dto);


    /// Update package product's customizable options and details
    /// Validates new options structure and updates CustomizableOptionsJson
    /// Ensures existing orders are not affected (they have snapshots)
    /// Permission: product.update
    /// Requirements: 6.1, 6.2

    Task<ApiResponse<ProductDetailDto>> UpdatePackageProductAsync(Guid actorUserId, Guid productId, ProductUpdateDto dto);


    /// Soft delete product
    /// Permission: product.delete

    Task<ApiResponse<bool>> DeleteProductAsync(Guid actorUserId, Guid productId);


    /// Get product by ID
    /// Permission: product.view (or public if IsActive)

    Task<ApiResponse<ProductDetailDto>> GetProductByIdAsync(Guid productId, Guid? actorUserId = null);


    /// Get product by slug (public)

    Task<ApiResponse<ProductDetailDto>> GetProductBySlugAsync(string slug, Guid? actorUserId = null);

    /// Get product by code (public)
    Task<ApiResponse<ProductDetailDto>> GetProductByBarcodeAsync(
    string code,
    Guid? actorUserId = null);


    // LIST & SEARCH (Public/Protected)



    /// Get paginated product list
    /// Public: Only active products
    /// Admin (product.list): All products

    Task<ApiResponse<PaginatedResult<ProductListDto>>> GetProductsAsync(
        ProductFilterDto filter,
        Guid? actorUserId = null);


    /// Get products by store

    Task<ApiResponse<List<ProductListDto>>> GetProductsByStoreAsync(Guid storeId, Guid? actorUserId = null);


    /// Get products by category

    Task<ApiResponse<List<ProductListDto>>> GetProductsByCategoryAsync(Guid categoryId, Guid? actorUserId = null);


    // INVENTORY MANAGEMENT



    /// Update stock quantity
    /// Permission: product.update_stock

    Task<ApiResponse<bool>> UpdateStockAsync(Guid actorUserId, Guid productId, int quantity);


    /// Get current stock

    Task<ApiResponse<int>> GetStockQuantityAsync(Guid productId);


    // STATUS MANAGEMENT



    /// Activate/Deactivate product
    /// Permission: product.update

    Task<ApiResponse<bool>> SetActiveStatusAsync(Guid actorUserId, Guid productId, bool isActive);


    /// Toggle featured status
    /// Permission: product.update

    Task<ApiResponse<bool>> SetFeaturedStatusAsync(Guid actorUserId, Guid productId, bool isFeatured);


    // USER INTERACTIONS (Public)



    /// Increment view count (public)

    Task<ApiResponse<bool>> IncrementViewCountAsync(Guid productId);


    /// Toggle favorite status (authenticated users)

    Task<ApiResponse<bool>> ToggleFavoriteAsync(Guid userId, Guid productId);


    /// Get user's favorite products

    Task<ApiResponse<List<ProductListDto>>> GetFavoriteProductsAsync(Guid userId);


    // PACKAGE PRODUCT VALIDATION



    /// Validate customizable options for a package product
    /// Checks JSON structure compliance, required fields, and quantity constraints
    /// Requirements: 1.2, 1.3, 7.2

    Task<ValidationResult> ValidateCustomizableOptionsAsync(List<CustomizableOptionDto> options);


    /// Validate customer customizations against a product's customizable options
    /// Verifies option exists and quantity is within min/max bounds
    /// Requirements: 3.1, 7.3

    Task<ValidationResult> ValidateCustomizationsAsync(Guid productId, List<CartItemCustomizationDto> customizations);
}