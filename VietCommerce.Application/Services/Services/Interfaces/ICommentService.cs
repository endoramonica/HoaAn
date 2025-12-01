using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Comments;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    // <summary>
    /// ✅ SECURE Service interface cho Comment operations
    /// - CustomerId được lấy từ ICurrentUser (JWT token)
    /// - KHÔNG nhận CustomerId từ client để tránh security flaw
    /// </summary>
    public interface ICommentService
    {
        /// <summary>
        /// Save comment (Create hoặc Update dựa trên CommentId)
        /// - CommentId = Guid.Empty → Tạo mới
        /// - CommentId có giá trị → Update
        /// CustomerId được lấy tự động từ JWT token
        /// </summary>
        /// <param name="dto">Data transfer object chứa thông tin comment</param>
        /// <returns>ApiResponse với CommentDto nếu thành công</returns>
        Task<ApiResponse<CommentDto>> SaveCommentAsync(SaveCommentDto dto);

        /// <summary>
        /// Tạo comment mới (root comment hoặc reply)
        /// CustomerId được lấy tự động từ JWT token
        /// </summary>
        Task<ApiResponse<CommentDto>> CreateCommentAsync(CreateCommentDto dto);

        /// <summary>
        /// Cập nhật comment (chỉ owner có quyền)
        /// </summary>
        Task<ApiResponse<CommentDto>> UpdateCommentAsync(Guid commentId, UpdateCommentDto dto);

        /// <summary>
        /// Xóa comment (soft delete, chỉ owner có quyền)
        /// </summary>
        Task<ApiResponse<bool>> DeleteCommentAsync(Guid commentId);

        /// <summary>
        /// Lấy danh sách comments của một post với pagination
        /// Sử dụng startIndex thay vì pageNumber (giống logic cũ)
        /// </summary>
        //Task<ApiResponse<List<CommentDto>>> GetCommentsAsync(
        //    Guid postId,
        //    int startIndex = 0,
        //    int pageSize = 20);

        /// <summary>
        /// Lấy danh sách comments với PaginatedResult (chuẩn REST API)
        /// </summary>
        Task<ApiResponse<PaginatedResult<CommentDto>>> GetCommentsByPostIdAsync(
            Guid postId,
            int pageNumber = 1,
            int pageSize = 20);

        /// <summary>
        /// Lấy chi tiết một comment kèm replies
        /// </summary>
        Task<ApiResponse<CommentDetailDto>> GetCommentByIdAsync(Guid commentId);

        /// <summary>
        /// Lấy replies của một comment
        /// </summary>
        Task<ApiResponse<List<CommentDto>>> GetRepliesByCommentIdAsync(Guid commentId);
    }
}
