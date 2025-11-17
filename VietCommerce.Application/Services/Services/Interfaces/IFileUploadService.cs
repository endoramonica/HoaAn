using Microsoft.AspNetCore.Http;
using VietCommerce.Core.DTOs.Media;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    public interface IFileUploadService
    {
        /// <summary>
        /// Upload nhiều file cùng lúc, tự động tạo folder theo ngày/tháng/năm.
        /// Nếu là ảnh, sẽ nén và tạo thumbnail.
        /// </summary>
        /// <param name="files">Danh sách file upload</param>
        /// <param name="allowedExtensions">Các định dạng file được phép</param>
        /// <param name="maxFileSize">Dung lượng tối đa của từng file (bytes)</param>
        /// <param name="baseFolderPaths">Các folder gốc (ví dụ: "uploads", "products")</param>
        /// <returns>Danh sách file đã upload kèm đường dẫn vật lý, URL và URL thumbnail (nếu ảnh)</returns>
        Task<ApiResponse<List<UploadedFileResult>>> SaveFilesAsync(
            IEnumerable<IFormFile> files,
            string[] allowedExtensions,
            long maxFileSize,
            params string[] baseFolderPaths);
        // DeleteFileAsync() - xóa file
        Task<ApiResponse<bool>> DeleteFileAsync(string filePath);
        // GenerateVideoThumbnailAsync() - tạo thumbnail
        Task GenerateVideoThumbnailAsync(string videoPath, string thumbnailPath, int width = 320, int height = 240, int atSecond = 1);
    }


}
