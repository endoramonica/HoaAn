using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Products;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services.Products;

public class ProductImageService : BaseService, IProductImageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileUploadService _fileStorage;

    public ProductImageService(
        IUnitOfWork unitOfWork,
        IFileUploadService fileStorage,
        ILogger<ProductImageService> logger,
        ICacheService cacheService)
        : base(logger, cacheService)
    {
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
    }

    /// <summary>
    /// Lấy danh sách ảnh của product
    /// </summary>
    public async Task<ApiResponse<List<ProductImageDto>>> GetProductImagesAsync(Guid productId)
    {
        return await GetImagesByProductIdAsync(productId, true);
    }

    /// <summary>
    /// Lấy danh sách ảnh của product (có option lọc)
    /// </summary>
    public async Task<ApiResponse<List<ProductImageDto>>> GetImagesByProductIdAsync(Guid productId, bool onlyImages = true)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            // Thử lấy từ cache trước
            var cacheKey = CreateCacheKey("product_images", productId.ToString(), onlyImages.ToString());
            var cached = await _cacheService?.GetAsync<List<ProductImageDto>>(cacheKey)!;
            if (cached != null)
            {
                LogInfo($"📦 Cache hit: {cacheKey}");
                return cached;
            }

            // Không có cache -> query DB
            var images = await _unitOfWork.ProductImages.GetByProductIdAsync(productId, onlyImages);

            // Map sang DTO
            var dtos = images.Select(MapToDto).ToList();

            // Cache lại 10 phút
            if (_cacheService != null)
            {
                await _cacheService.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(10));
            }

            return dtos;
        }, "GetProductImages", "Retrieved product images successfully");
    }

    /// <summary>
    /// Lấy ảnh chính của product
    /// </summary>
    public async Task<ApiResponse<ProductImageDto?>> GetMainImageAsync(Guid productId)
    {
        return await GetMainImageByProductIdAsync(productId);
    }

    /// <summary>
    /// Lấy ảnh chính của product (by ProductId)
    /// </summary>
    public async Task<ApiResponse<ProductImageDto?>> GetMainImageByProductIdAsync(Guid productId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            var cacheKey = CreateCacheKey("product_main_image", productId.ToString());
            var cached = await _cacheService?.GetAsync<ProductImageDto>(cacheKey)!;
            if (cached != null)
            {
                LogInfo($"📦 Cache hit: {cacheKey}");
                return cached;
            }

            var image = await _unitOfWork.ProductImages.GetMainImageAsync(productId);
            if (image == null) return null;

            var dto = MapToDto(image);

            if (_cacheService != null)
            {
                await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));
            }

            return dto;
        }, "GetMainImage", "Retrieved main image successfully");
    }

    /// <summary>
    /// Upload ảnh cho product
    /// </summary>
    public async Task<ApiResponse<List<ProductImageDto>>> UploadImagesAsync(
        UploadProductImageDto dto,
        IEnumerable<IFormFile> files)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            // Upload files lên server
            var uploadResult = await _fileStorage.SaveFilesAsync(
                files,
                new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" },
                maxFileSize: 5 * 1024 * 1024, // 5MB
                "uploads", "products");

            if (!uploadResult.Success || uploadResult.Data == null)
                throw new Exception("Failed to upload files");

            var productImages = new List<ProductImage>();

            // Lưu vào database
            foreach (var (file, index) in uploadResult.Data.Select((f, i) => (f, i)))
            {
                var displayOrder = await _unitOfWork.ProductImages.GetNextDisplayOrderAsync(dto.ProductId);

                var productImage = new ProductImage
                {
                    ProductId = dto.ProductId,
                    FilePath = file.FilePath,
                    Url = file.FileUrl,
                    ThumbnailUrl = file.ThumbnailUrl,
                    DisplayOrder = displayOrder,
                    MediaType = MediaTypeEnum.Image,
                    IsMain = dto.SetFirstAsMain && index == 0
                };

                await _unitOfWork.ProductImages.AddAsync(productImage);
                productImages.Add(productImage);
            }

            await _unitOfWork.SaveChangesAsync();

            // Invalidate cache
            await InvalidateProductImageCache(dto.ProductId);

            LogInfo($"✅ Uploaded {productImages.Count} images for Product {dto.ProductId}");

            // Map sang DTO
            return productImages.Select(MapToDto).ToList();

        }, "UploadProductImages", "Product images uploaded successfully");
    }

    /// <summary>
    /// Xóa ảnh
    /// </summary>
    public async Task<ApiResponse<bool>> DeleteImageAsync(Guid imageId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            var image = await _unitOfWork.ProductImages.GetByIdAsync(imageId);
            if (image == null)
                throw new KeyNotFoundException($"Image {imageId} not found");

            // Xóa file vật lý
            await _fileStorage.DeleteFileAsync(image.FilePath);

            // Xóa thumbnail nếu có
            if (!string.IsNullOrEmpty(image.ThumbnailUrl))
            {
                var thumbPath = image.FilePath.Replace(
                    Path.GetFileName(image.FilePath),
                    $"thumb_{Path.GetFileName(image.FilePath)}");

                if (File.Exists(thumbPath))
                    await _fileStorage.DeleteFileAsync(thumbPath);
            }

            // Xóa trong database
            await _unitOfWork.ProductImages.DeleteAsync(imageId);
            await _unitOfWork.SaveChangesAsync();

            // Invalidate cache
            await InvalidateProductImageCache(image.ProductId);

            LogInfo($"🗑️ Deleted image {imageId}");
            return true;

        }, "DeleteProductImage", "Image deleted successfully");
    }

    /// <summary>
    /// Đặt ảnh chính (dùng DTO)
    /// </summary>
    public async Task<ApiResponse<bool>> SetMainImageAsync(SetMainImageDto dto)
    {
        return await SetMainImageAsync(dto.ProductId, dto.ImageId);
    }

    /// <summary>
    /// Đặt ảnh chính (dùng Guid trực tiếp)
    /// </summary>
    public async Task<ApiResponse<bool>> SetMainImageAsync(Guid productId, Guid imageId)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            await _unitOfWork.ProductImages.SetMainImageAsync(productId, imageId);

            // Invalidate cache
            await InvalidateProductImageCache(productId);

            LogInfo($"✅ Set main image {imageId} for Product {productId}");
            return true;

        }, "SetMainImage", "Main image set successfully");
    }

    /// <summary>
    /// Cập nhật thứ tự hiển thị
    /// </summary>
    public async Task<ApiResponse<bool>> UpdateDisplayOrderAsync(UpdateImageOrderDto dto)
    {
        return await ExecuteAsApiResponseAsync(async () =>
        {
            var image = await _unitOfWork.ProductImages.GetByIdAsync(dto.ImageId);
            if (image == null)
                throw new KeyNotFoundException($"Image {dto.ImageId} not found");

            image.DisplayOrder = dto.NewDisplayOrder;
            _unitOfWork.ProductImages.Update(image); // Dùng Update thay vì UpdateAsync
            await _unitOfWork.SaveChangesAsync();

            // Invalidate cache
            await InvalidateProductImageCache(image.ProductId);

            LogInfo($"✅ Updated display order for image {dto.ImageId}");
            return true;

        }, "UpdateDisplayOrder", "Display order updated successfully");
    }

    #region Private Methods

    /// <summary>
    /// Map Entity sang DTO
    /// </summary>
    private static ProductImageDto MapToDto(ProductImage entity)
    {
        return new ProductImageDto
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            Url = entity.Url,
            ThumbnailUrl = entity.ThumbnailUrl,
            DisplayOrder = entity.DisplayOrder,
            MediaType = entity.MediaType,
            IsMain = entity.IsMain,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    /// <summary>
    /// Xóa cache liên quan đến product images
    /// </summary>
    private async Task InvalidateProductImageCache(Guid productId)
    {
        if (_cacheService == null) return;

        var cacheKeys = new[]
        {
            CreateCacheKey("product_images", productId.ToString()),
            CreateCacheKey("product_main_image", productId.ToString())
        };

        foreach (var key in cacheKeys)
        {
            await _cacheService.RemoveAsync(key);
        }

        LogInfo($"🗑️ Invalidated cache for product {productId}");
    }

    #endregion
}