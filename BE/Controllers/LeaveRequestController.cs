using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Models;
using VietCommerce.Application.Extensions;
using VietCommerce.Core.Entities.HRM;

namespace VietCommerce.AdminAPI.Controllers
{
    /// <summary>
    /// Controller quản lý Leave Request
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestController(ILeaveRequestService leaveRequestService)
        {
            _leaveRequestService = leaveRequestService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<LeaveRequestDto>>> GetLeaveRequests([FromQuery] PaginationParams pagination, [FromQuery] LeaveRequestFilters filters)
        {
            var paginatedResult = await _leaveRequestService.GetLeaveRequestsAsync(pagination, filters);
            var response = paginatedResult.ToResponse();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequestDto>> GetLeaveRequestById(Guid id)
        {
            var leaveRequest = await _leaveRequestService.GetLeaveRequestByIdAsync(id);
            if (leaveRequest == null) return NotFound();
            return Ok(leaveRequest);
        }

        [HttpPost]
        public async Task<ActionResult<LeaveRequestDto>> CreateLeaveRequest([FromBody] CreateLeaveRequestRequest request)
        {
            var created = await _leaveRequestService.CreateLeaveRequestAsync(request);
            return CreatedAtAction(nameof(GetLeaveRequestById), new { id = created.Id }, created);
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<LeaveRequestDto>> UpdateLeaveRequestStatus(Guid id, [FromBody] UpdateLeaveRequestStatusDto statusUpdate)
        {
            // Assuming UpdateLeaveRequestStatusDto contains LeaveRequestStatus and optional comments
            var updated = await _leaveRequestService.UpdateLeaveRequestStatusAsync(id, statusUpdate.Status, statusUpdate.Comments);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        
    }
}
