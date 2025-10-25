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

    // ========================= DELETE ROLE =========================
    public async Task<ApiResponse<bool>> DeleteRoleAsync(Guid roleId)
    {
        try
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
            if (role == null)
                return ApiResponse<bool>.FailureResponse("Role not found");

            var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(roleId);
            if (userRoles.Any())
                return ApiResponse<bool>.FailureResponse("Cannot delete role that is assigned to users");

            await _unitOfWork.RolePermissions.RemoveAllRolePermissionsAsync(roleId);
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

    // ========================= GET ROLE PERMISSIONS =========================
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

    // ========================= ASSIGN PERMISSION =========================
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

            // 🔹 Invalidate cache for all users with this role
            var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(roleId);
            foreach (var userRole in userRoles)
            {
                _permissionService.ClearUserPermissionsCache(userRole.UserId);
                await ((PermissionService)_permissionService).PublishInvalidationAsync(userRole.UserId, "role");
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

    // ========================= REMOVE PERMISSION =========================
    public async Task<ApiResponse<bool>> RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId)
    {
        try
        {
            await _unitOfWork.RolePermissions.RemovePermissionFromRoleAsync(roleId, permissionId);
            await _unitOfWork.SaveChangesAsync();

            // 🔹 Invalidate cache for all users with this role
            var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(roleId);
            foreach (var userRole in userRoles)
            {
                _permissionService.ClearUserPermissionsCache(userRole.UserId);
                await ((PermissionService)_permissionService).PublishInvalidationAsync(userRole.UserId, "role");
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

    // ========================= ASSIGN MULTIPLE PERMISSIONS =========================
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
                    await _unitOfWork.RolePermissions.AssignPermissionToRoleAsync(roleId, permissionId);
            }

            await _unitOfWork.SaveChangesAsync();

            // 🔹 Invalidate cache
            var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(roleId);
            foreach (var userRole in userRoles)
            {
                _permissionService.ClearUserPermissionsCache(userRole.UserId);
                await ((PermissionService)_permissionService).PublishInvalidationAsync(userRole.UserId, "role");
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

    // ========================= SYNC ROLE PERMISSIONS =========================
    public async Task<ApiResponse<bool>> SyncRolePermissionsAsync(Guid roleId, List<Guid> permissionIds)
    {
        try
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
            if (role == null)
                return ApiResponse<bool>.FailureResponse("Role not found");

            await _unitOfWork.RolePermissions.RemoveAllRolePermissionsAsync(roleId);

            foreach (var permissionId in permissionIds)
                await _unitOfWork.RolePermissions.AssignPermissionToRoleAsync(roleId, permissionId);

            await _unitOfWork.SaveChangesAsync();

            // 🔹 Invalidate cache
            var userRoles = await _unitOfWork.UserRoles.GetByRoleIdAsync(roleId);
            foreach (var userRole in userRoles)
            {
                _permissionService.ClearUserPermissionsCache(userRole.UserId);
                await ((PermissionService)_permissionService).PublishInvalidationAsync(userRole.UserId, "role");
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

    // ========================= ASSIGN ROLE TO USER =========================
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
            await ((PermissionService)_permissionService).PublishInvalidationAsync(userId, "role");

            _logger.LogInformation("Role {RoleId} assigned to user {UserId}", roleId, userId);
            return ApiResponse<bool>.SuccessResponse(true, "Role assigned to user successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
            return ApiResponse<bool>.FailureResponse("Failed to assign role to user");
        }
    }

    // ========================= REMOVE ROLE FROM USER =========================
    public async Task<ApiResponse<bool>> RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        try
        {
            await _unitOfWork.UserRoles.RemoveRoleFromUserAsync(userId, roleId);
            await _unitOfWork.SaveChangesAsync();

            _permissionService.ClearUserPermissionsCache(userId);
            await ((PermissionService)_permissionService).PublishInvalidationAsync(userId, "role");

            _logger.LogInformation("Role {RoleId} removed from user {UserId}", roleId, userId);
            return ApiResponse<bool>.SuccessResponse(true, "Role removed from user successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
            return ApiResponse<bool>.FailureResponse("Failed to remove role from user");
        }
    }

    // ========================= ASSIGN MULTIPLE ROLES =========================
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
                    await _unitOfWork.UserRoles.AssignRoleToUserAsync(userId, roleId);
            }

            await _unitOfWork.SaveChangesAsync();

            _permissionService.ClearUserPermissionsCache(userId);
            await ((PermissionService)_permissionService).PublishInvalidationAsync(userId, "role");

            _logger.LogInformation("Assigned {RoleCount} roles to user {UserId}", roleIds.Count, userId);
            return ApiResponse<bool>.SuccessResponse(true, "Roles assigned to user successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning roles to user {UserId}", userId);
            return ApiResponse<bool>.FailureResponse("Failed to assign roles to user");
        }
    }

    // ========================= SYNC USER ROLES =========================
    public async Task<ApiResponse<bool>> SyncUserRolesAsync(Guid userId, List<Guid> roleIds)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.FailureResponse("User not found");

            await _unitOfWork.UserRoles.RemoveAllUserRolesAsync(userId);

            foreach (var roleId in roleIds)
                await _unitOfWork.UserRoles.AssignRoleToUserAsync(userId, roleId);

            await _unitOfWork.SaveChangesAsync();

            _permissionService.ClearUserPermissionsCache(userId);
            await ((PermissionService)_permissionService).PublishInvalidationAsync(userId, "role");

            _logger.LogInformation("Synced {RoleCount} roles for user {UserId}", roleIds.Count, userId);
            return ApiResponse<bool>.SuccessResponse(true, "User roles synchronized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing roles for user {UserId}", userId);
            return ApiResponse<bool>.FailureResponse("Failed to sync user roles");
        }
    }

    // ========================= CHECK ROLE =========================
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
