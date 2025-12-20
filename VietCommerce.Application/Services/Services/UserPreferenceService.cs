using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Service for managing user preferences related to ritual recommendations
    /// Handles dismissals, disabling rituals, and preference tracking
    /// </summary>
    public class UserPreferenceService : IUserPreferenceService
    {
        private readonly IRitualDismissalRepository _dismissalRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserPreferenceService> _logger;

        public UserPreferenceService(
            IRitualDismissalRepository dismissalRepository,
            IUnitOfWork unitOfWork,
            ILogger<UserPreferenceService> logger)
        {
            _dismissalRepository = dismissalRepository ?? throw new ArgumentNullException(nameof(dismissalRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Records a dismissal for a user and ritual
        /// Used when user temporarily hides a recommendation
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID being dismissed</param>
        /// <param name="reason">Optional reason for dismissal</param>
        /// <returns>Task representing the async operation</returns>
        public async Task RecordDismissalAsync(Guid userId, Guid ritualId, string? reason)
        {
            try
            {
                // Validate inputs
                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("User ID cannot be empty", nameof(userId));
                }

                if (ritualId == Guid.Empty)
                {
                    throw new ArgumentException("Ritual ID cannot be empty", nameof(ritualId));
                }

                // Record the dismissal (temporary, not disabled)
                await _dismissalRepository.RecordDismissalAsync(userId, ritualId, false, reason);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    $"Recorded dismissal for user {userId} and ritual {ritualId}. Reason: {reason ?? "Not provided"}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recording dismissal for user {userId} and ritual {ritualId}");
                throw;
            }
        }

        /// <summary>
        /// Disables a ritual for a user in the current session
        /// Used when user explicitly indicates they're not interested in a ritual
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID (for context, though disabling is per-user)</param>
        /// <param name="ritualId">The ritual ID to disable</param>
        /// <returns>Task representing the async operation</returns>
        public async Task DisableRitualAsync(Guid userId, string sessionId, Guid ritualId)
        {
            try
            {
                // Validate inputs
                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("User ID cannot be empty", nameof(userId));
                }

                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    throw new ArgumentException("Session ID cannot be null or empty", nameof(sessionId));
                }

                if (ritualId == Guid.Empty)
                {
                    throw new ArgumentException("Ritual ID cannot be empty", nameof(ritualId));
                }

                // Check if already disabled
                var isDisabled = await _dismissalRepository.IsRitualDisabledAsync(userId, ritualId);
                if (isDisabled)
                {
                    _logger.LogInformation($"Ritual {ritualId} is already disabled for user {userId}");
                    return;
                }

                // Record the disable (permanent for this user)
                await _dismissalRepository.RecordDismissalAsync(
                    userId,
                    ritualId,
                    true,
                    "User explicitly disabled ritual");

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    $"Disabled ritual {ritualId} for user {userId} in session {sessionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error disabling ritual {ritualId} for user {userId} in session {sessionId}");
                throw;
            }
        }

        /// <summary>
        /// Checks if a ritual is disabled for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID (for context)</param>
        /// <param name="ritualId">The ritual ID to check</param>
        /// <returns>True if ritual is disabled, false otherwise</returns>
        public async Task<bool> IsRitualDisabledAsync(Guid userId, string sessionId, Guid ritualId)
        {
            try
            {
                // Validate inputs
                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("User ID cannot be empty", nameof(userId));
                }

                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    throw new ArgumentException("Session ID cannot be null or empty", nameof(sessionId));
                }

                if (ritualId == Guid.Empty)
                {
                    throw new ArgumentException("Ritual ID cannot be empty", nameof(ritualId));
                }

                // Check if ritual is disabled
                var isDisabled = await _dismissalRepository.IsRitualDisabledAsync(userId, ritualId);

                _logger.LogInformation(
                    $"Checked if ritual {ritualId} is disabled for user {userId}: {isDisabled}");

                return isDisabled;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if ritual {ritualId} is disabled for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Gets the dismissal count for a user and ritual
        /// Used to determine if a ritual should be shown less frequently
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID</param>
        /// <returns>Number of times the ritual has been dismissed</returns>
        public async Task<int> GetDismissalCountAsync(Guid userId, Guid ritualId)
        {
            try
            {
                // Validate inputs
                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("User ID cannot be empty", nameof(userId));
                }

                if (ritualId == Guid.Empty)
                {
                    throw new ArgumentException("Ritual ID cannot be empty", nameof(ritualId));
                }

                // Get dismissal count
                var count = await _dismissalRepository.GetDismissalCountAsync(userId, ritualId);

                _logger.LogInformation(
                    $"Retrieved dismissal count for user {userId} and ritual {ritualId}: {count}");

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting dismissal count for user {userId} and ritual {ritualId}");
                throw;
            }
        }

        /// <summary>
        /// Gets all disabled rituals for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of disabled ritual IDs</returns>
        public async Task<List<Guid>> GetDisabledRitualsAsync(Guid userId)
        {
            try
            {
                // Validate input
                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("User ID cannot be empty", nameof(userId));
                }

                // Get disabled rituals
                var disabledRituals = await _dismissalRepository.GetDisabledRitualsAsync(userId);

                _logger.LogInformation(
                    $"Retrieved {disabledRituals.Count} disabled rituals for user {userId}");

                return disabledRituals;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting disabled rituals for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Re-enables a ritual for a user (removes the disable)
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="ritualId">The ritual ID to re-enable</param>
        /// <returns>True if re-enabled, false if not found</returns>
        public async Task<bool> ReEnableRitualAsync(Guid userId, Guid ritualId)
        {
            try
            {
                // Validate inputs
                if (userId == Guid.Empty)
                {
                    throw new ArgumentException("User ID cannot be empty", nameof(userId));
                }

                if (ritualId == Guid.Empty)
                {
                    throw new ArgumentException("Ritual ID cannot be empty", nameof(ritualId));
                }

                // Remove the dismissal
                var removed = await _dismissalRepository.RemoveDismissalAsync(userId, ritualId);

                if (removed)
                {
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation($"Re-enabled ritual {ritualId} for user {userId}");
                }
                else
                {
                    _logger.LogInformation($"No dismissal found for ritual {ritualId} and user {userId}");
                }

                return removed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error re-enabling ritual {ritualId} for user {userId}");
                throw;
            }
        }
    }
}
