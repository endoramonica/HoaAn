using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Data.Repositories.Interfaces;

public interface IUserRoleRepository : IGenericRepository<UserRole>
{
    Task<List<UserRole>> GetByUserIdAsync(Guid userId);
    Task<List<UserRole>> GetByRoleIdAsync(Guid roleId);
    Task<bool> UserHasRoleAsync(Guid userId, Guid roleId);
    Task AssignRoleToUserAsync(Guid userId, Guid roleId);
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);
    Task RemoveAllUserRolesAsync(Guid userId);
}
