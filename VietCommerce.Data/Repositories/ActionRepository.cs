using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Rituals;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Action entities
    /// </summary>
    public class ActionRepository : GenericRepository<ActionEntity>, IActionRepository
    {
        public ActionRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets all actions for a specific user session
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <returns>List of actions in the session</returns>
        public async Task<List<ActionEntity>> GetActionsBySessionAsync(Guid userId, string sessionId)
        {
            return await _context.Actions
                .Where(a => a.UserId == userId && a.SessionId == sessionId)
                .OrderBy(a => a.ActionTimestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Gets recent actions for a user within a time window
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <param name="minutesBack">Number of minutes to look back</param>
        /// <returns>List of recent actions</returns>
        public async Task<List<ActionEntity>> GetRecentActionsAsync(Guid userId, string sessionId, int minutesBack = 30)
        {
            var cutoffTime = DateTime.UtcNow.AddMinutes(-minutesBack);

            return await _context.Actions
                .Where(a => a.UserId == userId && 
                           a.SessionId == sessionId && 
                           a.ActionTimestamp >= cutoffTime)
                .OrderBy(a => a.ActionTimestamp)
                .ToListAsync();
        }

        /// <summary>
        /// Deletes all actions for a specific session
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <returns>Number of actions deleted</returns>
        public async Task<int> DeleteSessionActionsAsync(string sessionId)
        {
            var actions = await _context.Actions
                .Where(a => a.SessionId == sessionId)
                .ToListAsync();

            _context.Actions.RemoveRange(actions);
            await _context.SaveChangesAsync();

            return actions.Count;
        }
    }
}
