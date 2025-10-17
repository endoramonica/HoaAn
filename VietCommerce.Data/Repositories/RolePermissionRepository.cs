using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

public class RolePermissionRepository : GenericRepository<RolePermission>, IRolePermissionRepository
{
    public RolePermissionRepository(AppDbContext context) : base(context) { }

    public async Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId)
    {
        return await _dbSet
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();
    }

    public async Task<List<RolePermission>> GetByPermissionIdAsync(Guid permissionId)
    {
        return await _dbSet
            .Include(rp => rp.Role)
            .Include(rp => rp.Permission)
            .Where(rp => rp.PermissionId == permissionId)
            .ToListAsync();
    }

    public async Task<bool> RoleHasPermissionAsync(Guid roleId, Guid permissionId)
    {
        return await _dbSet.AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
    }

    public async Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId)
    {
        var rolePermission = new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId
        };

        await _dbSet.AddAsync(rolePermission);
    }

    public async Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId)
    {
        var rolePermission = await _dbSet.FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
        if (rolePermission != null)
        {
            _dbSet.Remove(rolePermission);
        }
    }

    public async Task RemoveAllRolePermissionsAsync(Guid roleId)
    {
        var rolePermissions = await _dbSet.Where(rp => rp.RoleId == roleId).ToListAsync();
        _dbSet.RemoveRange(rolePermissions);
    }
}
