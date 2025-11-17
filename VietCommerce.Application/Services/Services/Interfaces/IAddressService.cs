using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Address;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    public interface  IAddressService
    {
        #region CUSTOMER METHODS
        Task<ApiResponse<List<AddressResponseDto>>> GetMyAddressesAsync();
        Task<ApiResponse<AddressResponseDto>> GetAddressByIdAsync(Guid id);
        Task<ApiResponse<AddressResponseDto>> CreateAddressAsync(CreateAddressDto dto);
        Task<ApiResponse<AddressResponseDto>> UpdateAddressAsync(Guid id, UpdateAddressDto dto);
        Task<ApiResponse<bool>> DeleteAddressAsync(Guid id);
        Task<ApiResponse<bool>> SetDefaultAddressAsync(Guid id);
        #endregion
        #region ADMIN METHODS
        Task<ApiResponse<List<AddressResponseDto>>> GetAllAddressesAsync(GetAddressesQueryDto query);
        Task<ApiResponse<AddressResponseDto>> AdminGetAddressByIdAsync(Guid id);
        Task<ApiResponse<AddressResponseDto>> AdminUpdateAddressAsync(Guid id, UpdateAddressDto dto);
        Task<ApiResponse<bool>> AdminDeleteAddressAsync(Guid id);
        #endregion


    }
}
