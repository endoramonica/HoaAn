using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.DTOs.Rituals;
using VietCommerce.Core.Entities.Rituals;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services
{
    /// <summary>
    /// Service for generating recommendations based on ritual pattern matching
    /// Handles missing item identification and audit logging
    /// </summary>
    public class RecommendationService : IRecommendationService
    {
        private readonly ISequentialPatternMatcher _patternMatcher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RecommendationService> _logger;

        public RecommendationService(
            ISequentialPatternMatcher patternMatcher,
            IUnitOfWork unitOfWork,
            IProductRepository productRepository,
            IMapper mapper,
            ILogger<RecommendationService> logger)
        {
            _patternMatcher = patternMatcher ?? throw new ArgumentNullException(nameof(patternMatcher));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Generates a recommendation based on user action sequence and cart items
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <param name="actionSequence">The sequence of user actions</param>
        /// <param name="cartItems">The items currently in the user's cart</param>
        /// <returns>RecommendationPayload with matched ritual and missing items, or null if no match</returns>
        public async Task<RecommendationPayloadDto?> GenerateRecommendationAsync(
            Guid userId,
            string sessionId,
            List<ActionDto> actionSequence,
            List<CartItemDto> cartItems)
        {
            try
            {
                // Validate inputs
                if (actionSequence == null || actionSequence.Count == 0)
                {
                    _logger.LogInformation($"No action sequence provided for user {userId} in session {sessionId}");
                    return null;
                }

                // Check if pattern matcher is initialized
                if (!_patternMatcher.IsInitialized())
                {
                    _logger.LogWarning("Pattern matcher not initialized. Cannot generate recommendations.");
                    return null;
                }

                // Match pattern against action sequence
                var matchResult = _patternMatcher.MatchPattern(actionSequence);

                // If no pattern matched, return null
                if (!matchResult.Matched)
                {
                    _logger.LogInformation($"No ritual pattern matched for user {userId} in session {sessionId}");
                    return null;
                }

                // Get the matched ritual
                var ritual = _patternMatcher.GetRitualById(matchResult.RitualId);
                if (ritual == null)
                {
                    _logger.LogError($"Ritual {matchResult.RitualId} not found in manifest");
                    return null;
                }

                // Identify missing items
                var missingItemIds = await GetMissingItemsAsync(ritual, cartItems);

                // Build system report
                var systemReport = BuildSystemReport(matchResult, ritual, missingItemIds);

                // Create recommendation payload
                var payload = new RecommendationPayloadDto
                {
                    RitualId = matchResult.RitualId,
                    RitualName = matchResult.RitualName,
                    ConfidenceScore = matchResult.ConfidenceScore,
                    MissingItems = missingItemIds,
                    MatchingMetadata = matchResult.MatchingMetadata,
                    SystemReport = systemReport
                };

                _logger.LogInformation(
                    $"Generated recommendation for user {userId}: ritual={ritual.Name}, confidence={matchResult.ConfidenceScore}, missingItems={missingItemIds.Count}");

                return payload;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating recommendation for user {userId} in session {sessionId}");
                return null;
            }
        }

        /// <summary>
        /// Identifies missing items for a detected ritual based on cart contents
        /// </summary>
        /// <param name="ritual">The detected ritual</param>
        /// <param name="cartItems">The items currently in the user's cart</param>
        /// <returns>List of product IDs that are missing from the cart</returns>
        public async Task<List<Guid>> GetMissingItemsAsync(RitualDto ritual, List<CartItemDto> cartItems)
        {
            try
            {
                var missingItems = new List<Guid>();

                // Get all product IDs currently in cart
                var cartProductIds = cartItems?.Select(ci => ci.ProductId).ToHashSet() ?? new HashSet<Guid>();

                // Iterate through required items for the ritual
                foreach (var requiredItem in ritual.RequiredItems)
                {
                    // Check each product ID in the required items
                    foreach (var productId in requiredItem.ProductIds)
                    {
                        // If product is not in cart, add to missing items
                        if (!cartProductIds.Contains(productId))
                        {
                            missingItems.Add(productId);
                        }
                    }
                }

                _logger.LogInformation($"Identified {missingItems.Count} missing items for ritual {ritual.Name}");

                return missingItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error identifying missing items for ritual {ritual.Name}");
                return new List<Guid>();
            }
        }

        /// <summary>
        /// Logs a recommendation for audit trail and analytics
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="sessionId">The session ID</param>
        /// <param name="payload">The recommendation payload to log</param>
        /// <returns>The ID of the created log entry</returns>
        public async Task<Guid> LogRecommendationAsync(Guid userId, string sessionId, RecommendationPayloadDto payload)
        {
            try
            {
                // Serialize metadata and system report to JSON
                var matchingMetadataJson = JsonSerializer.Serialize(payload.MatchingMetadata);
                var systemReportJson = JsonSerializer.Serialize(payload.SystemReport);
                var missingItemsJson = JsonSerializer.Serialize(payload.MissingItems);

                // Create recommendation log entity
                var logEntity = new RecommendationLogEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    SessionId = sessionId,
                    RitualId = Guid.Parse(payload.RitualId),
                    ConfidenceScore = payload.ConfidenceScore,
                    MissingItemsJson = missingItemsJson,
                    MatchingMetadataJson = matchingMetadataJson,
                    SystemReportJson = systemReportJson,
                    DisplayedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                // Add to database
                await _unitOfWork.RecommendationLogs.AddAsync(logEntity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Logged recommendation {logEntity.Id} for user {userId}");

                return logEntity.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error logging recommendation for user {userId}");
                throw;
            }
        }

        /// <summary>
        /// Records user interaction with a recommendation
        /// </summary>
        /// <param name="logId">The recommendation log ID</param>
        /// <param name="interaction">The interaction type (viewed, dismissed, clicked)</param>
        /// <returns>True if successfully recorded, false otherwise</returns>
        public async Task<bool> RecordInteractionAsync(Guid logId, string interaction)
        {
            try
            {
                var log = await _unitOfWork.RecommendationLogs.GetByIdAsync(logId);
                if (log == null)
                {
                    _logger.LogWarning($"Recommendation log {logId} not found");
                    return false;
                }

                log.UserInteraction = interaction;
                log.InteractionAt = DateTime.UtcNow;

                _unitOfWork.RecommendationLogs.Update(log);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Recorded interaction '{interaction}' for recommendation {logId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recording interaction for recommendation {logId}");
                return false;
            }
        }

        /// <summary>
        /// Gets the product details for missing items
        /// </summary>
        /// <param name="missingItemIds">List of product IDs that are missing</param>
        /// <returns>List of product DTOs for the missing items</returns>
        public async Task<List<ProductListDto>> GetMissingItemDetailsAsync(List<Guid> missingItemIds)
        {
            try
            {
                if (missingItemIds == null || missingItemIds.Count == 0)
                {
                    return new List<ProductListDto>();
                }

                var products = new List<ProductListDto>();

                // Fetch each product by ID
                foreach (var productId in missingItemIds)
                {
                    var product = await _productRepository.GetByIdAsync(productId);
                    if (product != null)
                    {
                        var productDto = _mapper.Map<ProductListDto>(product);
                        products.Add(productDto);
                    }
                    else
                    {
                        _logger.LogWarning($"Product {productId} not found in catalog");
                    }
                }

                _logger.LogInformation($"Retrieved details for {products.Count} missing items");

                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving missing item details");
                return new List<ProductListDto>();
            }
        }

        /// <summary>
        /// Builds a system report with detailed matching logic and reasoning
        /// </summary>
        /// <param name="matchResult">The pattern match result</param>
        /// <param name="ritual">The matched ritual</param>
        /// <param name="missingItemIds">The missing item IDs</param>
        /// <returns>SystemReportDto with detailed information</returns>
        private SystemReportDto BuildSystemReport(MatchResultDto matchResult, RitualDto ritual, List<Guid> missingItemIds)
        {
            var report = new SystemReportDto
            {
                MatchedPattern = ritual.ActionSequencePattern,
                MatchingSteps = new List<string>
                {
                    $"Analyzed action sequence of length {matchResult.MatchingMetadata.TotalSequenceLength}",
                    $"Matched {matchResult.MatchingMetadata.MatchedSequenceLength} actions from ritual pattern",
                    $"Calculated confidence score: {matchResult.ConfidenceScore:P}",
                    $"Ritual '{ritual.Name}' confidence threshold: {ritual.ConfidenceThreshold:P}",
                    $"Pattern match successful - confidence exceeds threshold"
                },
                ReasonsForMissingItems = new Dictionary<Guid, string>()
            };

            // Add reasons for each missing item
            foreach (var missingItemId in missingItemIds)
            {
                var requiredItem = ritual.RequiredItems
                    .FirstOrDefault(ri => ri.ProductIds.Contains(missingItemId));

                if (requiredItem != null)
                {
                    report.ReasonsForMissingItems[missingItemId] =
                        $"Required for ritual '{ritual.Name}' in category {requiredItem.CategoryId}";
                }
            }

            return report;
        }
    }
}
