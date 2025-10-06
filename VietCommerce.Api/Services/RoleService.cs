// VietCommerce.Api/Services/RoleService.cs
using Microsoft.Extensions.Logging;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Roles;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Api.Services;

public class RoleService 
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        IUnitOfWork unitOfWork,
        IPermissionService permissionService,
        ILogger<RoleService> logger)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _logger = logger;
    }

    // public async Task<ApiResponse<RoleDetailDTO>> GetRoleByIdAsync(Guid roleId)
    // {
    //     try
    //     {
    //         var role = await _unitOfWork.Roles.GetRoleWithPermissionsAsync(roleId);
    //         if (role == null)
    //             return ApiResponse<RoleDetailDTO>.FailureResponse("Role not found");

    //         var roleDto = new RoleDetailDTO
    //         {
    //             Id = role.Id,
    //             Name = role.Name,
    //             Description = role.Description,
    //             CreatedAt = role.CreatedAt,
    //             UpdatedAt = role.UpdatedAt,
    //             Permissions = role.RolePermissions.Select(rp => new PermissionListDTO
    //             {
    //                 Id = rp.Permission.Id,
    //                 Name = rp.Permission.Name,
    //                 Description = rp.Permission.Description
    //             }).ToList()
    //         };

    //         return ApiResponse<RoleDetailDTO>.SuccessResponse(roleDto, "Role retrieved successfully");
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error retrieving role {RoleId}", roleId);
    //         return ApiResponse<RoleDetailDTO>.FailureResponse("Failed to retrieve role");
    //     }
    // }

    // public async Task<ApiResponse<PaginatedResult<RoleListDTO>>> GetRolesAsync(int page, int pageSize, string? searchTerm = null)
    // {
    //     try
    //     {
    //         var roles = await _unitOfWork.Roles.GetAllAsync();
            
    //         if (!string.IsNullOrWhiteSpace(searchTerm))
    //         {
    //             roles = roles.Where(r => r.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
    //                                    (!string.IsNullOrEmpty(r.Description) && r.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
    //                         .ToList();
    //         }

    //         var totalCount = roles.Count;
    //         var pagedRoles = roles.Skip((page - 1) * pageSize).Take(pageSize);

    //         var roleDtos = pagedRoles.Select(r => new RoleListDTO
    //         {
    //             Id = r.Id,
    //             Name = r.Name,
    //             Description = r.Description,
    //             CreatedAt = r.CreatedAt,
    //             UpdatedAt = r.UpdatedAt
    //         }).ToList();

    //         var result = new PaginatedResult<RoleListDTO>
    //         {
    //             Data = roleDtos,
    //             TotalCount = totalCount,
    //             Page = page,
    //             PageSize = pageSize,
    //             TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
    //         };

    //         return ApiResponse<PaginatedResult<RoleListDTO>>.SuccessResponse(result, "Roles retrieved successfully");
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error retrieving roles");
    //         return ApiResponse<PaginatedResult<RoleListDTO>>.FailureResponse("Failed to retrieve roles");
    //     }
    // }

    // public async Task<ApiResponse<RoleDetailDTO>> CreateRoleAsync(RoleCreateDTO request)
    // {
    //     try
    //     {
    //         if (await RoleExistsAsync(request.Name))
    //             return ApiResponse<RoleDetailDTO>.FailureResponse("Role name already exists");

    //         var role = new Role
    //         {
    //             Name = request.Name,
    //             Description = request.Description
    //         };

    //         await _unitOfWork.Roles.AddAsync(role);
    //         await _unitOfWork.SaveChangesAsync();

    //         var createdRole = await _unitOfWork.Roles.GetRoleWithPermissionsAsync(role.Id);
    //         var roleDto = new RoleDetailDTO
    //         {
    //             Id = createdRole!.Id,
    //             Name = createdRole.Name,
    //             Description = createdRole.Description,
    //             CreatedAt = createdRole.CreatedAt,
    //             UpdatedAt = createdRole.UpdatedAt,
    //             Permissions = new List<PermissionListDTO>()
    //         };

    //         _logger.LogInformation("Role {RoleName} created successfully with ID {RoleId}", request.Name, role.Id);
    //         return ApiResponse<RoleDetailDTO>.SuccessResponse(roleDto, "Role created successfully");
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error creating role {RoleName}", request.Name);
    //         return ApiResponse<RoleDetailDTO>.FailureResponse("Failed to create role");
    //     }
    // }

    // public async Task<ApiResponse<RoleDetailDTO>> UpdateRoleAsync(Guid roleId, RoleUpdateDTO request)
    // {
    //     try
    //     {
    //         var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
    //         if (role == null)
    //             return ApiResponse<RoleDetailDTO>.FailureResponse("Role not found");

    //         if (role.Name != request.Name && await RoleExistsAsync(request.Name))
    //             return ApiResponse<RoleDetailDTO>.FailureResponse("Role name already exists");

    //         role.Name = request.Name;
    //         role.Description = request.Description;

    //         _unitOfWork.Roles.Update(role);
    //         await _unitOfWork.SaveChangesAsync();

    //         var updatedRole = await _unitOfWork.Roles.GetRoleWithPermissionsAsync(roleId);
    //         var roleDto = new RoleDetailDTO
    //         {
    //             Id = updatedRole!.Id,
    //             Name = updatedRole.Name,
    //             Description = updatedRole.Description,
    //             CreatedAt = updatedRole.CreatedAt,
    //             UpdatedAt = updatedRole.UpdatedAt,
    //             Permissions = updatedRole.RolePermissions.Select(rp => new PermissionListDTO
    //             {
    //                 Id = rp.Permission.Id,
    //                 Name = rp.Permission.Name,
    //                 Description = rp.Permission.Description
    //             }).ToList()
    //         };

    //         _logger.LogInformation("Role {RoleId} updated successfully", roleId);
    //         return ApiResponse<RoleDetailDTO>.SuccessResponse(roleDto, "Role updated successfully");
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error updating role {RoleId}", roleId);
    //         return ApiResponse<RoleDetailDTO>.FailureResponse("Failed to update role");
    //     }
    // }

    public async Task<ApiResponse<bool>> DeleteRoleAsync(Guid roleId)
    {
        try
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
            if (role == null)
                return ApiResponse<bool>.FailureResponse("Role not found");

            // Check if role is assigned to any users
            var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(roleId);
            if (userRoles.Any())
                return ApiResponse<bool>.FailureResponse("Cannot delete role that is assigned to users");

            // Remove all permissions from role first
            await _unitOfWork.RolePermissions.RemoveAllRolePermissionsAsync(roleId);
            
            // Delete the role
            _unitOfWork.Roles.Delete(role);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Role {RoleId} deleted successfully", roleId);
            return ApiResponse<bool>.SuccessResponse(true, "Role deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", roleId);
            return ApiResponse<bool>.FailureResponse("Failed to delete role");
        }
    }

    public async Task<ApiResponse<List<Permission>>> GetRolePermissionsAsync(Guid roleId)
    {
        try
        {
            var permissions = await _unitOfWork.Permissions.GetRolePermissionsAsync(roleId);
            return ApiResponse<List<Permission>>.SuccessResponse(permissions, "Role permissions retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for role {RoleId}", roleId);
            return ApiResponse<List<Permission>>.FailureResponse("Failed to retrieve role permissions");
        }
    }

    public async Task<ApiResponse<bool>> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId)
    {
        try
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
            if (role == null)
                return ApiResponse<bool>.FailureResponse("Role not found");

            var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
            if (permission == null)
                return ApiResponse<bool>.FailureResponse("Permission not found");

            var hasPermission = await _unitOfWork.RolePermissions.RoleHasPermissionAsync(roleId, permissionId);
            if (hasPermission)
                return ApiResponse<bool>.SuccessResponse(true, "Role already has this permission");

            await _unitOfWork.RolePermissions.AssignPermissionToRoleAsync(roleId, permissionId);
            await _unitOfWork.SaveChangesAsync();

            // Clear cache for all users with this role
            var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(roleId);
            foreach (var userRole in userRoles)
            {
                _permissionService.ClearUserPermissionsCache(userRole.UserId);
            }

            _logger.LogInformation("Permission {PermissionId} assigned to role {RoleId}", permissionId, roleId);
            return ApiResponse<bool>.SuccessResponse(true, "Permission assigned to role successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permission {PermissionId} to role {RoleId}", permissionId, roleId);
            return ApiResponse<bool>.FailureResponse("Failed to assign permission to role");
        }
    }

    public async Task<ApiResponse<bool>> RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId)
    {
        try
        {
            await _unitOfWork.RolePermissions.RemovePermissionFromRoleAsync(roleId, permissionId);
            await _unitOfWork.SaveChangesAsync();

            // Clear cache for all users with this role
            var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(roleId);
            foreach (var userRole in userRoles)
            {
                _permissionService.ClearUserPermissionsCache(userRole.UserId);
            }

            _logger.LogInformation("Permission {PermissionId} removed from role {RoleId}", permissionId, roleId);
            return ApiResponse<bool>.SuccessResponse(true, "Permission removed from role successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing permission {PermissionId} from role {RoleId}", permissionId, roleId);
            return ApiResponse<bool>.FailureResponse("Failed to remove permission from role");
        }
    }

    public async Task<ApiResponse<bool>> AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds)
    {
        try
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
            if (role == null)
                return ApiResponse<bool>.FailureResponse("Role not found");

            foreach (var permissionId in permissionIds)
            {
                var hasPermission = await _unitOfWork.RolePermissions.RoleHasPermissionAsync(roleId, permissionId);
                if (!hasPermission)
                {
                    await _unitOfWork.RolePermissions.AssignPermissionToRoleAsync(roleId, permissionId);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            // Clear cache for all users with this role
            var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(roleId);
            foreach (var userRole in userRoles)
            {
                _permissionService.ClearUserPermissionsCache(userRole.UserId);
            }

            _logger.LogInformation("Assigned {PermissionCount} permissions to role {RoleId}", permissionIds.Count, roleId);
            return ApiResponse<bool>.SuccessResponse(true, "Permissions assigned to role successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permissions to role {RoleId}", roleId);
            return ApiResponse<bool>.FailureResponse("Failed to assign permissions to role");
        }
    }

    public async Task<ApiResponse<bool>> SyncRolePermissionsAsync(Guid roleId, List<Guid> permissionIds)
    {
        try
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
            if (role == null)
                return ApiResponse<bool>.FailureResponse("Role not found");

            // Remove all existing permissions
            await _unitOfWork.RolePermissions.RemoveAllRolePermissionsAsync(roleId);

            // Add new permissions
            foreach (var permissionId in permissionIds)
            {
                await _unitOfWork.RolePermissions.AssignPermissionToRoleAsync(roleId, permissionId);
            }

            await _unitOfWork.SaveChangesAsync();

            // Clear cache for all users with this role
            var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(roleId);
            foreach (var userRole in userRoles)
            {
                _permissionService.ClearUserPermissionsCache(userRole.UserId);
            }

            _logger.LogInformation("Synced {PermissionCount} permissions for role {RoleId}", permissionIds.Count, roleId);
            return ApiResponse<bool>.SuccessResponse(true, "Role permissions synchronized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing permissions for role {RoleId}", roleId);
            return ApiResponse<bool>.FailureResponse("Failed to sync role permissions");
        }
    }

    public async Task<ApiResponse<bool>> AssignRoleToUserAsync(Guid userId, Guid roleId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("User not found");

            var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
            if (role == null)
                return ApiResponse<bool>.FailureResponse("Role not found");

            var hasRole = await _unitOfWork.UserRoles.UserHasRoleAsync(userId, roleId);
            if (hasRole)
                return ApiResponse<bool>.SuccessResponse(true, "User already has this role");

            await _unitOfWork.UserRoles.AssignRoleToUserAsync(userId, roleId);
            await _unitOfWork.SaveChangesAsync();

            _permissionService.ClearUserPermissionsCache(userId);

            _logger.LogInformation("Role {RoleId} assigned to user {UserId}", roleId, userId);
            return ApiResponse<bool>.SuccessResponse(true, "Role assigned to user successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
            return ApiResponse<bool>.FailureResponse("Failed to assign role to user");
        }
    }

    public async Task<ApiResponse<bool>> RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        try
        {
            await _unitOfWork.UserRoles.RemoveRoleFromUserAsync(userId, roleId);
            await _unitOfWork.SaveChangesAsync();

            _permissionService.ClearUserPermissionsCache(userId);

            _logger.LogInformation("Role {RoleId} removed from user {UserId}", roleId, userId);
            return ApiResponse<bool>.SuccessResponse(true, "Role removed from user successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
            return ApiResponse<bool>.FailureResponse("Failed to remove role from user");
        }
    }

    public async Task<ApiResponse<bool>> AssignRolesToUserAsync(Guid userId, List<Guid> roleIds)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("User not found");

            foreach (var roleId in roleIds)
            {
                var hasRole = await _unitOfWork.UserRoles.UserHasRoleAsync(userId, roleId);
                if (!hasRole)
                {
                    await _unitOfWork.UserRoles.AssignRoleToUserAsync(userId, roleId);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            _permissionService.ClearUserPermissionsCache(userId);

            _logger.LogInformation("Assigned {RoleCount} roles to user {UserId}", roleIds.Count, userId);
            return ApiResponse<bool>.SuccessResponse(true, "Roles assigned to user successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning roles to user {UserId}", userId);
            return ApiResponse<bool>.FailureResponse("Failed to assign roles to user");
        }
    }

    public async Task<ApiResponse<bool>> SyncUserRolesAsync(Guid userId, List<Guid> roleIds)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("User not found");

            // Remove all existing roles
            await _unitOfWork.UserRoles.RemoveAllUserRolesAsync(userId);

            // Add new roles
            foreach (var roleId in roleIds)
            {
                await _unitOfWork.UserRoles.AssignRoleToUserAsync(userId, roleId);
            }

            await _unitOfWork.SaveChangesAsync();
            _permissionService.ClearUserPermissionsCache(userId);

            _logger.LogInformation("Synced {RoleCount} roles for user {UserId}", roleIds.Count, userId);
            return ApiResponse<bool>.SuccessResponse(true, "User roles synchronized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing roles for user {UserId}", userId);
            return ApiResponse<bool>.FailureResponse("Failed to sync user roles");
        }
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        try
        {
            return await _unitOfWork.Roles.RoleExistsAsync(roleName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if role exists: {RoleName}", roleName);
            return false;
        }
    }

    public async Task<bool> UserHasRoleAsync(Guid userId, Guid roleId)
    {
        try
        {
            return await _unitOfWork.UserRoles.UserHasRoleAsync(userId, roleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} has role {RoleId}", userId, roleId);
            return false;
        }
    }
}
