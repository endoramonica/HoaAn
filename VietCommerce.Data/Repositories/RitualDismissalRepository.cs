using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Rituals;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    /// <summary>
    /// Repository implementation for RitualDismissal entities
    /// Handles data access for ritual dismissals and user preferences
    /// </summary>
    public class RitualDismissalRepository : GenericRepository<RitualDismissalEntity>, IRitualDismissalRepository
    {
        public RitualDismissalRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets all dismissals for a specific user and ritual
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID</param>
        /// <returns>List of dismissal records</returns>
        public async Task<List<RitualDismissalEntity>> GetDismissalsByUserAndRitualAsync(Guid userId, Guid ritualId)
        {
            return await _context.RitualDismissals
                .Where(d => d.UserId == userId && d.RitualId == ritualId)
                .OrderByDescending(d => d.DismissedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the count of dismissals for a user and ritual
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID</param>
        /// <returns>Number of dismissals</returns>
        public async Task<int> GetDismissalCountAsync(Guid userId, Guid ritualId)
        {
            return await _context.RitualDismissals
                .CountAsync(d => d.UserId == userId && d.RitualId == ritualId);
        }

        /// <summary>
        /// Checks if a ritual is disabled for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID</param>
        /// <returns>True if ritual is disabled, false otherwise</returns>
        public async Task<bool> IsRitualDisabledAsync(Guid userId, Guid ritualId)
        {
            return await _context.RitualDismissals
                .AnyAsync(d => d.UserId == userId && d.RitualId == ritualId && d.IsDisabled);
        }

        /// <summary>
        /// Gets all disabled rituals for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of disabled ritual IDs</returns>
        public async Task<List<Guid>> GetDisabledRitualsAsync(Guid userId)
        {
            return await _context.RitualDismissals
                .Where(d => d.UserId == userId && d.IsDisabled)
                .Select(d => d.RitualId)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// Records a dismissal for a user and ritual
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID</param>
        /// <param name="isDisabled">Whether this is a permanent disable</param>
        /// <param name="reason">Optional dismissal reason</param>
        /// <returns>The created dismissal entity</returns>
        public async Task<RitualDismissalEntity> RecordDismissalAsync(Guid userId, Guid ritualId, bool isDisabled, string? reason)
        {
            var dismissal = new RitualDismissalEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RitualId = ritualId,
                DismissedAt = DateTime.UtcNow,
                IsDisabled = isDisabled,
                DismissalReason = reason,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            await AddAsync(dismissal);
            return dismissal;
        }

        /// <summary>
        /// Removes a dismissal record (user changes their mind)
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID</param>
        /// <returns>True if removed, false if not found</returns>
        public async Task<bool> RemoveDismissalAsync(Guid userId, Guid ritualId)
        {
            var dismissal = await _context.RitualDismissals
                .FirstOrDefaultAsync(d => d.UserId == userId && d.RitualId == ritualId);

            if (dismissal == null)
            {
                return false;
            }

            Delete(dismissal);
            return true;
        }

        /// <summary>
        /// Gets all dismissals for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of all dismissals for the user</returns>
        public async Task<List<RitualDismissalEntity>> GetUserDismissalsAsync(Guid userId)
        {
            return await _context.RitualDismissals
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.DismissedAt)
                .ToListAsync();
        }
    }
}
