using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Admin;

namespace VietCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IAdminService adminService, ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? role = null)
    {
        var response = await _adminService.GetAllUsersAsync(pageNumber, pageSize, search, role);
        return Ok(response);
    }

    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var response = await _adminService.GetUserByIdAsync(id);
        if (!response.Success || response.Data == null)
            return NotFound(response);
        return Ok(response);
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] AdminUserCreateDTO dto)
    {
        var response = await _adminService.CreateUserAsync(dto);
        if (!response.Success)
            return BadRequest(response);
        return CreatedAtAction(nameof(GetUserById), new { id = response.Data.Id }, response);
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] AdminUserUpdateDTO dto)
    {
        var response = await _adminService.UpdateUserAsync(id, dto);
        if (!response.Success)
            return BadRequest(response);
        return Ok(response);
    }

    [HttpPost("users/{id}/assign-roles")]
    public async Task<IActionResult> AssignRolesToUser(Guid id, [FromBody] List<Guid> roleIds)
    {
        var response = await _adminService.AssignRolesToUserAsync(id, roleIds);
        if (!response.Success)
            return BadRequest(response);
        return Ok(response);
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var response = await _adminService.DeleteUserAsync(id);
        return Ok(response);
    }
}
