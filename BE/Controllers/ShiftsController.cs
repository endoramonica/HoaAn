using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VietCommerce.Application.Extensions;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.HRM;
using VietCommerce.Core.Models;

namespace VietCommerce.AdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Yêu cầu đăng nhập cho controller này
    public class ShiftsController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public ShiftsController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        [HttpGet]
        public async Task<IActionResult> GetShifts([FromQuery] PaginationParams pagination, [FromQuery] ShiftFilters filters)
        {
            var result = await _shiftService.GetShiftsAsync(pagination, filters);
            return Ok(result.ToResponse());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShiftById(Guid id)
        {
            var shift = await _shiftService.GetShiftByIdAsync(id);
            return Ok(shift);
        }

        [HttpGet("current/{userId}")]
        public async Task<IActionResult> GetCurrentShift(Guid userId)
        {
            var shift = await _shiftService.GetCurrentShiftAsync(userId);
            if (shift == null)
                return NotFound();

            return Ok(shift);
        }

        [HttpPost("open")]
        public async Task<IActionResult> OpenShift([FromBody] OpenShiftRequest request)
        {
            var shift = await _shiftService.OpenShiftAsync(request);
            return CreatedAtAction(nameof(GetShiftById), new { id = shift.Id }, shift);
        }

        [HttpPost("close/{id}")]
        public async Task<IActionResult> CloseShift(Guid id, [FromBody] CloseShiftRequest request)
        {
            var shift = await _shiftService.CloseShiftAsync(id, request);
            return Ok(shift);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShift(Guid id, [FromBody] UpdateShiftRequest request)
        {
            var shift = await _shiftService.UpdateShiftAsync(id, request);
            return Ok(shift);
        }
    }
}
