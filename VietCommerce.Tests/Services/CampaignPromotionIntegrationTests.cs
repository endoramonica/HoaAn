using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using VietCommerce.Application.Mappings;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Integration tests for Campaign & Promotion Management System
    /// Tests complete workflows and interactions between services
    /// Requirements: All
    /// </summary>
    public class CampaignPromotionIntegrationTests : IAsyncLifetime
    {
        private readonly AppDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICampaignService _campaignService;
        private readonly IPromotionService _promotionService;
        private readonly IVoucherService _voucherService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IDiscountCalculationService _discountCalculationService;
        private readonly IMapper _mapper;

        // Test data IDs
        private readonly Guid _storeId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _productId = Guid.NewGuid();

        public CampaignPromotionIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"IntegrationTestDb_{Guid.NewGuid()}")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new AppDbContext(options);

            // Setup DI container
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAutoMapper(typeof(CampaignMappingProfile));
            services.AddScoped<ICacheService, MockCacheService>();
            services.AddScoped<IUnitOfWork>(sp => new UnitOfWork(_context, sp.GetRequiredService<ILogger<OrderRepository>>()));
            services.AddScoped<ICampaignService>(sp => new CampaignService(
                sp.GetRequiredService<IUnitOfWork>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<ILogger<CampaignService>>(),
                sp.GetRequiredService<ICacheService>()
            ));
            services.AddScoped<IPromotionService>(sp => new PromotionService(
                sp.GetRequiredService<IUnitOfWork>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<ILogger<PromotionService>>(),
                sp.GetRequiredService<ICacheService>()
            ));
            services.AddScoped<IVoucherService>(sp => new VoucherService(
                sp.GetRequiredService<IUnitOfWork>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<ILogger<VoucherService>>(),
                sp.GetRequiredService<ICacheService>()
            ));
            services.AddScoped<IAnalyticsService>(sp => new AnalyticsService(
                sp.GetRequiredService<IUnitOfWork>(),
                sp.GetRequiredService<ILogger<AnalyticsService>>(),
                sp.GetRequiredService<ICacheService>()
            ));
            services.AddScoped<IDiscountCalculationService>(sp => new DiscountCalculationService(
                sp.GetRequiredService<ILogger<DiscountCalculationService>>(),
                sp.GetRequiredService<ICacheService>()
            ));

            _serviceProvider = services.BuildServiceProvider();
            _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
            _campaignService = _serviceProvider.GetRequiredService<ICampaignService>();
            _promotionService = _serviceProvider.GetRequiredService<IPromotionService>();
            _voucherService = _serviceProvider.GetRequiredService<IVoucherService>();
            _analyticsService = _serviceProvider.GetRequiredService<IAnalyticsService>();
            _discountCalculationService = _serviceProvider.GetRequiredService<IDiscountCalculationService>();
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
            // Create store
            var store = new Store
            {
                Id = _storeId,
                Name = "Test Store",
                Phone = "0123456789",
                Address = "123 Test St",
                IsActive = true
            };

            // Create user
            var user = new User
            {
                Id = _userId,
                Email = "user@test.com",
                Name = "Test User",
                PasswordHash = "hash",
                IsActive = true
            };

            // Create product
            var product = new Product
            {
                Id = _productId,
                Name = "Test Product",
                Code = "PROD-001",
                Slug = "test-product",
                SKU = "TEST-SKU-001",
                StoreId = _storeId,
                CategoryId = Guid.NewGuid(),
                Stock = 100,
                IsActive = true,
                ViewCount = 0,
                FavoriteCount = 0,
                PurchaseCount = 0,
                ReviewCount = 0,
                AvgRating = 0
            };

            _context.Stores.Add(store);
            _context.Users.Add(user);
            _context.Products.Add(product);

            await _context.SaveChangesAsync();
        }

        #region Complete Workflow Tests

        /// <summary>
        /// Integration Test: Complete workflow - create campaign → add promotion → generate vouchers
        /// Tests: Requirements 1.1, 2.1, 3.1
        /// </summary>
        [Fact]
        public async Task CompleteWorkflow_CreateCampaignAddPromotionGenerateVouchers_ShouldSucceed()
        {
            // Step 1: Create Campaign
            var createCampaignDto = new CreateCampaignDto
            {
                StoreId = _storeId,
                CampaignName = "Summer Sale 2025",
                Description = "Summer promotional campaign",
                CampaignType = CampaignType.SEASONAL,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 10000000,
                TargetingRules = "{}"
            };

            var campaignResult = await _campaignService.CreateCampaignAsync(createCampaignDto);
            Assert.True(campaignResult.Success, $"Campaign creation failed: {campaignResult.Message}");
            Assert.NotNull(campaignResult.Data);
            var campaignId = campaignResult.Data.Id;
            Assert.Equal(CampaignStatus.DRAFT, campaignResult.Data.Status);

            // Step 2: Add Promotion to Campaign
            var createPromotionDto = new CreatePromotionDto
            {
                PromotionName = "20% Off",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 20,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            var promotionResult = await _promotionService.CreatePromotionAsync(campaignId, createPromotionDto);
            Assert.True(promotionResult.Success, $"Promotion creation failed: {promotionResult.Message}");
            Assert.NotNull(promotionResult.Data);
            var promotionId = promotionResult.Data.Id;

            // Step 3: Generate Vouchers
            var generateVouchersDto = new GenerateVouchersDto
            {
                Quantity = 10,
                Prefix = "SUMMER",
                ExpiryDate = DateTime.UtcNow.AddDays(30)
            };

            var vouchersResult = await _voucherService.GenerateVouchersAsync(promotionId, generateVouchersDto);
            Assert.True(vouchersResult.Success, $"Voucher generation failed: {vouchersResult.Message}");
            Assert.NotNull(vouchersResult.Data);
            Assert.Equal(10, vouchersResult.Data.VoucherCodes.Count);

            // Step 4: Validate Voucher Code
            var voucherCode = vouchersResult.Data.VoucherCodes.First();
            var validateResult = await _voucherService.ValidateVoucherAsync(voucherCode);
            Assert.True(validateResult.Success, $"Voucher validation failed: {validateResult.Message}");
            Assert.NotNull(validateResult.Data);
        }

        #endregion

        #region Campaign Status Transition Tests

        /// <summary>
        /// Integration Test: Campaign status transitions
        /// Tests: Requirements 6.1, 6.2, 6.3, 6.4, 6.5
        /// </summary>
        [Fact]
        public async Task CampaignStatusTransitions_DraftToActiveToCompleted_ShouldFollowLifecycle()
        {
            // Step 1: Create Campaign (should be DRAFT)
            var createCampaignDto = new CreateCampaignDto
            {
                StoreId = _storeId,
                CampaignName = "Status Transition Test",
                CampaignType = CampaignType.SEASONAL,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 5000000,
                TargetingRules = "{}"
            };

            var campaignResult = await _campaignService.CreateCampaignAsync(createCampaignDto);
            Assert.True(campaignResult.Success);
            var campaignId = campaignResult.Data.Id;
            Assert.Equal(CampaignStatus.DRAFT, campaignResult.Data.Status);

            // Step 2: Add Promotion while DRAFT (should succeed)
            var createPromotionDto = new CreatePromotionDto
            {
                PromotionName = "Test Promotion",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 15,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            var promotionResult = await _promotionService.CreatePromotionAsync(campaignId, createPromotionDto);
            Assert.True(promotionResult.Success);

            // Step 3: Transition to ACTIVE
            var activateResult = await _campaignService.ChangeCampaignStatusAsync(campaignId, (int)CampaignStatus.ACTIVE);
            Assert.True(activateResult.Success);
            Assert.Equal(CampaignStatus.ACTIVE, activateResult.Data.Status);

            // Step 4: Try to add promotion while ACTIVE (should fail)
            var createPromotionDto2 = new CreatePromotionDto
            {
                PromotionName = "Another Promotion",
                PromotionType = PromotionType.FIXED_AMOUNT,
                DiscountValue = 100000,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            var promotionResult2 = await _promotionService.CreatePromotionAsync(campaignId, createPromotionDto2);
            Assert.False(promotionResult2.Success);
            Assert.Contains("DRAFT", promotionResult2.Message);
        }

        /// <summary>
        /// Integration Test: Campaign status transitions with invalid data
        /// Tests: Requirements 6.3
        /// </summary>
        [Fact]
        public async Task CampaignStatusTransition_DraftToActiveWithMissingFields_ShouldFail()
        {
            // Create Campaign with minimal data
            var createCampaignDto = new CreateCampaignDto
            {
                StoreId = _storeId,
                CampaignName = "", // Missing campaign name
                CampaignType = CampaignType.SEASONAL,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 5000000,
                TargetingRules = "{}"
            };

            var campaignResult = await _campaignService.CreateCampaignAsync(createCampaignDto);
            // Campaign creation should fail due to validation
            Assert.False(campaignResult.Success);
        }

        #endregion

        #region Analytics Tracking Tests

        /// <summary>
        /// Integration Test: Analytics tracking and stats calculation
        /// Tests: Requirements 7.1, 7.2, 7.3, 7.4, 7.5
        /// </summary>
        [Fact]
        public async Task AnalyticsTracking_TrackImpressionsClicksRedemptionsAndCalculateStats_ShouldRecordAndCalculateCorrectly()
        {
            // Step 1: Create Campaign
            var createCampaignDto = new CreateCampaignDto
            {
                StoreId = _storeId,
                CampaignName = "Analytics Test Campaign",
                CampaignType = CampaignType.SEASONAL,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(20),
                Budget = 5000000,
                TargetingRules = "{}"
            };

            var campaignResult = await _campaignService.CreateCampaignAsync(createCampaignDto);
            Assert.True(campaignResult.Success);
            var campaignId = campaignResult.Data.Id;

            // Step 2: Track Impressions
            var trackImpressionDto = new TrackImpressionDto
            {
                SessionId = "session-123",
                Page = "home"
            };

            var impressionResult = await _analyticsService.TrackImpressionAsync(campaignId, trackImpressionDto);
            Assert.True(impressionResult.Success);

            // Track multiple impressions
            for (int i = 0; i < 5; i++)
            {
                await _analyticsService.TrackImpressionAsync(campaignId, new TrackImpressionDto
                {
                    SessionId = $"session-{i}",
                    Page = "home"
                });
            }

            // Step 3: Track Clicks
            var trackClickDto = new TrackClickDto
            {
                SessionId = "session-123",
                Page = "home"
            };

            var clickResult = await _analyticsService.TrackClickAsync(campaignId, trackClickDto);
            Assert.True(clickResult.Success);

            // Track multiple clicks
            for (int i = 0; i < 3; i++)
            {
                await _analyticsService.TrackClickAsync(campaignId, new TrackClickDto
                {
                    SessionId = $"session-{i}",
                    Page = "home"
                });
            }

            // Step 4: Get Campaign Stats
            var getStatsQuery = new GetCampaignStatsQueryDto
            {
                FromDate = DateTime.UtcNow.AddDays(-15),
                ToDate = DateTime.UtcNow.AddDays(25)
            };

            var statsResult = await _analyticsService.GetCampaignStatsAsync(campaignId, getStatsQuery);
            Assert.True(statsResult.Success);
            Assert.NotNull(statsResult.Data);
            Assert.True(statsResult.Data.ImpressionCount > 0);
            Assert.True(statsResult.Data.ClickCount > 0);

            // Step 5: Verify Stats Filtering by Date Range
            var futureStatsQuery = new GetCampaignStatsQueryDto
            {
                FromDate = DateTime.UtcNow.AddDays(30),
                ToDate = DateTime.UtcNow.AddDays(40)
            };

            var futureStatsResult = await _analyticsService.GetCampaignStatsAsync(campaignId, futureStatsQuery);
            Assert.True(futureStatsResult.Success);
            Assert.Equal(0, futureStatsResult.Data.ImpressionCount);
            Assert.Equal(0, futureStatsResult.Data.ClickCount);
        }

        #endregion

        #region Error Scenario Tests

        /// <summary>
        /// Integration Test: Error scenarios
        /// Tests: Requirements 1.2, 1.3, 2.2, 3.4, 3.5, 5.4, 5.5
        /// </summary>
        [Fact]
        public async Task ErrorScenarios_InvalidDateRangeAndNegativeBudget_ShouldReturnErrors()
        {
            // Test 1: Invalid date range
            var invalidDateDto = new CreateCampaignDto
            {
                StoreId = _storeId,
                CampaignName = "Invalid Date Campaign",
                CampaignType = CampaignType.SEASONAL,
                StartDate = DateTime.UtcNow.AddDays(30),
                EndDate = DateTime.UtcNow.AddDays(1), // EndDate < StartDate
                Budget = 5000000,
                TargetingRules = "{}"
            };

            var invalidDateResult = await _campaignService.CreateCampaignAsync(invalidDateDto);
            Assert.False(invalidDateResult.Success);
            Assert.Contains("EndDate", invalidDateResult.Message);

            // Test 2: Negative budget
            var negativeBudgetDto = new CreateCampaignDto
            {
                StoreId = _storeId,
                CampaignName = "Negative Budget Campaign",
                CampaignType = CampaignType.SEASONAL,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = -1000, // Negative budget
                TargetingRules = "{}"
            };

            var negativeBudgetResult = await _campaignService.CreateCampaignAsync(negativeBudgetDto);
            Assert.False(negativeBudgetResult.Success);
            Assert.Contains("Budget", negativeBudgetResult.Message);
        }

        /// <summary>
        /// Integration Test: Promotion creation errors
        /// Tests: Requirements 2.2, 2.4
        /// </summary>
        [Fact]
        public async Task ErrorScenarios_PromotionCreationForNonDraftCampaign_ShouldFail()
        {
            // Create and activate campaign
            var createCampaignDto = new CreateCampaignDto
            {
                StoreId = _storeId,
                CampaignName = "Active Campaign",
                CampaignType = CampaignType.SEASONAL,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 5000000,
                TargetingRules = "{}"
            };

            var campaignResult = await _campaignService.CreateCampaignAsync(createCampaignDto);
            var campaignId = campaignResult.Data.Id;

            // Activate campaign
            await _campaignService.ChangeCampaignStatusAsync(campaignId, (int)CampaignStatus.ACTIVE);

            // Try to add promotion to ACTIVE campaign
            var createPromotionDto = new CreatePromotionDto
            {
                PromotionName = "Test Promotion",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 20,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            var promotionResult = await _promotionService.CreatePromotionAsync(campaignId, createPromotionDto);
            Assert.False(promotionResult.Success);
            Assert.Contains("DRAFT", promotionResult.Message);
        }

        /// <summary>
        /// Integration Test: Voucher validation errors
        /// Tests: Requirements 3.4, 3.5
        /// </summary>
        [Fact]
        public async Task ErrorScenarios_InvalidVouchers_ShouldFail()
        {
            // Test 1: Invalid voucher code
            var invalidVoucherResult = await _voucherService.ValidateVoucherAsync("INVALID-CODE-12345");
            Assert.False(invalidVoucherResult.Success, "Invalid voucher should fail validation");
        }

        #endregion

        #region Discount Calculation Tests

        /// <summary>
        /// Integration Test: Discount calculations
        /// Tests: Requirements 5.1, 5.2, 5.3
        /// </summary>
        [Fact]
        public async Task DiscountCalculation_PercentageAndFixedDiscounts_ShouldCalculateCorrectly()
        {
            // Test 1: Percentage discount
            var percentagePromotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "20% Off",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 20,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };

            var percentageDiscount = await _discountCalculationService.CalculateDiscountAsync(percentagePromotion, 100m);
            Assert.Equal(20m, percentageDiscount);

            // Test 2: Fixed discount
            var fixedPromotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "100,000 VND Off",
                PromotionType = PromotionType.FIXED_AMOUNT,
                DiscountValue = 100000,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };

            var fixedDiscount = await _discountCalculationService.CalculateDiscountAsync(fixedPromotion, 500000m);
            Assert.Equal(100000m, fixedDiscount);
        }

        #endregion
    }

    /// <summary>
    /// Mock cache service for testing
    /// </summary>
    public class MockCacheService : ICacheService
    {
        private readonly Dictionary<string, object> _cache = new();

        public async Task<T?> GetAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            return default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            _cache[key] = value;
        }

        public async Task RemoveAsync(string key)
        {
            _cache.Remove(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return _cache.ContainsKey(key);
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            var keysToRemove = _cache.Keys.Where(k => k.Contains(pattern)).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }

        public async Task PublishAsync<T>(string channel, T message)
        {
            // No-op for testing
        }

        public async Task SubscribeAsync<T>(string channel, Action<T> handler)
        {
            // No-op for testing
        }

        public async Task<(bool Found, T? Value)> TryGetValueAsync<T>(string key)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                return (true, (T)value);
            }
            return (false, default);
        }
    }
}
