using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Rituals;
using VietCommerce.Core.Entities.Rituals;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Service for tracking user actions in the ritual recommendation system
    /// Handles action recording, retrieval, and session management
    /// </summary>
    public class ActionTrackingService : IActionTrackingService
    {
        private readonly IActionRepository _actionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ActionTrackingService> _logger;

        public ActionTrackingService(
            IActionRepository actionRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ActionTrackingService> logger)
        {
            _actionRepository = actionRepository ?? throw new ArgumentNullException(nameof(actionRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Tracks a user action and stores it in the database
        /// </summary>
        /// <param name="userId">The user ID performing the action</param>
        /// <param name="sessionId">The session ID for grouping actions</param>
        /// <param name="actionType">The type of action (e.g., "ViewProduct", "AddToCart")</param>
        /// <param name="metadata">Optional metadata about the action</param>
        /// <returns>The ID of the created action record</returns>
        public async Task<Guid> TrackActionAsync(
            Guid userId,
            string sessionId,
            string actionType,
            Dictionary<string, object>? metadata = null)
        {
            return await TrackActionAsync(userId, sessionId, actionType, null, null, metadata);
        }

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
        public async Task<Guid> TrackActionAsync(
            Guid userId,
            string sessionId,
            string actionType,
            Guid? productId,
            Guid? categoryId,
            Dictionary<string, object>? metadata = null)
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

                if (string.IsNullOrWhiteSpace(actionType))
                {
                    throw new ArgumentException("Action type cannot be null or empty", nameof(actionType));
                }

                // Serialize metadata to JSON
                var metadataJson = metadata != null ? JsonSerializer.Serialize(metadata) : null;

                // Create action entity
                var actionEntity = new ActionEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    SessionId = sessionId,
                    ActionType = actionType,
                    ActionTimestamp = DateTime.UtcNow,
                    ProductId = productId,
                    CategoryId = categoryId,
                    MetadataJson = metadataJson,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                // Add to database
                await _actionRepository.AddAsync(actionEntity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    $"Tracked action for user {userId}: type={actionType}, session={sessionId}, productId={productId}, categoryId={categoryId}");

                return actionEntity.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error tracking action for user {userId} in session {sessionId}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the action sequence for a user session
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <returns>List of actions in chronological order</returns>
        public async Task<List<ActionDto>> GetActionSequenceAsync(Guid userId, string sessionId)
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

                // Retrieve actions from repository
                var actionEntities = await _actionRepository.GetActionsBySessionAsync(userId, sessionId);

                // Map to DTOs
                var actionDtos = _mapper.Map<List<ActionDto>>(actionEntities);

                _logger.LogInformation(
                    $"Retrieved {actionDtos.Count} actions for user {userId} in session {sessionId}");

                return actionDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving action sequence for user {userId} in session {sessionId}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves recent actions for a user session within a time window
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <param name="minutesBack">Number of minutes to look back (default: 30)</param>
        /// <returns>List of recent actions in chronological order</returns>
        public async Task<List<ActionDto>> GetRecentActionsAsync(Guid userId, string sessionId, int minutesBack = 30)
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

                if (minutesBack <= 0)
                {
                    throw new ArgumentException("Minutes back must be greater than 0", nameof(minutesBack));
                }

                // Retrieve recent actions from repository
                var actionEntities = await _actionRepository.GetRecentActionsAsync(userId, sessionId, minutesBack);

                // Map to DTOs
                var actionDtos = _mapper.Map<List<ActionDto>>(actionEntities);

                _logger.LogInformation(
                    $"Retrieved {actionDtos.Count} recent actions for user {userId} in session {sessionId} (last {minutesBack} minutes)");

                return actionDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving recent actions for user {userId} in session {sessionId}");
                throw;
            }
        }

        /// <summary>
        /// Clears all actions for a specific session
        /// Used when user navigates away from ritual-related products
        /// </summary>
        /// <param name="sessionId">The session ID to clear</param>
        /// <returns>Number of actions deleted</returns>
        public async Task<int> ClearActionSequenceAsync(string sessionId)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    throw new ArgumentException("Session ID cannot be null or empty", nameof(sessionId));
                }

                // Delete all actions for the session
                var deletedCount = await _actionRepository.DeleteSessionActionsAsync(sessionId);

                _logger.LogInformation($"Cleared {deletedCount} actions for session {sessionId}");

                return deletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error clearing action sequence for session {sessionId}");
                throw;
            }
        }

        /// <summary>
        /// Clears all actions for a user and session combination
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <returns>Number of actions deleted</returns>
        public async Task<int> ClearUserSessionActionsAsync(Guid userId, string sessionId)
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

                // Retrieve all actions for the user session
                var actionEntities = await _actionRepository.GetActionsBySessionAsync(userId, sessionId);

                // Delete them
                foreach (var action in actionEntities)
                {
                    _actionRepository.Delete(action);
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    $"Cleared {actionEntities.Count} actions for user {userId} in session {sessionId}");

                return actionEntities.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error clearing user session actions for user {userId} in session {sessionId}");
                throw;
            }
        }
    }
}
