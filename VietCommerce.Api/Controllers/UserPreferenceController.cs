using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VietCommerce.Application.Helpers;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Controllers
{
    /// <summary>
    /// Controller for managing user preferences related to ritual recommendations
    /// Handles dismissals and disabling of rituals
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserPreferenceController : ControllerBase
    {
        private readonly IUserPreferenceService _userPreferenceService;
        private readonly ILogger<UserPreferenceController> _logger;

        public UserPreferenceController(
            IUserPreferenceService userPreferenceService,
            ILogger<UserPreferenceController> logger)
        {
            _userPreferenceService = userPreferenceService ?? throw new ArgumentNullException(nameof(userPreferenceService));
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
        /// Records a dismissal for a ritual recommendation
        /// Used when user temporarily hides a recommendation
        /// 
        /// Requirements: 6.2
        /// - Records dismissal for a user and ritual
        /// - Reduces future recommendations of that type
        /// - Allows user to dismiss without permanently disabling
        /// </summary>
        /// <param name="request">Request containing ritual ID and optional reason</param>
        /// <returns>Success status</returns>
        /// <response code="200">Dismissal recorded successfully</response>
        /// <response code="400">Invalid request or missing required fields</response>
        /// <response code="401">Unauthorized (user not authenticated)</response>
        /// <response code="500">Server error during dismissal recording</response>
        [HttpPost("dismiss-ritual")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserPreferenceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DismissRitual([FromBody] DismissRitualRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                {
                    _logger.LogWarning("Null request received for dismiss ritual");
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = "Request cannot be null"
                    });
                }

                // Validate ritual ID
                if (request.RitualId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid ritual ID received for dismiss ritual");
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = "Ritual ID cannot be empty"
                    });
                }

                // Get user ID (required for dismissal tracking)
                var userId = GetCurrentUserId();
                if (userId == null || userId == Guid.Empty)
                {
                    _logger.LogWarning("Unauthorized attempt to dismiss ritual without user ID");
                    return Unauthorized(new ErrorResponse
                    {
                        Success = false,
                        Message = "User authentication is required to dismiss recommendations"
                    });
                }

                _logger.LogInformation(
                    "Recording dismissal for user {UserId} and ritual {RitualId}. Reason: {Reason}",
                    userId,
                    request.RitualId,
                    request.Reason ?? "Not provided");

                // Record the dismissal
                await _userPreferenceService.RecordDismissalAsync(
                    userId.Value,
                    request.RitualId,
                    request.Reason);

                _logger.LogInformation(
                    "Successfully recorded dismissal for user {UserId} and ritual {RitualId}",
                    userId,
                    request.RitualId);

                return Ok(new UserPreferenceResponse
                {
                    Success = true,
                    Message = "Ritual dismissal recorded successfully"
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument in dismiss ritual request");
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = $"Invalid request: {ex.Message}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording ritual dismissal");
                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while recording the dismissal. Please try again later."
                });
            }
        }

        /// <summary>
        /// Disables a ritual for a user in the current session
        /// Used when user explicitly indicates they're not interested in a ritual
        /// 
        /// Requirements: 6.3
        /// - Disables pattern detection for that ritual in the current session
        /// - Prevents recommendations for the disabled ritual
        /// - Allows user to explicitly opt-out of ritual detection
        /// </summary>
        /// <param name="request">Request containing ritual ID</param>
        /// <returns>Success status</returns>
        /// <response code="200">Ritual disabled successfully</response>
        /// <response code="400">Invalid request or missing required fields</response>
        /// <response code="401">Unauthorized (user not authenticated)</response>
        /// <response code="500">Server error during ritual disabling</response>
        [HttpPost("disable-ritual")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserPreferenceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DisableRitual([FromBody] DisableRitualRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                {
                    _logger.LogWarning("Null request received for disable ritual");
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = "Request cannot be null"
                    });
                }

                // Validate ritual ID
                if (request.RitualId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid ritual ID received for disable ritual");
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = "Ritual ID cannot be empty"
                    });
                }

                // Get user ID (required for disabling)
                var userId = GetCurrentUserId();
                if (userId == null || userId == Guid.Empty)
                {
                    _logger.LogWarning("Unauthorized attempt to disable ritual without user ID");
                    return Unauthorized(new ErrorResponse
                    {
                        Success = false,
                        Message = "User authentication is required to disable rituals"
                    });
                }

                // Get or create session ID
                var sessionId = GetOrCreateSessionId();

                _logger.LogInformation(
                    "Disabling ritual {RitualId} for user {UserId} in session {SessionId}",
                    request.RitualId,
                    userId,
                    SessionIdHelper.FormatSessionIdForLogging(sessionId));

                // Disable the ritual
                await _userPreferenceService.DisableRitualAsync(
                    userId.Value,
                    sessionId,
                    request.RitualId);

                _logger.LogInformation(
                    "Successfully disabled ritual {RitualId} for user {UserId}",
                    request.RitualId,
                    userId);

                return Ok(new UserPreferenceResponse
                {
                    Success = true,
                    Message = "Ritual disabled successfully"
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument in disable ritual request");
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = $"Invalid request: {ex.Message}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling ritual");
                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while disabling the ritual. Please try again later."
                });
            }
        }
    }

    /// <summary>
    /// Request model for dismissing a ritual recommendation
    /// </summary>
    public class DismissRitualRequest
    {
        /// <summary>
        /// The ID of the ritual to dismiss
        /// </summary>
        public Guid RitualId { get; set; }

        /// <summary>
        /// Optional reason for dismissal
        /// </summary>
        public string? Reason { get; set; }
    }

    /// <summary>
    /// Request model for disabling a ritual
    /// </summary>
    public class DisableRitualRequest
    {
        /// <summary>
        /// The ID of the ritual to disable
        /// </summary>
        public Guid RitualId { get; set; }
    }

    /// <summary>
    /// Response model for user preference operations
    /// </summary>
    public class UserPreferenceResponse
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
