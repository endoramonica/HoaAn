using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Rituals;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    /// <summary>
    /// Repository implementation for RecommendationLog entities
    /// </summary>
    public class RecommendationLogRepository : GenericRepository<RecommendationLogEntity>, IRecommendationLogRepository
    {
        public RecommendationLogRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets all recommendations for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of recommendation logs</returns>
        public async Task<List<RecommendationLogEntity>> GetUserRecommendationsAsync(Guid userId)
        {
            return await _context.RecommendationLogs
                .Where(rl => rl.UserId == userId)
                .OrderByDescending(rl => rl.DisplayedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets recommendations for a specific session
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <returns>List of recommendation logs</returns>
        public async Task<List<RecommendationLogEntity>> GetSessionRecommendationsAsync(string sessionId)
        {
            return await _context.RecommendationLogs
                .Where(rl => rl.SessionId == sessionId)
                .OrderByDescending(rl => rl.DisplayedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets recommendations for a specific ritual
        /// </summary>
        /// <param name="ritualId">The ritual ID</param>
        /// <returns>List of recommendation logs</returns>
        public async Task<List<RecommendationLogEntity>> GetRitualRecommendationsAsync(Guid ritualId)
        {
            return await _context.RecommendationLogs
                .Where(rl => rl.RitualId == ritualId)
                .OrderByDescending(rl => rl.DisplayedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets recommendations with a specific user interaction
        /// </summary>
        /// <param name="interaction">The interaction type (viewed, dismissed, clicked)</param>
        /// <returns>List of recommendation logs</returns>
        public async Task<List<RecommendationLogEntity>> GetRecommendationsByInteractionAsync(string interaction)
        {
            return await _context.RecommendationLogs
                .Where(rl => rl.UserInteraction == interaction)
                .OrderByDescending(rl => rl.InteractionAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the most recent recommendation for a user in a session
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <returns>The most recent recommendation log or null</returns>
        public async Task<RecommendationLogEntity?> GetMostRecentRecommendationAsync(Guid userId, string sessionId)
        {
            return await _context.RecommendationLogs
                .Where(rl => rl.UserId == userId && rl.SessionId == sessionId)
                .OrderByDescending(rl => rl.DisplayedAt)
                .FirstOrDefaultAsync();
        }
    }
}
