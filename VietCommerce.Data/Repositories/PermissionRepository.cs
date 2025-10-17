using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(AppDbContext context) : base(context) { }

    public async Task<List<Permission>> GetUserPermissionsAsync(Guid userId)
    {
        return await _context.Set<UserRole>()
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> UserHasPermissionAsync(Guid userId, string permissionName)
    {
        return await _context.Set<UserRole>()
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .AnyAsync(rp => rp.Permission.Name.ToLower() == permissionName.ToLower());
    }

    public async Task<List<Permission>> GetRolePermissionsAsync(Guid roleId)
    {
        return await _context.Set<RolePermission>()
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.Permission)
            .ToListAsync();
    }

    public async Task<bool> PermissionExistsAsync(string permissionName)
    {
        return await _dbSet.AnyAsync(p => p.Name.ToLower() == permissionName.ToLower());
    }

    public async Task<Permission?> GetByNameAsync(string permissionName)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Name.ToLower() == permissionName.ToLower());
    }
}
