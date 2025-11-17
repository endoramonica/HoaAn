using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.DTOs.Address;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;
using VietCommerce.Core.Common.Extensions;
using Microsoft.AspNetCore.Http; // Thêm để dùng IHttpContextAccessor

namespace VietCommerce.Application.Services.Services
{
    public class AddressService : BaseService , IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddressService(
            ILogger<AddressService> logger,
            IUnitOfWork unitOfWork,
            ICurrentUser currentUser,
            IMapper mapper,
            ICacheService cacheService,
            IHttpContextAccessor httpContextAccessor) // Inject IHttpContextAccessor
            : base(logger, cacheService)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        #region CUSTOMER METHODS

        /// <summary>
        /// Lấy tất cả địa chỉ của customer hiện tại
        /// </summary>
        public async Task<ApiResponse<List<AddressResponseDto>>> GetMyAddressesAsync()
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                var customerId = await GetCurrentCustomerIdAsync();
                var cacheKey = CreateCacheKey("addresses", "customer", customerId);
                var addresses = await GetFromCacheOrExecuteAsync(
                    cacheKey,
                    async () => await _unitOfWork.CustomerAddresses.GetByCustomerIdAsync(customerId),
                    TimeSpan.FromMinutes(10)
                );
                var result = _mapper.Map<List<AddressResponseDto>>(addresses);
                LogInfo($"Retrieved {result.Count} addresses for customer {customerId}");
                return result;
            }, "GetMyAddresses", "Addresses retrieved successfully");
        }

        /// <summary>
        /// Lấy địa chỉ theo ID (chỉ của mình)
        /// </summary>
        public async Task<ApiResponse<AddressResponseDto>> GetAddressByIdAsync(Guid id)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(id, nameof(id));
                var customerId = await GetCurrentCustomerIdAsync();
                var address = await _unitOfWork.CustomerAddresses.GetByIdAndCustomerIdAsync(id, customerId);
                ThrowIf(address == null, "Address not found or access denied");
                var result = _mapper.Map<AddressResponseDto>(address);
                LogInfo($"Retrieved address {id} for customer {customerId}");
                return result;
            }, "GetAddressById", "Address retrieved successfully");
        }

        /// <summary>
        /// Tạo địa chỉ mới
        /// </summary>
        public async Task<ApiResponse<AddressResponseDto>> CreateAddressAsync(CreateAddressDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotNull(dto, nameof(dto));
                ValidateNotEmpty(dto.StreetAddress, nameof(dto.StreetAddress));
                var customerId = await GetCurrentCustomerIdAsync();
                var tenantId = await GetCurrentTenantIdAsync();

                if (dto.IsDefault)
                {
                    await _unitOfWork.CustomerAddresses.ClearDefaultAsync(customerId);
                }

                var address = _mapper.Map<CustomerAddress>(dto);
                address.Id = Guid.NewGuid();
                address.CustomerId = customerId;
                address.TenantId = tenantId;
                address.CreatedAt = DateTime.UtcNow;
                address.IsActive = true;

                await _unitOfWork.CustomerAddresses.AddAsync(address);
                await _unitOfWork.SaveChangesAsync();

                await InvalidateCacheAsync(CreateCacheKey("addresses", "customer", customerId));

                var result = _mapper.Map<AddressResponseDto>(address);
                LogInfo($"Created address {address.Id} for customer {customerId}");
                return result;
            }, "CreateAddress", "Address created successfully");
        }

        /// <summary>
        /// Cập nhật địa chỉ (partial update)
        /// </summary>
        public async Task<ApiResponse<AddressResponseDto>> UpdateAddressAsync(Guid id, UpdateAddressDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(id, nameof(id));
                ValidateNotNull(dto, nameof(dto));
                var customerId = await GetCurrentCustomerIdAsync();
                var address = await _unitOfWork.CustomerAddresses.GetByIdAndCustomerIdAsync(id, customerId);
                ThrowIf(address == null, "Address not found or access denied");

                if (dto.IsDefault == true && !address.IsDefault)
                {
                    await _unitOfWork.CustomerAddresses.ClearDefaultAsync(customerId);
                }

                _mapper.Map(dto, address);
                address.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.CustomerAddresses.Update(address);
                await _unitOfWork.SaveChangesAsync();

                await InvalidateCacheAsync(CreateCacheKey("addresses", "customer", customerId));

                var result = _mapper.Map<AddressResponseDto>(address);
                LogInfo($"Updated address {id} for customer {customerId}");
                return result;
            }, "UpdateAddress", "Address updated successfully");
        }

        /// <summary>
        /// Set địa chỉ làm mặc định
        /// </summary>
        public async Task<ApiResponse<bool>> SetDefaultAddressAsync(Guid id)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(id, nameof(id));
                var customerId = await GetCurrentCustomerIdAsync();
                var address = await _unitOfWork.CustomerAddresses.GetByIdAndCustomerIdAsync(id, customerId);
                ThrowIf(address == null, "Address not found or access denied");

                await _unitOfWork.CustomerAddresses.ClearDefaultAsync(customerId);
                address.IsDefault = true;
                address.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.CustomerAddresses.Update(address);
                await _unitOfWork.SaveChangesAsync();

                await InvalidateCacheAsync(CreateCacheKey("addresses", "customer", customerId));

                LogInfo($"Set address {id} as default for customer {customerId}");
                return true;
            }, "SetDefaultAddress", "Default address updated successfully");
        }

        /// <summary>
        /// Xóa mềm địa chỉ
        /// </summary>
        public async Task<ApiResponse<bool>> DeleteAddressAsync(Guid id)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(id, nameof(id));
                var customerId = await GetCurrentCustomerIdAsync();
                var address = await _unitOfWork.CustomerAddresses.GetByIdAndCustomerIdAsync(id, customerId);
                ThrowIf(address == null, "Address not found or access denied");

                address.SoftDelete(_currentUser.UserId);
                _unitOfWork.CustomerAddresses.Update(address);
                await _unitOfWork.SaveChangesAsync();

                await InvalidateCacheAsync(CreateCacheKey("addresses", "customer", customerId));

                LogInfo($"Soft deleted address {id} for customer {customerId}");
                return true;
            }, "DeleteAddress", "Address deleted successfully");
        }

        #endregion

        #region ADMIN METHODS

        /// <summary>
        /// Admin: Lấy tất cả địa chỉ (bỏ phân trang)
        /// </summary>
        public async Task<ApiResponse<List<AddressResponseDto>>> GetAllAddressesAsync(GetAddressesQueryDto query)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateNotNull(query, nameof(query));

                // 🔥 BỎ PHÂN TRANG → LẤY TOÀN BỘ ADDRESS
                var items = await _unitOfWork.CustomerAddresses.GetAllAsync();

                var dtos = _mapper.Map<List<AddressResponseDto>>(items);

                LogInfo($"Admin retrieved {dtos.Count} addresses");

                return dtos;

            }, "GetAllAddresses", "Addresses retrieved successfully");
        }

        /// <summary>
        /// Admin: Lấy địa chỉ theo ID
        /// </summary>
        public async Task<ApiResponse<AddressResponseDto>> AdminGetAddressByIdAsync(Guid id)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(id, nameof(id));
                var address = await _unitOfWork.CustomerAddresses.GetByIdAsync(id);
                ThrowIf(address == null || address.IsDeleted, "Address not found");

                var result = _mapper.Map<AddressResponseDto>(address);

                LogInfo($"Admin retrieved address {id}");
                return result;

            }, "AdminGetAddressById", "Address retrieved successfully");
        }

        /// <summary>
        /// Admin: Cập nhật địa chỉ
        /// </summary>
        public async Task<ApiResponse<AddressResponseDto>> AdminUpdateAddressAsync(Guid id, UpdateAddressDto dto)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(id, nameof(id));
                ValidateNotNull(dto, nameof(dto));

                var address = await _unitOfWork.CustomerAddresses.GetByIdAsync(id);
                ThrowIf(address == null || address.IsDeleted, "Address not found");

                var customerId = address.CustomerId;

                if (dto.IsDefault == true && !address.IsDefault)
                {
                    await _unitOfWork.CustomerAddresses.ClearDefaultAsync(customerId);
                }

                _mapper.Map(dto, address);
                address.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.CustomerAddresses.Update(address);
                await _unitOfWork.SaveChangesAsync();

                await InvalidateCacheAsync(CreateCacheKey("addresses", "customer", customerId));

                var result = _mapper.Map<AddressResponseDto>(address);
                LogInfo($"Admin updated address {id}");

                return result;

            }, "AdminUpdateAddress", "Address updated successfully");
        }

        /// <summary>
        /// Admin: Xóa mềm địa chỉ
        /// </summary>
        public async Task<ApiResponse<bool>> AdminDeleteAddressAsync(Guid id)
        {
            return await ExecuteAsApiResponseAsync(async () =>
            {
                ValidateId(id, nameof(id));

                var address = await _unitOfWork.CustomerAddresses.GetByIdAsync(id);
                ThrowIf(address == null || address.IsDeleted, "Address not found");

                var customerId = address.CustomerId;

                address.SoftDelete(_currentUser.UserId);

                _unitOfWork.CustomerAddresses.Update(address);
                await _unitOfWork.SaveChangesAsync();

                await InvalidateCacheAsync(CreateCacheKey("addresses", "customer", customerId));

                LogInfo($"Admin deleted address {id}");
                return true;

            }, "AdminDeleteAddress", "Address deleted successfully");
        }

        #endregion


        #region HELPER METHODS

        /// <summary>
        /// Lấy CustomerId từ claim "customerId" trong JWT
        /// </summary>
        private async Task<Guid> GetCurrentCustomerIdAsync()
        {
            // Lấy từ HttpContext (đã được JwtHelper thêm claim "customerId")
            var customerIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("customerId")?.Value;

            if (string.IsNullOrEmpty(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
            {
                throw new UnauthorizedAccessException("Customer ID not found in token. Please login again.");
            }

            // Optional: Kiểm tra customer có tồn tại không
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            ThrowIf(customer == null, "Customer profile not found");

            return customerId;
        }

        /// <summary>
        /// Lấy TenantId từ Customer
        /// </summary>
        private async Task<Guid> GetCurrentTenantIdAsync()
        {
            var customerId = await GetCurrentCustomerIdAsync();
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            return customer!.TenantId;
        }

        #endregion
    }
}