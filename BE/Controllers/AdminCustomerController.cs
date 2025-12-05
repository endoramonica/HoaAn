using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Application.Extensions;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Address;
using VietCommerce.Core.DTOs.CRM;
using VietCommerce.Core.DTOs.Customers;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Models;

namespace VietCommerce.AdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ✅ yêu cầu đăng nhập
    public class AdminCustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public AdminCustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        #region Customer Management

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CustomerListDto>>), 200)]
        public async Task<IActionResult> GetAll(
            [FromQuery] PaginationParams pagination,
            [FromQuery] CustomerFilters filters)
        {
            var result = await _customerService.GetCustomersAsync(pagination, filters);

            if (!result.Success)
                return BadRequest(result);

            // Transform PaginatedResult -> PaginatedResponse
            var responseData = result.Data.ToResponse();
            return Ok(ApiResponse<PaginatedResponse<CustomerListDto>>.SuccessResponse(responseData, result.Message));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerDetailDto>), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _customerService.GetCustomerByIdAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CustomerDetailDto>), 201)]
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.FailureResponse("Invalid request data"));

            var result = await _customerService.CreateCustomerAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerDetailDto>), 200)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<object>.FailureResponse("Invalid request data"));

            var result = await _customerService.UpdateCustomerAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<CustomerListDto>>), 200)]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            var result = await _customerService.SearchCustomersAsync(term);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/statistics")]
        [ProducesResponseType(typeof(ApiResponse<CustomerStatisticsDto>), 200)]
        public async Task<IActionResult> GetStatistics(Guid id)
        {
            var result = await _customerService.GetCustomerStatisticsAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        #endregion

        #region Orders

        [HttpGet("{id}/orders")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<OrderDetailDto>>), 200)]
        public async Task<IActionResult> GetCustomerOrders(
            Guid id,
            [FromQuery] PaginationParams pagination)
        {
            var result = await _customerService.GetCustomerOrdersAsync(id, pagination);

            if (!result.Success)
                return BadRequest(result);

            // Transform PaginatedResult -> PaginatedResponse
            var responseData = result.Data.ToResponse();
            return Ok(ApiResponse<PaginatedResponse<OrderDetailDto>>.SuccessResponse(responseData, result.Message));
        }

        [HttpGet("{id}/orders/summary")]
        [ProducesResponseType(typeof(ApiResponse<CustomerOrderSummaryDto>), 200)]
        public async Task<IActionResult> GetOrderSummary(Guid id)
        {
            var result = await _customerService.GetCustomerOrderSummaryAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        #endregion

        #region Addresses

        [HttpGet("{id}/addresses")]
        [ProducesResponseType(typeof(ApiResponse<List<CustomerAddressDto>>), 200)]
        public async Task<IActionResult> GetAddresses(Guid id)
        {
            var result = await _customerService.GetCustomerAddressesAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("addresses/{addressId}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerAddressDto>), 200)]
        public async Task<IActionResult> GetAddressById(Guid addressId)
        {
            var result = await _customerService.GetCustomerAddressByIdAsync(addressId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("addresses")]
        [ProducesResponseType(typeof(ApiResponse<CustomerAddressDto>), 200)]
        public async Task<IActionResult> CreateAddress([FromBody] CreateCustomerAddressRequest request)
        {
            var result = await _customerService.CreateCustomerAddressAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("addresses/{addressId}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerAddressDto>), 200)]
        public async Task<IActionResult> UpdateAddress(Guid addressId, [FromBody] UpdateCustomerAddressRequest request)
        {
            var result = await _customerService.UpdateCustomerAddressAsync(addressId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("addresses/{addressId}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> DeleteAddress(Guid addressId)
        {
            var result = await _customerService.DeleteCustomerAddressAsync(addressId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/addresses/{addressId}/default")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> SetDefaultAddress(Guid id, Guid addressId)
        {
            var result = await _customerService.SetDefaultAddressAsync(id, addressId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        #endregion

        #region CRM Interactions

        [HttpGet("{id}/interactions")]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CRMInteractionListDto>>), 200)]
        public async Task<IActionResult> GetInteractions(
            Guid id,
            [FromQuery] PaginationParams pagination,
            [FromQuery] CRMInteractionFilters filters)
        {
            var result = await _customerService.GetCustomerInteractionsAsync(id, pagination, filters);

            if (!result.Success)
                return BadRequest(result);

            var responseData = result.Data.ToResponse();
            return Ok(ApiResponse<PaginatedResponse<CRMInteractionListDto>>.SuccessResponse(responseData, result.Message));
        }

        [HttpGet("interactions/{interactionId}")]
        [ProducesResponseType(typeof(ApiResponse<CRMInteractionDto>), 200)]
        public async Task<IActionResult> GetInteractionById(Guid interactionId)
        {
            var result = await _customerService.GetInteractionByIdAsync(interactionId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("interactions")]
        [ProducesResponseType(typeof(ApiResponse<CRMInteractionDto>), 200)]
        public async Task<IActionResult> CreateInteraction([FromBody] CreateInteractionRequest request)
        {
            var result = await _customerService.CreateCustomerInteractionAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("interactions/{interactionId}")]
        [ProducesResponseType(typeof(ApiResponse<CRMInteractionDto>), 200)]
        public async Task<IActionResult> UpdateInteraction(Guid interactionId, [FromBody] UpdateInteractionRequest request)
        {
            var result = await _customerService.UpdateCustomerInteractionAsync(interactionId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("interactions/{interactionId}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> DeleteInteraction(Guid interactionId)
        {
            var result = await _customerService.DeleteCustomerInteractionAsync(interactionId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("interactions/{interactionId}/complete")]
        [ProducesResponseType(typeof(ApiResponse<CRMInteractionDto>), 200)]
        public async Task<IActionResult> CompleteInteraction(Guid interactionId)
        {
            var result = await _customerService.CompleteInteractionAsync(interactionId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("interactions/upcoming")]
        [ProducesResponseType(typeof(ApiResponse<List<CRMInteractionListDto>>), 200)]
        public async Task<IActionResult> GetUpcomingFollowUps([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var result = await _customerService.GetUpcomingFollowUpsAsync(from, to);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        #endregion

        #region Loyalty

        [HttpPost("{id}/loyalty/add")]
        [ProducesResponseType(typeof(ApiResponse<CustomerDetailDto>), 200)]
        public async Task<IActionResult> AddLoyaltyPoints(Guid id, [FromBody] AddPointsRequest request)
        {
            // Giả sử tạo DTO wrapper nhỏ cho body request nếu cần, hoặc dùng tham số trực tiếp nếu config cho phép
            // Ở đây tôi giả định bạn sẽ parse body để lấy points và reason
            var result = await _customerService.AddLoyaltyPointsAsync(id, request.Points, request.Reason);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/loyalty/deduct")]
        [ProducesResponseType(typeof(ApiResponse<CustomerDetailDto>), 200)]
        public async Task<IActionResult> DeductLoyaltyPoints(Guid id, [FromBody] DeductPointsRequest request)
        {
            var result = await _customerService.DeductLoyaltyPointsAsync(id, request.Points, request.Reason);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id}/tier")]
        [ProducesResponseType(typeof(ApiResponse<CustomerDetailDto>), 200)]
        public async Task<IActionResult> UpdateTier(Guid id, [FromBody] UpdateTierRequest request)
        {
            var result = await _customerService.UpdateCustomerTierAsync(id, request.Tier);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/loyalty-history")]
        [ProducesResponseType(typeof(ApiResponse<List<LoyaltyHistoryDto>>), 200)]
        public async Task<IActionResult> GetLoyaltyHistory(Guid id)
        {
            var result = await _customerService.GetLoyaltyHistoryAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        #endregion
    }

    
}