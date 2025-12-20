using VietCommerce.Core.DTOs.Rituals;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Interface for tracking user actions in the ritual recommendation system
    /// Handles action recording, retrieval, and session management
    /// </summary>
    public interface IActionTrackingService
    {
        /// <summary>
        /// Tracks a user action and stores it in the database
        /// </summary>
        /// <param name="userId">The user ID performing the action</param>
        /// <param name="sessionId">The session ID for grouping actions</param>
        /// <param name="actionType">The type of action (e.g., "ViewProduct", "AddToCart")</param>
        /// <param name="metadata">Optional metadata about the action</param>
        /// <returns>The ID of the created action record</returns>
        Task<Guid> TrackActionAsync(
            Guid userId,
            string sessionId,
            string actionType,
            Dictionary<string, object>? metadata = null);

        /// <summary>
        /// Tracks a user action with product and category information
        /// </summary>
        /// <param name="userId">The user ID performing the action</param>
        /// <param name="sessionId">The session ID for grouping actions</param>
        /// <param name="actionType">The type of action</param>
        /// <param name="productId">Optional product ID associated with the action</param>
        /// <param name="categoryId">Optional category ID associated with the action</param>
        /// <param name="metadata">Optional additional metadata</param>
        /// <returns>The ID of the created action record</returns>
        Task<Guid> TrackActionAsync(
            Guid userId,
            string sessionId,
            string actionType,
            Guid? productId,
            Guid? categoryId,
            Dictionary<string, object>? metadata = null);

        /// <summary>
        /// Retrieves the action sequence for a user session
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <returns>List of actions in chronological order</returns>
        Task<List<ActionDto>> GetActionSequenceAsync(Guid userId, string sessionId);

        /// <summary>
        /// Retrieves recent actions for a user session within a time window
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <param name="minutesBack">Number of minutes to look back (default: 30)</param>
        /// <returns>List of recent actions in chronological order</returns>
        Task<List<ActionDto>> GetRecentActionsAsync(Guid userId, string sessionId, int minutesBack = 30);

        /// <summary>
        /// Clears all actions for a specific session
        /// Used when user navigates away from ritual-related products
        /// </summary>
        /// <param name="sessionId">The session ID to clear</param>
        /// <returns>Number of actions deleted</returns>
        Task<int> ClearActionSequenceAsync(string sessionId);

        /// <summary>
        /// Clears all actions for a user and session combination
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <returns>Number of actions deleted</returns>
        Task<int> ClearUserSessionActionsAsync(Guid userId, string sessionId);
    }
}
