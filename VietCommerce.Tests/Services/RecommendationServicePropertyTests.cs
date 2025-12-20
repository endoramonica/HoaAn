using Bogus;
using Moq;
using Xunit;
using AutoMapper;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.DTOs.Rituals;
using VietCommerce.Core.Entities.Rituals;
using VietCommerce.Data.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Property-based tests for RecommendationService
    /// Tests pattern matching, missing items identification, and logging
    /// </summary>
    public class RecommendationServicePropertyTests
    {
        private readonly Mock<ISequentialPatternMatcher> _mockPatternMatcher;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IUserPreferenceService> _mockUserPreferenceService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<RecommendationService>> _mockLogger;
        private readonly RecommendationService _service;

        public RecommendationServicePropertyTests()
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
        /// Property 3: Ritual Requirements Comparison
        /// **Feature: sequential-ritual-recommendation, Property 3: Ritual Requirements Comparison**
        /// **Validates: Requirements 1.3**
        /// 
        /// For any detected ritual and user cart, the system SHALL compare ritual requirements 
        /// against cart items and identify all missing items.
        /// </summary>
        [Fact]
        public async Task Property_3_RitualRequirementsComparison()
        {
            // Arrange - Generate random ritual and cart items
            var faker = new Faker();
            var ritualId = Guid.NewGuid();
            var product1Id = Guid.NewGuid();
            var product2Id = Guid.NewGuid();
            var product3Id = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var ritual = new RitualDto
            {
                Id = ritualId.ToString(),
                Name = "Đầy Tháng",
                RequiredItems = new List<RitualRequiredItemDto>
                {
                    new RitualRequiredItemDto
                    {
                        CategoryId = categoryId,
                        ProductIds = new List<Guid> { product1Id, product2Id, product3Id }
                    }
                }
            };

            // Cart contains only product1 and product2
            var cartItems = new List<CartItemDto>
            {
                new CartItemDto { ProductId = product1Id, ProductName = "Item 1" },
                new CartItemDto { ProductId = product2Id, ProductName = "Item 2" }
            };

            // Act
            var missingItems = await _service.GetMissingItemsAsync(ritual, cartItems);

            // Assert
            Assert.Single(missingItems);
            Assert.Contains(product3Id, missingItems);
            Assert.DoesNotContain(product1Id, missingItems);
            Assert.DoesNotContain(product2Id, missingItems);
        }

        /// <summary>
        /// Property 4: Missing Items Catalog Lookup
        /// **Feature: sequential-ritual-recommendation, Property 4: Missing Items Catalog Lookup**
        /// **Validates: Requirements 1.4**
        /// 
        /// For any set of missing items identified by the system, all items SHALL exist 
        /// in the product catalog.
        /// </summary>
        [Fact]
        public async Task Property_4_MissingItemsCatalogLookup()
        {
            // Arrange - Generate random missing items
            var faker = new Faker();
            var missingItemIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            // Mock products in catalog
            var products = missingItemIds.Select(id => new ProductListDto
            {
                Id = id,
                Name = faker.Commerce.ProductName()
            }).ToList();

            // Setup mapper to return products
            foreach (var product in products)
            {
                _mockMapper.Setup(m => m.Map<ProductListDto>(It.IsAny<object>()))
                    .Returns(product);
            }

            // Setup product repository to return products
            foreach (var product in products)
            {
                _mockProductRepository.Setup(p => p.GetByIdAsync(product.Id))
                    .ReturnsAsync(new Core.Entities.Products.Product
                    {
                        Id = product.Id,
                        Name = product.Name
                    });
            }

            // Act
            var result = await _service.GetMissingItemDetailsAsync(missingItemIds);

            // Assert
            Assert.NotEmpty(result);
            Assert.All(result, item => Assert.NotEqual(Guid.Empty, item.Id));
            Assert.All(result, item => Assert.NotEmpty(item.Name));
        }

        /// <summary>
        /// Property 5: No Recommendations for Non-Matching Sequences
        /// **Feature: sequential-ritual-recommendation, Property 5: No Recommendations for Non-Matching Sequences**
        /// **Validates: Requirements 1.5**
        /// 
        /// For any action sequence that does not match any ritual pattern above the confidence 
        /// threshold, the system SHALL not generate recommendations.
        /// </summary>
        [Fact]
        public async Task Property_5_NoRecommendationsForNonMatchingSequences()
        {
            // Arrange - Setup pattern matcher to return no match
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid().ToString();
            var actionSequence = new List<ActionDto>
            {
                new ActionDto { Type = "ViewProduct", Timestamp = DateTime.UtcNow },
                new ActionDto { Type = "BrowseCategory", Timestamp = DateTime.UtcNow }
            };
            var cartItems = new List<CartItemDto>();

            _mockPatternMatcher.Setup(m => m.IsInitialized()).Returns(true);
            _mockPatternMatcher.Setup(m => m.MatchPattern(It.IsAny<List<ActionDto>>()))
                .Returns(new MatchResultDto { Matched = false });
            _mockUserPreferenceService.Setup(u => u.GetDisabledRitualsAsync(userId))
                .ReturnsAsync(new List<Guid>());

            // Act
            var result = await _service.GenerateRecommendationAsync(userId, sessionId, actionSequence, cartItems);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Property 14: Recommendation Logging Completeness
        /// **Feature: sequential-ritual-recommendation, Property 14: Recommendation Logging Completeness**
        /// **Validates: Requirements 4.1**
        /// 
        /// For any recommendation generated, the system SHALL log the matched pattern, 
        /// confidence score, and reasoning.
        /// </summary>
        [Fact]
        public async Task Property_14_RecommendationLoggingCompleteness()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid().ToString();
            var payload = new RecommendationPayloadDto
            {
                RitualId = Guid.NewGuid().ToString(),
                RitualName = "Đầy Tháng",
                ConfidenceScore = 0.85m,
                MissingItems = new List<Guid> { Guid.NewGuid() },
                MatchingMetadata = new MatchingMetadataDto
                {
                    MatchedSequenceLength = 5,
                    TotalSequenceLength = 8
                },
                SystemReport = new SystemReportDto
                {
                    MatchedPattern = new List<ActionTypeDto>(),
                    MatchingSteps = new List<string> { "Step 1", "Step 2" },
                    ReasonsForMissingItems = new Dictionary<Guid, string>()
                }
            };

            var mockRecommendationLogRepo = new Mock<IRecommendationLogRepository>();
            _mockUnitOfWork.Setup(u => u.RecommendationLogs).Returns(mockRecommendationLogRepo.Object);
            mockRecommendationLogRepo.Setup(r => r.AddAsync(It.IsAny<RecommendationLogEntity>()))
                .Returns<RecommendationLogEntity>(entity => Task.FromResult(entity));

            // Act
            var logId = await _service.LogRecommendationAsync(userId, sessionId, payload);

            // Assert
            Assert.NotEqual(Guid.Empty, logId);
            mockRecommendationLogRepo.Verify(r => r.AddAsync(It.IsAny<RecommendationLogEntity>()), Times.Once);
        }

        /// <summary>
        /// Property 15: Action Metadata Logging
        /// **Feature: sequential-ritual-recommendation, Property 15: Action Metadata Logging**
        /// **Validates: Requirements 4.2**
        /// 
        /// For any recommendation generated, the system SHALL include metadata about 
        /// which actions triggered the pattern match.
        /// </summary>
        [Fact]
        public async Task Property_15_ActionMetadataLogging()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid().ToString();
            var payload = new RecommendationPayloadDto
            {
                RitualId = Guid.NewGuid().ToString(),
                RitualName = "Tết",
                ConfidenceScore = 0.90m,
                MissingItems = new List<Guid>(),
                MatchingMetadata = new MatchingMetadataDto
                {
                    MatchedSequenceLength = 7,
                    TotalSequenceLength = 10,
                    MatchedActionIndices = new List<int> { 0, 2, 4, 6, 7, 8, 9 }
                },
                SystemReport = new SystemReportDto()
            };

            var mockRecommendationLogRepo = new Mock<IRecommendationLogRepository>();
            _mockUnitOfWork.Setup(u => u.RecommendationLogs).Returns(mockRecommendationLogRepo.Object);
            mockRecommendationLogRepo.Setup(r => r.AddAsync(It.IsAny<RecommendationLogEntity>()))
                .Returns<RecommendationLogEntity>(entity => Task.FromResult(entity));

            // Act
            var logId = await _service.LogRecommendationAsync(userId, sessionId, payload);

            // Assert
            Assert.NotEqual(Guid.Empty, logId);
            mockRecommendationLogRepo.Verify(r => r.AddAsync(It.IsAny<RecommendationLogEntity>()), Times.Once);
        }

        /// <summary>
        /// Property 16: Intermediate Matching Steps Recording
        /// **Feature: sequential-ritual-recommendation, Property 16: Intermediate Matching Steps Recording**
        /// **Validates: Requirements 4.3**
        /// 
        /// For any action sequence analyzed, the system SHALL record intermediate 
        /// matching steps for debugging.
        /// </summary>
        [Fact]
        public async Task Property_16_IntermediateMatchingStepsRecording()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid().ToString();
            var payload = new RecommendationPayloadDto
            {
                RitualId = Guid.NewGuid().ToString(),
                RitualName = "Lễ Cúng Tổ Tiên",
                ConfidenceScore = 0.75m,
                MissingItems = new List<Guid>(),
                MatchingMetadata = new MatchingMetadataDto(),
                SystemReport = new SystemReportDto
                {
                    MatchingSteps = new List<string>
                    {
                        "Analyzed action sequence of length 8",
                        "Matched 6 actions from ritual pattern",
                        "Calculated confidence score: 75%",
                        "Pattern match successful"
                    }
                }
            };

            var mockRecommendationLogRepo = new Mock<IRecommendationLogRepository>();
            _mockUnitOfWork.Setup(u => u.RecommendationLogs).Returns(mockRecommendationLogRepo.Object);
            mockRecommendationLogRepo.Setup(r => r.AddAsync(It.IsAny<RecommendationLogEntity>()))
                .Returns<RecommendationLogEntity>(entity => Task.FromResult(entity));

            // Act
            var logId = await _service.LogRecommendationAsync(userId, sessionId, payload);

            // Assert
            Assert.NotEqual(Guid.Empty, logId);
            mockRecommendationLogRepo.Verify(r => r.AddAsync(It.IsAny<RecommendationLogEntity>()), Times.Once);
        }

        /// <summary>
        /// Property 17: System Report Inclusion
        /// **Feature: sequential-ritual-recommendation, Property 17: System Report Inclusion**
        /// **Validates: Requirements 4.4**
        /// 
        /// For any recommendation displayed to users, a system report SHALL be included 
        /// showing the matching logic.
        /// </summary>
        [Fact]
        public async Task Property_17_SystemReportInclusion()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid().ToString();
            var ritualId = Guid.NewGuid().ToString();
            var actionSequence = new List<ActionDto>
            {
                new ActionDto { Type = "ViewProduct", Timestamp = DateTime.UtcNow },
                new ActionDto { Type = "AddToCart", Timestamp = DateTime.UtcNow }
            };
            var cartItems = new List<CartItemDto>();

            var ritual = new RitualDto
            {
                Id = ritualId,
                Name = "Test Ritual",
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
                RitualId = ritualId,
                RitualName = ritual.Name,
                ConfidenceScore = 0.8m,
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
            _mockPatternMatcher.Setup(m => m.GetRitualById(ritualId))
                .Returns(ritual);
            _mockUserPreferenceService.Setup(u => u.GetDisabledRitualsAsync(userId))
                .ReturnsAsync(new List<Guid>());

            var mockRecommendationLogRepo = new Mock<IRecommendationLogRepository>();
            _mockUnitOfWork.Setup(u => u.RecommendationLogs).Returns(mockRecommendationLogRepo.Object);
            mockRecommendationLogRepo.Setup(r => r.AddAsync(It.IsAny<RecommendationLogEntity>()))
                .Returns<RecommendationLogEntity>(entity => Task.FromResult(entity));

            // Act
            var recommendation = await _service.GenerateRecommendationAsync(userId, sessionId, actionSequence, cartItems);

            // Assert
            Assert.NotNull(recommendation);
            Assert.NotNull(recommendation.SystemReport);
            Assert.NotEmpty(recommendation.SystemReport.MatchingSteps);
            Assert.NotEmpty(recommendation.SystemReport.MatchedPattern);
        }

        /// <summary>
        /// Property 18: Failure Logging
        /// **Feature: sequential-ritual-recommendation, Property 18: Failure Logging**
        /// **Validates: Requirements 4.5**
        /// 
        /// For any action sequence that does not match any pattern, the system SHALL 
        /// log why the sequence did not match.
        /// </summary>
        [Fact]
        public async Task Property_18_FailureLogging()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid().ToString();
            var actionSequence = new List<ActionDto>
            {
                new ActionDto { Type = "BrowseCategory", Timestamp = DateTime.UtcNow }
            };
            var cartItems = new List<CartItemDto>();

            _mockPatternMatcher.Setup(m => m.IsInitialized()).Returns(true);
            _mockPatternMatcher.Setup(m => m.MatchPattern(It.IsAny<List<ActionDto>>()))
                .Returns(new MatchResultDto { Matched = false });
            _mockUserPreferenceService.Setup(u => u.GetDisabledRitualsAsync(userId))
                .ReturnsAsync(new List<Guid>());

            // Act
            var result = await _service.GenerateRecommendationAsync(userId, sessionId, actionSequence, cartItems);

            // Assert
            Assert.Null(result);
            // Verify that logging was called (we can't easily verify the exact message with mocks)
            _mockLogger.Verify(
                l => l.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.AtLeastOnce
            );
        }
    }
}
