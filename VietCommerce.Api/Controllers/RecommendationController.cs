using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VietCommerce.Application.Helpers;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Rituals;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Controllers
{
    /// <summary>
    /// Controller for the Sequential Ritual Recommendation System
    /// Handles pattern matching analysis and recommendation generation
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly ILogger<RecommendationController> _logger;

        public RecommendationController(
            IRecommendationService recommendationService,
            ILogger<RecommendationController> logger)
        {
            _recommendationService = recommendationService ?? throw new ArgumentNullException(nameof(recommendationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the current authenticated user ID from claims
        /// </summary>
        /// <returns>User ID if authenticated, null otherwise</returns>
        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        /// <summary>
        /// Gets or creates a session ID for guest users
        /// </summary>
        /// <returns>Session ID string</returns>
        private string GetOrCreateSessionId()
        {
            return SessionIdHelper.GetOrCreateSessionId(HttpContext, _logger);
        }

        /// <summary>
        /// Analyzes user action sequence to detect ritual patterns and generate recommendations
        /// 
        /// Requirements: 1.1, 1.2, 1.3, 1.4, 1.5
        /// - Accepts action sequence from frontend
        /// - Calls RecommendationService to match patterns
        /// - Returns RecommendationPayload with matched ritual and missing items
        /// - Handles invalid sequences gracefully
        /// </summary>
        /// <param name="request">Request containing action sequence and cart items</param>
        /// <returns>RecommendationPayload with matched ritual, missing items, and confidence score</returns>
        /// <response code="200">Recommendation generated successfully (may be null if no pattern matched)</response>
        /// <response code="400">Invalid request or action sequence</response>
        /// <response code="401">Unauthorized (for authenticated endpoint)</response>
        /// <response code="500">Server error during recommendation generation</response>
        [HttpPost("analyze")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AnalyzeRecommendationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AnalyzeActionSequence([FromBody] AnalyzeRecommendationRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                {
                    _logger.LogWarning("Null request received for recommendation analysis");
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = "Request cannot be null"
                    });
                }

                // Validate action sequence
                if (request.ActionSequence == null || request.ActionSequence.Count == 0)
                {
                    _logger.LogWarning("Empty action sequence received for recommendation analysis");
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = "Action sequence cannot be empty"
                    });
                }

                // Validate cart items
                if (request.CartItems == null)
                {
                    _logger.LogWarning("Null cart items received for recommendation analysis");
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = "Cart items cannot be null"
                    });
                }

                // Get user ID (optional for guest users)
                var userId = GetCurrentUserId() ?? Guid.Empty;

                // Get or create session ID
                var sessionId = GetOrCreateSessionId();

                _logger.LogInformation(
                    "Analyzing action sequence for user {UserId} in session {SessionId} with {ActionCount} actions",
                    userId == Guid.Empty ? "guest" : userId.ToString(),
                    SessionIdHelper.FormatSessionIdForLogging(sessionId),
                    request.ActionSequence.Count);

                // Generate recommendation
                var recommendation = await _recommendationService.GenerateRecommendationAsync(
                    userId,
                    sessionId,
                    request.ActionSequence,
                    request.CartItems);

                // If recommendation is null, no pattern matched
                if (recommendation == null)
                {
                    _logger.LogInformation(
                        "No ritual pattern matched for user {UserId} in session {SessionId}",
                        userId == Guid.Empty ? "guest" : userId.ToString(),
                        SessionIdHelper.FormatSessionIdForLogging(sessionId));

                    return Ok(new AnalyzeRecommendationResponse
                    {
                        Success = true,
                        Message = "No ritual pattern matched",
                        Data = null
                    });
                }

                // Log the recommendation
                var logId = await _recommendationService.LogRecommendationAsync(userId, sessionId, recommendation);

                _logger.LogInformation(
                    "Successfully generated recommendation {RecommendationId} for user {UserId}: ritual={RitualName}, confidence={ConfidenceScore}",
                    logId,
                    userId == Guid.Empty ? "guest" : userId.ToString(),
                    recommendation.RitualName,
                    recommendation.ConfidenceScore);

                return Ok(new AnalyzeRecommendationResponse
                {
                    Success = true,
                    Message = "Recommendation generated successfully",
                    Data = recommendation
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument in recommendation analysis");
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = $"Invalid request: {ex.Message}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing action sequence for recommendation");
                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while generating recommendations. Please try again later."
                });
            }
        }

        /// <summary>
        /// Records user interaction with a recommendation (viewed, dismissed, clicked)
        /// </summary>
        /// <param name="logId">The recommendation log ID</param>
        /// <param name="request">Request containing interaction type</param>
        /// <returns>Success status</returns>
        /// <response code="200">Interaction recorded successfully</response>
        /// <response code="400">Invalid request or interaction type</response>
        /// <response code="404">Recommendation log not found</response>
        /// <response code="500">Server error</response>
        [HttpPost("{logId}/interaction")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RecordInteraction(Guid logId, [FromBody] RecordInteractionRequest request)
        {
            try
            {
                // Validate request
                if (request == null || string.IsNullOrWhiteSpace(request.InteractionType))
                {
                    _logger.LogWarning("Invalid interaction request for log {LogId}", logId);
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = "Interaction type is required"
                    });
                }

                // Validate interaction type
                var validInteractions = new[] { "viewed", "dismissed", "clicked" };
                if (!validInteractions.Contains(request.InteractionType.ToLower()))
                {
                    _logger.LogWarning("Invalid interaction type '{InteractionType}' for log {LogId}", request.InteractionType, logId);
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = $"Invalid interaction type. Must be one of: {string.Join(", ", validInteractions)}"
                    });
                }

                _logger.LogInformation(
                    "Recording interaction '{InteractionType}' for recommendation log {LogId}",
                    request.InteractionType,
                    logId);

                // Record interaction
                var success = await _recommendationService.RecordInteractionAsync(logId, request.InteractionType.ToLower());

                if (!success)
                {
                    _logger.LogWarning("Failed to record interaction for log {LogId}", logId);
                    return NotFound(new ErrorResponse
                    {
                        Success = false,
                        Message = "Recommendation log not found"
                    });
                }

                return Ok(new SuccessResponse
                {
                    Success = true,
                    Message = "Interaction recorded successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording interaction for log {LogId}", logId);
                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while recording the interaction. Please try again later."
                });
            }
        }
    }

    /// <summary>
    /// Request model for analyzing action sequences
    /// </summary>
    public class AnalyzeRecommendationRequest
    {
        /// <summary>
        /// Sequence of user actions to analyze
        /// </summary>
        public List<ActionDto> ActionSequence { get; set; } = new();

        /// <summary>
        /// Items currently in the user's cart
        /// </summary>
        public List<Core.DTOs.Cart.CartItemDto> CartItems { get; set; } = new();
    }

    /// <summary>
    /// Response model for recommendation analysis
    /// </summary>
    public class AnalyzeRecommendationResponse
    {
        /// <summary>
        /// Whether the request was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message describing the result
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The generated recommendation (null if no pattern matched)
        /// </summary>
        public RecommendationPayloadDto? Data { get; set; }
    }

    /// <summary>
    /// Request model for recording user interactions
    /// </summary>
    public class RecordInteractionRequest
    {
        /// <summary>
        /// Type of interaction: "viewed", "dismissed", or "clicked"
        /// </summary>
        public string InteractionType { get; set; } = string.Empty;
    }

    /// <summary>
    /// Generic success response
    /// </summary>
    public class SuccessResponse
    {
        /// <summary>
        /// Whether the request was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message describing the result
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

}
