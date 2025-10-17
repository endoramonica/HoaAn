using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(AppDbContext context) : base(context) { }

    public async Task<List<UserRole>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(ur => ur.Role)
            .Include(ur => ur.User)
            .Where(ur => ur.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<UserRole>> GetByRoleIdAsync(Guid roleId)
    {
        return await _dbSet
            .Include(ur => ur.Role)
            .Include(ur => ur.User)
            .Where(ur => ur.RoleId == roleId)
            .ToListAsync();
    }

    public async Task<bool> UserHasRoleAsync(Guid userId, Guid roleId)
    {
        return await _dbSet.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }

    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId)
    {
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId
        };

        await _dbSet.AddAsync(userRole);
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        var userRole = await _dbSet.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        if (userRole != null)
        {
            _dbSet.Remove(userRole);
        }
    }

    public async Task RemoveAllUserRolesAsync(Guid userId)
    {
        var userRoles = await _dbSet.Where(ur => ur.UserId == userId).ToListAsync();
        _dbSet.RemoveRange(userRoles);
    }
}
