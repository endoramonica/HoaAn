using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Application.Services.EndUser.EndUser_Interfaces;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Wishlist;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.EndUser
{
    public class WishlistService : BaseService, IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper; // ✅ THÊM
        private const string CACHE_PREFIX = "wishlist";

        public WishlistService(
            ILogger<WishlistService> logger,
            ICacheService cacheService,
            IUnitOfWork unitOfWork,
            IMapper mapper) // ✅ THÊM PARAMETER
            : base(logger, cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper; // ✅ THÊM
        }

        public async Task<ApiResponse<List<WishlistItemDto>>> GetWishlistAsync(Guid userId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId, nameof(userId));

                var cacheKey = CreateCacheKey(CACHE_PREFIX, "user", userId);

                return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
                {
                    var items = await _unitOfWork.ProductFavorites.GetByUserIdWithProductAsync(userId);

                    // ✅ CẬP NHẬT: Dùng AutoMapper thay vì MapToDto manual
                    return _mapper.Map<List<WishlistItemDto>>(items);
                }, TimeSpan.FromMinutes(30));

            }, "GetWishlist", "Lấy danh sách yêu thích thành công");
        }

        public async Task<ApiResponse<WishlistItemDto>> AddToWishlistAsync(Guid userId, Guid productId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId, nameof(userId));
                ValidateId(productId, nameof(productId));

                // Kiểm tra sản phẩm tồn tại
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                ThrowIf(product == null, "Sản phẩm không tồn tại");
                ThrowIf(product!.IsDeleted, "Sản phẩm đã bị xóa");

                // Kiểm tra đã có trong wishlist chưa
                var exists = await _unitOfWork.ProductFavorites.ExistsByUserAndProductAsync(userId, productId);
                ThrowIf(exists, "Sản phẩm đã có trong danh sách yêu thích");

                // Thêm vào wishlist
                var favorite = new ProductFavorite
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ProductId = productId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.ProductFavorites.AddAsync(favorite);

                // Update FavoriteCount
                product.FavoriteCount++;
                _unitOfWork.Products.Update(product);

                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                await InvalidateWishlistCache(userId, productId);

                // ✅ CẬP NHẬT: Load lại với AutoMapper
                var added = await _unitOfWork.ProductFavorites.GetByUserAndProductAsync(userId, productId);
                var items = await _unitOfWork.ProductFavorites.GetByUserIdWithProductAsync(userId);
                var result = items.FirstOrDefault(x => x.Id == added!.Id);

                return _mapper.Map<WishlistItemDto>(result);

            }, "AddToWishlist", "Đã thêm vào danh sách yêu thích");
        }

        public async Task<ApiResponse<bool>> RemoveFromWishlistAsync(Guid userId, Guid wishlistItemId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId, nameof(userId));
                ValidateId(wishlistItemId, nameof(wishlistItemId));

                var item = await _unitOfWork.ProductFavorites.GetByIdAsync(wishlistItemId);
                ThrowIf(item == null, "Không tìm thấy item trong wishlist");
                ThrowIf(item!.UserId != userId, "Không có quyền xóa item này");

                var productId = item.ProductId;

                // Xóa wishlist item
                _unitOfWork.ProductFavorites.Delete(item);

                // Update FavoriteCount
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product != null && product.FavoriteCount > 0)
                {
                    product.FavoriteCount--;
                    _unitOfWork.Products.Update(product);
                }

                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                await InvalidateWishlistCache(userId, productId);

                return true;

            }, "RemoveFromWishlist", "Đã xóa khỏi danh sách yêu thích");
        }

        public async Task<ApiResponse<bool>> RemoveByProductIdAsync(Guid userId, Guid productId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId, nameof(userId));
                ValidateId(productId, nameof(productId));

                var removed = await _unitOfWork.ProductFavorites.RemoveByUserAndProductAsync(userId, productId);
                ThrowIf(!removed, "Sản phẩm không có trong danh sách yêu thích");

                // Update FavoriteCount
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product != null && product.FavoriteCount > 0)
                {
                    product.FavoriteCount--;
                    _unitOfWork.Products.Update(product);
                }

                await _unitOfWork.SaveChangesAsync();

                // Invalidate cache
                await InvalidateWishlistCache(userId, productId);

                return true;

            }, "RemoveByProductId", "Đã xóa khỏi danh sách yêu thích");
        }

        public async Task<ApiResponse<IsInWishlistResponse>> IsInWishlistAsync(Guid userId, Guid productId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId, nameof(userId));
                ValidateId(productId, nameof(productId));

                var cacheKey = CreateCacheKey(CACHE_PREFIX, "check", userId, productId);

                var isInWishlist = await GetFromCacheOrExecuteAsync(cacheKey, async () =>
                {
                    return await _unitOfWork.ProductFavorites.ExistsByUserAndProductAsync(userId, productId);
                }, TimeSpan.FromMinutes(15));

                return new IsInWishlistResponse { IsInWishlist = isInWishlist };

            }, "IsInWishlist", "Kiểm tra thành công");
        }

        public async Task<ApiResponse<ToggleWishlistResponse>> ToggleWishlistAsync(Guid userId, Guid productId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId, nameof(userId));
                ValidateId(productId, nameof(productId));

                var exists = await _unitOfWork.ProductFavorites.ExistsByUserAndProductAsync(userId, productId);

                if (exists)
                {
                    // Xóa khỏi wishlist
                    await _unitOfWork.ProductFavorites.RemoveByUserAndProductAsync(userId, productId);

                    // Update FavoriteCount
                    var product = await _unitOfWork.Products.GetByIdAsync(productId);
                    if (product != null && product.FavoriteCount > 0)
                    {
                        product.FavoriteCount--;
                        _unitOfWork.Products.Update(product);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await InvalidateWishlistCache(userId, productId);

                    return new ToggleWishlistResponse { IsInWishlist = false };
                }
                else
                {
                    // Thêm vào wishlist
                    var product = await _unitOfWork.Products.GetByIdAsync(productId);
                    ThrowIf(product == null, "Sản phẩm không tồn tại");
                    ThrowIf(product!.IsDeleted, "Sản phẩm đã bị xóa");

                    var favorite = new ProductFavorite
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        ProductId = productId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.ProductFavorites.AddAsync(favorite);

                    product.FavoriteCount++;
                    _unitOfWork.Products.Update(product);

                    await _unitOfWork.SaveChangesAsync();
                    await InvalidateWishlistCache(userId, productId);

                    return new ToggleWishlistResponse { IsInWishlist = true };
                }

            }, "ToggleWishlist", "Toggle thành công");
        }

        public async Task<ApiResponse<bool>> ClearWishlistAsync(Guid userId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId, nameof(userId));

                await using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    // Lấy tất cả items
                    var items = await _unitOfWork.ProductFavorites
                        .FindAsync(pf => pf.UserId == userId);

                    var itemsList = items.ToList();
                    if (!itemsList.Any())
                        return true;

                    // Update FavoriteCount cho tất cả products
                    var productIds = itemsList.Select(x => x.ProductId).Distinct();
                    foreach (var productId in productIds)
                    {
                        var product = await _unitOfWork.Products.GetByIdAsync(productId);
                        if (product != null && product.FavoriteCount > 0)
                        {
                            product.FavoriteCount--;
                            _unitOfWork.Products.Update(product);
                        }
                    }

                    // Xóa toàn bộ wishlist
                    var count = await _unitOfWork.ProductFavorites.ClearByUserIdAsync(userId);
                    await _unitOfWork.SaveChangesAsync();

                    await _unitOfWork.CommitTransactionAsync();

                    // Invalidate cache
                    await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX}:*{userId}*");

                    LogInfo($"Đã xóa {count} items khỏi wishlist của user {userId}");
                    return true;
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }

            }, "ClearWishlist", "Đã xóa toàn bộ danh sách yêu thích");
        }

        public async Task<ApiResponse<bool>> MoveAllToCartAsync(Guid userId)
        {
            return await ExecuteAsApiResponseAsync<bool>(async () =>
            {
                ValidateId(userId, nameof(userId));

                // TODO: Implement logic move to cart
                // Cần inject ICartService và gọi AddToCartAsync cho từng item
                // Sau đó clear wishlist

                throw new NotImplementedException(
                    "Tính năng Move to Cart chưa được implement. " +
                    "Vui lòng inject ICartService và implement logic.");

            }, "MoveAllToCart", "Đã di chuyển tất cả vào giỏ hàng");
        }
        public async Task<ApiResponse<int>> GetWishlistCountAsync(Guid userId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(userId, nameof(userId));

                var cacheKey = CreateCacheKey(CACHE_PREFIX, "count", userId);

                return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
                {
                    // Đếm trực tiếp từ DB - tối ưu hơn lấy toàn bộ danh sách
                    return await _unitOfWork.ProductFavorites.CountByUserIdAsync(userId);
                }, TimeSpan.FromMinutes(15)); // Cache ngắn hơn vì count thay đổi thường xuyên

            }, "GetWishlistCount", "Lấy số lượng wishlist thành công");
        }
        #region Private Methods

        // ✅ XÓA TOÀN BỘ METHOD MapToDto - KHÔNG CẦN NỮA
        // AutoMapper sẽ tự động map thông qua ProductFavoriteMappingProfile

        private async Task InvalidateWishlistCache(Guid userId, Guid productId)
        {
            await InvalidateMultipleCachesAsync(
                CreateCacheKey(CACHE_PREFIX, "user", userId),
                CreateCacheKey(CACHE_PREFIX, "count", userId),
                CreateCacheKey(CACHE_PREFIX, "check", userId, productId)
            );
        }

        #endregion
    }
}