using System.Diagnostics;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services;
using VietCommerce.Core.DTOs.Media;
using VietCommerce.Core.Helpers;
using VietCommerce.Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;


namespace VietCommerce.Application.Services.Services
{
    public class FileUploadService : BaseService, IFileUploadService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public FileUploadService(
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            ILogger<FileUploadService> logger,
            ICacheService? cacheService = null)
            : base(logger, cacheService)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public async Task<ApiResponse<List<UploadedFileResult>>> SaveFilesAsync(
            IEnumerable<IFormFile> files,
            string[] allowedExtensions,
            long maxFileSize,
            params string[] baseFolderPaths)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                if (files == null || !files.Any())
                    throw new ArgumentException("No files provided.");

                var result = new List<UploadedFileResult>();

                // Folder theo ngày/tháng/năm
                string dateFolder = Path.Combine(DateTime.UtcNow.Year.ToString(),
                                                 DateTime.UtcNow.Month.ToString("D2"),
                                                 DateTime.UtcNow.Day.ToString("D2"));
                var targetFolderPath = Path.Combine(new[] { _webHostEnvironment.WebRootPath }
                                                   .Concat(baseFolderPaths)
                                                   .Concat(new[] { dateFolder })
                                                   .ToArray());

                if (!Directory.Exists(targetFolderPath))
                    Directory.CreateDirectory(targetFolderPath);

                var domainUrl = _configuration.GetValue<string>("Domain")?.TrimEnd('/');

                foreach (var file in files)
                {
                    var extension = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Select(e => e.ToLower()).Contains(extension))
                        throw new InvalidOperationException($"File type {extension} is not allowed.");

                    if (file.Length > maxFileSize)
                        throw new InvalidOperationException($"File size exceeds limit of {maxFileSize} bytes.");

                    var newFileName = $"{Guid.NewGuid()}_{DateTime.UtcNow.Ticks}{extension}";
                    var fullFilePath = Path.Combine(targetFolderPath, newFileName);
                    string? thumbnailUrl = null;

                    try
                    {
                        if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                        {
                            // Ảnh: nén + resize
                            await ImageHelper.CompressResizeAndSaveAsync(file.OpenReadStream(), fullFilePath);

                            // Tạo thumbnail
                            var thumbFileName = $"thumb_{newFileName}";
                            var thumbPath = Path.Combine(targetFolderPath, thumbFileName);
                            file.OpenReadStream().Position = 0; // reset stream
                            await ImageHelper.CreateThumbnailAsync(file.OpenReadStream(), thumbPath);

                            thumbnailUrl = $"{domainUrl}/{string.Join('/', baseFolderPaths)}/{dateFolder}/{thumbFileName}";
                        }
                        else if (extension == ".mp4" || extension == ".mov" || extension == ".avi")
                        {
                            // Video: lưu file
                            using var fs = new FileStream(fullFilePath, FileMode.Create);
                            await file.CopyToAsync(fs);

                            // Tạo thumbnail video bằng FFmpeg
                            var thumbFileName = $"thumb_{Path.GetFileNameWithoutExtension(newFileName)}.jpg";
                            var thumbPath = Path.Combine(targetFolderPath, thumbFileName);
                            await GenerateVideoThumbnailAsync(fullFilePath, thumbPath);
                            thumbnailUrl = $"{domainUrl}/{string.Join('/', baseFolderPaths)}/{dateFolder}/{thumbFileName}";
                        }
                        else
                        {
                            // File khác -> lưu bình thường
                            using var fs = new FileStream(fullFilePath, FileMode.Create);
                            await file.CopyToAsync(fs);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new IOException($"Error saving file {file.FileName}.", ex);
                    }

                    var fileUrl = $"{domainUrl}/{string.Join('/', baseFolderPaths)}/{dateFolder}/{newFileName}";
                    result.Add(new UploadedFileResult
                    {
                        FilePath = fullFilePath,
                        FileUrl = fileUrl,
                        ThumbnailUrl = thumbnailUrl
                    });
                }

                // Cache metadata file
                if (_cacheService != null)
                {
                    string cacheKey = CreateCacheKey("uploaded_files", string.Join("-", baseFolderPaths), dateFolder);
                    await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
                    LogInfo($"📦 Metadata files cached: {cacheKey}");
                }

                return result;
            }, "FileUpload", "Files uploaded successfully");
        }

        /// <summary>
        /// Xóa file vật lý khỏi server (bao gồm cả thumbnail nếu có)
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteFileAsync(string filePath)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentException("File path cannot be empty.");

                if (!File.Exists(filePath))
                {
                    LogWarning($"⚠️ File not found: {filePath}");
                    return await Task.FromResult(false);
                }

                // Xóa file chính
                File.Delete(filePath);
                LogInfo($"🗑️ Deleted file: {filePath}");

                // Xóa thumbnail nếu có
                var directory = Path.GetDirectoryName(filePath);
                var fileName = Path.GetFileName(filePath);
                var thumbPath = Path.Combine(directory!, $"thumb_{fileName}");

                if (File.Exists(thumbPath))
                {
                    File.Delete(thumbPath);
                    LogInfo($"🗑️ Deleted thumbnail: {thumbPath}");
                }

                return await Task.FromResult(true);

            }, "DeleteFile", "File deleted successfully");
        }

        /// <summary>
        /// Tạo thumbnail cho video bằng FFmpeg
        /// </summary>
        public async Task GenerateVideoThumbnailAsync(string videoPath, string thumbnailPath, int width = 320, int height = 240, int atSecond = 1)
        {
            try
            {
                var ffmpegPath = "ffmpeg"; // hoặc đường dẫn tuyệt đối tới ffmpeg.exe
                var args = $"-y -i \"{videoPath}\" -ss {atSecond} -vframes 1 -vf scale={width}:{height} \"{thumbnailPath}\"";

                var psi = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var process = Process.Start(psi);
                await process!.WaitForExitAsync();

                LogInfo($"✅ Generated video thumbnail: {thumbnailPath}");
            }
            catch (Exception ex)
            {
                LogWarning($"⚠️ Cannot generate video thumbnail for {videoPath}: {ex.Message}");
            }
        }
    }
}