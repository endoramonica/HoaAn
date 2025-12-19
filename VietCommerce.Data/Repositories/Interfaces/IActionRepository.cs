using VietCommerce.Core.Entities.Rituals;

namespace VietCommerce.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Action entities
    /// </summary>
    public interface IActionRepository : IGenericRepository<ActionEntity>
    {
        /// <summary>
        /// Gets all actions for a specific user session
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <returns>List of actions in the session</returns>
        Task<List<ActionEntity>> GetActionsBySessionAsync(Guid userId, string sessionId);

        /// <summary>
        /// Gets recent actions for a user within a time window
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <param name="minutesBack">Number of minutes to look back</param>
        /// <returns>List of recent actions</returns>
        Task<List<ActionEntity>> GetRecentActionsAsync(Guid userId, string sessionId, int minutesBack = 30);

        /// <summary>
        /// Deletes all actions for a specific session
        /// </summary>
        /// <param name="sessionId">The session ID</param>
        /// <returns>Number of actions deleted</returns>
        Task<int> DeleteSessionActionsAsync(string sessionId);
    }
}
