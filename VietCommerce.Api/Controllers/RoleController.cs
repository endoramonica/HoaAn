using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Permissions;
using VietCommerce.Core.DTOs.Roles;
using VietCommerce.Core.DTOs.UserRoles;

namespace VietCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly ILogger<RoleController> _logger;

    public RoleController(IRoleService roleService, ILogger<RoleController> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    [HttpGet]
    [RequirePermission("role.read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRoles([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
    {
        var result = await _roleService.GetRolesAsync(page, pageSize, searchTerm);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id}")]
    [RequirePermission("role.read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoleById(Guid id)
    {
        var result = await _roleService.GetRoleByIdAsync(id);
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpPost]
    [RequirePermission("role.create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRole([FromBody] RoleCreateDTO request)
    {
        var result = await _roleService.CreateRoleAsync(request);
        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetRoleById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    [RequirePermission("role.update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] RoleUpdateDTO request)
    {
        var result = await _roleService.UpdateRoleAsync(id, request);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [RequirePermission("role.delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var result = await _roleService.DeleteRoleAsync(id);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id}/permissions")]
    [RequirePermission("role.read", "permission.read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRolePermissions(Guid id)
    {
        var result = await _roleService.GetRolePermissionsAsync(id);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{roleId}/permissions/{permissionId}")]
    [RequirePermission("role.update", "permission.assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignPermissionToRole(Guid roleId, Guid permissionId)
    {
        var result = await _roleService.AssignPermissionToRoleAsync(roleId, permissionId);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("{roleId}/permissions/{permissionId}")]
    [RequirePermission("role.update", "permission.assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemovePermissionFromRole(Guid roleId, Guid permissionId)
    {
        var result = await _roleService.RemovePermissionFromRoleAsync(roleId, permissionId);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("{id}/permissions/sync")]
    [RequirePermission("role.update", "permission.assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncRolePermissions(Guid id, [FromBody] List<Guid> permissionIds)
    {
        var result = await _roleService.SyncRolePermissionsAsync(id, permissionIds);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}
