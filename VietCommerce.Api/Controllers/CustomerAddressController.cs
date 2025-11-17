using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.Common.Attributes;
using VietCommerce.Core.Common.Constants;
using VietCommerce.Core.DTOs.Address;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Controllers.V1   // để cùng group V1
{
    /// <summary>
    /// Customer Address Management API
    /// </summary>
    [ApiController]
    [Route("api/v1/customer/addresses")]
    [Authorize]
    [RequirePermission(PermissionConstants.CustomerAddressManage)]
    [Produces("application/json")]
    // Bỏ ApiExplorerSettings để Swagger mặc định lấy vào doc "v1"
    public class CustomerAddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public CustomerAddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        #region GET all addresses
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<AddressResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<List<AddressResponseDto>>>> GetMyAddresses()
        {
            var result = await _addressService.GetMyAddressesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
        #endregion

        #region GET by id
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<AddressResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<AddressResponseDto>>> GetAddressById(Guid id)
        {
            var result = await _addressService.GetAddressByIdAsync(id);
            if (!result.Success)
            {
                bool notFound = result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true
                             || result.Message?.Contains("access denied", StringComparison.OrdinalIgnoreCase) == true;
                return notFound ? NotFound(result) : BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region CREATE
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AddressResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<AddressResponseDto>>> CreateAddress([FromBody] CreateAddressDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                return BadRequest(ApiResponse<AddressResponseDto>.FailureResponse("Invalid input data", errors));
            }

            var result = await _addressService.CreateAddressAsync(dto);
            if (!result.Success) return BadRequest(result);

            return result.Data != null
                ? CreatedAtAction(nameof(GetAddressById), new { id = result.Data.Id }, result)
                : Ok(result);
        }
        #endregion

        #region UPDATE
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<AddressResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<AddressResponseDto>>> UpdateAddress(Guid id, [FromBody] UpdateAddressDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                return BadRequest(ApiResponse<AddressResponseDto>.FailureResponse("Invalid input data", errors));
            }

            var result = await _addressService.UpdateAddressAsync(id, dto);
            if (!result.Success)
            {
                bool notFound = result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true
                             || result.Message?.Contains("access denied", StringComparison.OrdinalIgnoreCase) == true;
                return notFound ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        #endregion

        #region SET DEFAULT
        [HttpPost("{id:guid}/set-default")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> SetDefaultAddress(Guid id)
        {
            var result = await _addressService.SetDefaultAddressAsync(id);
            if (!result.Success)
            {
                bool notFound = result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true
                             || result.Message?.Contains("access denied", StringComparison.OrdinalIgnoreCase) == true;
                return notFound ? NotFound(result) : BadRequest(result);
            }
            return Ok(result);
        }
        #endregion

        #region DELETE
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAddress(Guid id)
        {
            var result = await _addressService.DeleteAddressAsync(id);
            if (!result.Success)
            {
                bool notFound = result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true
                             || result.Message?.Contains("access denied", StringComparison.OrdinalIgnoreCase) == true;
                return notFound ? NotFound(result) : BadRequest(result);
            }
            return Ok(result);
        }
        #endregion
    }
}
