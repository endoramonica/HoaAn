using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;


namespace VietCommerce.Core.Helpers
{
    public static class ImageHelper
    {
        /// <summary>
        /// Nén và resize ảnh, lưu xuống đường dẫn đích
        /// </summary>
        /// <param name="sourceStream">Stream gốc</param>
        /// <param name="destinationPath">Đường dẫn lưu file</param>
        /// <param name="maxWidth">Chiều rộng tối đa (mặc định 1024)</param>
        /// <param name="maxHeight">Chiều cao tối đa (mặc định 1024)</param>
        /// <param name="quality">Chất lượng ảnh 0-100 (mặc định 75)</param>
        public static async Task CompressResizeAndSaveAsync(Stream sourceStream, string destinationPath, int maxWidth = 1024, int maxHeight = 1024, int quality = 75)
        {
            using var image = await Image.LoadAsync(sourceStream);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxWidth, maxHeight)
            }));

            var encoder = new JpegEncoder { Quality = quality };
            await image.SaveAsync(destinationPath, encoder);
        }

        /// <summary>
        /// Tạo thumbnail ảnh
        /// </summary>
        /// <param name="sourceStream">Stream gốc</param>
        /// <param name="thumbnailPath">Đường dẫn lưu thumbnail</param>
        /// <param name="width">Chiều rộng thumbnail (mặc định 200)</param>
        /// <param name="height">Chiều cao thumbnail (mặc định 200)</param>
        /// <param name="quality">Chất lượng ảnh 0-100</param>
        public static async Task CreateThumbnailAsync(Stream sourceStream, string thumbnailPath, int width = 200, int height = 200, int quality = 75)
        {
            using var image = await Image.LoadAsync(sourceStream);

            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Crop,
                Size = new Size(width, height)
            }));

            var encoder = new JpegEncoder { Quality = quality };
            await image.SaveAsync(thumbnailPath, encoder);
        }
    }
}
