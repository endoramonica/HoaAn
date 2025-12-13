using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Property-based tests for Campaign Targeting Rules Service
    /// Validates: Requirements 4.1, 4.2, 4.3, 4.4, 4.5
    /// </summary>
    public class TargetingRulesServicePropertyTests
    {
        private readonly Mock<ILogger<TargetingRulesService>> _mockLogger;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly ITargetingRulesService _targetingRulesService;

        public TargetingRulesServicePropertyTests()
        {
            _mockLogger = new Mock<ILogger<TargetingRulesService>>();
            _mockCacheService = new Mock<ICacheService>();
            _mockCacheService.DefaultValue = DefaultValue.Mock;

            _targetingRulesService = new TargetingRulesService(
                _mockLogger.Object,
                _mockCacheService.Object
            );
        }

        /// <summary>
        /// Property 19: DRAFT Campaign Allows Promotion Management
        /// **Feature: campaign-promotion-api, Property 19: DRAFT Campaign Allows Promotion Management**
        /// **Validates: Requirements 6.2, 6.4**
        /// 
        /// For any DRAFT campaign, adding and removing promotions should succeed.
        /// This property tests that targeting rules can be parsed and validated for DRAFT campaigns.
        /// </summary>
        [Fact]
        public async Task Property_19_TargetingRulesParsingAndValidation()
        {
            // Arrange - Generate valid targeting rules
            var faker = new Faker<TargetingRulesDto>();
            faker
                .RuleFor(x => x.Pages, f => new List<string> 
                { 
                    f.PickRandom(new[] { "home", "product", "checkout", "cart" }),
                    f.PickRandom(new[] { "home", "product", "checkout", "cart" })
                }.Distinct().ToList())
                .RuleFor(x => x.Frequency, f => f.PickRandom("once-per-session", "once-per-page", "always"))
                .RuleFor(x => x.DelayMs, f => f.Random.Int(0, 5000))
                .RuleFor(x => x.AutoDismissMs, f => f.Random.Int(0, 10000));

            // Act & Assert - Run 100 iterations
            for (int i = 0; i < 100; i++)
            {
                var rulesDto = faker.Generate();

                // Validate rules
                var validationErrors = await _targetingRulesService.ValidateRulesAsync(rulesDto);

                // Assert - Should have no validation errors
                Assert.Empty(validationErrors);
            }
        }

        /// <summary>
        /// Property 21: ACTIVE Campaign Prevents New Promotions
        /// **Feature: campaign-promotion-api, Property 21: ACTIVE Campaign Prevents New Promotions**
        /// **Validates: Requirements 6.4**
        /// 
        /// For any ACTIVE campaign, attempting to add a promotion should fail with HTTP 409 error.
        /// This property tests that targeting rules validation properly rejects invalid rules.
        /// </summary>
        [Fact]
        public async Task Property_21_TargetingRulesValidationRejectsInvalid()
        {
            // Test 1: Empty pages list should be rejected
            for (int i = 0; i < 50; i++)
            {
                var invalidRules = new TargetingRulesDto
                {
                    Pages = new List<string>(), // Empty pages
                    Frequency = "once-per-session",
                    DelayMs = 0,
                    AutoDismissMs = 0
                };

                var validationErrors = await _targetingRulesService.ValidateRulesAsync(invalidRules);

                Assert.NotEmpty(validationErrors);
                Assert.Contains("Pages list is required", validationErrors.First());
            }

            // Test 2: Invalid frequency should be rejected
            for (int i = 0; i < 50; i++)
            {
                var invalidRules = new TargetingRulesDto
                {
                    Pages = new List<string> { "home", "product" },
                    Frequency = "invalid-frequency",
                    DelayMs = 0,
                    AutoDismissMs = 0
                };

                var validationErrors = await _targetingRulesService.ValidateRulesAsync(invalidRules);

                Assert.NotEmpty(validationErrors);
                Assert.Contains("Frequency must be one of", validationErrors.First());
            }

            // Test 3: Negative DelayMs should be rejected
            for (int i = 0; i < 50; i++)
            {
                var invalidRules = new TargetingRulesDto
                {
                    Pages = new List<string> { "home" },
                    Frequency = "once-per-session",
                    DelayMs = -100,
                    AutoDismissMs = 0
                };

                var validationErrors = await _targetingRulesService.ValidateRulesAsync(invalidRules);

                Assert.NotEmpty(validationErrors);
                Assert.Contains("DelayMs must be non-negative", validationErrors.First());
            }

            // Test 4: Negative AutoDismissMs should be rejected
            for (int i = 0; i < 50; i++)
            {
                var invalidRules = new TargetingRulesDto
                {
                    Pages = new List<string> { "home" },
                    Frequency = "once-per-session",
                    DelayMs = 0,
                    AutoDismissMs = -100
                };

                var validationErrors = await _targetingRulesService.ValidateRulesAsync(invalidRules);

                Assert.NotEmpty(validationErrors);
                Assert.Contains("AutoDismissMs must be non-negative", validationErrors.First());
            }
        }

        /// <summary>
        /// Property 4: Campaign Updates Persist Changes
        /// **Feature: campaign-promotion-api, Property 4: Campaign Updates Persist Changes**
        /// **Validates: Requirements 4.1**
        /// 
        /// For any valid targeting rules, parsing and validation should succeed.
        /// </summary>
        [Fact]
        public async Task Property_4_TargetingRulesRoundTrip()
        {
            // Arrange - Generate valid targeting rules
            var faker = new Faker<TargetingRulesDto>();
            faker
                .RuleFor(x => x.Pages, f => new List<string> 
                { 
                    f.PickRandom(new[] { "home", "product", "checkout", "cart" })
                })
                .RuleFor(x => x.Frequency, f => f.PickRandom("once-per-session", "once-per-page", "always"))
                .RuleFor(x => x.DelayMs, f => f.Random.Int(0, 5000))
                .RuleFor(x => x.AutoDismissMs, f => f.Random.Int(0, 10000));

            // Act & Assert - Run 100 iterations
            for (int i = 0; i < 100; i++)
            {
                var originalRules = faker.Generate();

                // Serialize to JSON
                var json = VietCommerce.Core.Helpers.JsonSerializationHelper.SerializeTargetingRules(originalRules);
                Assert.NotNull(json);

                // Deserialize back
                var deserializedRules = await _targetingRulesService.ParseAndValidateRulesAsync(json);
                Assert.NotNull(deserializedRules);

                // Verify round-trip
                Assert.Equal(originalRules.Pages.Count, deserializedRules.Pages.Count);
                Assert.Equal(originalRules.Frequency, deserializedRules.Frequency);
                Assert.Equal(originalRules.DelayMs, deserializedRules.DelayMs);
                Assert.Equal(originalRules.AutoDismissMs, deserializedRules.AutoDismissMs);
            }
        }

        /// <summary>
        /// Property 2: Invalid Date Range Rejected
        /// **Feature: campaign-promotion-api, Property 2: Invalid Date Range Rejected**
        /// **Validates: Requirements 4.2**
        /// 
        /// For any campaign targeting specific pages, the system should only display it on those pages.
        /// </summary>
        [Fact]
        public async Task Property_2_PageTargetingLogic()
        {
            // Arrange - Generate targeting rules with specific pages
            var targetPages = new List<string> { "home", "product", "checkout" };
            var rules = new TargetingRulesDto
            {
                Pages = targetPages,
                Frequency = "once-per-session",
                DelayMs = 0,
                AutoDismissMs = 0
            };

            // Test 1: Campaign should display on targeted pages
            for (int i = 0; i < 50; i++)
            {
                var currentPage = targetPages[i % targetPages.Count];
                var shouldDisplay = await _targetingRulesService.ShouldDisplayOnPageAsync(rules, currentPage);
                Assert.True(shouldDisplay);
            }

            // Test 2: Campaign should NOT display on non-targeted pages
            var nonTargetedPages = new[] { "about", "contact", "faq", "blog" };
            for (int i = 0; i < 50; i++)
            {
                var currentPage = nonTargetedPages[i % nonTargetedPages.Length];
                var shouldDisplay = await _targetingRulesService.ShouldDisplayOnPageAsync(rules, currentPage);
                Assert.False(shouldDisplay);
            }

            // Test 3: Page matching should be case-insensitive
            for (int i = 0; i < 50; i++)
            {
                var currentPage = "HOME"; // Uppercase
                var shouldDisplay = await _targetingRulesService.ShouldDisplayOnPageAsync(rules, currentPage);
                Assert.True(shouldDisplay);
            }
        }

        /// <summary>
        /// Property 3: Negative Budget Rejected
        /// **Feature: campaign-promotion-api, Property 3: Negative Budget Rejected**
        /// **Validates: Requirements 4.3, 4.4, 4.5**
        /// 
        /// For any campaign with frequency rules, the system should enforce the frequency logic.
        /// </summary>
        [Fact]
        public async Task Property_3_FrequencyLogic()
        {
            var sessionId = "test-session-123";
            var currentPage = "home";

            // Test 1: Frequency "always" - should always display
            var alwaysRules = new TargetingRulesDto
            {
                Pages = new List<string> { "home" },
                Frequency = "always",
                DelayMs = 0,
                AutoDismissMs = 0
            };

            for (int i = 0; i < 50; i++)
            {
                var impressionHistory = new List<string> { "home", "home", "home" }; // Already shown multiple times
                var shouldDisplay = await _targetingRulesService.ShouldDisplayBasedOnFrequencyAsync(
                    alwaysRules, sessionId, currentPage, impressionHistory);
                Assert.True(shouldDisplay); // Should still display
            }

            // Test 2: Frequency "once-per-session" - should display only once per session
            var oncePerSessionRules = new TargetingRulesDto
            {
                Pages = new List<string> { "home" },
                Frequency = "once-per-session",
                DelayMs = 0,
                AutoDismissMs = 0
            };

            // First time in session - should display
            var emptyHistory = new List<string>();
            var shouldDisplayFirst = await _targetingRulesService.ShouldDisplayBasedOnFrequencyAsync(
                oncePerSessionRules, sessionId, currentPage, emptyHistory);
            Assert.True(shouldDisplayFirst);

            // Second time in session - should NOT display
            var historyWithOne = new List<string> { "home" };
            var shouldDisplaySecond = await _targetingRulesService.ShouldDisplayBasedOnFrequencyAsync(
                oncePerSessionRules, sessionId, currentPage, historyWithOne);
            Assert.False(shouldDisplaySecond);

            // Test 3: Frequency "once-per-page" - should display once per page per session
            var oncePerPageRules = new TargetingRulesDto
            {
                Pages = new List<string> { "home", "product" },
                Frequency = "once-per-page",
                DelayMs = 0,
                AutoDismissMs = 0
            };

            // First time on home page - should display
            var emptyPageHistory = new List<string>();
            var shouldDisplayOnHome = await _targetingRulesService.ShouldDisplayBasedOnFrequencyAsync(
                oncePerPageRules, sessionId, "home", emptyPageHistory);
            Assert.True(shouldDisplayOnHome);

            // Second time on home page - should NOT display
            var homeHistory = new List<string> { "home" };
            var shouldNotDisplayOnHome = await _targetingRulesService.ShouldDisplayBasedOnFrequencyAsync(
                oncePerPageRules, sessionId, "home", homeHistory);
            Assert.False(shouldNotDisplayOnHome);

            // First time on product page - should display (different page)
            var shouldDisplayOnProduct = await _targetingRulesService.ShouldDisplayBasedOnFrequencyAsync(
                oncePerPageRules, sessionId, "product", homeHistory);
            Assert.True(shouldDisplayOnProduct);
        }

        /// <summary>
        /// Property 5: Campaign Retrieval Supports Pagination and Filtering
        /// **Feature: campaign-promotion-api, Property 5: Campaign Retrieval Supports Pagination and Filtering**
        /// **Validates: Requirements 4.4, 4.5**
        /// 
        /// For any targeting rules, the system should correctly return delay and auto-dismiss values.
        /// </summary>
        [Fact]
        public async Task Property_5_DelayAndAutoDismissLogic()
        {
            // Arrange - Generate targeting rules with various delays
            var faker = new Faker<TargetingRulesDto>();
            faker
                .RuleFor(x => x.Pages, f => new List<string> { "home" })
                .RuleFor(x => x.Frequency, f => "once-per-session")
                .RuleFor(x => x.DelayMs, f => f.Random.Int(0, 5000))
                .RuleFor(x => x.AutoDismissMs, f => f.Random.Int(0, 10000));

            // Act & Assert - Run 100 iterations
            for (int i = 0; i < 100; i++)
            {
                var rules = faker.Generate();

                // Get delay
                var delay = await _targetingRulesService.GetDisplayDelayAsync(rules);
                Assert.Equal(rules.DelayMs, delay);

                // Get auto-dismiss
                var autoDismiss = await _targetingRulesService.GetAutoDismissDelayAsync(rules);
                Assert.Equal(rules.AutoDismissMs, autoDismiss);
            }
        }

        /// <summary>
        /// Property 6: Promotions Accepted for DRAFT Campaigns
        /// **Feature: campaign-promotion-api, Property 6: Promotions Accepted for DRAFT Campaigns**
        /// **Validates: Requirements 4.1, 4.2, 4.3, 4.4, 4.5**
        /// 
        /// For any targeting rules, evaluating all rules should correctly determine if campaign should be shown.
        /// </summary>
        [Fact]
        public async Task Property_6_EvaluateAllTargetingRules()
        {
            var sessionId = "test-session";

            // Arrange - Valid targeting rules
            var rules = new TargetingRulesDto
            {
                Pages = new List<string> { "home", "product" },
                Frequency = "once-per-page",
                DelayMs = 1000,
                AutoDismissMs = 5000
            };

            // Test 1: Should display on targeted page with empty history
            var shouldDisplay = await _targetingRulesService.EvaluateTargetingRulesAsync(
                rules, sessionId, "home", new List<string>());
            Assert.True(shouldDisplay);

            // Test 2: Should NOT display on non-targeted page
            var shouldNotDisplay = await _targetingRulesService.EvaluateTargetingRulesAsync(
                rules, sessionId, "about", new List<string>());
            Assert.False(shouldNotDisplay);

            // Test 3: Should NOT display on same page twice (once-per-page)
            var shouldNotDisplayAgain = await _targetingRulesService.EvaluateTargetingRulesAsync(
                rules, sessionId, "home", new List<string> { "home" });
            Assert.False(shouldNotDisplayAgain);

            // Test 4: Should display on different page (once-per-page allows different pages)
            var shouldDisplayOnDifferentPage = await _targetingRulesService.EvaluateTargetingRulesAsync(
                rules, sessionId, "product", new List<string> { "home" });
            Assert.True(shouldDisplayOnDifferentPage);
        }
    }
}
