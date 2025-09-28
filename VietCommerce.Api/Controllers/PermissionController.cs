using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.Attributes;
using VietCommerce.Core.DTOs.Permissions;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class PermissionController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PermissionController> _logger;

    public PermissionController(IUnitOfWork unitOfWork, ILogger<PermissionController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [RequirePermission("permission.read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPermissions([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? searchTerm = null)
    {
        try
        {
            var permissions = await _unitOfWork.Permissions.GetAllAsync();
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                permissions = permissions.Where(p => 
                    p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(p.Description) && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            var totalCount = permissions.Count;
            var pagedPermissions = permissions.Skip((page - 1) * pageSize).Take(pageSize);

            var permissionDtos = pagedPermissions.Select(p => new PermissionListDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                RoleCount = p.RolePermissions.Count
            }).ToList();

            var result = new
            {
                Success = true,
                Data = new
                {
                    Data = permissionDtos,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                },
                Message = "Permissions retrieved successfully"
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions");
            return BadRequest(new { Success = false, Message = "Failed to retrieve permissions" });
        }
    }

    [HttpGet("{id}")]
    [RequirePermission("permission.read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPermissionById(Guid id)
    {
        try
        {
            var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
            if (permission == null)
                return NotFound(new { Success = false, Message = "Permission not found" });

            var permissionDto = new PermissionDetailDTO
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                CreatedAt = permission.CreatedAt,
                UpdatedAt = permission.UpdatedAt,
                Roles = permission.RolePermissions.Select(rp => new RoleListDTO
                {
                    Id = rp.Role.Id,
                    Name = rp.Role.Name,
                    Description = rp.Role.Description,
                    CreatedAt = rp.Role.CreatedAt,
                    UpdatedAt = rp.Role.UpdatedAt
                }).ToList()
            };

            return Ok(new { Success = true, Data = permissionDto, Message = "Permission retrieved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permission {PermissionId}", id);
            return BadRequest(new { Success = false, Message = "Failed to retrieve permission" });
        }
    }

    [HttpPost]
    [RequirePermission("permission.create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePermission([FromBody] PermissionCreateDTO request)
    {
        try
        {
            var exists = await _unitOfWork.Permissions.PermissionExistsAsync(request.Name);
            if (exists)
                return BadRequest(new { Success = false, Message = "Permission name already exists" });

            var permission = new Core.Entities.Users.Permission
            {
                Name = request.Name,
                Description = request.Description
            };

            await _unitOfWork.Permissions.AddAsync(permission);
            await _unitOfWork.SaveChangesAsync();

            var permissionDto = new PermissionDetailDTO
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                CreatedAt = permission.CreatedAt,
                UpdatedAt = permission.UpdatedAt,
                Roles = new List<RoleListDTO>()
            };

            _logger.LogInformation("Permission {PermissionName} created successfully", request.Name);
            return CreatedAtAction(nameof(GetPermissionById), new { id = permission.Id }, 
                new { Success = true, Data = permissionDto, Message = "Permission created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating permission {PermissionName}", request.Name);
            return BadRequest(new { Success = false, Message = "Failed to create permission" });
        }
    }

    [HttpPut("{id}")]
    [RequirePermission("permission.update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePermission(Guid id, [FromBody] PermissionUpdateDTO request)
    {
        try
        {
            var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
            if (permission == null)
                return NotFound(new { Success = false, Message = "Permission not found" });

            if (permission.Name != request.Name)
            {
                var exists = await _unitOfWork.Permissions.PermissionExistsAsync(request.Name);
                if (exists)
                    return BadRequest(new { Success = false, Message = "Permission name already exists" });
            }

            permission.Name = request.Name;
            permission.Description = request.Description;

            _unitOfWork.Permissions.Update(permission);
            await _unitOfWork.SaveChangesAsync();

            var permissionDto = new PermissionDetailDTO
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                CreatedAt = permission.CreatedAt,
                UpdatedAt = permission.UpdatedAt,
                Roles = permission.RolePermissions.Select(rp => new RoleListDTO
                {
                    Id = rp.Role.Id,
                    Name = rp.Role.Name,
                    Description = rp.Role.Description,
                    CreatedAt = rp.Role.CreatedAt,
                    UpdatedAt = rp.Role.UpdatedAt
                }).ToList()
            };

            _logger.LogInformation("Permission {PermissionId} updated successfully", id);
            return Ok(new { Success = true, Data = permissionDto, Message = "Permission updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permission {PermissionId}", id);
            return BadRequest(new { Success = false, Message = "Failed to update permission" });
        }
    }

    [HttpDelete("{id}")]
    [RequirePermission("permission.delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeletePermission(Guid id)
    {
        try
        {
            var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
            if (permission == null)
                return NotFound(new { Success = false, Message = "Permission not found" });

            var rolePermissions = await _unitOfWork.RolePermissions.GetByPermissionIdAsync(id);
            if (rolePermissions.Any())
                return BadRequest(new { Success = false, Message = "Cannot delete permission that is assigned to roles" });

            _unitOfWork.Permissions.Delete(permission);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Permission {PermissionId} deleted successfully", id);
            return Ok(new { Success = true, Data = true, Message = "Permission deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting permission {PermissionId}", id);
            return BadRequest(new { Success = false, Message = "Failed to delete permission" });
        }
    }
}
