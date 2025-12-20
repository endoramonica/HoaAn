using VietCommerce.Core.Entities.Rituals;

namespace VietCommerce.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for RitualDismissal entities
    /// Handles data access for ritual dismissals and user preferences
    /// </summary>
    public interface IRitualDismissalRepository : IGenericRepository<RitualDismissalEntity>
    {
        /// <summary>
        /// Gets all dismissals for a specific user and ritual
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID</param>
        /// <returns>List of dismissal records</returns>
        Task<List<RitualDismissalEntity>> GetDismissalsByUserAndRitualAsync(Guid userId, Guid ritualId);

        /// <summary>
        /// Gets the count of dismissals for a user and ritual
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID</param>
        /// <returns>Number of dismissals</returns>
        Task<int> GetDismissalCountAsync(Guid userId, Guid ritualId);

        /// <summary>
        /// Checks if a ritual is disabled for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID</param>
        /// <returns>True if ritual is disabled, false otherwise</returns>
        Task<bool> IsRitualDisabledAsync(Guid userId, Guid ritualId);

        /// <summary>
        /// Gets all disabled rituals for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of disabled ritual IDs</returns>
        Task<List<Guid>> GetDisabledRitualsAsync(Guid userId);

        /// <summary>
        /// Records a dismissal for a user and ritual
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID</param>
        /// <param name="isDisabled">Whether this is a permanent disable</param>
        /// <param name="reason">Optional dismissal reason</param>
        /// <returns>The created dismissal entity</returns>
        Task<RitualDismissalEntity> RecordDismissalAsync(Guid userId, Guid ritualId, bool isDisabled, string? reason);

        /// <summary>
        /// Removes a dismissal record (user changes their mind)
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID</param>
        /// <returns>True if removed, false if not found</returns>
        Task<bool> RemoveDismissalAsync(Guid userId, Guid ritualId);

        /// <summary>
        /// Gets all dismissals for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of all dismissals for the user</returns>
        Task<List<RitualDismissalEntity>> GetUserDismissalsAsync(Guid userId);
    }
}
