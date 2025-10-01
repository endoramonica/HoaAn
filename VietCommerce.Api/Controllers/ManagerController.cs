using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Manager;

namespace VietCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public class ManagerController : ControllerBase
{
    private readonly IManagerService _managerService;
    private readonly ILogger<ManagerController> _logger;

    public ManagerController(IManagerService managerService, ILogger<ManagerController> logger)
    {
        _managerService = managerService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetManagers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var response = await _managerService.GetManagersAsync(pageNumber, pageSize, search);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetManagerById(Guid id)
    {
        var response = await _managerService.GetManagerByIdAsync(id);
        if (!response.Success || response.Data == null)
            return NotFound(response);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateManager([FromBody] ManagerCreateDTO dto)
    {
        var response = await _managerService.CreateManagerAsync(dto);
        if (!response.Success)
            return BadRequest(response);
        return CreatedAtAction(nameof(GetManagerById), new { id = response.Data.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateManager(Guid id, [FromBody] ManagerUpdateDTO dto)
    {
        var response = await _managerService.UpdateManagerAsync(id, dto);
        if (!response.Success)
            return BadRequest(response);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteManager(Guid id)
    {
        var response = await _managerService.DeleteManagerAsync(id);
        return Ok(response);
    }
}
