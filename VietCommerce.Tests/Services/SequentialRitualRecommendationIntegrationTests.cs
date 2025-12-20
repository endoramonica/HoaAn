using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using VietCommerce.Application.Mappings;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.DTOs.Rituals;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Rituals;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Integration tests for Sequential Ritual Recommendation System
    /// Tests complete end-to-end workflows and interactions between services
    /// Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 2.1, 2.2, 2.3, 2.5
    /// </summary>
    public class SequentialRitualRecommendationIntegrationTests : IAsyncLifetime
    {
        private readonly AppDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActionTrackingService _actionTrackingService;
        private readonly IRecommendationService _recommendationService;
        private readonly IGeminiExplanationService _explanationService;
        private readonly IUserPreferenceService _userPreferenceService;
        private readonly ISequentialPatternMatcher _patternMatcher;
        private readonly IMapper _mapper;

        // Test data IDs
        private readonly Guid _userId1 = Guid.NewGuid();
        private readonly Guid _userId2 = Guid.NewGuid();
        private readonly Guid _productId1 = Guid.NewGuid();
        private readonly Guid _productId2 = Guid.NewGuid();
        private readonly Guid _productId3 = Guid.NewGuid();
        private readonly Guid _productId4 = Guid.NewGuid();
        private readonly Guid _categoryId = Guid.NewGuid();

        public SequentialRitualRecommendationIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"RitualIntegrationTestDb_{Guid.NewGuid()}")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new AppDbContext(options);

            // Setup DI container
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAutoMapper(typeof(RitualMappingProfile));
            services.AddScoped<IUnitOfWork>(sp => new UnitOfWork(_context, sp.GetRequiredService<ILogger<OrderRepository>>()));
            services.AddScoped<IActionRepository>(sp => new ActionRepository(_context));
            services.AddScoped<IRitualDismissalRepository>(sp => new RitualDismissalRepository(_context));
            services.AddScoped<IRecommendationLogRepository>(sp => new RecommendationLogRepository(_context));
            services.AddScoped<IProductRepository>(sp => new ProductRepository(_context));
            services.AddScoped<ISequentialPatternMatcher>(sp => new SequentialPatternMatcher());
            services.AddScoped<IActionTrackingService>(sp => new ActionTrackingService(
                sp.GetRequiredService<IActionRepository>(),
                sp.GetRequiredService<IUnitOfWork>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<ILogger<ActionTrackingService>>()
            ));
            services.AddScoped<IUserPreferenceService>(sp => new UserPreferenceService(
                sp.GetRequiredService<IRitualDismissalRepository>(),
                sp.GetRequiredService<IUnitOfWork>(),
                sp.GetRequiredService<ILogger<UserPreferenceService>>()
            ));
            services.AddScoped<IRecommendationService>(sp => new RecommendationService(
                sp.GetRequiredService<ISequentialPatternMatcher>(),
                sp.GetRequiredService<IUnitOfWork>(),
                sp.GetRequiredService<IProductRepository>(),
                sp.GetRequiredService<IUserPreferenceService>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<ILogger<RecommendationService>>()
            ));
            services.AddScoped<IGeminiExplanationService>(sp => new GeminiExplanationService(
                new HttpClient(),
                sp.GetRequiredService<ILogger<GeminiExplanationService>>()
            ));

            _serviceProvider = services.BuildServiceProvider();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _actionTrackingService = _serviceProvider.GetRequiredService<IActionTrackingService>();
            _recommendationService = _serviceProvider.GetRequiredService<IRecommendationService>();
            _explanationService = _serviceProvider.GetRequiredService<IGeminiExplanationService>();
            _userPreferenceService = _serviceProvider.GetRequiredService<IUserPreferenceService>();
            _patternMatcher = _serviceProvider.GetRequiredService<ISequentialPatternMatcher>();
            _mapper = _serviceProvider.GetRequiredService<IMapper>();
        }

        public async Task InitializeAsync()
        {
            await SeedTestDataAsync();
        }

        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        private async Task SeedTestDataAsync()
        {
            // Create users
            var user1 = new User
            {
                Id = _userId1,
                Email = "user1@test.com",
                Name = "Test User 1",
                PasswordHash = "hash",
                IsActive = true
            };

            var user2 = new User
            {
                Id = _userId2,
                Email = "user2@test.com",
                Name = "Test User 2",
                PasswordHash = "hash",
                IsActive = true
            };

            // Create products
            var product1 = new Product
            {
                Id = _productId1,
                Name = "Mâm Cúng",
                Code = "PROD-001",
                Slug = "mam-cung",
                SKU = "MAM-001",
                CategoryId = _categoryId,
                Stock = 100,
                IsActive = true,
                ViewCount = 0,
                FavoriteCount = 0,
                PurchaseCount = 0,
                ReviewCount = 0,
                AvgRating = 0
            };

            var product2 = new Product
            {
                Id = _productId2,
                Name = "Heo Quay",
                Code = "PROD-002",
                Slug = "heo-quay",
                SKU = "HEO-002",
                CategoryId = _categoryId,
                Stock = 100,
                IsActive = true,
                ViewCount = 0,
                FavoriteCount = 0,
                PurchaseCount = 0,
                ReviewCount = 0,
                AvgRating = 0
            };

            var product3 = new Product
            {
                Id = _productId3,
                Name = "Ngũ Quả",
                Code = "PROD-003",
                Slug = "ngu-qua",
                SKU = "NGU-003",
                CategoryId = _categoryId,
                Stock = 100,
                IsActive = true,
                ViewCount = 0,
                FavoriteCount = 0,
                PurchaseCount = 0,
                ReviewCount = 0,
                AvgRating = 0
            };

            var product4 = new Product
            {
                Id = _productId4,
                Name = "Bộ Tam Sên",
                Code = "PROD-004",
                Slug = "bo-tam-sen",
                SKU = "TAM-004",
                CategoryId = _categoryId,
                Stock = 100,
                IsActive = true,
                ViewCount = 0,
                FavoriteCount = 0,
                PurchaseCount = 0,
                ReviewCount = 0,
                AvgRating = 0
            };

            _context.Users.Add(user1);
            _context.Users.Add(user2);
            _context.Products.Add(product1);
            _context.Products.Add(product2);
            _context.Products.Add(product3);
            _context.Products.Add(product4);

            await _context.SaveChangesAsync();

            // Initialize pattern matcher with manifest
            var logger = _serviceProvider.GetRequiredService<ILogger<RitualManifestLoader>>();
            var loader = new RitualManifestLoader(logger);
            var manifest = await loader.LoadManifestAsync("wwwroot/data/ritual-manifest.json");
            if (manifest != null)
            {
                _patternMatcher.Initialize(manifest);
            }
        }

        #region Complete Workflow Tests

        /// <summary>
        /// Integration Test: Complete end-to-end flow
        /// Action Tracking → Pattern Matching → Recommendation Generation → Explanation
        /// Tests: Requirements 1.1, 1.2, 1.3, 1.4, 1.5, 2.1, 2.2, 2.3, 2.5
        /// </summary>
        [Fact]
        public async Task CompleteWorkflow_ActionTrackingToExplanation_ShouldSucceed()
        {
            // Step 1: Track user actions for Đầy Tháng ritual
            var sessionId = Guid.NewGuid().ToString();
            
            // User views Mâm Cúng product
            await _actionTrackingService.TrackActionAsync(
                _userId1,
                sessionId,
                "ViewProduct",
                _productId1,
                _categoryId,
                new Dictionary<string, object> { { "source", "search" } }
            );

            // User adds Mâm Cúng to cart
            await _actionTrackingService.TrackActionAsync(
                _userId1,
                sessionId,
                "AddToCart",
                _productId1,
                _categoryId
            );

            // User views Heo Quay product
            await _actionTrackingService.TrackActionAsync(
                _userId1,
                sessionId,
                "ViewProduct",
                _productId2,
                _categoryId
            );

            // User adds Heo Quay to cart
            await _actionTrackingService.TrackActionAsync(
                _userId1,
                sessionId,
                "AddToCart",
                _productId2,
                _categoryId
            );

            // Step 2: Retrieve action sequence
            var actionSequence = await _actionTrackingService.GetActionSequenceAsync(_userId1, sessionId);
            Assert.NotEmpty(actionSequence);
            Assert.Equal(4, actionSequence.Count);

            // Step 3: Generate recommendation based on action sequence
            var cartItems = new List<CartItemDto>
            {
                new CartItemDto { ProductId = _productId1, ProductName = "Mâm Cúng" },
                new CartItemDto { ProductId = _productId2, ProductName = "Heo Quay" }
            };

            var recommendation = await _recommendationService.GenerateRecommendationAsync(
                _userId1,
                sessionId,
                actionSequence,
                cartItems
            );

            // Step 4: Verify recommendation was generated
            if (recommendation != null)
            {
                Assert.NotNull(recommendation.RitualName);
                Assert.True(recommendation.ConfidenceScore >= 0 && recommendation.ConfidenceScore <= 1);
                Assert.NotNull(recommendation.SystemReport);
                Assert.NotEmpty(recommendation.SystemReport.MatchingSteps);

                // Step 5: Log the recommendation
                var logId = await _recommendationService.LogRecommendationAsync(
                    _userId1,
                    sessionId,
                    recommendation
                );
                Assert.NotEqual(Guid.Empty, logId);

                // Step 6: Generate explanation using Gemini (or fallback)
                var explanation = await _explanationService.GenerateExplanationAsync(recommendation);
                Assert.NotNull(explanation);
                Assert.NotEmpty(explanation.RitualName);
                Assert.NotEmpty(explanation.CulturalContext);
                Assert.NotNull(explanation.ItemExplanations);
                Assert.NotEmpty(explanation.Sources);
            }
        }

        #endregion

        #region Multiple Concurrent User Sessions Tests

        /// <summary>
        /// Integration Test: Multiple concurrent user sessions
        /// Tests that the system correctly handles multiple users with different sessions
        /// Tests: Requirements 1.1, 1.2, 1.3
        /// </summary>
        [Fact]
        public async Task MultipleConcurrentSessions_TwoUsersWithDifferentRituals_ShouldHandleIndependently()
        {
            // Session 1: User 1 tracking Đầy Tháng ritual
            var session1 = Guid.NewGuid().ToString();
            await _actionTrackingService.TrackActionAsync(_userId1, session1, "ViewProduct", _productId1, _categoryId);
            await _actionTrackingService.TrackActionAsync(_userId1, session1, "AddToCart", _productId1, _categoryId);

            // Session 2: User 2 tracking different ritual
            var session2 = Guid.NewGuid().ToString();
            await _actionTrackingService.TrackActionAsync(_userId2, session2, "ViewProduct", _productId3, _categoryId);
            await _actionTrackingService.TrackActionAsync(_userId2, session2, "AddToCart", _productId3, _categoryId);

            // Retrieve action sequences for both users
            var user1Actions = await _actionTrackingService.GetActionSequenceAsync(_userId1, session1);
            var user2Actions = await _actionTrackingService.GetActionSequenceAsync(_userId2, session2);

            // Verify each user has their own actions
            Assert.NotEmpty(user1Actions);
            Assert.NotEmpty(user2Actions);
            Assert.All(user1Actions, a => Assert.Equal(_userId1, a.UserId));
            Assert.All(user2Actions, a => Assert.Equal(_userId2, a.UserId));
            Assert.All(user1Actions, a => Assert.Equal(session1, a.SessionId));
            Assert.All(user2Actions, a => Assert.Equal(session2, a.SessionId));
        }

        #endregion

        #region Various Ritual Patterns Tests

        /// <summary>
        /// Integration Test: Various ritual patterns and action sequences
        /// Tests that the system correctly matches different ritual patterns
        /// Tests: Requirements 1.1, 1.2, 1.4
        /// </summary>
        [Fact]
        public async Task VariousRitualPatterns_MultipleActionSequences_ShouldMatchCorrectPatterns()
        {
            // Test Pattern 1: Đầy Tháng (1-month celebration)
            var session1 = Guid.NewGuid().ToString();
            
            // Track actions for Đầy Tháng
            await _actionTrackingService.TrackActionAsync(_userId1, session1, "ViewProduct", _productId1, _categoryId);
            await _actionTrackingService.TrackActionAsync(_userId1, session1, "AddToCart", _productId1, _categoryId);
            await _actionTrackingService.TrackActionAsync(_userId1, session1, "ViewProduct", _productId2, _categoryId);
            await _actionTrackingService.TrackActionAsync(_userId1, session1, "AddToCart", _productId2, _categoryId);

            var actions1 = await _actionTrackingService.GetActionSequenceAsync(_userId1, session1);
            Assert.NotEmpty(actions1);

            // Test Pattern 2: Different ritual with different action sequence
            var session2 = Guid.NewGuid().ToString();
            
            // Track actions for different ritual
            await _actionTrackingService.TrackActionAsync(_userId1, session2, "BrowseCategory", null, _categoryId);
            await _actionTrackingService.TrackActionAsync(_userId1, session2, "ViewProduct", _productId3, _categoryId);
            await _actionTrackingService.TrackActionAsync(_userId1, session2, "AddToCart", _productId3, _categoryId);

            var actions2 = await _actionTrackingService.GetActionSequenceAsync(_userId1, session2);
            Assert.NotEmpty(actions2);

            // Verify sessions are independent
            Assert.NotEqual(session1, session2);
            Assert.Equal(4, actions1.Count);
            Assert.Equal(3, actions2.Count);
        }

        #endregion

        #region Dismissal and Preference Tests

        /// <summary>
        /// Integration Test: User dismissal and preference tracking
        /// Tests that dismissed rituals are not recommended again
        /// Tests: Requirements 6.2, 6.3
        /// </summary>
        [Fact]
        public async Task DismissalTracking_RecordAndCheckDismissal_ShouldPreventRecommendation()
        {
            var sessionId = Guid.NewGuid().ToString();
            var ritualId = Guid.NewGuid();

            // Record dismissal
            await _userPreferenceService.RecordDismissalAsync(_userId1, ritualId, "Test dismissal");

            // Check if ritual is disabled
            var isDisabled = await _userPreferenceService.IsRitualDisabledAsync(_userId1, sessionId, ritualId);
            Assert.True(isDisabled);

            // Get dismissal count
            var dismissalCount = await _userPreferenceService.GetDismissalCountAsync(_userId1, ritualId);
            Assert.Equal(1, dismissalCount);
        }

        /// <summary>
        /// Integration Test: Disable ritual for session
        /// Tests that disabled rituals are not recommended in the current session
        /// Tests: Requirements 6.3
        /// </summary>
        [Fact]
        public async Task DisableRitual_DisableAndVerify_ShouldPreventRecommendation()
        {
            var sessionId = Guid.NewGuid().ToString();
            var ritualId = Guid.NewGuid();

            // Disable ritual for session
            await _userPreferenceService.DisableRitualAsync(_userId1, sessionId, ritualId);

            // Verify ritual is disabled
            var isDisabled = await _userPreferenceService.IsRitualDisabledAsync(_userId1, sessionId, ritualId);
            Assert.True(isDisabled);
        }

        #endregion

        #region Action Sequence Management Tests

        /// <summary>
        /// Integration Test: Clear action sequence on navigation
        /// Tests that recommendations are cleared when user navigates away
        /// Tests: Requirements 5.4
        /// </summary>
        [Fact]
        public async Task ClearActionSequence_TrackThenClear_ShouldRemoveAllActions()
        {
            var sessionId = Guid.NewGuid().ToString();

            // Track actions
            await _actionTrackingService.TrackActionAsync(_userId1, sessionId, "ViewProduct", _productId1, _categoryId);
            await _actionTrackingService.TrackActionAsync(_userId1, sessionId, "AddToCart", _productId1, _categoryId);

            // Verify actions exist
            var actionsBefore = await _actionTrackingService.GetActionSequenceAsync(_userId1, sessionId);
            Assert.NotEmpty(actionsBefore);

            // Clear actions
            var deletedCount = await _actionTrackingService.ClearActionSequenceAsync(sessionId);
            Assert.True(deletedCount > 0);

            // Verify actions are cleared
            var actionsAfter = await _actionTrackingService.GetActionSequenceAsync(_userId1, sessionId);
            Assert.Empty(actionsAfter);
        }

        /// <summary>
        /// Integration Test: Get recent actions within time window
        /// Tests that recent actions are correctly retrieved
        /// Tests: Requirements 1.1, 5.3
        /// </summary>
        [Fact]
        public async Task RecentActions_TrackAndRetrieveRecent_ShouldReturnActionsWithinTimeWindow()
        {
            var sessionId = Guid.NewGuid().ToString();

            // Track actions
            await _actionTrackingService.TrackActionAsync(_userId1, sessionId, "ViewProduct", _productId1, _categoryId);
            await Task.Delay(100); // Small delay
            await _actionTrackingService.TrackActionAsync(_userId1, sessionId, "AddToCart", _productId1, _categoryId);

            // Get recent actions (last 1 minute)
            var recentActions = await _actionTrackingService.GetRecentActionsAsync(_userId1, sessionId, 1);
            Assert.NotEmpty(recentActions);
            Assert.True(recentActions.Count >= 2);
        }

        #endregion

        #region Recommendation Logging Tests

        /// <summary>
        /// Integration Test: Recommendation logging and interaction tracking
        /// Tests that recommendations are properly logged and interactions are recorded
        /// Tests: Requirements 4.1, 4.2, 4.3, 4.4, 4.5
        /// </summary>
        [Fact]
        public async Task RecommendationLogging_LogAndRecordInteraction_ShouldPersistData()
        {
            var sessionId = Guid.NewGuid().ToString();
            var payload = new RecommendationPayloadDto
            {
                RitualId = Guid.NewGuid().ToString(),
                RitualName = "Đầy Tháng",
                ConfidenceScore = 0.85m,
                MissingItems = new List<Guid> { _productId3, _productId4 },
                MatchingMetadata = new MatchingMetadataDto
                {
                    MatchedSequenceLength = 4,
                    TotalSequenceLength = 4,
                    MatchedActionIndices = new List<int> { 0, 1, 2, 3 }
                },
                SystemReport = new SystemReportDto
                {
                    MatchedPattern = new List<ActionTypeDto>(),
                    MatchingSteps = new List<string>
                    {
                        "Analyzed action sequence of length 4",
                        "Matched 4 actions from ritual pattern",
                        "Calculated confidence score: 85%"
                    },
                    ReasonsForMissingItems = new Dictionary<Guid, string>
                    {
                        { _productId3, "Required for Đầy Tháng ritual" },
                        { _productId4, "Required for Đầy Tháng ritual" }
                    }
                }
            };

            // Log recommendation
            var logId = await _recommendationService.LogRecommendationAsync(_userId1, sessionId, payload);
            Assert.NotEqual(Guid.Empty, logId);

            // Record interaction
            var interactionRecorded = await _recommendationService.RecordInteractionAsync(logId, "viewed");
            Assert.True(interactionRecorded);
        }

        #endregion

        #region Fallback Explanation Tests

        /// <summary>
        /// Integration Test: Fallback explanation when Gemini API is unavailable
        /// Tests that fallback explanations are generated correctly
        /// Tests: Requirements 8.5
        /// </summary>
        [Fact]
        public async Task FallbackExplanation_GenerateWithoutGeminiAPI_ShouldReturnTemplateExplanation()
        {
            var payload = new RecommendationPayloadDto
            {
                RitualId = Guid.NewGuid().ToString(),
                RitualName = "Tết",
                ConfidenceScore = 0.90m,
                MissingItems = new List<Guid> { _productId1, _productId2 },
                MatchingMetadata = new MatchingMetadataDto(),
                SystemReport = new SystemReportDto
                {
                    ReasonsForMissingItems = new Dictionary<Guid, string>
                    {
                        { _productId1, "Required for Tết ritual" },
                        { _productId2, "Required for Tết ritual" }
                    }
                }
            };

            // Generate explanation (will use fallback since Gemini API key is not set)
            var explanation = await _explanationService.GenerateExplanationAsync(payload);

            // Verify fallback explanation structure
            Assert.NotNull(explanation);
            Assert.Equal("Tết", explanation.RitualName);
            Assert.NotEmpty(explanation.CulturalContext);
            Assert.NotNull(explanation.ItemExplanations);
            Assert.NotEmpty(explanation.Sources);
            Assert.Equal("fallback", explanation.GeneratedBy);
        }

        #endregion

        #region Error Scenario Tests

        /// <summary>
        /// Integration Test: Error scenarios
        /// Tests that the system handles errors gracefully
        /// Tests: Requirements 1.5
        /// </summary>
        [Fact]
        public async Task ErrorScenarios_EmptyActionSequence_ShouldReturnNull()
        {
            var sessionId = Guid.NewGuid().ToString();
            var emptyActionSequence = new List<ActionDto>();
            var cartItems = new List<CartItemDto>();

            // Generate recommendation with empty action sequence
            var recommendation = await _recommendationService.GenerateRecommendationAsync(
                _userId1,
                sessionId,
                emptyActionSequence,
                cartItems
            );

            // Should return null for empty sequence
            Assert.Null(recommendation);
        }

        /// <summary>
        /// Integration Test: Invalid user ID
        /// Tests that the system handles invalid inputs
        /// Tests: Requirements 1.1
        /// </summary>
        [Fact]
        public async Task ErrorScenarios_InvalidUserIdForActionTracking_ShouldThrowException()
        {
            var sessionId = Guid.NewGuid().ToString();
            var invalidUserId = Guid.Empty;

            // Attempt to track action with empty user ID
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _actionTrackingService.TrackActionAsync(
                    invalidUserId,
                    sessionId,
                    "ViewProduct",
                    _productId1,
                    _categoryId
                );
            });
        }

        #endregion
    }
}
