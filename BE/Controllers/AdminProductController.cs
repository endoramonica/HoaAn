using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Models;

namespace VietCommerce.AdminApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IProductService productService,
            ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }


        // HELPER: Get current user ID from JWT claims


        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }


        // CREATE (Protected)



        /// <summary>
        /// Create a new product (regular or package product with customizable options).
        /// 
        /// For package products:
        /// - Include Details array with fixed items (e.g., "Cá chép (3 con)", "Mũ giấy (3 cái)")
        /// - Include CustomizableOptions array for items customers can modify
        /// 
        /// For regular products:
        /// - Leave Details and CustomizableOptions null or empty
        /// </summary>
        /// <remarks>
        /// Required permission: product.create
        /// 
        /// Example package product request:
        /// {
        ///   "name": "Mâm Cúng Ông Công Ông Táo",
        ///   "code": "MAM-CONG-TAO-001",
        ///   "price": 3500000,
        ///   "stockQuantity": 10,
        ///   "details": ["Cá chép (3 con)", "Mũ giấy (3 cái)"],
        ///   "customizableOptions": [
        ///     {
        ///       "id": "opt-xoi",
        ///       "name": "Xôi gấc đậu xanh",
        ///       "baseQuantity": 5,
        ///       "unitPrice": 45000,
        ///       "minQuantity": 5,
        ///       "maxQuantity": 100,
        ///       "unit": "dĩa"
        ///     }
        ///   ]
        /// }
        /// </remarks>

        [HttpPost]
        [Authorize] // JWT required
        [ProducesResponseType(typeof(ApiResponse<ProductDetailDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCreateDto dto)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _productService.CreateProductAsync(userId.Value, dto);

            if (!result.Success)
            {
                return result.Message.Contains("Access denied")
                    ? Forbid()
                    : BadRequest(result);
            }

            return Ok(result);
        }


        // UPDATE (Protected)



        /// <summary>
        /// Update an existing product (regular or package product).
        /// 
        /// For package products:
        /// - Include Details array to update fixed items
        /// - Include CustomizableOptions array to update customizable items
        /// - Changes apply only to new orders; existing orders retain snapshots
        /// 
        /// For regular products:
        /// - Leave Details and CustomizableOptions null or empty
        /// </summary>
        /// <remarks>
        /// Required permission: product.update
        /// 
        /// Example package product update request:
        /// {
        ///   "name": "Mâm Cúng Ông Công Ông Táo - Updated",
        ///   "price": 3600000,
        ///   "details": ["Cá chép (3 con)", "Mũ giấy (3 cái)", "Hương đèn (1 bộ)"],
        ///   "customizableOptions": [
        ///     {
        ///       "id": "opt-xoi",
        ///       "name": "Xôi gấc đậu xanh",
        ///       "baseQuantity": 5,
        ///       "unitPrice": 50000,
        ///       "minQuantity": 5,
        ///       "maxQuantity": 100,
        ///       "unit": "dĩa"
        ///     }
        ///   ]
        /// }
        /// </remarks>

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<ProductDetailDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            // Check if this is a package product update (has Details or CustomizableOptions)
            bool isPackageUpdate = (dto.Details != null && dto.Details.Count > 0) ||
                                   (dto.CustomizableOptions != null && dto.CustomizableOptions.Count > 0);

            ApiResponse<ProductDetailDto> result;

            if (isPackageUpdate)
            {
                // Use UpdatePackageProductAsync for package product updates
                result = await _productService.UpdatePackageProductAsync(userId.Value, id, dto);
            }
            else
            {
                // Use standard UpdateProductAsync for regular product updates
                result = await _productService.UpdateProductAsync(userId.Value, id, dto);
            }

            if (!result.Success)
            {
                if (result.Message.Contains("Access denied"))
                    return Forbid();
                if (result.Message.Contains("not found"))
                    return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }


        // DELETE (Protected)



        /// Soft delete a product


        /// Required permission: product.delete

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _productService.DeleteProductAsync(userId.Value, id);

            if (!result.Success)
            {
                if (result.Message.Contains("Access denied"))
                    return Forbid();
                if (result.Message.Contains("not found"))
                    return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }


        // GET BY ID (Public/Protected)



        /// Get product by ID


        /// Public: Only active products
        /// Admin (product.view): All products

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ProductDetailDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _productService.GetProductByIdAsync(id, userId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }


        // GET BY SLUG (Public)



        /// Get product by slug (SEO-friendly URL)

        [HttpGet("slug/{slug}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ProductDetailDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProductBySlug(string slug)
        {
            var userId = GetCurrentUserId();
            var result = await _productService.GetProductBySlugAsync(slug, userId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }


        // LIST (Public/Protected)



        /// Get paginated product list


        /// Public: Only active products
        /// Admin (product.list): All products

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ProductListDto>>), 200)]
        public async Task<IActionResult> GetProducts([FromQuery] ProductFilterDto filter)
        {
            var userId = GetCurrentUserId();
            var result = await _productService.GetProductsAsync(filter, userId);

            // ✅ Return đúng cấu trúc - không wrap ApiResponse
            return Ok(new
            {
                success = true,
                data = result,
                message = "Products retrieved successfully"
            });
        }


        // GET BY STORE



        /// Get products by store ID

        [HttpGet("store/{storeId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<List<ProductListDto>>), 200)]
        public async Task<IActionResult> GetProductsByStore(Guid storeId)
        {
            var userId = GetCurrentUserId();
            var result = await _productService.GetProductsByStoreAsync(storeId, userId);

            return Ok(result);
        }


        // GET BY CATEGORY



        /// Get products by category ID

        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<List<ProductListDto>>), 200)]
        public async Task<IActionResult> GetProductsByCategory(Guid categoryId)
        {
            var userId = GetCurrentUserId();
            var result = await _productService.GetProductsByCategoryAsync(categoryId, userId);

            return Ok(result);
        }


        // STOCK MANAGEMENT



        /// Update product stock quantity


        /// Required permission: product.update_stock

        [HttpPatch("{id}/stock")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateStock(Guid id, [FromBody] int quantity)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _productService.UpdateStockAsync(userId.Value, id, quantity);

            if (!result.Success && result.Message.Contains("Access denied"))
            {
                return Forbid();
            }

            return Ok(result);
        }


        /// Get current stock quantity

        [HttpGet("{id}/stock")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        public async Task<IActionResult> GetStock(Guid id)
        {
            var result = await _productService.GetStockQuantityAsync(id);
            return Ok(result);
        }


        // STATUS MANAGEMENT



        /// Activate/deactivate product


        /// Required permission: product.update

        [HttpPatch("{id}/active")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> SetActiveStatus(Guid id, [FromBody] bool isActive)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _productService.SetActiveStatusAsync(userId.Value, id, isActive);

            if (!result.Success && result.Message.Contains("Access denied"))
            {
                return Forbid();
            }

            return Ok(result);
        }


        /// Toggle featured status


        /// Required permission: product.update

        [HttpPatch("{id}/featured")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> SetFeaturedStatus(Guid id, [FromBody] bool isFeatured)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _productService.SetFeaturedStatusAsync(userId.Value, id, isFeatured);

            if (!result.Success && result.Message.Contains("Access denied"))
            {
                return Forbid();
            }

            return Ok(result);
        }


        // USER INTERACTIONS



        /// Increment product view count

        [HttpPost("{id}/view")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> IncrementViewCount(Guid id)
        {
            var result = await _productService.IncrementViewCountAsync(id);
            return Ok(result);
        }


        /// Toggle product favorite status

        [HttpPost("{id}/favorite")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> ToggleFavorite(Guid id)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _productService.ToggleFavoriteAsync(userId.Value, id);
            return Ok(result);
        }


        /// Get user's favorite products

        [HttpGet("favorites")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<List<ProductListDto>>), 200)]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var result = await _productService.GetFavoriteProductsAsync(userId.Value);
            return Ok(result);
        }
    }
}
