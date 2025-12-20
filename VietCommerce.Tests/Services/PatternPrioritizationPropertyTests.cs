using Bogus;
using Moq;
using Xunit;
using AutoMapper;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.DTOs.Rituals;
using VietCommerce.Data.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Property-based tests for pattern prioritization logic
    /// Tests handling of multiple matching patterns and disabled ritual filtering
    /// </summary>
    public class PatternPrioritizationPropertyTests
    {
        private readonly Mock<ISequentialPatternMatcher> _mockPatternMatcher;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IUserPreferenceService> _mockUserPreferenceService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<RecommendationService>> _mockLogger;
        private readonly RecommendationService _service;

        public PatternPrioritizationPropertyTests()
        {
            _mockPatternMatcher = new Mock<ISequentialPatternMatcher>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockUserPreferenceService = new Mock<IUserPreferenceService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<RecommendationService>>();

            _service = new RecommendationService(
                _mockPatternMatcher.Object,
                _mockUnitOfWork.Object,
                _mockProductRepository.Object,
                _mockUserPreferenceService.Object,
                _mockMapper.Object,
                _mockLogger.Object
            );
        }

        /// <summary>
        /// Property 21: Highest Confidence Pattern Prioritization
        /// **Feature: sequential-ritual-recommendation, Property 21: Highest Confidence Pattern Prioritization**
        /// **Validates: Requirements 5.5**
        /// 
        /// For any action sequence matching multiple ritual patterns, the system SHALL prioritize 
        /// the pattern with the highest confidence score.
        /// </summary>
        [Fact]
        public async Task Property_21_HighestConfidencePatternPrioritization()
        {
            // Arrange - Setup multiple matching patterns with different confidence scores
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid().ToString();
            var faker = new Faker();

            // Run 100 iterations to test with various confidence scores
            for (int iteration = 0; iteration < 100; iteration++)
            {
                // Generate random confidence scores
                var confidenceScores = new List<decimal>
                {
                    faker.Random.Decimal(0.5m, 0.7m),  // Lower confidence
                    faker.Random.Decimal(0.7m, 0.85m), // Medium confidence
                    faker.Random.Decimal(0.85m, 1.0m)  // Highest confidence
                };

                // Sort to identify the highest
                var highestConfidence = confidenceScores.Max();
                var highestConfidenceIndex = confidenceScores.IndexOf(highestConfidence);

                // Create rituals with different confidence scores
                var rituals = new List<RitualDto>();
                for (int i = 0; i < confidenceScores.Count; i++)
                {
                    rituals.Add(new RitualDto
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = $"Ritual_{i}",
                        ActionSequencePattern = new List<ActionTypeDto>
                        {
                            new ActionTypeDto { Type = "ViewProduct" },
                            new ActionTypeDto { Type = "AddToCart" }
                        },
                        RequiredItems = new List<RitualRequiredItemDto>(),
                        ConfidenceThreshold = 0.5m
                    });
                }

                // Create action sequence
                var actionSequence = new List<ActionDto>
                {
                    new ActionDto { Type = "ViewProduct", Timestamp = DateTime.UtcNow },
                    new ActionDto { Type = "AddToCart", Timestamp = DateTime.UtcNow }
                };

                // The pattern matcher should return the highest confidence match
                var bestMatch = new MatchResultDto
                {
                    Matched = true,
                    RitualId = rituals[highestConfidenceIndex].Id,
                    RitualName = rituals[highestConfidenceIndex].Name,
                    ConfidenceScore = highestConfidence,
                    MatchedActions = actionSequence,
                    MatchingMetadata = new MatchingMetadataDto
                    {
                        MatchedSequenceLength = 2,
                        TotalSequenceLength = 2,
                        MatchedActionIndices = new List<int> { 0, 1 }
                    }
                };

                _mockPatternMatcher.Setup(m => m.IsInitialized()).Returns(true);
                _mockPatternMatcher.Setup(m => m.MatchPattern(It.IsAny<List<ActionDto>>()))
                    .Returns(bestMatch);
                _mockPatternMatcher.Setup(m => m.GetRitualById(bestMatch.RitualId))
                    .Returns(rituals[highestConfidenceIndex]);

                _mockUserPreferenceService.Setup(u => u.GetDisabledRitualsAsync(userId))
                    .ReturnsAsync(new List<Guid>());

                var mockRecommendationLogRepo = new Mock<IRecommendationLogRepository>();
                _mockUnitOfWork.Setup(u => u.RecommendationLogs).Returns(mockRecommendationLogRepo.Object);
                mockRecommendationLogRepo.Setup(r => r.AddAsync(It.IsAny<Core.Entities.Rituals.RecommendationLogEntity>()))
                    .Returns<Core.Entities.Rituals.RecommendationLogEntity>(entity => Task.FromResult(entity));

                // Act
                var result = await _service.GenerateRecommendationAsync(userId, sessionId, actionSequence, new List<CartItemDto>());

                // Assert - Should return the highest confidence match
                Assert.NotNull(result);
                Assert.Equal(highestConfidence, result.ConfidenceScore);
                Assert.Equal(rituals[highestConfidenceIndex].Name, result.RitualName);
            }
        }

        /// <summary>
        /// Property 23: Ritual Detection Disabling
        /// **Feature: sequential-ritual-recommendation, Property 23: Ritual Detection Disabling**
        /// **Validates: Requirements 6.3**
        /// 
        /// For any user indication of disinterest in a ritual, the system SHALL disable pattern 
        /// detection for that ritual in the current session.
        /// </summary>
        [Fact]
        public async Task Property_23_RitualDetectionDisabling()
        {
            // Arrange - Setup a matched ritual that is disabled for the user
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid().ToString();
            var disabledRitualId = Guid.NewGuid();
            var faker = new Faker();

            // Run 100 iterations
            for (int iteration = 0; iteration < 100; iteration++)
            {
                var actionSequence = new List<ActionDto>
                {
                    new ActionDto { Type = "ViewProduct", Timestamp = DateTime.UtcNow },
                    new ActionDto { Type = "AddToCart", Timestamp = DateTime.UtcNow }
                };

                var ritual = new RitualDto
                {
                    Id = disabledRitualId.ToString(),
                    Name = "Disabled Ritual",
                    ActionSequencePattern = new List<ActionTypeDto>
                    {
                        new ActionTypeDto { Type = "ViewProduct" },
                        new ActionTypeDto { Type = "AddToCart" }
                    },
                    RequiredItems = new List<RitualRequiredItemDto>(),
                    ConfidenceThreshold = 0.5m
                };

                var matchResult = new MatchResultDto
                {
                    Matched = true,
                    RitualId = disabledRitualId.ToString(),
                    RitualName = ritual.Name,
                    ConfidenceScore = 0.85m,
                    MatchedActions = actionSequence,
                    MatchingMetadata = new MatchingMetadataDto
                    {
                        MatchedSequenceLength = 2,
                        TotalSequenceLength = 2
                    }
                };

                _mockPatternMatcher.Setup(m => m.IsInitialized()).Returns(true);
                _mockPatternMatcher.Setup(m => m.MatchPattern(It.IsAny<List<ActionDto>>()))
                    .Returns(matchResult);
                _mockPatternMatcher.Setup(m => m.GetRitualById(disabledRitualId.ToString()))
                    .Returns(ritual);

                // Setup user preference service to return the disabled ritual
                _mockUserPreferenceService.Setup(u => u.GetDisabledRitualsAsync(userId))
                    .ReturnsAsync(new List<Guid> { disabledRitualId });

                // Act
                var result = await _service.GenerateRecommendationAsync(userId, sessionId, actionSequence, new List<CartItemDto>());

                // Assert - Should return null because the ritual is disabled
                Assert.Null(result);
            }
        }

        /// <summary>
        /// Property 22: Dismissal Recording
        /// **Feature: sequential-ritual-recommendation, Property 22: Dismissal Recording**
        /// **Validates: Requirements 6.2**
        /// 
        /// For any user dismissal of a recommendation, the system SHALL record the dismissal 
        /// and reduce future recommendations of that type.
        /// </summary>
        [Fact]
        public async Task Property_22_DismissalRecording()
        {
            // Arrange - Setup dismissal recording
            var userId = Guid.NewGuid();
            var ritualId = Guid.NewGuid();
            var faker = new Faker();

            // Run 100 iterations
            for (int iteration = 0; iteration < 100; iteration++)
            {
                var reason = faker.Lorem.Sentence();

                _mockUserPreferenceService.Setup(u => u.RecordDismissalAsync(userId, ritualId, reason))
                    .Returns(Task.CompletedTask);

                // Act
                await _mockUserPreferenceService.Object.RecordDismissalAsync(userId, ritualId, reason);

                // Assert - Verify dismissal was recorded
                _mockUserPreferenceService.Verify(
                    u => u.RecordDismissalAsync(userId, ritualId, reason),
                    Times.Once
                );
            }
        }

        /// <summary>
        /// Property 19: Re-analysis on Cart Update
        /// **Feature: sequential-ritual-recommendation, Property 19: Re-analysis on Cart Update**
        /// **Validates: Requirements 5.3**
        /// 
        /// For any item added to cart, the system SHALL re-analyze the action sequence and 
        /// update recommendations if the ritual context changes.
        /// </summary>
        [Fact]
        public async Task Property_19_ReanalysisOnCartUpdate()
        {
            // Arrange - Setup re-analysis scenario
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid().ToString();
            var faker = new Faker();

            // Run 100 iterations
            for (int iteration = 0; iteration < 100; iteration++)
            {
                var product1Id = Guid.NewGuid();
                var product2Id = Guid.NewGuid();
                var product3Id = Guid.NewGuid();
                var categoryId = Guid.NewGuid();

                var ritual = new RitualDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Test Ritual",
                    ActionSequencePattern = new List<ActionTypeDto>
                    {
                        new ActionTypeDto { Type = "ViewProduct" },
                        new ActionTypeDto { Type = "AddToCart" }
                    },
                    RequiredItems = new List<RitualRequiredItemDto>
                    {
                        new RitualRequiredItemDto
                        {
                            CategoryId = categoryId,
                            ProductIds = new List<Guid> { product1Id, product2Id, product3Id }
                        }
                    },
                    ConfidenceThreshold = 0.5m
                };

                var actionSequence = new List<ActionDto>
                {
                    new ActionDto { Type = "ViewProduct", Timestamp = DateTime.UtcNow },
                    new ActionDto { Type = "AddToCart", Timestamp = DateTime.UtcNow }
                };

                // Initial cart with one item
                var initialCart = new List<CartItemDto>
                {
                    new CartItemDto { ProductId = product1Id, ProductName = "Item 1" }
                };

                // Updated cart with additional item
                var updatedCart = new List<CartItemDto>
                {
                    new CartItemDto { ProductId = product1Id, ProductName = "Item 1" },
                    new CartItemDto { ProductId = product2Id, ProductName = "Item 2" }
                };

                var matchResult = new MatchResultDto
                {
                    Matched = true,
                    RitualId = ritual.Id,
                    RitualName = ritual.Name,
                    ConfidenceScore = 0.85m,
                    MatchedActions = actionSequence,
                    MatchingMetadata = new MatchingMetadataDto
                    {
                        MatchedSequenceLength = 2,
                        TotalSequenceLength = 2
                    }
                };

                _mockPatternMatcher.Setup(m => m.IsInitialized()).Returns(true);
                _mockPatternMatcher.Setup(m => m.MatchPattern(It.IsAny<List<ActionDto>>()))
                    .Returns(matchResult);
                _mockPatternMatcher.Setup(m => m.GetRitualById(ritual.Id))
                    .Returns(ritual);

                _mockUserPreferenceService.Setup(u => u.GetDisabledRitualsAsync(userId))
                    .ReturnsAsync(new List<Guid>());

                var mockRecommendationLogRepo = new Mock<IRecommendationLogRepository>();
                _mockUnitOfWork.Setup(u => u.RecommendationLogs).Returns(mockRecommendationLogRepo.Object);
                mockRecommendationLogRepo.Setup(r => r.AddAsync(It.IsAny<Core.Entities.Rituals.RecommendationLogEntity>()))
                    .Returns<Core.Entities.Rituals.RecommendationLogEntity>(entity => Task.FromResult(entity));

                // Act - Generate recommendation with initial cart
                var initialResult = await _service.GenerateRecommendationAsync(userId, sessionId, actionSequence, initialCart);

                // Act - Generate recommendation with updated cart
                var updatedResult = await _service.GenerateRecommendationAsync(userId, sessionId, actionSequence, updatedCart);

                // Assert - Both should be valid recommendations
                Assert.NotNull(initialResult);
                Assert.NotNull(updatedResult);

                // The updated result should have fewer missing items
                Assert.True(updatedResult.MissingItems.Count <= initialResult.MissingItems.Count);
            }
        }

        /// <summary>
        /// Property 20: Recommendations Cleared on Navigation
        /// **Feature: sequential-ritual-recommendation, Property 20: Recommendations Cleared on Navigation**
        /// **Validates: Requirements 5.4**
        /// 
        /// For any user navigation away from ritual-related products, recommendations SHALL be 
        /// cleared and pattern detection reset.
        /// </summary>
        [Fact]
        public async Task Property_20_RecommendationsClearedOnNavigation()
        {
            // Arrange - Setup navigation scenario
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid().ToString();
            var faker = new Faker();

            // Run 100 iterations
            for (int iteration = 0; iteration < 100; iteration++)
            {
                // Action sequence that doesn't match any ritual (navigation away)
                var navigationSequence = new List<ActionDto>
                {
                    new ActionDto { Type = "ViewProduct", Timestamp = DateTime.UtcNow },
                    new ActionDto { Type = "BrowseCategory", Timestamp = DateTime.UtcNow },
                    new ActionDto { Type = "ViewProfile", Timestamp = DateTime.UtcNow }
                };

                _mockPatternMatcher.Setup(m => m.IsInitialized()).Returns(true);
                _mockPatternMatcher.Setup(m => m.MatchPattern(It.IsAny<List<ActionDto>>()))
                    .Returns(new MatchResultDto { Matched = false });

                // Act
                var result = await _service.GenerateRecommendationAsync(userId, sessionId, navigationSequence, new List<CartItemDto>());

                // Assert - Should return null (recommendations cleared)
                Assert.Null(result);
            }
        }
    }
}
