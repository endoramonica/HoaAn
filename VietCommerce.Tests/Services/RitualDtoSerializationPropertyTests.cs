using Bogus;
using System.Text.Json;
using Xunit;
using VietCommerce.Core.DTOs.Rituals;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Property-based tests for Ritual DTO serialization round-trip
    /// Validates: Requirements 7.1
    /// </summary>
    public class RitualDtoSerializationPropertyTests
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        /// <summary>
        /// Property 1: DTO Serialization Round Trip
        /// **Feature: sequential-ritual-recommendation, Property 1: DTO Serialization Round Trip**
        /// **Validates: Requirements 7.1**
        /// 
        /// For any valid RitualDto, serializing to JSON and then deserializing should produce 
        /// an equivalent object with all properties preserved.
        /// </summary>
        [Fact]
        public void Property_1_RitualDtoSerializationRoundTrip()
        {
            // Arrange - Generate random RitualDto instances
            var faker = new Faker<RitualDto>();
            faker
                .RuleFor(x => x.Id, f => f.Random.Guid().ToString())
                .RuleFor(x => x.Name, f => f.Lorem.Word())
                .RuleFor(x => x.ActionSequencePattern, f => GenerateActionTypeList(f))
                .RuleFor(x => x.RequiredItems, f => GenerateRequiredItemsList(f))
                .RuleFor(x => x.ConfidenceThreshold, f => f.Random.Decimal(0, 1))
                .RuleFor(x => x.CulturalSignificance, f => f.Lorem.Sentence())
                .RuleFor(x => x.Sources, f => f.Make(f.Random.Int(1, 3), () => f.Internet.Url()));

            // Act & Assert - Run 100 iterations
            for (int i = 0; i < 100; i++)
            {
                var originalDto = faker.Generate();

                // Serialize to JSON
                var json = JsonSerializer.Serialize(originalDto, _jsonOptions);

                // Deserialize back
                var deserializedDto = JsonSerializer.Deserialize<RitualDto>(json, _jsonOptions);

                // Assert - All properties should be preserved
                Assert.NotNull(deserializedDto);
                Assert.Equal(originalDto.Id, deserializedDto.Id);
                Assert.Equal(originalDto.Name, deserializedDto.Name);
                Assert.Equal(originalDto.ConfidenceThreshold, deserializedDto.ConfidenceThreshold);
                Assert.Equal(originalDto.CulturalSignificance, deserializedDto.CulturalSignificance);
                Assert.Equal(originalDto.Sources.Count, deserializedDto.Sources?.Count ?? 0);
                Assert.Equal(originalDto.ActionSequencePattern.Count, deserializedDto.ActionSequencePattern?.Count ?? 0);
                Assert.Equal(originalDto.RequiredItems.Count, deserializedDto.RequiredItems?.Count ?? 0);
            }
        }

        /// <summary>
        /// Property 2: ActionDto Serialization Round Trip
        /// **Feature: sequential-ritual-recommendation, Property 1: DTO Serialization Round Trip**
        /// **Validates: Requirements 7.1**
        /// 
        /// For any valid ActionDto, serializing to JSON and then deserializing should produce 
        /// an equivalent object with all properties preserved.
        /// </summary>
        [Fact]
        public void Property_2_ActionDtoSerializationRoundTrip()
        {
            // Arrange - Generate random ActionDto instances
            var faker = new Faker<ActionDto>();
            faker
                .RuleFor(x => x.Id, f => Guid.NewGuid())
                .RuleFor(x => x.UserId, f => Guid.NewGuid())
                .RuleFor(x => x.SessionId, f => f.Random.Guid().ToString())
                .RuleFor(x => x.Type, f => f.PickRandom("ViewProduct", "AddToCart", "BrowseCategory"))
                .RuleFor(x => x.Timestamp, f => f.Date.Recent())
                .RuleFor(x => x.ProductId, f => f.Random.Bool() ? Guid.NewGuid() : null)
                .RuleFor(x => x.CategoryId, f => f.Random.Bool() ? Guid.NewGuid() : null)
                .RuleFor(x => x.Metadata, f => GenerateMetadata(f));

            // Act & Assert - Run 100 iterations
            for (int i = 0; i < 100; i++)
            {
                var originalDto = faker.Generate();

                // Serialize to JSON
                var json = JsonSerializer.Serialize(originalDto, _jsonOptions);

                // Deserialize back
                var deserializedDto = JsonSerializer.Deserialize<ActionDto>(json, _jsonOptions);

                // Assert - All properties should be preserved
                Assert.NotNull(deserializedDto);
                Assert.Equal(originalDto.Id, deserializedDto.Id);
                Assert.Equal(originalDto.UserId, deserializedDto.UserId);
                Assert.Equal(originalDto.SessionId, deserializedDto.SessionId);
                Assert.Equal(originalDto.Type, deserializedDto.Type);
                Assert.Equal(originalDto.ProductId, deserializedDto.ProductId);
                Assert.Equal(originalDto.CategoryId, deserializedDto.CategoryId);
            }
        }

        /// <summary>
        /// Property 3: RecommendationPayloadDto Serialization Round Trip
        /// **Feature: sequential-ritual-recommendation, Property 1: DTO Serialization Round Trip**
        /// **Validates: Requirements 7.1**
        /// 
        /// For any valid RecommendationPayloadDto, serializing to JSON and then deserializing 
        /// should produce an equivalent object with all properties preserved.
        /// </summary>
        [Fact]
        public void Property_3_RecommendationPayloadDtoSerializationRoundTrip()
        {
            // Arrange - Generate random RecommendationPayloadDto instances
            var faker = new Faker<RecommendationPayloadDto>();
            faker
                .RuleFor(x => x.RitualId, f => f.Random.Guid().ToString())
                .RuleFor(x => x.RitualName, f => f.Lorem.Word())
                .RuleFor(x => x.ConfidenceScore, f => f.Random.Decimal(0, 1))
                .RuleFor(x => x.MissingItems, f => f.Make(f.Random.Int(0, 5), () => Guid.NewGuid()))
                .RuleFor(x => x.MatchingMetadata, f => GenerateMatchingMetadata(f))
                .RuleFor(x => x.SystemReport, f => GenerateSystemReport(f));

            // Act & Assert - Run 100 iterations
            for (int i = 0; i < 100; i++)
            {
                var originalDto = faker.Generate();

                // Serialize to JSON
                var json = JsonSerializer.Serialize(originalDto, _jsonOptions);

                // Deserialize back
                var deserializedDto = JsonSerializer.Deserialize<RecommendationPayloadDto>(json, _jsonOptions);

                // Assert - All properties should be preserved
                Assert.NotNull(deserializedDto);
                Assert.Equal(originalDto.RitualId, deserializedDto.RitualId);
                Assert.Equal(originalDto.RitualName, deserializedDto.RitualName);
                Assert.Equal(originalDto.ConfidenceScore, deserializedDto.ConfidenceScore);
                Assert.Equal(originalDto.MissingItems.Count, deserializedDto.MissingItems.Count);
            }
        }

        /// <summary>
        /// Property 4: ExplanationPayloadDto Serialization Round Trip
        /// **Feature: sequential-ritual-recommendation, Property 1: DTO Serialization Round Trip**
        /// **Validates: Requirements 7.1**
        /// 
        /// For any valid ExplanationPayloadDto, serializing to JSON and then deserializing 
        /// should produce an equivalent object with all properties preserved.
        /// </summary>
        [Fact]
        public void Property_4_ExplanationPayloadDtoSerializationRoundTrip()
        {
            // Arrange - Generate random ExplanationPayloadDto instances
            var faker = new Faker<ExplanationPayloadDto>();
            faker
                .RuleFor(x => x.RitualName, f => f.Lorem.Word())
                .RuleFor(x => x.CulturalContext, f => f.Lorem.Sentence())
                .RuleFor(x => x.ItemExplanations, f => GenerateItemExplanations(f))
                .RuleFor(x => x.Sources, f => f.Make(f.Random.Int(1, 3), () => f.Internet.Url()))
                .RuleFor(x => x.GeneratedBy, f => f.PickRandom("gemini", "fallback"));

            // Act & Assert - Run 100 iterations
            for (int i = 0; i < 100; i++)
            {
                var originalDto = faker.Generate();

                // Serialize to JSON
                var json = JsonSerializer.Serialize(originalDto, _jsonOptions);

                // Deserialize back
                var deserializedDto = JsonSerializer.Deserialize<ExplanationPayloadDto>(json, _jsonOptions);

                // Assert - All properties should be preserved
                Assert.NotNull(deserializedDto);
                Assert.Equal(originalDto.RitualName, deserializedDto.RitualName);
                Assert.Equal(originalDto.CulturalContext, deserializedDto.CulturalContext);
                Assert.Equal(originalDto.GeneratedBy, deserializedDto.GeneratedBy);
                Assert.Equal(originalDto.ItemExplanations.Count, deserializedDto.ItemExplanations.Count);
                Assert.Equal(originalDto.Sources.Count, deserializedDto.Sources.Count);
            }
        }

        // Helper methods for generating test data

        private List<ActionTypeDto> GenerateActionTypeList(Faker faker)
        {
            return faker.Make(faker.Random.Int(1, 5), () => new ActionTypeDto
            {
                Type = faker.PickRandom("ViewProduct", "AddToCart", "BrowseCategory"),
                ProductCategoryId = faker.Random.Bool() ? Guid.NewGuid() : null,
                Metadata = GenerateMetadata(faker)
            }).ToList();
        }

        private List<RitualRequiredItemDto> GenerateRequiredItemsList(Faker faker)
        {
            return faker.Make(faker.Random.Int(1, 3), () => new RitualRequiredItemDto
            {
                CategoryId = Guid.NewGuid(),
                ProductIds = faker.Make(faker.Random.Int(1, 3), () => Guid.NewGuid()).ToList()
            }).ToList();
        }

        private Dictionary<string, object>? GenerateMetadata(Faker faker)
        {
            if (faker.Random.Bool())
            {
                return new Dictionary<string, object>
                {
                    { "key1", faker.Lorem.Word() },
                    { "key2", faker.Random.Int() }
                };
            }
            return null;
        }

        private MatchingMetadataDto GenerateMatchingMetadata(Faker faker)
        {
            return new MatchingMetadataDto
            {
                MatchedSequenceLength = faker.Random.Int(1, 10),
                TotalSequenceLength = faker.Random.Int(5, 20),
                MatchedActionIndices = faker.Make(faker.Random.Int(1, 5), () => faker.Random.Int(0, 20)).ToList()
            };
        }

        private SystemReportDto GenerateSystemReport(Faker faker)
        {
            return new SystemReportDto
            {
                MatchedPattern = GenerateActionTypeList(faker),
                MatchingSteps = faker.Make(faker.Random.Int(1, 3), () => faker.Lorem.Sentence()).ToList(),
                ReasonsForMissingItems = new Dictionary<Guid, string>
                {
                    { Guid.NewGuid(), faker.Lorem.Sentence() },
                    { Guid.NewGuid(), faker.Lorem.Sentence() }
                }
            };
        }

        private List<ItemExplanationDto> GenerateItemExplanations(Faker faker)
        {
            return faker.Make(faker.Random.Int(1, 3), () => new ItemExplanationDto
            {
                ProductId = Guid.NewGuid(),
                ProductName = faker.Commerce.ProductName(),
                WhyNeeded = faker.Lorem.Sentence(),
                TraditionalUsage = faker.Lorem.Sentence()
            }).ToList();
        }
    }
}
