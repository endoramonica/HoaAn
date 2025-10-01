// Updated IRoleRepository.cs
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories.Interfaces;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetRoleWithPermissionsAsync(Guid roleId);
    Task<List<Role>> GetUserRolesAsync(Guid userId);
    Task<bool> RoleExistsAsync(string roleName);
    
    // New method added
    Task<Role?> GetRoleByNameAsync(string roleName);
}
