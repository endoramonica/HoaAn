using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.Common.Attributes;
using VietCommerce.Core.Common.Constants;
using VietCommerce.Core.DTOs.Address;
using VietCommerce.Core.Models;

namespace VietCommerce.AdminApi.Controllers
{
    /// <summary>
    /// Admin Address Management API
    /// </summary>
    [ApiController]
    [Route("api/addresses")]
    [Authorize]
    [RequirePermission(PermissionConstants.AdminAddressManage)]
    [RequirePermission(PermissionConstants.AdminAddressRead)]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "Admin")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        /// <summary>
        /// Get all addresses with filtering (Admin)
        /// </summary>
        /// <param name="query">Filter parameters</param>
        /// <response code="200">Returns list of addresses</response>
        /// <response code="400">Invalid query parameters</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden - Insufficient permissions</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<AddressResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<List<AddressResponseDto>>>> GetAll([FromQuery] GetAddressesQueryDto query)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequest(ApiResponse<List<AddressResponseDto>>.FailureResponse(
                    message: "Invalid query parameters",
                    errors: errors));
            }

            var result = await _addressService.GetAllAddressesAsync(query);

            return result.Success
                ? Ok(result)
                : BadRequest(result);
        }

        /// <summary>
        /// Get address by ID (Admin)
        /// </summary>
        /// <param name="id">Address ID</param>
        /// <response code="200">Returns address details</response>
        /// <response code="404">Address not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<AddressResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<AddressResponseDto>>> GetById(Guid id)
        {
            var result = await _addressService.AdminGetAddressByIdAsync(id);

            if (!result.Success)
            {
                return result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true
                    ? NotFound(result)
                    : BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update address (Admin)
        /// </summary>
        /// <param name="id">Address ID</param>
        /// <param name="dto">Updated data</param>
        /// <response code="200">Address updated</response>
        /// <response code="400">Invalid data</response>
        /// <response code="404">Address not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<AddressResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<AddressResponseDto>>> Update(
            Guid id,
            [FromBody] UpdateAddressDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequest(ApiResponse<AddressResponseDto>.FailureResponse(
                    message: "Invalid input data",
                    errors: errors));
            }

            var result = await _addressService.AdminUpdateAddressAsync(id, dto);

            if (!result.Success)
            {
                return result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true
                    ? NotFound(result)
                    : BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete address (soft/hard - Admin)
        /// </summary>
        /// <param name="id">Address ID</param>
        /// <response code="200">Address deleted</response>
        /// <response code="404">Address not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _addressService.AdminDeleteAddressAsync(id);

            if (!result.Success)
            {
                return result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true
                    ? NotFound(result)
                    : BadRequest(result);
            }

            return Ok(result);
        }
    }
}