// RoleRepository.cs implementation
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetRoleWithPermissionsAsync(Guid roleId)
    {
        return await _dbSet
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId);
    }

    public async Task<List<Role>> GetUserRolesAsync(Guid userId)
    {
        return await _context.Set<UserRole>()
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Select(ur => ur.Role)
            .ToListAsync();
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _dbSet.AnyAsync(r => r.Name == roleName);
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.Name == roleName);
    }
}
