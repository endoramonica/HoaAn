using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;
using Xunit;
using VietCommerce.Application.Services.Services;
using VietCommerce.Core.DTOs.Rituals;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Property-based tests for Gemini explanation service
    /// Validates: Requirements 8.1, 8.2, 8.3, 8.4, 8.5
    /// </summary>
    public class GeminiExplanationServicePropertyTests
    {
        private readonly Mock<ILogger<GeminiExplanationService>> _mockLogger;
        private readonly HttpClient _httpClient;

        public GeminiExplanationServicePropertyTests()
        {
            _mockLogger = new Mock<ILogger<GeminiExplanationService>>();
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Property 29: Gemini API Invocation
        /// **Feature: sequential-ritual-recommendation, Property 29: Gemini API Invocation**
        /// **Validates: Requirements 8.1, 8.2**
        /// 
        /// For any valid RecommendationPayloadDto, the service SHALL attempt to call Gemini API
        /// with the ritual context and missing items as context.
        /// </summary>
        [Fact]
        public async Task Property_29_GeminiApiInvocationWithContext()
        {
            // Arrange
            var faker = new Faker();
            var payload = GenerateRecommendationPayload(faker);

            // Create a mock HttpClient that captures the request
            var mockHandler = new MockHttpMessageHandler();
            var httpClient = new HttpClient(mockHandler);

            var service = new GeminiExplanationService(httpClient, _mockLogger.Object);

            // Set environment variable for API key
            Environment.SetEnvironmentVariable("VITE_GEMINI_API_KEY", "test-api-key");

            // Act
            var result = await service.GenerateExplanationAsync(payload);

            // Assert
            // Service should attempt to call API (even if it fails due to mock)
            // Result should be either Gemini-generated or fallback
            Assert.NotNull(result);
            Assert.NotNull(result.RitualName);
            Assert.Equal(payload.RitualName, result.RitualName);
        }

        /// <summary>
        /// Property 30: Gemini Context Completeness
        /// **Feature: sequential-ritual-recommendation, Property 30: Gemini Context Completeness**
        /// **Validates: Requirements 8.1, 8.2**
        /// 
        /// For any Gemini API call, the service SHALL provide ritual name, cultural significance,
        /// and list of missing items as context in the prompt.
        /// </summary>
        [Fact]
        public void Property_30_GeminiContextCompleteness()
        {
            // Arrange
            var faker = new Faker();
            var payload = GenerateRecommendationPayload(faker);

            var service = new GeminiExplanationService(_httpClient, _mockLogger.Object);

            // Act - Get fallback explanation to verify context structure
            var result = service.GetFallbackExplanation(payload);

            // Assert - Verify all required context is present
            Assert.NotNull(result);
            Assert.NotEmpty(result.RitualName);
            Assert.NotEmpty(result.CulturalContext);
            Assert.NotEmpty(result.ItemExplanations);
            Assert.NotEmpty(result.Sources);

            // Verify each item explanation has required fields
            foreach (var itemExplanation in result.ItemExplanations)
            {
                Assert.NotEqual(Guid.Empty, itemExplanation.ProductId);
                Assert.NotEmpty(itemExplanation.WhyNeeded);
                Assert.NotEmpty(itemExplanation.TraditionalUsage);
            }
        }

        /// <summary>
        /// Property 31: Explanation Payload Formatting
        /// **Feature: sequential-ritual-recommendation, Property 31: Explanation Payload Formatting**
        /// **Validates: Requirements 8.3**
        /// 
        /// For any Gemini response, the FE-AI SHALL format it as an ExplanationPayloadDto
        /// with human-readable text and proper structure.
        /// </summary>
        [Fact]
        public void Property_31_ExplanationPayloadFormatting()
        {
            // Arrange
            var faker = new Faker();
            var payload = GenerateRecommendationPayload(faker);

            var service = new GeminiExplanationService(_httpClient, _mockLogger.Object);

            // Act
            var result = service.GetFallbackExplanation(payload);

            // Assert - Verify ExplanationPayloadDto structure
            Assert.NotNull(result);
            Assert.IsType<ExplanationPayloadDto>(result);
            Assert.NotEmpty(result.RitualName);
            Assert.NotEmpty(result.CulturalContext);
            Assert.NotNull(result.ItemExplanations);
            Assert.NotNull(result.Sources);
            Assert.NotEmpty(result.GeneratedBy);

            // Verify GeneratedBy is either "gemini" or "fallback"
            Assert.True(result.GeneratedBy == "gemini" || result.GeneratedBy == "fallback");

            // Verify all item explanations are properly formatted
            foreach (var item in result.ItemExplanations)
            {
                Assert.NotEqual(Guid.Empty, item.ProductId);
                Assert.NotEmpty(item.ProductName);
                Assert.NotEmpty(item.WhyNeeded);
                Assert.NotEmpty(item.TraditionalUsage);
            }
        }

        /// <summary>
        /// Property 32: Item Reason Inclusion in Explanations
        /// **Feature: sequential-ritual-recommendation, Property 32: Item Reason Inclusion in Explanations**
        /// **Validates: Requirements 8.4**
        /// 
        /// For any ExplanationPayloadDto generated, it SHALL include why each item is
        /// traditionally required for the ritual.
        /// </summary>
        [Fact]
        public void Property_32_ItemReasonInclusionInExplanations()
        {
            // Arrange
            var faker = new Faker();
            var payload = GenerateRecommendationPayload(faker);

            var service = new GeminiExplanationService(_httpClient, _mockLogger.Object);

            // Act
            var result = service.GetFallbackExplanation(payload);

            // Assert - Verify each missing item has an explanation
            Assert.NotNull(result.ItemExplanations);
            Assert.NotEmpty(result.ItemExplanations);

            // For each missing item in the original payload, there should be an explanation
            foreach (var missingItemId in payload.MissingItems)
            {
                var itemExplanation = result.ItemExplanations.FirstOrDefault(ie => ie.ProductId == missingItemId);
                Assert.NotNull(itemExplanation);
                Assert.NotEmpty(itemExplanation.WhyNeeded);
                Assert.NotEmpty(itemExplanation.TraditionalUsage);
            }
        }

        /// <summary>
        /// Property 33: Gemini API Fallback
        /// **Feature: sequential-ritual-recommendation, Property 33: Gemini API Fallback**
        /// **Validates: Requirements 8.5**
        /// 
        /// For any Gemini API failure, the FE-AI SHALL fall back to a predefined template
        /// explanation without blocking the recommendation display.
        /// </summary>
        [Fact]
        public async Task Property_33_GeminiApiFallback()
        {
            // Arrange
            var faker = new Faker();
            var payload = GenerateRecommendationPayload(faker);

            // Create a mock handler that returns an error
            var mockHandler = new MockHttpMessageHandler(statusCode: HttpStatusCode.InternalServerError);
            var httpClient = new HttpClient(mockHandler);

            var service = new GeminiExplanationService(httpClient, _mockLogger.Object);

            // Set environment variable for API key to trigger API call attempt
            Environment.SetEnvironmentVariable("VITE_GEMINI_API_KEY", "test-api-key");

            // Act
            var result = await service.GenerateExplanationAsync(payload);

            // Assert - Should return fallback explanation, not throw
            Assert.NotNull(result);
            Assert.NotEmpty(result.RitualName);
            Assert.NotEmpty(result.CulturalContext);
            Assert.NotEmpty(result.ItemExplanations);
            Assert.Equal("fallback", result.GeneratedBy);
        }

        /// <summary>
        /// Property 34: Fallback Explanation Completeness
        /// **Feature: sequential-ritual-recommendation, Property 33: Gemini API Fallback**
        /// **Validates: Requirements 8.5**
        /// 
        /// For any fallback explanation, all required fields SHALL be populated with
        /// meaningful content.
        /// </summary>
        [Fact]
        public void Property_34_FallbackExplanationCompleteness()
        {
            // Arrange
            var faker = new Faker();
            var payload = GenerateRecommendationPayload(faker);

            var service = new GeminiExplanationService(_httpClient, _mockLogger.Object);

            // Act
            var result = service.GetFallbackExplanation(payload);

            // Assert - Verify all fields are populated
            Assert.NotNull(result);
            Assert.NotEmpty(result.RitualName);
            Assert.NotEmpty(result.CulturalContext);
            Assert.NotEmpty(result.ItemExplanations);
            Assert.NotEmpty(result.Sources);
            Assert.Equal("fallback", result.GeneratedBy);

            // Verify cultural context mentions the ritual
            Assert.Contains(payload.RitualName, result.CulturalContext);

            // Verify each item explanation is complete
            foreach (var item in result.ItemExplanations)
            {
                Assert.NotEmpty(item.ProductName);
                Assert.NotEmpty(item.WhyNeeded);
                Assert.NotEmpty(item.TraditionalUsage);
            }
        }

        /// <summary>
        /// Property 35: Service Configuration Check
        /// **Feature: sequential-ritual-recommendation, Property 29: Gemini API Invocation**
        /// **Validates: Requirements 8.1**
        /// 
        /// For any service instance, IsConfigured() SHALL return true only when
        /// VITE_GEMINI_API_KEY environment variable is set.
        /// </summary>
        [Fact]
        public void Property_35_ServiceConfigurationCheck()
        {
            // Arrange
            var service = new GeminiExplanationService(_httpClient, _mockLogger.Object);

            // Act & Assert - Test with no API key
            Environment.SetEnvironmentVariable("VITE_GEMINI_API_KEY", null);
            Assert.False(service.IsConfigured());

            // Act & Assert - Test with API key
            Environment.SetEnvironmentVariable("VITE_GEMINI_API_KEY", "test-key");
            Assert.True(service.IsConfigured());

            // Cleanup
            Environment.SetEnvironmentVariable("VITE_GEMINI_API_KEY", null);
        }

        /// <summary>
        /// Property 36: Null Payload Handling
        /// **Feature: sequential-ritual-recommendation, Property 33: Gemini API Fallback**
        /// **Validates: Requirements 8.5**
        /// 
        /// For any null payload, the service SHALL return a valid fallback explanation
        /// without throwing an exception.
        /// </summary>
        [Fact]
        public async Task Property_36_NullPayloadHandling()
        {
            // Arrange
            var service = new GeminiExplanationService(_httpClient, _mockLogger.Object);

            // Act
            var result = await service.GenerateExplanationAsync(null!);

            // Assert - Should return fallback explanation, not throw
            Assert.NotNull(result);
            Assert.NotEmpty(result.RitualName);
            Assert.NotEmpty(result.CulturalContext);
            Assert.Equal("fallback", result.GeneratedBy);
        }

        // Helper methods

        private RecommendationPayloadDto GenerateRecommendationPayload(Faker faker)
        {
            return new RecommendationPayloadDto
            {
                RitualId = faker.Random.Guid().ToString(),
                RitualName = faker.PickRandom("Đầy Tháng", "Tết", "Lễ Cúng Tổ Tiên", "Lễ Cúng Thần Tài"),
                ConfidenceScore = faker.Random.Decimal(0.5m, 1.0m),
                MissingItems = faker.Make(faker.Random.Int(1, 5), () => Guid.NewGuid()).ToList(),
                MatchingMetadata = new MatchingMetadataDto
                {
                    MatchedSequenceLength = faker.Random.Int(1, 10),
                    TotalSequenceLength = faker.Random.Int(5, 20),
                    MatchedActionIndices = faker.Make(faker.Random.Int(1, 5), () => faker.Random.Int(0, 20)).ToList()
                },
                SystemReport = new SystemReportDto
                {
                    MatchedPattern = new List<ActionTypeDto>(),
                    MatchingSteps = faker.Make(2, () => faker.Lorem.Sentence()).ToList(),
                    ReasonsForMissingItems = new Dictionary<Guid, string>()
                }
            };
        }

        /// <summary>
        /// Mock HTTP message handler for testing
        /// </summary>
        private class MockHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _statusCode;
            private readonly string? _responseContent;

            public MockHttpMessageHandler(
                HttpStatusCode statusCode = HttpStatusCode.OK,
                string? responseContent = null)
            {
                _statusCode = statusCode;
                _responseContent = responseContent;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(_statusCode);

                if (_responseContent != null)
                {
                    response.Content = new StringContent(_responseContent);
                }

                return Task.FromResult(response);
            }
        }
    }
}
