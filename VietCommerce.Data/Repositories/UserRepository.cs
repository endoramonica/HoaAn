// Data/Repositories/UserRepository.cs
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.DTOs.Users;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email.ToLower());
    }
    public async Task<User?> GetByEmailWithRolesAndPermissionsAsync(string email)
    {
        return await _context.Users
            .Include(u => u.UserRoles) // User → UserRole
                .ThenInclude(ur => ur.Role) // → Role
                    .ThenInclude(r => r.RolePermissions) // → RolePermission
                        .ThenInclude(rp => rp.Permission) // → Permission
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdWithRolesAsync(Guid id)
    {
        return await _dbSet
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.Store)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailWithRolesAsync(string email)
    {
        return await _dbSet
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.Store)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email.ToLower());
    }

    public async Task<(IEnumerable<UserListDTO> users, int totalCount)> GetUsersPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null)
    {
        var query = _dbSet
            .Include(u => u.Store)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(u => 
                u.Email.ToLower().Contains(lowerSearchTerm) ||
                (u.Name != null && u.Name.ToLower().Contains(lowerSearchTerm)) ||
                (u.Phone != null && u.Phone.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync();

        var users = await query
            .OrderBy(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserListDTO
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.Name,
                PhoneNumber = u.Phone,
                IsActive = u.IsActive,
                StoreName = u.Store.Name,
                CreatedDate = u.CreatedAt,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .ToListAsync();

        return (users, totalCount);
    }
}
