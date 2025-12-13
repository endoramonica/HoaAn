using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Property-based tests for Campaign Statistics Calculation
    /// **Feature: campaign-promotion-api, Property 26: Campaign Stats Calculation**
    /// **Validates: Requirements 7.4**
    /// </summary>
    public class CampaignStatsCalculationPropertyTests
    {
        private Mock<IUnitOfWork> CreateMockUnitOfWork()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCampaignRepository = new Mock<ICampaignRepository>();
            var mockImpressionRepository = new Mock<ICampaignImpressionRepository>();
            var mockClickRepository = new Mock<ICampaignClickRepository>();
            var mockRedemptionRepository = new Mock<IVoucherRedemptionRepository>();

            // Setup UnitOfWork
            mockUnitOfWork.Setup(u => u.Campaigns).Returns(mockCampaignRepository.Object);
            mockUnitOfWork.Setup(u => u.CampaignImpressions).Returns(mockImpressionRepository.Object);
            mockUnitOfWork.Setup(u => u.CampaignClicks).Returns(mockClickRepository.Object);
            mockUnitOfWork.Setup(u => u.VoucherRedemptions).Returns(mockRedemptionRepository.Object);

            return mockUnitOfWork;
        }

        private Mock<ICacheService> CreateMockCacheService()
        {
            var mockCacheService = new Mock<ICacheService>();
            mockCacheService.DefaultValue = DefaultValue.Mock;
            mockCacheService
                .Setup(c => c.GetAsync<CampaignStatsDto>(It.IsAny<string>()))
                .ReturnsAsync((CampaignStatsDto)null!);
            return mockCacheService;
        }

        /// <summary>
        /// Property 26: Campaign Stats Calculation
        /// For any campaign with tracked impressions, clicks, and redemptions, requesting stats should return accurate counts and calculated rates.
        /// **Validates: Requirements 7.4**
        /// </summary>
        [Fact]
        public async Task Property_26_CampaignStatsCalculation()
        {
            // **Feature: campaign-promotion-api, Property 26: Campaign Stats Calculation**
            // **Validates: Requirements 7.4**

            var faker = new Faker();

            // Run 100 iterations with random data
            for (int i = 0; i < 100; i++)
            {
                // Create fresh mocks for each iteration to avoid cache issues
                var mockUnitOfWork = new Mock<IUnitOfWork>();
                var mockCampaignRepository = new Mock<ICampaignRepository>();
                var mockImpressionRepository = new Mock<ICampaignImpressionRepository>();
                var mockClickRepository = new Mock<ICampaignClickRepository>();
                var mockRedemptionRepository = new Mock<IVoucherRedemptionRepository>();
                var mockLogger = new Mock<ILogger<AnalyticsService>>();
                var mockCacheService = new Mock<ICacheService>();

                // Setup UnitOfWork
                mockUnitOfWork.Setup(u => u.Campaigns).Returns(mockCampaignRepository.Object);
                mockUnitOfWork.Setup(u => u.CampaignImpressions).Returns(mockImpressionRepository.Object);
                mockUnitOfWork.Setup(u => u.CampaignClicks).Returns(mockClickRepository.Object);
                mockUnitOfWork.Setup(u => u.VoucherRedemptions).Returns(mockRedemptionRepository.Object);

                // Disable caching for tests
                mockCacheService
                    .Setup(c => c.GetAsync<CampaignStatsDto>(It.IsAny<string>()))
                    .ReturnsAsync((CampaignStatsDto)null!);

                var analyticsService = new AnalyticsService(
                    mockUnitOfWork.Object,
                    mockLogger.Object,
                    mockCacheService.Object
                );

                var campaignId = Guid.NewGuid();

                // Setup campaign
                var campaign = new Campaign
                {
                    Id = campaignId,
                    CampaignName = faker.Commerce.ProductName(),
                    StartDate = DateTime.UtcNow.AddDays(-30),
                    EndDate = DateTime.UtcNow.AddDays(30),
                    Budget = 1000000,
                    Status = Core.Enums.Marketing.CampaignStatus.ACTIVE
                };

                mockCampaignRepository
                    .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(campaign);

                // Generate random stats data
                var impressionCount = faker.Random.Int(10, 1000);
                var clickCount = faker.Random.Int(1, Math.Min(impressionCount, 500));
                var redemptionCount = faker.Random.Int(1, Math.Min(clickCount, 100)); // At least 1 redemption
                var totalDiscount = faker.Random.Decimal(100, 1000000); // At least 100 discount

                // Setup repository methods
                mockImpressionRepository
                    .Setup(r => r.GetImpressionCountByDateRangeAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>()))
                    .ReturnsAsync(impressionCount);

                mockClickRepository
                    .Setup(r => r.GetClickCountByDateRangeAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>()))
                    .ReturnsAsync(clickCount);

                // Create redemption records
                var redemptions = new List<VoucherRedemption>();
                if (redemptionCount > 0)
                {
                    var discountPerRedemption = totalDiscount / redemptionCount;
                    for (int j = 0; j < redemptionCount; j++)
                    {
                        redemptions.Add(new VoucherRedemption
                        {
                            Id = Guid.NewGuid(),
                            VoucherId = Guid.NewGuid(),
                            DiscountAmount = (decimal)discountPerRedemption,
                            RedeemedAt = DateTime.UtcNow.AddSeconds(-faker.Random.Int(0, 86400))
                        });
                    }
                }

                mockRedemptionRepository
                    .Setup(r => r.GetRedemptionsByDateRangeAsync(
                        campaignId,
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>()))
                    .ReturnsAsync(redemptions);

                // Get stats
                var query = new GetCampaignStatsQueryDto
                {
                    FromDate = DateTime.UtcNow.AddDays(-30),
                    ToDate = DateTime.UtcNow
                };

                var result = await analyticsService.GetCampaignStatsAsync(campaignId, query);

                // Verify success
                Assert.True(result.Success, $"Iteration {i}: Stats retrieval should succeed");

                // Verify counts
                Assert.Equal(impressionCount, result.Data.ImpressionCount);
                Assert.Equal(clickCount, result.Data.ClickCount);
                Assert.Equal(redemptionCount, result.Data.RedemptionCount);

                // Verify CTR calculation
                var expectedCTR = impressionCount > 0 ? (double)clickCount / impressionCount * 100 : 0;
                Assert.Equal(expectedCTR, result.Data.ClickThroughRate, 2);

                // Verify redemption rate calculation
                var expectedRedemptionRate = clickCount > 0 ? (double)redemptionCount / clickCount * 100 : 0;
                Assert.Equal(expectedRedemptionRate, result.Data.RedemptionRate, 2);

                // Verify conversion rate calculation
                var expectedConversionRate = impressionCount > 0 ? (double)redemptionCount / impressionCount * 100 : 0;
                Assert.Equal(expectedConversionRate, result.Data.ConversionRate, 2);

                // Verify total discount
                Assert.Equal(totalDiscount, result.Data.TotalDiscount, 2);
            }
        }

        /// <summary>
        /// Property 26 Edge Case: Zero Impressions
        /// For any campaign with zero impressions, CTR and conversion rate should be 0.
        /// **Validates: Requirements 7.4**
        /// </summary>
        [Fact]
        public async Task Property_26_EdgeCase_ZeroImpressions()
        {
            // **Feature: campaign-promotion-api, Property 26: Campaign Stats Calculation (Edge Case)**
            // **Validates: Requirements 7.4**

            var mockUnitOfWork = CreateMockUnitOfWork();
            var mockCacheService = CreateMockCacheService();
            var mockLogger = new Mock<ILogger<AnalyticsService>>();

            var analyticsService = new AnalyticsService(
                mockUnitOfWork.Object,
                mockLogger.Object,
                mockCacheService.Object
            );

            var campaignId = Guid.NewGuid();
            var campaign = new Campaign
            {
                Id = campaignId,
                CampaignName = "Test Campaign",
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 1000000,
                Status = Core.Enums.Marketing.CampaignStatus.ACTIVE
            };

            mockUnitOfWork.Setup(u => u.Campaigns).Returns(new Mock<ICampaignRepository>().Object);
            mockUnitOfWork.Setup(u => u.Campaigns.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            mockUnitOfWork.Setup(u => u.CampaignImpressions.GetImpressionCountByDateRangeAsync(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
                .ReturnsAsync(0);

            mockUnitOfWork.Setup(u => u.CampaignClicks.GetClickCountByDateRangeAsync(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
                .ReturnsAsync(0);

            mockUnitOfWork.Setup(u => u.VoucherRedemptions.GetRedemptionsByDateRangeAsync(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
                .ReturnsAsync(new List<VoucherRedemption>());

            var query = new GetCampaignStatsQueryDto
            {
                FromDate = DateTime.UtcNow.AddDays(-30),
                ToDate = DateTime.UtcNow
            };

            var result = await analyticsService.GetCampaignStatsAsync(campaignId, query);

            // Verify success
            Assert.True(result.Success, "Stats retrieval should succeed");

            // Verify rates are 0
            Assert.Equal(0, result.Data.ClickThroughRate);
            Assert.Equal(0, result.Data.RedemptionRate);
            Assert.Equal(0, result.Data.ConversionRate);
        }

        /// <summary>
        /// Property 26 Edge Case: Invalid Campaign ID
        /// For any invalid campaign ID, stats retrieval should fail with KeyNotFoundException.
        /// **Validates: Requirements 7.4**
        /// </summary>
        [Fact]
        public async Task Property_26_EdgeCase_InvalidCampaignId()
        {
            // **Feature: campaign-promotion-api, Property 26: Campaign Stats Calculation (Edge Case)**
            // **Validates: Requirements 7.4**

            var mockUnitOfWork = CreateMockUnitOfWork();
            var mockCacheService = CreateMockCacheService();
            var mockLogger = new Mock<ILogger<AnalyticsService>>();

            var analyticsService = new AnalyticsService(
                mockUnitOfWork.Object,
                mockLogger.Object,
                mockCacheService.Object
            );

            var invalidCampaignId = Guid.NewGuid();

            mockUnitOfWork.Setup(u => u.Campaigns.GetByIdAsync(invalidCampaignId))
                .ReturnsAsync((Campaign)null);

            var query = new GetCampaignStatsQueryDto
            {
                FromDate = DateTime.UtcNow.AddDays(-30),
                ToDate = DateTime.UtcNow
            };

            var result = await analyticsService.GetCampaignStatsAsync(invalidCampaignId, query);

            // Verify failure
            Assert.False(result.Success, "Stats retrieval should fail for invalid campaign");
            Assert.Contains("not found", result.Message.ToLower(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Property 26 Edge Case: Invalid Date Range
        /// For any stats query with ToDate < FromDate, retrieval should fail with ArgumentException.
        /// **Validates: Requirements 7.4**
        /// </summary>
        [Fact]
        public async Task Property_26_EdgeCase_InvalidDateRange()
        {
            // **Feature: campaign-promotion-api, Property 26: Campaign Stats Calculation (Edge Case)**
            // **Validates: Requirements 7.4**

            var mockUnitOfWork = CreateMockUnitOfWork();
            var mockCacheService = CreateMockCacheService();
            var mockLogger = new Mock<ILogger<AnalyticsService>>();

            var analyticsService = new AnalyticsService(
                mockUnitOfWork.Object,
                mockLogger.Object,
                mockCacheService.Object
            );

            var campaignId = Guid.NewGuid();
            var campaign = new Campaign
            {
                Id = campaignId,
                CampaignName = "Test Campaign",
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 1000000,
                Status = Core.Enums.Marketing.CampaignStatus.ACTIVE
            };

            mockUnitOfWork.Setup(u => u.Campaigns.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            var query = new GetCampaignStatsQueryDto
            {
                FromDate = DateTime.UtcNow,
                ToDate = DateTime.UtcNow.AddDays(-30) // ToDate < FromDate
            };

            var result = await analyticsService.GetCampaignStatsAsync(campaignId, query);

            // Verify failure
            Assert.False(result.Success, "Stats retrieval should fail for invalid date range");
        }
    }
}
