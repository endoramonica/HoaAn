using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services;

public class PermissionService : IPermissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService; // ← CHANGED from IMemoryCache
    private readonly ILogger<PermissionService> _logger;

    // Cache TTL từ config (có thể inject IConfiguration nếu muốn dynamic)
    private static readonly TimeSpan PermissionCacheTtl = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan RoleCacheTtl = TimeSpan.FromMinutes(30);

    public PermissionService(
        IUnitOfWork unitOfWork,
        ICacheService cacheService, // ← CHANGED
        ILogger<PermissionService> logger)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService; // ← CHANGED
        _logger = logger;
    }

    // ============================================
    // PERMISSIONS
    // ============================================

    public async Task<List<Permission>> GetUserPermissionsAsync(Guid userId)
    {
        var cacheKey = $"permissions:user:{userId}"; // ← UPDATED key pattern

        // Try get from Redis cache
        var cachedPermissions = await _cacheService.GetAsync<List<Permission>>(cacheKey);
        if (cachedPermissions != null)
        {
            _logger.LogDebug("Cache HIT: Retrieved {Count} permissions for user {UserId} from Redis",
                cachedPermissions.Count, userId);
            return cachedPermissions;
        }

        // Cache MISS - fetch from DB
        _logger.LogDebug("Cache MISS: Fetching permissions for user {UserId} from database", userId);

        try
        {
            var permissions = await _unitOfWork.Permissions.GetUserPermissionsAsync(userId);

            // Save to Redis cache
            await _cacheService.SetAsync(cacheKey, permissions, PermissionCacheTtl);

            _logger.LogInformation("Cached {Count} permissions for user {UserId}, TTL: {Ttl}min",
                permissions.Count, userId, PermissionCacheTtl.TotalMinutes);

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
        {
            _logger.LogWarning("CheckUserPermissionAsync called with empty permission name for user {UserId}", userId);
            return false;
        }

        try
        {
            // Option 1: Use cached permissions list
            var permissions = await GetUserPermissionsAsync(userId);
            var hasPermission = permissions.Any(p =>
                p.Name.Equals(permissionName, StringComparison.OrdinalIgnoreCase));

            _logger.LogDebug("Permission check for user {UserId}, permission '{Permission}': {Result}",
                userId, permissionName, hasPermission);

            return hasPermission;

            // Option 2: Direct DB check (uncomment if you prefer this approach)
            // return await _unitOfWork.Permissions.UserHasPermissionAsync(userId, permissionName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission '{Permission}' for user {UserId}",
                permissionName, userId);
            return false;
        }
    }

    public async Task<List<string>> GetUserPermissionNamesAsync(Guid userId)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        return permissions.Select(p => p.Name).ToList();
    }

    // ============================================
    // ROLES
    // ============================================

    public async Task<List<Role>> GetUserRolesAsync(Guid userId)
    {
        var cacheKey = $"roles:user:{userId}"; // ← UPDATED key pattern

        // Try get from Redis cache
        var cachedRoles = await _cacheService.GetAsync<List<Role>>(cacheKey);
        if (cachedRoles != null)
        {
            _logger.LogDebug("Cache HIT: Retrieved {Count} roles for user {UserId} from Redis",
                cachedRoles.Count, userId);
            return cachedRoles;
        }

        // Cache MISS - fetch from DB
        _logger.LogDebug("Cache MISS: Fetching roles for user {UserId} from database", userId);

        try
        {
            var roles = await _unitOfWork.Roles.GetUserRolesAsync(userId);

            // Save to Redis cache
            await _cacheService.SetAsync(cacheKey, roles, RoleCacheTtl);

            _logger.LogInformation("Cached {Count} roles for user {UserId}, TTL: {Ttl}min",
                roles.Count, userId, RoleCacheTtl.TotalMinutes);

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
        {
            _logger.LogWarning("CheckUserRoleAsync called with empty role name for user {UserId}", userId);
            return false;
        }

        try
        {
            var userRoles = await GetUserRolesAsync(userId);
            var hasRole = userRoles.Any(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));

            _logger.LogDebug("Role check for user {UserId}, role '{Role}': {Result}",
                userId, roleName, hasRole);

            return hasRole;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role '{Role}' for user {UserId}", roleName, userId);
            return false;
        }
    }

    // ============================================
    // ACCESS VALIDATION
    // ============================================

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

            _logger.LogWarning("Access denied for user {UserId}. Required permissions: {Permissions}",
                userId, string.Join(", ", requiredPermissions));

            return ApiResponse<bool>.FailureResponse("Access denied - insufficient permissions");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating access for user {UserId}", userId);
            return ApiResponse<bool>.FailureResponse("Access validation failed");
        }
    }

    // ============================================
    // CACHE MANAGEMENT
    // ============================================

    public async Task RefreshUserPermissionsCache(Guid userId)
    {
        _logger.LogInformation("Refreshing permissions cache for user {UserId}", userId);

        // Clear existing cache
        ClearUserPermissionsCache(userId);

        // Pre-warm cache
        await GetUserPermissionsAsync(userId);
        await GetUserRolesAsync(userId);

        _logger.LogInformation("Permissions cache refreshed for user {UserId}", userId);
    }

    public void ClearUserPermissionsCache(Guid userId)
    {
        try
        {
            // Use Fire-and-Forget pattern for cache clearing (don't await)
            _ = Task.Run(async () =>
            {
                await _cacheService.RemoveAsync($"permissions:user:{userId}");
                await _cacheService.RemoveAsync($"roles:user:{userId}");

                _logger.LogDebug("Cleared cache for user {UserId}", userId);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache for user {UserId}", userId);
        }
    }

    // ============================================
    // PUB/SUB INVALIDATION (NEW)
    // ============================================

    
    /// Publish cache invalidation event to Redis Pub/Sub
    /// This ensures cache is cleared across all API instances
    
    public async Task PublishInvalidationAsync(Guid userId, string invalidationType = "user")
    {
        try
        {
            var message = new CacheInvalidationMessage
            {
                UserId = userId,
                Type = invalidationType,
                Timestamp = DateTime.UtcNow
            };

            await _cacheService.PublishAsync("permissions:invalidate", message);

            _logger.LogInformation("Published cache invalidation for user {UserId}, type: {Type}",
                userId, invalidationType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing cache invalidation for user {UserId}", userId);
        }
    }
}