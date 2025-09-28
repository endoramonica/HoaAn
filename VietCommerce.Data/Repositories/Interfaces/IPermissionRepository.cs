using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Data.Repositories.Interfaces;

public interface IPermissionRepository : IGenericRepository<Permission>
{
    Task<List<Permission>> GetUserPermissionsAsync(Guid userId);
    Task<bool> UserHasPermissionAsync(Guid userId, string permissionName);
    Task<List<Permission>> GetRolePermissionsAsync(Guid roleId);
    Task<bool> PermissionExistsAsync(string permissionName);
    Task<Permission?> GetByNameAsync(string permissionName);
}
