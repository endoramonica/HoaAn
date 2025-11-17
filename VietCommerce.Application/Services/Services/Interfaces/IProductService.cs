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

    
    /// Soft delete product
    /// Permission: product.delete
    
    Task<ApiResponse<bool>> DeleteProductAsync(Guid actorUserId, Guid productId);

    
    /// Get product by ID
    /// Permission: product.view (or public if IsActive)
    
    Task<ApiResponse<ProductDetailDto>> GetProductByIdAsync(Guid productId, Guid? actorUserId = null);

    
    /// Get product by slug (public)
    
    Task<ApiResponse<ProductDetailDto>> GetProductBySlugAsync(string slug, Guid? actorUserId = null);

    
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
}