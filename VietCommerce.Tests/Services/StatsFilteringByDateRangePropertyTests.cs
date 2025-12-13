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
    /// Property-based tests for Stats Filtering by Date Range
    /// **Feature: campaign-promotion-api, Property 27: Stats Filtering by Date Range**
    /// **Validates: Requirements 7.5**
    /// </summary>
    public class StatsFilteringByDateRangePropertyTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICampaignRepository> _mockCampaignRepository;
        private readonly Mock<ICampaignImpressionRepository> _mockImpressionRepository;
        private readonly Mock<ICampaignClickRepository> _mockClickRepository;
        private readonly Mock<IVoucherRedemptionRepository> _mockRedemptionRepository;
        private readonly Mock<ILogger<AnalyticsService>> _mockLogger;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly AnalyticsService _analyticsService;

        public StatsFilteringByDateRangePropertyTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCampaignRepository = new Mock<ICampaignRepository>();
            _mockImpressionRepository = new Mock<ICampaignImpressionRepository>();
            _mockClickRepository = new Mock<ICampaignClickRepository>();
            _mockRedemptionRepository = new Mock<IVoucherRedemptionRepository>();
            _mockLogger = new Mock<ILogger<AnalyticsService>>();
            _mockCacheService = new Mock<ICacheService>();

            // Setup UnitOfWork
            _mockUnitOfWork.Setup(u => u.Campaigns).Returns(_mockCampaignRepository.Object);
            _mockUnitOfWork.Setup(u => u.CampaignImpressions).Returns(_mockImpressionRepository.Object);
            _mockUnitOfWork.Setup(u => u.CampaignClicks).Returns(_mockClickRepository.Object);
            _mockUnitOfWork.Setup(u => u.VoucherRedemptions).Returns(_mockRedemptionRepository.Object);

            // Setup cache service - disable caching for tests
            _mockCacheService.DefaultValue = DefaultValue.Mock;
            _mockCacheService
                .Setup(c => c.GetAsync<CampaignStatsDto>(It.IsAny<string>()))
                .ReturnsAsync((CampaignStatsDto)null!);

            _analyticsService = new AnalyticsService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockCacheService.Object
            );
        }

        /// <summary>
        /// Property 27: Stats Filtering by Date Range
        /// For any campaign stats request with date range filters, only events within the range should be included in calculations.
        /// **Validates: Requirements 7.5**
        /// </summary>
        [Fact]
        public async Task Property_27_StatsFilteringByDateRange()
        {
            // **Feature: campaign-promotion-api, Property 27: Stats Filtering by Date Range**
            // **Validates: Requirements 7.5**

            var faker = new Faker();
            var campaignId = Guid.NewGuid();
            var campaignStartDate = DateTime.UtcNow.AddDays(-60);
            var campaignEndDate = DateTime.UtcNow.AddDays(30);

            // Setup campaign
            var campaign = new Campaign
            {
                Id = campaignId,
                CampaignName = faker.Commerce.ProductName(),
                StartDate = campaignStartDate,
                EndDate = campaignEndDate,
                Budget = 1000000,
                Status = Core.Enums.Marketing.CampaignStatus.ACTIVE
            };

            _mockCampaignRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(campaign);

            // Run 100 iterations with random date ranges
            for (int i = 0; i < 100; i++)
            {
                // Generate random date range within campaign period
                var rangeStart = campaignStartDate.AddDays(faker.Random.Int(0, 30));
                var rangeEnd = rangeStart.AddDays(faker.Random.Int(1, 20));

                // Generate random stats data
                var impressionCountInRange = faker.Random.Int(10, 500);
                var clickCountInRange = faker.Random.Int(1, Math.Min(impressionCountInRange, 200));
                var redemptionCountInRange = faker.Random.Int(0, Math.Min(clickCountInRange, 50));
                var totalDiscountInRange = faker.Random.Decimal(0, 500000);

                // Setup repository methods to return data only for the specified range
                _mockImpressionRepository
                    .Setup(r => r.GetImpressionCountByDateRangeAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>()))
                    .ReturnsAsync(impressionCountInRange);

                _mockClickRepository
                    .Setup(r => r.GetClickCountByDateRangeAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>()))
                    .ReturnsAsync(clickCountInRange);

                // Create redemption records within the range
                var redemptions = new List<VoucherRedemption>();
                for (int j = 0; j < redemptionCountInRange; j++)
                {
                    var redemptionDate = rangeStart.AddSeconds(faker.Random.Int(0, (int)(rangeEnd - rangeStart).TotalSeconds));
                    redemptions.Add(new VoucherRedemption
                    {
                        Id = Guid.NewGuid(),
                        VoucherId = Guid.NewGuid(),
                        DiscountAmount = totalDiscountInRange / redemptionCountInRange,
                        RedeemedAt = redemptionDate
                    });
                }

                _mockRedemptionRepository
                    .Setup(r => r.GetRedemptionsByDateRangeAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>()))
                    .ReturnsAsync(redemptions);

                // Get stats with specific date range
                var query = new GetCampaignStatsQueryDto
                {
                    FromDate = rangeStart,
                    ToDate = rangeEnd
                };

                var result = await _analyticsService.GetCampaignStatsAsync(campaignId, query);

                // Verify success
                Assert.True(result.Success, $"Iteration {i}: Stats retrieval should succeed");

                // Verify only data within range is included
                Assert.Equal(impressionCountInRange, result.Data.ImpressionCount);
                Assert.Equal(clickCountInRange, result.Data.ClickCount);
                Assert.Equal(redemptionCountInRange, result.Data.RedemptionCount);

                // Verify date range is set correctly in response
                Assert.Equal(rangeStart, result.Data.FromDate);
                Assert.Equal(rangeEnd, result.Data.ToDate);

                // Verify calculations are based on filtered data
                var expectedCTR = impressionCountInRange > 0 ? (double)clickCountInRange / impressionCountInRange * 100 : 0;
                Assert.Equal(expectedCTR, result.Data.ClickThroughRate, 2);

                var expectedRedemptionRate = clickCountInRange > 0 ? (double)redemptionCountInRange / clickCountInRange * 100 : 0;
                Assert.Equal(expectedRedemptionRate, result.Data.RedemptionRate, 2);

                var expectedConversionRate = impressionCountInRange > 0 ? (double)redemptionCountInRange / impressionCountInRange * 100 : 0;
                Assert.Equal(expectedConversionRate, result.Data.ConversionRate, 2);
            }
        }

        /// <summary>
        /// Property 27 Edge Case: Default Date Range
        /// For any stats query without explicit date range, the system should use campaign start date and current time.
        /// **Validates: Requirements 7.5**
        /// </summary>
        [Fact]
        public async Task Property_27_EdgeCase_DefaultDateRange()
        {
            // **Feature: campaign-promotion-api, Property 27: Stats Filtering by Date Range (Edge Case)**
            // **Validates: Requirements 7.5**

            var campaignId = Guid.NewGuid();
            var campaignStartDate = DateTime.UtcNow.AddDays(-30);
            var campaignEndDate = DateTime.UtcNow.AddDays(30);

            var campaign = new Campaign
            {
                Id = campaignId,
                CampaignName = "Test Campaign",
                StartDate = campaignStartDate,
                EndDate = campaignEndDate,
                Budget = 1000000,
                Status = Core.Enums.Marketing.CampaignStatus.ACTIVE
            };

            _mockCampaignRepository
                .Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            // Setup repository methods - should be called with campaign start date and current time
            _mockImpressionRepository
                .Setup(r => r.GetImpressionCountByDateRangeAsync(
                    campaignId,
                    It.Is<DateTime>(d => d.Date == campaignStartDate.Date),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(100);

            _mockClickRepository
                .Setup(r => r.GetClickCountByDateRangeAsync(
                    campaignId,
                    It.Is<DateTime>(d => d.Date == campaignStartDate.Date),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(50);

            _mockRedemptionRepository
                .Setup(r => r.GetRedemptionsByDateRangeAsync(
                    campaignId,
                    It.Is<DateTime>(d => d.Date == campaignStartDate.Date),
                    It.IsAny<DateTime>()))
                .ReturnsAsync(new List<VoucherRedemption>
                {
                    new VoucherRedemption { Id = Guid.NewGuid(), VoucherId = Guid.NewGuid(), DiscountAmount = 1000 }
                });

            // Get stats without explicit date range
            var query = new GetCampaignStatsQueryDto
            {
                FromDate = null,
                ToDate = null
            };

            var result = await _analyticsService.GetCampaignStatsAsync(campaignId, query);

            // Verify success
            Assert.True(result.Success, "Stats retrieval should succeed with default date range");

            // Verify data was retrieved
            Assert.Equal(100, result.Data.ImpressionCount);
            Assert.Equal(50, result.Data.ClickCount);
            Assert.Equal(1, result.Data.RedemptionCount);
        }

        /// <summary>
        /// Property 27 Edge Case: Single Day Range
        /// For any stats query with FromDate == ToDate, the system should return stats for that single day.
        /// **Validates: Requirements 7.5**
        /// </summary>
        [Fact]
        public async Task Property_27_EdgeCase_SingleDayRange()
        {
            // **Feature: campaign-promotion-api, Property 27: Stats Filtering by Date Range (Edge Case)**
            // **Validates: Requirements 7.5**

            var campaignId = Guid.NewGuid();
            var singleDay = DateTime.UtcNow.Date;

            var campaign = new Campaign
            {
                Id = campaignId,
                CampaignName = "Test Campaign",
                StartDate = singleDay.AddDays(-30),
                EndDate = singleDay.AddDays(30),
                Budget = 1000000,
                Status = Core.Enums.Marketing.CampaignStatus.ACTIVE
            };

            _mockCampaignRepository
                .Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            _mockImpressionRepository
                .Setup(r => r.GetImpressionCountByDateRangeAsync(
                    campaignId,
                    singleDay,
                    singleDay))
                .ReturnsAsync(25);

            _mockClickRepository
                .Setup(r => r.GetClickCountByDateRangeAsync(
                    campaignId,
                    singleDay,
                    singleDay))
                .ReturnsAsync(10);

            _mockRedemptionRepository
                .Setup(r => r.GetRedemptionsByDateRangeAsync(
                    campaignId,
                    singleDay,
                    singleDay))
                .ReturnsAsync(new List<VoucherRedemption>());

            var query = new GetCampaignStatsQueryDto
            {
                FromDate = singleDay,
                ToDate = singleDay
            };

            var result = await _analyticsService.GetCampaignStatsAsync(campaignId, query);

            // Verify success
            Assert.True(result.Success, "Stats retrieval should succeed for single day range");

            // Verify data for single day
            Assert.Equal(25, result.Data.ImpressionCount);
            Assert.Equal(10, result.Data.ClickCount);
            Assert.Equal(0, result.Data.RedemptionCount);
        }

        /// <summary>
        /// Property 27 Edge Case: Empty Date Range
        /// For any stats query with a date range that has no events, the system should return zero counts.
        /// **Validates: Requirements 7.5**
        /// </summary>
        [Fact]
        public async Task Property_27_EdgeCase_EmptyDateRange()
        {
            // **Feature: campaign-promotion-api, Property 27: Stats Filtering by Date Range (Edge Case)**
            // **Validates: Requirements 7.5**

            var campaignId = Guid.NewGuid();
            var emptyRangeStart = DateTime.UtcNow.AddDays(-100);
            var emptyRangeEnd = DateTime.UtcNow.AddDays(-90);

            var campaign = new Campaign
            {
                Id = campaignId,
                CampaignName = "Test Campaign",
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 1000000,
                Status = Core.Enums.Marketing.CampaignStatus.ACTIVE
            };

            _mockCampaignRepository
                .Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            _mockImpressionRepository
                .Setup(r => r.GetImpressionCountByDateRangeAsync(
                    campaignId,
                    emptyRangeStart,
                    emptyRangeEnd))
                .ReturnsAsync(0);

            _mockClickRepository
                .Setup(r => r.GetClickCountByDateRangeAsync(
                    campaignId,
                    emptyRangeStart,
                    emptyRangeEnd))
                .ReturnsAsync(0);

            _mockRedemptionRepository
                .Setup(r => r.GetRedemptionsByDateRangeAsync(
                    campaignId,
                    emptyRangeStart,
                    emptyRangeEnd))
                .ReturnsAsync(new List<VoucherRedemption>());

            var query = new GetCampaignStatsQueryDto
            {
                FromDate = emptyRangeStart,
                ToDate = emptyRangeEnd
            };

            var result = await _analyticsService.GetCampaignStatsAsync(campaignId, query);

            // Verify success
            Assert.True(result.Success, "Stats retrieval should succeed for empty date range");

            // Verify zero counts
            Assert.Equal(0, result.Data.ImpressionCount);
            Assert.Equal(0, result.Data.ClickCount);
            Assert.Equal(0, result.Data.RedemptionCount);
            Assert.Equal(0, result.Data.ClickThroughRate);
            Assert.Equal(0, result.Data.RedemptionRate);
            Assert.Equal(0, result.Data.ConversionRate);
        }
    }
}
