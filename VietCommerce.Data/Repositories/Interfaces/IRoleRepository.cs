using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Data.Repositories.Interfaces;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<List<Role>> GetUserRolesAsync(Guid userId);
    Task<Role?> GetRoleWithPermissionsAsync(Guid roleId);
    Task<List<Role>> GetRolesWithPermissionsAsync();
    Task<bool> RoleExistsAsync(string roleName);
    Task<Role?> GetByNameAsync(string roleName);
}
