// VietCommerce.Api/Services/Interfaces/IPermissionService.cs
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Models;
namespace VietCommerce.Api.Services.Interfaces;
public interface IPermissionService
{
    Task<List<Permission>> GetUserPermissionsAsync(Guid userId);
    Task<bool> CheckUserPermissionAsync(Guid userId, string permissionName);
    Task<List<string>> GetUserPermissionNamesAsync(Guid userId);
    Task<List<Role>> GetUserRolesAsync(Guid userId);
    Task<bool> CheckUserRoleAsync(Guid userId, string roleName);
    Task<ApiResponse<bool>> ValidateUserAccessAsync(Guid userId, string[] requiredPermissions);
    Task RefreshUserPermissionsCache(Guid userId);
    void ClearUserPermissionsCache(Guid userId);
}
