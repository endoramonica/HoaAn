using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Customers;

namespace VietCommerce.Core.DTOs.Comments
{
    /// <summary>
    /// DTO để tạo/update comment (Save operation)
    /// Nếu CommentId = Guid.Empty → Tạo mới
    /// Nếu CommentId có giá trị → Update
    /// </summary>
    public class SaveCommentDto
    {
        public Guid CommentId { get; set; } = Guid.Empty; // Empty = Create, có giá trị = Update
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid? ParentCommentId { get; set; } // Null = root comment, có giá trị = reply
    }

    /// <summary>
    /// DTO để tạo comment mới
    /// </summary>
    public class CreateCommentDto
    {
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid? ParentCommentId { get; set; }
    }

    /// <summary>
    /// DTO để update comment
    /// </summary>
    public class UpdateCommentDto
    {
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO response cơ bản cho comment
    /// </summary>
    public class CommentDto
    {
        public Guid CommentId { get; set; }
        public Guid PostId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAvatar { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime AddedOn { get; set; }
        public Guid? ParentCommentId { get; set; }
        public int RepliesCount { get; set; }
        public bool IsOwnedByCurrentUser { get; set; }
    }

    /// <summary>
    /// DTO preview ngắn gọn cho comment (dùng trong PostFeedDto)
    /// </summary>
    public class CommentPreviewDto
    {
        public Guid CommentId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerAvatar { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime AddedOn { get; set; }
    }

    /// <summary>
    /// DTO chi tiết với replies
    /// </summary>
    public class CommentDetailDto : CommentDto
    {
        public List<CommentDto> Replies { get; set; } = new();
    }

}
