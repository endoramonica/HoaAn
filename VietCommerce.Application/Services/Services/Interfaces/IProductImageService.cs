using Microsoft.AspNetCore.Http;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces;

public interface IProductImageService
{
    // Get methods
    Task<ApiResponse<List<ProductImageDto>>> GetProductImagesAsync(Guid productId);
    Task<ApiResponse<List<ProductImageDto>>> GetImagesByProductIdAsync(Guid productId, bool onlyImages = true);
    Task<ApiResponse<ProductImageDto?>> GetMainImageAsync(Guid productId);
    Task<ApiResponse<ProductImageDto?>> GetMainImageByProductIdAsync(Guid productId);

    // Upload & Delete
    Task<ApiResponse<List<ProductImageDto>>> UploadImagesAsync(UploadProductImageDto dto, IEnumerable<IFormFile> files);
    Task<ApiResponse<bool>> DeleteImageAsync(Guid imageId);

    // Set main image (2 signatures)
    Task<ApiResponse<bool>> SetMainImageAsync(SetMainImageDto dto);
    Task<ApiResponse<bool>> SetMainImageAsync(Guid productId, Guid imageId);

    // Update display order
    Task<ApiResponse<bool>> UpdateDisplayOrderAsync(UpdateImageOrderDto dto);
}