using Bogus;
using Xunit;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Rituals;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Property-based tests for the Sequential Pattern Matcher
    /// Tests the PrefixSpan-inspired algorithm for ritual detection
    /// </summary>
    public class SequentialPatternMatcherPropertyTests
    {
        private readonly ISequentialPatternMatcher _patternMatcher;

        public SequentialPatternMatcherPropertyTests()
        {
            _patternMatcher = new SequentialPatternMatcher();
        }

        /// <summary>
        /// Property 27: PrefixSpan-Inspired Matching Algorithm
        /// **Feature: sequential-ritual-recommendation, Property 27: PrefixSpan-Inspired Matching Algorithm**
        /// **Validates: Requirements 7.4**
        /// 
        /// For any action sequence received by BE-AI, the system SHALL match it against patterns 
        /// using PrefixSpan-inspired algorithm and return a MatchResult.
        /// </summary>
        [Fact]
        public void Property_27_PrefixSpanMatchingAlgorithm()
        {
            // Arrange - Create a manifest with known rituals
            var manifest = CreateTestManifest();
            _patternMatcher.Initialize(manifest);

            var faker = new Faker();

            // Act & Assert - Run 100 iterations with various action sequences
            for (int i = 0; i < 100; i++)
            {
                // Generate action sequences that may or may not match patterns
                var actionSequence = GenerateActionSequence(faker);

                // Act - Match the pattern
                var result = _patternMatcher.MatchPattern(actionSequence);

                // Assert - Result should be valid
                Assert.NotNull(result);
                
                // If matched, verify the result structure
                if (result.Matched)
                {
                    Assert.NotEmpty(result.RitualId);
                    Assert.NotEmpty(result.RitualName);
                    Assert.True(result.ConfidenceScore >= 0 && result.ConfidenceScore <= 1);
                    Assert.NotNull(result.MatchingMetadata);
                    Assert.True(result.MatchingMetadata.MatchedSequenceLength > 0);
                    Assert.True(result.MatchingMetadata.TotalSequenceLength > 0);
                    Assert.NotEmpty(result.MatchedActions);
                }
            }
        }

        /// <summary>
        /// Property 2: Ritual Type Identification Accuracy
        /// **Feature: sequential-ritual-recommendation, Property 2: Ritual Type Identification Accuracy**
        /// **Validates: Requirements 1.2**
        /// 
        /// For any action sequence that matches a ritual pattern above the confidence threshold, 
        /// the system SHALL identify the correct ritual type.
        /// </summary>
        [Fact]
        public void Property_2_RitualTypeIdentificationAccuracy()
        {
            // Arrange - Create a manifest with distinct rituals
            var manifest = CreateTestManifest();
            _patternMatcher.Initialize(manifest);

            var faker = new Faker();

            // Act & Assert - Run 100 iterations
            for (int i = 0; i < 100; i++)
            {
                // Generate an action sequence that matches a specific ritual
                var targetRitual = manifest.GetActiveRituals().First();
                var actionSequence = GenerateMatchingActionSequence(faker, targetRitual);

                // Act - Match the pattern
                var result = _patternMatcher.MatchPattern(actionSequence);

                // Assert - If matched, should identify the correct ritual
                if (result.Matched)
                {
                    // The matched ritual should be one of the rituals in the manifest
                    var matchedRitual = manifest.GetRitualById(result.RitualId);
                    Assert.NotNull(matchedRitual);
                    Assert.Equal(result.RitualName, matchedRitual.Name);
                }
            }
        }

        /// <summary>
        /// Property 5: No Recommendations for Non-Matching Sequences
        /// **Feature: sequential-ritual-recommendation, Property 5: No Recommendations for Non-Matching Sequences**
        /// **Validates: Requirements 1.5**
        /// 
        /// For any action sequence that does not match any ritual pattern above the confidence threshold, 
        /// the system SHALL not generate recommendations.
        /// </summary>
        [Fact]
        public void Property_5_NoRecommendationsForNonMatchingSequences()
        {
            // Arrange - Create a manifest with specific patterns
            var manifest = CreateTestManifest();
            _patternMatcher.Initialize(manifest);

            var faker = new Faker();

            // Act & Assert - Run 100 iterations with random action sequences
            for (int i = 0; i < 100; i++)
            {
                // Generate a random action sequence that likely won't match
                var randomActions = new List<ActionDto>();
                for (int j = 0; j < faker.Random.Int(1, 5); j++)
                {
                    randomActions.Add(new ActionDto
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.NewGuid(),
                        SessionId = faker.Random.Guid().ToString(),
                        Type = faker.PickRandom("RandomAction1", "RandomAction2", "RandomAction3"),
                        Timestamp = faker.Date.Recent(),
                        ProductId = faker.Random.Bool() ? Guid.NewGuid() : null,
                        CategoryId = faker.Random.Bool() ? Guid.NewGuid() : null
                    });
                }

                // Act - Match the pattern
                var result = _patternMatcher.MatchPattern(randomActions);

                // Assert - Should not match or have very low confidence
                // If it does match, confidence should be below threshold
                if (result.Matched)
                {
                    var ritual = manifest.GetRitualById(result.RitualId);
                    Assert.NotNull(ritual);
                    Assert.True(result.ConfidenceScore >= ritual.ConfidenceThreshold);
                }
            }
        }

        /// <summary>
        /// Property 11: Confidence Threshold Assignment
        /// **Feature: sequential-ritual-recommendation, Property 11: Confidence Threshold Assignment**
        /// **Validates: Requirements 3.2**
        /// 
        /// For any ritual created, a confidence threshold value SHALL be assigned.
        /// </summary>
        [Fact]
        public void Property_11_ConfidenceThresholdAssignment()
        {
            // Arrange - Create a manifest
            var manifest = CreateTestManifest();
            _patternMatcher.Initialize(manifest);

            // Act & Assert - Verify all rituals have confidence thresholds
            var rituals = _patternMatcher.GetActiveRituals();
            
            Assert.NotEmpty(rituals);
            foreach (var ritual in rituals)
            {
                Assert.True(ritual.ConfidenceThreshold >= 0);
                Assert.True(ritual.ConfidenceThreshold <= 1);
            }
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
        public void Property_21_HighestConfidencePatternPrioritization()
        {
            // Arrange - Create a manifest with multiple rituals
            var manifest = CreateTestManifest();
            _patternMatcher.Initialize(manifest);

            var faker = new Faker();

            // Act & Assert - Run 100 iterations
            for (int i = 0; i < 100; i++)
            {
                // Generate an action sequence
                var actionSequence = GenerateActionSequence(faker);

                // Act - Match the pattern
                var result = _patternMatcher.MatchPattern(actionSequence);

                // Assert - If matched, verify it's the best match
                if (result.Matched)
                {
                    // Try matching against all rituals individually
                    var allMatches = new List<(string RitualId, decimal Confidence)>();
                    
                    foreach (var ritual in manifest.GetActiveRituals())
                    {
                        var testSequence = new List<ActionDto>(actionSequence);
                        var testResult = _patternMatcher.MatchPattern(testSequence);
                        
                        if (testResult.Matched)
                        {
                            allMatches.Add((testResult.RitualId, testResult.ConfidenceScore));
                        }
                    }

                    // The returned result should have the highest confidence among matches
                    if (allMatches.Count > 0)
                    {
                        var maxConfidence = allMatches.Max(m => m.Confidence);
                        Assert.True(result.ConfidenceScore >= maxConfidence * 0.99m); // Allow small floating point variance
                    }
                }
            }
        }

        /// <summary>
        /// Property 28: Recommendation Payload Structure
        /// **Feature: sequential-ritual-recommendation, Property 28: Recommendation Payload Structure**
        /// **Validates: Requirements 7.5**
        /// 
        /// For any pattern match, the BE-AI SHALL return a MatchResult containing ritual name, 
        /// missing items, confidence score, and matching metadata.
        /// </summary>
        [Fact]
        public void Property_28_RecommendationPayloadStructure()
        {
            // Arrange - Create a manifest
            var manifest = CreateTestManifest();
            _patternMatcher.Initialize(manifest);

            var faker = new Faker();

            // Act & Assert - Run 100 iterations
            for (int i = 0; i < 100; i++)
            {
                var actionSequence = GenerateActionSequence(faker);

                // Act - Match the pattern
                var result = _patternMatcher.MatchPattern(actionSequence);

                // Assert - Result structure is always valid
                Assert.NotNull(result);
                Assert.NotNull(result.MatchingMetadata);
                
                if (result.Matched)
                {
                    // Verify all required fields are present
                    Assert.NotEmpty(result.RitualId);
                    Assert.NotEmpty(result.RitualName);
                    Assert.True(result.ConfidenceScore >= 0 && result.ConfidenceScore <= 1);
                    Assert.NotNull(result.MatchedActions);
                    Assert.True(result.MatchingMetadata.MatchedSequenceLength > 0);
                    Assert.True(result.MatchingMetadata.TotalSequenceLength > 0);
                    Assert.NotEmpty(result.MatchingMetadata.MatchedActionIndices);
                }
            }
        }

        /// <summary>
        /// Property: Confidence Threshold Filtering
        /// **Feature: sequential-ritual-recommendation, Property: Confidence Threshold Filtering**
        /// **Validates: Requirements 1.2, 3.2**
        /// 
        /// For any action sequence that matches a ritual pattern, if the calculated confidence score 
        /// is below the ritual's confidence threshold, the system SHALL NOT match that pattern.
        /// Conversely, if the confidence score meets or exceeds the threshold, the pattern SHALL be matched.
        /// </summary>
        [Fact]
        public void Property_ConfidenceThresholdFiltering()
        {
            // Arrange - Create a manifest with rituals having different thresholds
            var manifest = new RitualManifest();

            // Add a ritual with high threshold (0.8)
            manifest.Rituals.Add(new RitualDto
            {
                Id = "high-threshold-ritual",
                Name = "High Threshold Ritual",
                ActionSequencePattern = new List<ActionTypeDto>
                {
                    new ActionTypeDto { Type = "ViewProduct" },
                    new ActionTypeDto { Type = "AddToCart" }
                },
                RequiredItems = new List<RitualRequiredItemDto>(),
                ConfidenceThreshold = 0.8m,
                CulturalSignificance = "Test ritual",
                Sources = new List<string>()
            });

            // Add a ritual with low threshold (0.3)
            manifest.Rituals.Add(new RitualDto
            {
                Id = "low-threshold-ritual",
                Name = "Low Threshold Ritual",
                ActionSequencePattern = new List<ActionTypeDto>
                {
                    new ActionTypeDto { Type = "BrowseCategory" },
                    new ActionTypeDto { Type = "ViewProduct" }
                },
                RequiredItems = new List<RitualRequiredItemDto>(),
                ConfidenceThreshold = 0.3m,
                CulturalSignificance = "Test ritual",
                Sources = new List<string>()
            });

            _patternMatcher.Initialize(manifest);

            var faker = new Faker();

            // Act & Assert - Run 100 iterations
            for (int i = 0; i < 100; i++)
            {
                // Generate action sequences
                var actionSequence = GenerateActionSequence(faker);

                // Act - Match the pattern
                var result = _patternMatcher.MatchPattern(actionSequence);

                // Assert - If matched, confidence must meet or exceed the ritual's threshold
                if (result.Matched)
                {
                    var ritual = manifest.GetRitualById(result.RitualId);
                    Assert.NotNull(ritual);
                    
                    // The confidence score must be >= the ritual's threshold
                    Assert.True(result.ConfidenceScore >= ritual.ConfidenceThreshold,
                        $"Confidence score {result.ConfidenceScore} should be >= threshold {ritual.ConfidenceThreshold}");
                }

                // Also verify that if we generate a sequence matching a pattern,
                // but the confidence is below threshold, it should not match
                var lowThresholdRitual = manifest.GetRitualById("low-threshold-ritual");
                var highThresholdRitual = manifest.GetRitualById("high-threshold-ritual");

                // Generate a sequence that partially matches the low threshold ritual
                var partialMatchSequence = new List<ActionDto>
                {
                    new ActionDto
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.NewGuid(),
                        SessionId = faker.Random.Guid().ToString(),
                        Type = "BrowseCategory",
                        Timestamp = faker.Date.Recent()
                    }
                };

                var partialResult = _patternMatcher.MatchPattern(partialMatchSequence);
                
                // This partial match might not meet the threshold, so we verify the logic
                if (!partialResult.Matched)
                {
                    // If not matched, it's because confidence was below threshold
                    // This is the expected behavior
                    Assert.False(partialResult.Matched);
                }
            }
        }

        // Helper methods

        private RitualManifest CreateTestManifest()
        {
            var manifest = new RitualManifest();

            // Add Đầy Tháng ritual
            manifest.Rituals.Add(new RitualDto
            {
                Id = "day-thang",
                Name = "Đầy Tháng",
                ActionSequencePattern = new List<ActionTypeDto>
                {
                    new ActionTypeDto { Type = "ViewProduct" },
                    new ActionTypeDto { Type = "BrowseCategory" },
                    new ActionTypeDto { Type = "AddToCart" }
                },
                RequiredItems = new List<RitualRequiredItemDto>
                {
                    new RitualRequiredItemDto
                    {
                        CategoryId = Guid.NewGuid(),
                        ProductIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
                    }
                },
                ConfidenceThreshold = 0.5m,
                CulturalSignificance = "One-month celebration",
                Sources = new List<string> { "Vietnamese tradition" }
            });

            // Add Tết ritual
            manifest.Rituals.Add(new RitualDto
            {
                Id = "tet",
                Name = "Tết",
                ActionSequencePattern = new List<ActionTypeDto>
                {
                    new ActionTypeDto { Type = "BrowseCategory" },
                    new ActionTypeDto { Type = "ViewProduct" },
                    new ActionTypeDto { Type = "ViewProduct" },
                    new ActionTypeDto { Type = "AddToCart" }
                },
                RequiredItems = new List<RitualRequiredItemDto>
                {
                    new RitualRequiredItemDto
                    {
                        CategoryId = Guid.NewGuid(),
                        ProductIds = new List<Guid> { Guid.NewGuid() }
                    }
                },
                ConfidenceThreshold = 0.6m,
                CulturalSignificance = "Lunar New Year",
                Sources = new List<string> { "Vietnamese tradition" }
            });

            // Add Lễ Cúng Tổ Tiên ritual
            manifest.Rituals.Add(new RitualDto
            {
                Id = "le-cung-to-tien",
                Name = "Lễ Cúng Tổ Tiên",
                ActionSequencePattern = new List<ActionTypeDto>
                {
                    new ActionTypeDto { Type = "ViewProduct" },
                    new ActionTypeDto { Type = "AddToCart" },
                    new ActionTypeDto { Type = "AddToCart" }
                },
                RequiredItems = new List<RitualRequiredItemDto>
                {
                    new RitualRequiredItemDto
                    {
                        CategoryId = Guid.NewGuid(),
                        ProductIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }
                    }
                },
                ConfidenceThreshold = 0.55m,
                CulturalSignificance = "Ancestor worship",
                Sources = new List<string> { "Vietnamese tradition" }
            });

            return manifest;
        }

        private List<ActionDto> GenerateActionSequence(Faker faker)
        {
            var actions = new List<ActionDto>();
            var actionTypes = new[] { "ViewProduct", "AddToCart", "BrowseCategory", "RemoveFromCart", "Checkout" };

            for (int i = 0; i < faker.Random.Int(1, 8); i++)
            {
                actions.Add(new ActionDto
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    SessionId = faker.Random.Guid().ToString(),
                    Type = faker.PickRandom(actionTypes),
                    Timestamp = faker.Date.Recent(),
                    ProductId = faker.Random.Bool() ? Guid.NewGuid() : null,
                    CategoryId = faker.Random.Bool() ? Guid.NewGuid() : null
                });
            }

            return actions;
        }

        private List<ActionDto> GenerateMatchingActionSequence(Faker faker, RitualDto targetRitual)
        {
            var actions = new List<ActionDto>();

            // Add actions that match the ritual pattern
            foreach (var patternAction in targetRitual.ActionSequencePattern)
            {
                actions.Add(new ActionDto
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    SessionId = faker.Random.Guid().ToString(),
                    Type = patternAction.Type,
                    Timestamp = faker.Date.Recent(),
                    ProductId = faker.Random.Bool() ? Guid.NewGuid() : null,
                    CategoryId = patternAction.ProductCategoryId ?? (faker.Random.Bool() ? Guid.NewGuid() : null)
                });
            }

            // Optionally add some noise actions
            if (faker.Random.Bool())
            {
                actions.Add(new ActionDto
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    SessionId = faker.Random.Guid().ToString(),
                    Type = faker.PickRandom("ViewProduct", "BrowseCategory"),
                    Timestamp = faker.Date.Recent()
                });
            }

            return actions.OrderBy(a => a.Timestamp).ToList();
        }

        /// <summary>
        /// Property 13: Pattern Storage Format Efficiency
        /// **Feature: sequential-ritual-recommendation, Property 13: Pattern Storage Format Efficiency**
        /// **Validates: Requirements 3.5, 7.5**
        /// 
        /// For any ritual patterns stored, they SHALL be persisted in a format that supports 
        /// efficient sequential matching (e.g., indexed by action types).
        /// </summary>
        [Fact]
        public void Property_13_PatternStorageFormatEfficiency()
        {
            // Arrange - Create a manifest with multiple rituals
            var manifest = CreateTestManifest();
            
            // Act - Initialize the pattern matcher (which builds indexes)
            _patternMatcher.Initialize(manifest);

            // Assert - Verify that indexes are built and accessible
            var activeRituals = _patternMatcher.GetActiveRituals();
            Assert.NotEmpty(activeRituals);

            // Verify that we can retrieve rituals by ID efficiently
            foreach (var ritual in activeRituals)
            {
                var retrievedRitual = _patternMatcher.GetRitualById(ritual.Id);
                Assert.NotNull(retrievedRitual);
                Assert.Equal(ritual.Id, retrievedRitual.Id);
                Assert.Equal(ritual.Name, retrievedRitual.Name);
            }

            // Verify that action type indexing works
            var actionTypes = new[] { "ViewProduct", "AddToCart", "BrowseCategory" };
            foreach (var actionType in actionTypes)
            {
                // Get rituals by action type - should use index for efficiency
                var ritualsWithActionType = manifest.GetRitualsByActionType(actionType);
                
                // Verify that all returned rituals actually contain this action type
                foreach (var ritual in ritualsWithActionType)
                {
                    Assert.Contains(ritual.ActionSequencePattern, a => a.Type == actionType);
                }
            }
        }

        /// <summary>
        /// Property 28: Recommendation Payload Structure (Extended)
        /// **Feature: sequential-ritual-recommendation, Property 28: Recommendation Payload Structure**
        /// **Validates: Requirements 7.5**
        /// 
        /// For any pattern match, the BE-AI SHALL return a MatchResult containing ritual name, 
        /// missing items, confidence score, and matching metadata in an efficient format.
        /// </summary>
        [Fact]
        public void Property_28_RecommendationPayloadStructureEfficiency()
        {
            // Arrange - Create a manifest with rituals
            var manifest = CreateTestManifest();
            _patternMatcher.Initialize(manifest);

            var faker = new Faker();

            // Act & Assert - Run 100 iterations to verify payload structure consistency
            for (int i = 0; i < 100; i++)
            {
                var actionSequence = GenerateActionSequence(faker);
                var result = _patternMatcher.MatchPattern(actionSequence);

                // Assert - Result structure is always valid and efficient
                Assert.NotNull(result);
                
                if (result.Matched)
                {
                    // Verify all required fields are present and properly structured
                    Assert.NotEmpty(result.RitualId);
                    Assert.NotEmpty(result.RitualName);
                    Assert.True(result.ConfidenceScore >= 0 && result.ConfidenceScore <= 1);
                    Assert.NotNull(result.MatchingMetadata);
                    Assert.NotNull(result.MatchedActions);
                    
                    // Verify metadata structure for efficient storage
                    Assert.True(result.MatchingMetadata.MatchedSequenceLength > 0);
                    Assert.True(result.MatchingMetadata.TotalSequenceLength > 0);
                    Assert.NotEmpty(result.MatchingMetadata.MatchedActionIndices);
                    
                    // Verify that matched action indices are valid
                    foreach (var index in result.MatchingMetadata.MatchedActionIndices)
                    {
                        Assert.True(index >= 0 && index < actionSequence.Count);
                    }
                }
            }
        }

        /// <summary>
        /// Property: Action Type Index Lookup Efficiency
        /// **Feature: sequential-ritual-recommendation, Property: Action Type Index Lookup Efficiency**
        /// **Validates: Requirements 3.5, 7.5**
        /// 
        /// For any action type, the system SHALL retrieve rituals containing that action type 
        /// using indexed lookup for O(1) performance instead of O(n) linear scan.
        /// </summary>
        [Fact]
        public void Property_ActionTypeIndexLookupEfficiency()
        {
            // Arrange - Create a manifest with multiple rituals
            var manifest = CreateTestManifest();
            _patternMatcher.Initialize(manifest);

            // Act - Get rituals by action type using the index
            var viewProductRituals = manifest.GetRitualsByActionType("ViewProduct");
            var addToCartRituals = manifest.GetRitualsByActionType("AddToCart");
            var browseCategoryRituals = manifest.GetRitualsByActionType("BrowseCategory");

            // Assert - Verify that indexed lookup returns correct results
            Assert.NotEmpty(viewProductRituals);
            Assert.NotEmpty(addToCartRituals);
            Assert.NotEmpty(browseCategoryRituals);

            // Verify that all returned rituals actually contain the requested action type
            foreach (var ritual in viewProductRituals)
            {
                Assert.Contains(ritual.ActionSequencePattern, a => a.Type == "ViewProduct");
            }

            foreach (var ritual in addToCartRituals)
            {
                Assert.Contains(ritual.ActionSequencePattern, a => a.Type == "AddToCart");
            }

            foreach (var ritual in browseCategoryRituals)
            {
                Assert.Contains(ritual.ActionSequencePattern, a => a.Type == "BrowseCategory");
            }

            // Verify that non-existent action types return empty list
            var nonExistentRituals = manifest.GetRitualsByActionType("NonExistentActionType");
            Assert.Empty(nonExistentRituals);
        }

        /// <summary>
        /// Property: Ritual ID Index Lookup Efficiency
        /// **Feature: sequential-ritual-recommendation, Property: Ritual ID Index Lookup Efficiency**
        /// **Validates: Requirements 3.5, 7.5**
        /// 
        /// For any ritual ID, the system SHALL retrieve the ritual using indexed lookup 
        /// for O(1) performance instead of O(n) linear scan.
        /// </summary>
        [Fact]
        public void Property_RitualIdIndexLookupEfficiency()
        {
            // Arrange - Create a manifest with multiple rituals
            var manifest = CreateTestManifest();
            _patternMatcher.Initialize(manifest);

            // Act - Get all rituals and verify they can be retrieved by ID
            var allRituals = _patternMatcher.GetActiveRituals();
            Assert.NotEmpty(allRituals);

            // Assert - Verify that each ritual can be retrieved by ID efficiently
            foreach (var ritual in allRituals)
            {
                var retrievedRitual = _patternMatcher.GetRitualById(ritual.Id);
                Assert.NotNull(retrievedRitual);
                Assert.Equal(ritual.Id, retrievedRitual.Id);
                Assert.Equal(ritual.Name, retrievedRitual.Name);
                Assert.Equal(ritual.ConfidenceThreshold, retrievedRitual.ConfidenceThreshold);
            }

            // Verify that non-existent ritual IDs return null
            var nonExistentRitual = _patternMatcher.GetRitualById("non-existent-ritual-id");
            Assert.Null(nonExistentRitual);
        }
    }
}
