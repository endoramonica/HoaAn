using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Data.Repositories.Interfaces;

public interface IRolePermissionRepository : IGenericRepository<RolePermission>
{
    Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId);
    Task<List<RolePermission>> GetByPermissionIdAsync(Guid permissionId);
    Task<bool> RoleHasPermissionAsync(Guid roleId, Guid permissionId);
    Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId);
    Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId);
    Task RemoveAllRolePermissionsAsync(Guid roleId);
}
