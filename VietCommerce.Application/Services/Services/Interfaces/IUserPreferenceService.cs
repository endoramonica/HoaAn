namespace VietCommerce.Application.Services.Services.Interfaces
{
    /// <summary>
    /// Interface for managing user preferences related to ritual recommendations
    /// Handles dismissals, disabling rituals, and preference tracking
    /// </summary>
    public interface IUserPreferenceService
    {
        /// <summary>
        /// Records a dismissal for a user and ritual
        /// Used when user temporarily hides a recommendation
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID being dismissed</param>
        /// <param name="reason">Optional reason for dismissal</param>
        /// <returns>Task representing the async operation</returns>
        Task RecordDismissalAsync(Guid userId, Guid ritualId, string? reason);

        /// <summary>
        /// Disables a ritual for a user in the current session
        /// Used when user explicitly indicates they're not interested in a ritual
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID (for context, though disabling is per-user)</param>
        /// <param name="ritualId">The ritual ID to disable</param>
        /// <returns>Task representing the async operation</returns>
        Task DisableRitualAsync(Guid userId, string sessionId, Guid ritualId);

        /// <summary>
        /// Checks if a ritual is disabled for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID (for context)</param>
        /// <param name="ritualId">The ritual ID to check</param>
        /// <returns>True if ritual is disabled, false otherwise</returns>
        Task<bool> IsRitualDisabledAsync(Guid userId, string sessionId, Guid ritualId);

        /// <summary>
        /// Gets the dismissal count for a user and ritual
        /// Used to determine if a ritual should be shown less frequently
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID</param>
        /// <returns>Number of times the ritual has been dismissed</returns>
        Task<int> GetDismissalCountAsync(Guid userId, Guid ritualId);

        /// <summary>
        /// Gets all disabled rituals for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of disabled ritual IDs</returns>
        Task<List<Guid>> GetDisabledRitualsAsync(Guid userId);

        /// <summary>
        /// Re-enables a ritual for a user (removes the disable)
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID to re-enable</param>
        /// <returns>True if re-enabled, false if not found</returns>
        Task<bool> ReEnableRitualAsync(Guid userId, Guid ritualId);
    }
}
