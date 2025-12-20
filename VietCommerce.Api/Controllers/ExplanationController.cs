using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Rituals;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Controllers
{
    /// <summary>
    /// Controller for the FE-AI Explanation Engine
    /// Handles natural language explanation generation for recommendations using Gemini API
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ExplanationController : ControllerBase
    {
        private readonly IGeminiExplanationService _geminiExplanationService;
        private readonly ILogger<ExplanationController> _logger;

        public ExplanationController(
            IGeminiExplanationService geminiExplanationService,
            ILogger<ExplanationController> logger)
        {
            _geminiExplanationService = geminiExplanationService ?? throw new ArgumentNullException(nameof(geminiExplanationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Generates a natural language explanation for a recommendation using Gemini API
        /// 
        /// Requirements: 2.1, 2.2, 2.3, 2.5
        /// - Accepts RecommendationPayload from frontend
        /// - Calls GeminiExplanationService to generate explanation
        /// - Returns ExplanationPayload with human-readable text
        /// - Includes fallback logic if Gemini API fails
        /// </summary>
        /// <param name="request">Request containing the recommendation payload</param>
        /// <returns>ExplanationPayload with human-readable explanation</returns>
        /// <response code="200">Explanation generated successfully</response>
        /// <response code="400">Invalid request or recommendation payload</response>
        /// <response code="500">Server error during explanation generation</response>
        [HttpPost("generate")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GenerateExplanationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateExplanation([FromBody] GenerateExplanationRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                {
                    _logger.LogWarning("Null request received for explanation generation");
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = "Request cannot be null"
                    });
                }

                // Validate recommendation payload
                if (request.RecommendationPayload == null)
                {
                    _logger.LogWarning("Null recommendation payload received for explanation generation");
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = "Recommendation payload cannot be null"
                    });
                }

                // Validate ritual name
                if (string.IsNullOrWhiteSpace(request.RecommendationPayload.RitualName))
                {
                    _logger.LogWarning("Empty ritual name in recommendation payload");
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = "Ritual name cannot be empty"
                    });
                }

                // Validate missing items
                if (request.RecommendationPayload.MissingItems == null || request.RecommendationPayload.MissingItems.Count == 0)
                {
                    _logger.LogWarning("No missing items in recommendation payload");
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = "Missing items list cannot be empty"
                    });
                }

                _logger.LogInformation(
                    "Generating explanation for ritual '{RitualName}' with confidence score {ConfidenceScore} and {MissingItemCount} missing items",
                    request.RecommendationPayload.RitualName,
                    request.RecommendationPayload.ConfidenceScore,
                    request.RecommendationPayload.MissingItems.Count);

                // Generate explanation using Gemini API (with fallback)
                var explanation = await _geminiExplanationService.GenerateExplanationAsync(request.RecommendationPayload);

                // Validate explanation
                if (explanation == null)
                {
                    _logger.LogWarning("Explanation generation returned null for ritual '{RitualName}'", request.RecommendationPayload.RitualName);
                    return StatusCode(500, new ErrorResponse
                    {
                        Success = false,
                        Message = "Failed to generate explanation"
                    });
                }

                _logger.LogInformation(
                    "Successfully generated explanation for ritual '{RitualName}' using {GeneratedBy}",
                    explanation.RitualName,
                    explanation.GeneratedBy);

                return Ok(new GenerateExplanationResponse
                {
                    Success = true,
                    Message = "Explanation generated successfully",
                    Data = explanation
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument in explanation generation");
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = $"Invalid request: {ex.Message}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating explanation");
                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while generating the explanation. Please try again later."
                });
            }
        }

        /// <summary>
        /// Gets the fallback explanation template for a recommendation
        /// Useful for testing or when Gemini API is not available
        /// </summary>
        /// <param name="request">Request containing the recommendation payload</param>
        /// <returns>ExplanationPayload with template-based explanation</returns>
        /// <response code="200">Fallback explanation generated successfully</response>
        /// <response code="400">Invalid request or recommendation payload</response>
        /// <response code="500">Server error</response>
        [HttpPost("fallback")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GenerateExplanationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult GetFallbackExplanation([FromBody] GenerateExplanationRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                {
                    _logger.LogWarning("Null request received for fallback explanation");
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = "Request cannot be null"
                    });
                }

                // Validate recommendation payload
                if (request.RecommendationPayload == null)
                {
                    _logger.LogWarning("Null recommendation payload received for fallback explanation");
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = "Recommendation payload cannot be null"
                    });
                }

                _logger.LogInformation(
                    "Generating fallback explanation for ritual '{RitualName}'",
                    request.RecommendationPayload.RitualName);

                // Generate fallback explanation
                var explanation = _geminiExplanationService.GetFallbackExplanation(request.RecommendationPayload);

                // Validate explanation
                if (explanation == null)
                {
                    _logger.LogWarning("Fallback explanation generation returned null");
                    return StatusCode(500, new ErrorResponse
                    {
                        Success = false,
                        Message = "Failed to generate fallback explanation"
                    });
                }

                _logger.LogInformation(
                    "Successfully generated fallback explanation for ritual '{RitualName}'",
                    explanation.RitualName);

                return Ok(new GenerateExplanationResponse
                {
                    Success = true,
                    Message = "Fallback explanation generated successfully",
                    Data = explanation
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating fallback explanation");
                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while generating the fallback explanation. Please try again later."
                });
            }
        }

        /// <summary>
        /// Checks if the Gemini API is configured and available
        /// </summary>
        /// <returns>Status indicating if Gemini API is available</returns>
        /// <response code="200">Status check successful</response>
        [HttpGet("status")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GeminiStatusResponse), StatusCodes.Status200OK)]
        public IActionResult GetGeminiStatus()
        {
            try
            {
                var isConfigured = _geminiExplanationService.IsConfigured();

                _logger.LogInformation("Gemini API status check: {IsConfigured}", isConfigured ? "configured" : "not configured");

                return Ok(new GeminiStatusResponse
                {
                    Success = true,
                    IsConfigured = isConfigured,
                    Message = isConfigured
                        ? "Gemini API is configured and available"
                        : "Gemini API is not configured. Fallback explanations will be used."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking Gemini API status");
                return StatusCode(500, new ErrorResponse
                {
                    Success = false,
                    Message = "An error occurred while checking Gemini API status."
                });
            }
        }
    }

    /// <summary>
    /// Request model for generating explanations
    /// </summary>
    public class GenerateExplanationRequest
    {
        /// <summary>
        /// The recommendation payload from BE-AI containing ritual and missing items
        /// </summary>
        public RecommendationPayloadDto RecommendationPayload { get; set; } = new();
    }

    /// <summary>
    /// Response model for explanation generation
    /// </summary>
    public class GenerateExplanationResponse
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
        /// The generated explanation
        /// </summary>
        public ExplanationPayloadDto? Data { get; set; }
    }

    /// <summary>
    /// Response model for Gemini API status check
    /// </summary>
    public class GeminiStatusResponse
    {
        /// <summary>
        /// Whether the request was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Whether Gemini API is configured
        /// </summary>
        public bool IsConfigured { get; set; }

        /// <summary>
        /// Status message
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

}
