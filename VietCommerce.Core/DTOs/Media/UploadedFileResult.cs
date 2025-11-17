using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VietCommerce.Core.DTOs.Media
{
    /// <summary>
    /// Kết quả trả về của mỗi file upload
    /// </summary>
    public class UploadedFileResult
    {
        /// <summary>Đường dẫn vật lý trên server</summary>
        public string FilePath { get; set; }

        /// <summary>URL truy cập file</summary>
        public string FileUrl { get; set; }

        /// <summary>URL thumbnail (chỉ có với ảnh, null nếu file không phải ảnh)</summary>
        public string? ThumbnailUrl { get; set; }
    }
}
