using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Authorization;
using VietCommerce.Core.DTOs.UserRoles;

namespace VietCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class UserRoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<UserRoleController> _logger;

    public UserRoleController(IRoleService roleService, IPermissionService permissionService, ILogger<UserRoleController> logger)
    {
        _roleService = roleService;
        _permissionService = permissionService;
        _logger = logger;
    }

    [HttpGet("users/{userId}/roles")]
    [RequirePermission("user.read", "role.read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRoles(Guid userId)
    {
        try
        {
            var roles = await _permissionService.GetUserRolesAsync(userId);
            var permissions = await _permissionService.GetUserPermissionsAsync(userId);

            var result = new UserRoleDetailDTO
            {
                User = new Core.DTOs.Users.UserListDTO { Id = userId }, // You might want to populate this properly
                Roles = roles.Select(r => new Core.DTOs.Roles.RoleListDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                }).ToList(),
                EffectivePermissions = permissions.Select(p => new Core.DTOs.Permissions.PermissionListDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList(),
                LastUpdated = DateTime.UtcNow
            };

            return Ok(new { Success = true, Data = result, Message = "User roles retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user {UserId}", userId);
            return BadRequest(new { Success = false, Message = "Failed to retrieve user roles" });
        }
    }

    [HttpPost("users/{userId}/roles")]
    [RequirePermission("user.update", "role.assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignRoleToUser(Guid userId, [FromBody] AssignRoleDTO request)
    {
        if (request.UserId != userId)
            return BadRequest(new { Success = false, Message = "User ID mismatch" });

        var result = await _roleService.AssignRoleToUserAsync(userId, request.RoleId);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("users/{userId}/roles/{roleId}")]
    [RequirePermission("user.update", "role.assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveRoleFromUser(Guid userId, Guid roleId)
    {
        var result = await _roleService.RemoveRoleFromUserAsync(userId, roleId);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("users/{userId}/roles/sync")]
    [RequirePermission("user.update", "role.assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SyncUserRoles(Guid userId, [FromBody] List<Guid> roleIds)
    {
        var result = await _roleService.SyncUserRolesAsync(userId, roleIds);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("users/{userId}/permissions")]
    [RequirePermission("user.read", "permission.read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserPermissions(Guid userId)
    {
        try
        {
            var permissions = await _permissionService.GetUserPermissionNamesAsync(userId);
            var roles = await _permissionService.GetUserRolesAsync(userId);

            var result = new UserAuthorizationInfoDTO
            {
                UserId = userId,
                Roles = roles.Select(r => new Core.DTOs.Roles.RoleListDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                }).ToList(),
                Permissions = (await _permissionService.GetUserPermissionsAsync(userId)).Select(p => new Core.DTOs.Permissions.PermissionListDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList(),
                LastUpdated = DateTime.UtcNow
            };

            return Ok(new { Success = true, Data = result, Message = "User permissions retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for user {UserId}", userId);
            return BadRequest(new { Success = false, Message = "Failed to retrieve user permissions" });
        }
    }

    [HttpPost("check-permission")]
    [RequirePermission("permission.check")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckUserPermission([FromBody] BulkPermissionCheckDTO request)
    {
        try
        {
            var results = new Dictionary<string, bool>();
            var allowedPermissions = new List<string>();
            var deniedPermissions = new List<string>();

            foreach (var permission in request.Permissions)
            {
                var hasPermission = await _permissionService.CheckUserPermissionAsync(request.UserId, permission);
                results[permission] = hasPermission;

                if (hasPermission)
                    allowedPermissions.Add(permission);
                else
                    deniedPermissions.Add(permission);
            }

            var result = new BulkPermissionCheckResultDTO
            {
                UserId = request.UserId,
                PermissionResults = results,
                AllowedPermissions = allowedPermissions,
                DeniedPermissions = deniedPermissions,
                CheckedAt = DateTime.UtcNow
            };

            return Ok(new { Success = true, Data = result, Message = "Permission check completed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permissions for user {UserId}", request.UserId);
            return BadRequest(new { Success = false, Message = "Permission check failed" });
        }
    }

    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyRolesAndPermissions()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { Success = false, Message = "Invalid user token" });

        try
        {
            var roles = await _permissionService.GetUserRolesAsync(userId);
            var permissions = await _permissionService.GetUserPermissionsAsync(userId);

            var result = new UserAuthorizationInfoDTO
            {
                UserId = userId,
                UserEmail = User.FindFirstValue(ClaimTypes.Email) ?? "",
                UserName = User.FindFirstValue(ClaimTypes.Name) ?? "",
                Roles = roles.Select(r => new Core.DTOs.Roles.RoleListDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                }).ToList(),
                Permissions = permissions.Select(p => new Core.DTOs.Permissions.PermissionListDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList(),
                LastUpdated = DateTime.UtcNow
            };

            return Ok(new { Success = true, Data = result, Message = "User authorization info retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving authorization info for user {UserId}", userId);
            return BadRequest(new { Success = false, Message = "Failed to retrieve authorization info" });
        }
    }
}
