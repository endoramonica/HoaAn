using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Service interface for Voucher management
    /// Handles all business logic for voucher operations
    /// Requirements: 3.1, 3.2, 3.3, 3.4, 3.5
    /// </summary>
    public interface IVoucherService
    {
        /// <summary>
        /// Generate voucher codes for a promotion
        /// Requirements: 3.1
        /// </summary>
        Task<ApiResponse<GenerateVouchersResultDto>> GenerateVouchersAsync(
            Guid promotionId,
            GenerateVouchersDto dto);

        /// <summary>
        /// Get vouchers for a promotion with pagination
        /// Requirements: 3.1
        /// </summary>
        Task<ApiResponse<PaginatedResult<VoucherDto>>> GetVouchersForPromotionAsync(
            Guid promotionId,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// Validate a voucher code
        /// Requirements: 3.2, 3.4
        /// </summary>
        Task<ApiResponse<VoucherDto>> ValidateVoucherAsync(string code);

        /// <summary>
        /// Get voucher by code
        /// Requirements: 3.2
        /// </summary>
        Task<ApiResponse<VoucherDto>> GetVoucherByCodeAsync(string code);

        /// <summary>
        /// Delete a voucher (soft delete)
        /// Requirements: 3.1
        /// </summary>
        Task<ApiResponse<bool>> DeleteVoucherAsync(Guid voucherId);

        /// <summary>
        /// Update voucher usage
        /// Requirements: 3.5
        /// </summary>
        Task<ApiResponse<bool>> UpdateVoucherUsageAsync(string code, Guid? usedBy = null);

        /// <summary>
        /// Remove a voucher from a cart
        /// Clears the applied voucher and resets discount
        /// Requirements: 3.2
        /// </summary>
        Task<ApiResponse<bool>> RemoveVoucherAsync(Guid cartId);
    }
}
