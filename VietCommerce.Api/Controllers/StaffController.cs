using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Staff;

namespace VietCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin,Manager")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;
    private readonly ILogger<StaffController> _logger;

    public StaffController(IStaffService staffService, ILogger<StaffController> logger)
    {
        _staffService = staffService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetStaff([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var response = await _staffService.GetStaffAsync(pageNumber, pageSize, search);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStaffById(Guid id)
    {
        var response = await _staffService.GetStaffByIdAsync(id);
        if (!response.Success || response.Data == null)
            return NotFound(response);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStaff([FromBody] StaffCreateDTO dto)
    {
        var response = await _staffService.CreateStaffAsync(dto);
        if (!response.Success)
            return BadRequest(response);
        return CreatedAtAction(nameof(GetStaffById), new { id = response.Data.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStaff(Guid id, [FromBody] StaffUpdateDTO dto)
    {
        var response = await _staffService.UpdateStaffAsync(id, dto);
        if (!response.Success)
            return BadRequest(response);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteStaff(Guid id)
    {
        var response = await _staffService.DeleteStaffAsync(id);
        return Ok(response);
    }
}
