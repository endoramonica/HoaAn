using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context) { }

      

        public async Task<List<RefreshToken>> GetByUserIdAsync(Guid userId) =>
            await _dbSet.Where(rt => rt.UserId == userId).ToListAsync();

        public async Task<RefreshToken?> GetValidTokenAsync(string token) =>
            await _dbSet.Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(rt => rt.Token == token && rt.ExpiresAt > DateTime.UtcNow);

        //public async Task InvalidateTokensAsync(Guid userId)
        //{
        //    var tokens = await _dbSet.Where(rt => rt.UserId == userId && rt.IsActive).ToListAsync();
        //    foreach (var token in tokens)
        //    {
        //        token.IsActive = false;
        //    }
        //}
    }

}
