using AutoMapper;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services
{
    public class VoucherService : BaseService, IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Random _random = new();

        public VoucherService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<VoucherService> logger,
            ICacheService cacheService)
            : base(logger, cacheService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponse<GenerateVouchersResultDto>> GenerateVouchersAsync(
            Guid promotionId,
            GenerateVouchersDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(promotionId, nameof(promotionId));
                if (dto.Quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than 0", nameof(dto.Quantity));
                if (dto.ExpiryDate <= DateTime.UtcNow)
                    throw new ArgumentException("ExpiryDate must be in the future", nameof(dto.ExpiryDate));

                var promotion = await _unitOfWork.Promotions.GetByIdAsync(promotionId);
                if (promotion == null)
                    throw new KeyNotFoundException($"Promotion with ID '{promotionId}' not found");

                var vouchers = new List<Voucher>();
                var generatedCodes = new List<string>();
                var prefix = dto.Prefix ?? "VOUCHER";

                for (int i = 0; i < dto.Quantity; i++)
                {
                    string code;
                    int attempts = 0;
                    const int maxAttempts = 10;

                    do
                    {
                        code = GenerateUniqueCode(prefix);
                        attempts++;
                        if (attempts > maxAttempts)
                            throw new InvalidOperationException($"Failed to generate unique voucher code after {maxAttempts} attempts");
                    } while (await _unitOfWork.Vouchers.CodeExistsAsync(code));

                    var voucher = new Voucher
                    {
                        Id = Guid.NewGuid(),
                        Code = code,
                        PromotionId = promotionId,
                        ExpiryDate = dto.ExpiryDate,
                        UsageCount = 0,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    vouchers.Add(voucher);
                    generatedCodes.Add(code);
                }

                await _unitOfWork.Vouchers.BulkInsertAsync(vouchers);
                await _unitOfWork.SaveChangesAsync();

                return new GenerateVouchersResultDto
                {
                    GeneratedCount = generatedCodes.Count,
                    VoucherCodes = generatedCodes,
                    PromotionId = promotionId
                };
            }, "GenerateVouchers");
        }

        public async Task<ApiResponse<PaginatedResult<VoucherDto>>> GetVouchersForPromotionAsync(
            Guid promotionId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(promotionId, nameof(promotionId));
                var promotion = await _unitOfWork.Promotions.GetByIdAsync(promotionId);
                if (promotion == null)
                    throw new KeyNotFoundException($"Promotion with ID '{promotionId}' not found");

                var paginatedVouchers = await _unitOfWork.Vouchers.GetByPromotionIdAsync(
                    promotionId,
                    pageNumber,
                    pageSize);

                return new PaginatedResult<VoucherDto>
                {
                    Items = _mapper.Map<List<VoucherDto>>(paginatedVouchers.Items),
                    PageNumber = paginatedVouchers.PageNumber,
                    PageSize = paginatedVouchers.PageSize,
                    TotalItems = paginatedVouchers.TotalItems,
                    TotalPages = paginatedVouchers.TotalPages
                };
            }, "GetVouchersForPromotion");
        }

        public async Task<ApiResponse<VoucherDto>> ValidateVoucherAsync(string code)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(code, nameof(code));
                
                // Perform detailed validation
                var (isValid, errorMessage) = await _unitOfWork.Vouchers.ValidateVoucherDetailedAsync(code);
                if (!isValid)
                    throw new InvalidOperationException(errorMessage ?? "Voucher code is invalid");

                var voucher = await _unitOfWork.Vouchers.GetByCodeAsync(code);
                if (voucher == null)
                    throw new KeyNotFoundException($"Voucher with code '{code}' not found");

                return _mapper.Map<VoucherDto>(voucher);
            }, "ValidateVoucher");
        }

        public async Task<ApiResponse<VoucherDto>> GetVoucherByCodeAsync(string code)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(code, nameof(code));
                var voucher = await _unitOfWork.Vouchers.GetByCodeAsync(code);
                if (voucher == null)
                    throw new KeyNotFoundException($"Voucher with code '{code}' not found");

                return _mapper.Map<VoucherDto>(voucher);
            }, "GetVoucherByCode");
        }

        public async Task<ApiResponse<bool>> DeleteVoucherAsync(Guid voucherId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(voucherId, nameof(voucherId));
                var voucher = await _unitOfWork.Vouchers.GetByIdAsync(voucherId);
                if (voucher == null)
                    throw new KeyNotFoundException($"Voucher with ID '{voucherId}' not found");

                voucher.IsDeleted = true;
                voucher.DeletedAt = DateTime.UtcNow;

                _unitOfWork.Vouchers.Update(voucher);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }, "DeleteVoucher");
        }

        public async Task<ApiResponse<bool>> UpdateVoucherUsageAsync(string code, Guid? usedBy = null)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotEmpty(code, nameof(code));
                var voucher = await _unitOfWork.Vouchers.GetByCodeAsync(code);
                if (voucher == null)
                    throw new KeyNotFoundException($"Voucher with code '{code}' not found");

                voucher.UsageCount++;
                voucher.LastUsedAt = DateTime.UtcNow;
                voucher.LastUsedBy = usedBy;

                _unitOfWork.Vouchers.Update(voucher);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }, "UpdateVoucherUsage");
        }

        public async Task<ApiResponse<bool>> RemoveVoucherAsync(Guid cartId)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(cartId, nameof(cartId));
                
                var cart = await _unitOfWork.Carts.GetByIdAsync(cartId);
                if (cart == null)
                    throw new KeyNotFoundException($"Cart with ID '{cartId}' not found");

                // Clear voucher information from cart
                cart.AppliedVoucherId = null;
                cart.AppliedVoucherCode = null;
                cart.DiscountAmount = 0;

                _unitOfWork.Carts.Update(cart);
                await _unitOfWork.SaveChangesAsync();

                LogInfo($"✅ Voucher removed from cart {cartId}");

                return true;
            }, "RemoveVoucher");
        }

        private string GenerateUniqueCode(string prefix)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var suffix = new string(Enumerable.Range(0, 8)
                .Select(_ => chars[_random.Next(chars.Length)])
                .ToArray());

            return $"{prefix}-{suffix}";
        }
    }
}
