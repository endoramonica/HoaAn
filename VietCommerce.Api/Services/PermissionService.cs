// VietCommerce.Api/Services/PermissionService.cs
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;
namespace VietCommerce.Api.Services;
public class PermissionService : IPermissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PermissionService> _logger;
    private const int CacheExpirationMinutes = 30;
    public PermissionService(
        IUnitOfWork unitOfWork,
        IMemoryCache cache,
        ILogger<PermissionService> logger)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
        _logger = logger;
    }
    public async Task<List<Permission>> GetUserPermissionsAsync(Guid userId)
    {
        var cacheKey = $"user_permissions_{userId}";
        if (_cache.TryGetValue(cacheKey, out List<Permission>? cachedPermissions) && cachedPermissions != null)
        {
            return cachedPermissions;
        }
        try
        {
            var permissions = await _unitOfWork.Permissions.GetUserPermissionsAsync(userId);
            _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(CacheExpirationMinutes));
            _logger.LogDebug("Retrieved {PermissionCount} permissions for user {UserId}", permissions.Count, userId);
            return permissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for user {UserId}", userId);
            return new List<Permission>();
        }
    }
    public async Task<bool> CheckUserPermissionAsync(Guid userId, string permissionName)
    {
        if (string.IsNullOrWhiteSpace(permissionName))
            return false;
        try
        {
            var hasPermission = await _unitOfWork.Permissions.UserHasPermissionAsync(userId, permissionName);
            _logger.LogDebug("Permission check for user {UserId}, permission '{Permission}': {Result}", userId, permissionName, hasPermission);
            return hasPermission;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission '{Permission}' for user {UserId}", permissionName, userId);
            return false;
        }
    }
    public async Task<List<string>> GetUserPermissionNamesAsync(Guid userId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        return permissions.Select(p => p.Name).ToList();
    }
    public async Task<List<Role>> GetUserRolesAsync(Guid userId)
    {
        var cacheKey = $"user_roles_{userId}";
        if (_cache.TryGetValue(cacheKey, out List<Role>? cachedRoles) && cachedRoles != null)
        {
            return cachedRoles;
        }
        try
        {
            var roles = await _unitOfWork.Roles.GetUserRolesAsync(userId);
            _cache.Set(cacheKey, roles, TimeSpan.FromMinutes(CacheExpirationMinutes));
            _logger.LogDebug("Retrieved {RoleCount} roles for user {UserId}", roles.Count, userId);
            return roles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user {UserId}", userId);
            return new List<Role>();
        }
    }
    public async Task<bool> CheckUserRoleAsync(Guid userId, string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return false;
        try
        {
            var userRoles = await GetUserRolesAsync(userId);
            var hasRole = userRoles.Any(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            _logger.LogDebug("Role check for user {UserId}, role '{Role}': {Result}", userId, roleName, hasRole);
            return hasRole;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role '{Role}' for user {UserId}", roleName, userId);
            return false;
        }
    }
    public async Task<ApiResponse<bool>> ValidateUserAccessAsync(Guid userId, string[] requiredPermissions)
    {
        try
        {
            if (requiredPermissions == null || requiredPermissions.Length == 0)
                return ApiResponse<bool>.SuccessResponse(true, "No permissions required");
            foreach (var permission in requiredPermissions)
            {
                var hasPermission = await CheckUserPermissionAsync(userId, permission);
                if (hasPermission)
                {
                    return ApiResponse<bool>.SuccessResponse(true, "Access granted");
                }
            }
            return ApiResponse<bool>.FailureResponse("Access denied - insufficient permissions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating access for user {UserId}", userId);
            return ApiResponse<bool>.FailureResponse("Access validation failed");
        }
    }
    public async Task RefreshUserPermissionsCache(Guid userId)
    {
        ClearUserPermissionsCache(userId);
        await GetUserPermissionsAsync(userId);
        await GetUserRolesAsync(userId);
        _logger.LogInformation("Refreshed permissions cache for user {UserId}", userId);
    }
    public void ClearUserPermissionsCache(Guid userId)
    {
        _cache.Remove($"user_permissions_{userId}");
        _cache.Remove($"user_roles_{userId}");
        _logger.LogDebug("Cleared permissions cache for user {UserId}", userId);
    }
}
