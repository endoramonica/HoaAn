using VietCommerce.Core.Entities.Rituals;

namespace VietCommerce.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for RecommendationLog entities
    /// </summary>
    public interface IRecommendationLogRepository : IGenericRepository<RecommendationLogEntity>
    {
        /// <summary>
        /// Gets all recommendations for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of recommendation logs</returns>
        Task<List<RecommendationLogEntity>> GetUserRecommendationsAsync(Guid userId);

        /// <summary>
        /// Gets recommendations for a specific session
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <returns>List of recommendation logs</returns>
        Task<List<RecommendationLogEntity>> GetSessionRecommendationsAsync(string sessionId);

        /// <summary>
        /// Gets recommendations for a specific ritual
        /// </summary>
        /// <param name="ritualId">The ritual ID</param>
        /// <returns>List of recommendation logs</returns>
        Task<List<RecommendationLogEntity>> GetRitualRecommendationsAsync(Guid ritualId);

        /// <summary>
        /// Gets recommendations with a specific user interaction
        /// </summary>
        /// <param name="interaction">The interaction type (viewed, dismissed, clicked)</param>
        /// <returns>List of recommendation logs</returns>
        Task<List<RecommendationLogEntity>> GetRecommendationsByInteractionAsync(string interaction);

        /// <summary>
        /// Gets the most recent recommendation for a user in a session
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <returns>The most recent recommendation log or null</returns>
        Task<RecommendationLogEntity?> GetMostRecentRecommendationAsync(Guid userId, string sessionId);
    }
}
