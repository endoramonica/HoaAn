using AutoMapper;
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
    /// Property-based tests for Campaign Impression Tracking
    /// **Feature: campaign-promotion-api, Property 23: Impression Tracking Records Events**
    /// **Validates: Requirements 7.1**
    /// </summary>
    public class ImpressionTrackingPropertyTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICampaignRepository> _mockCampaignRepository;
        private readonly Mock<ICampaignImpressionRepository> _mockImpressionRepository;
        private readonly Mock<ILogger<AnalyticsService>> _mockLogger;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly AnalyticsService _analyticsService;

        public ImpressionTrackingPropertyTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCampaignRepository = new Mock<ICampaignRepository>();
            _mockImpressionRepository = new Mock<ICampaignImpressionRepository>();
            _mockLogger = new Mock<ILogger<AnalyticsService>>();
            _mockCacheService = new Mock<ICacheService>();

            // Setup UnitOfWork
            _mockUnitOfWork.Setup(u => u.Campaigns).Returns(_mockCampaignRepository.Object);
            _mockUnitOfWork.Setup(u => u.CampaignImpressions).Returns(_mockImpressionRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Setup cache service
            _mockCacheService.DefaultValue = DefaultValue.Mock;

            _analyticsService = new AnalyticsService(
                _mockUnitOfWork.Object,
                _mockLogger.Object,
                _mockCacheService.Object
            );
        }

        /// <summary>
        /// Property 23: Impression Tracking Records Events
        /// For any active campaign, tracking an impression should result in a CampaignImpression record being created.
        /// **Validates: Requirements 7.1**
        /// </summary>
        [Fact]
        public async Task Property_23_ImpressionTrackingRecordsEvents()
        {
            // **Feature: campaign-promotion-api, Property 23: Impression Tracking Records Events**
            // **Validates: Requirements 7.1**

            var faker = new Faker();
            var campaignId = Guid.NewGuid();

            // Setup campaign
            var campaign = new Campaign
            {
                Id = campaignId,
                CampaignName = faker.Commerce.ProductName(),
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(10),
                Budget = 1000000,
                Status = Core.Enums.Marketing.CampaignStatus.ACTIVE
            };

            _mockCampaignRepository
                .Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            // Run 100 iterations with random data
            for (int i = 0; i < 100; i++)
            {
                // Generate random impression data
                var sessionId = faker.Random.Guid().ToString();
                var page = faker.Internet.Url();
                var recordedAt = DateTime.UtcNow.AddSeconds(-faker.Random.Int(0, 3600));

                var dto = new TrackImpressionDto
                {
                    SessionId = sessionId,
                    Page = page,
                    RecordedAt = recordedAt
                };

                // Track impression
                var result = await _analyticsService.TrackImpressionAsync(campaignId, dto);

                // Verify success
                Assert.True(result.Success, $"Iteration {i}: Impression tracking should succeed");
                Assert.True(result.Data, $"Iteration {i}: Result data should be true");

                // Verify impression was added to repository
                _mockImpressionRepository.Verify(
                    r => r.AddAsync(It.Is<CampaignImpression>(x =>
                        x.CampaignId == campaignId &&
                        x.SessionId == sessionId &&
                        x.Page == page &&
                        x.RecordedAt == recordedAt
                    )),
                    Times.Once,
                    $"Iteration {i}: Impression should be added to repository"
                );

                // Reset mocks for next iteration
                _mockImpressionRepository.Reset();
                _mockUnitOfWork.Reset();
                _mockUnitOfWork.Setup(u => u.Campaigns).Returns(_mockCampaignRepository.Object);
                _mockUnitOfWork.Setup(u => u.CampaignImpressions).Returns(_mockImpressionRepository.Object);
                _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
                _mockCampaignRepository.Setup(r => r.GetByIdAsync(campaignId)).ReturnsAsync(campaign);
            }
        }

        /// <summary>
        /// Property 23 Edge Case: Invalid Campaign ID
        /// For any invalid campaign ID, tracking an impression should fail with KeyNotFoundException.
        /// **Validates: Requirements 7.1**
        /// </summary>
        [Fact]
        public async Task Property_23_EdgeCase_InvalidCampaignId()
        {
            // **Feature: campaign-promotion-api, Property 23: Impression Tracking Records Events (Edge Case)**
            // **Validates: Requirements 7.1**

            var invalidCampaignId = Guid.NewGuid();

            _mockCampaignRepository
                .Setup(r => r.GetByIdAsync(invalidCampaignId))
                .ReturnsAsync((Campaign)null);

            var dto = new TrackImpressionDto
            {
                SessionId = "session-123",
                Page = "home"
            };

            // Track impression with invalid campaign
            var result = await _analyticsService.TrackImpressionAsync(invalidCampaignId, dto);

            // Verify failure
            Assert.False(result.Success, "Impression tracking should fail for invalid campaign");
            Assert.Contains("not found", result.Message.ToLower(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Property 23 Edge Case: Empty Session ID
        /// For any impression with empty session ID, tracking should fail with validation error.
        /// **Validates: Requirements 7.1**
        /// </summary>
        [Fact]
        public async Task Property_23_EdgeCase_EmptySessionId()
        {
            // **Feature: campaign-promotion-api, Property 23: Impression Tracking Records Events (Edge Case)**
            // **Validates: Requirements 7.1**

            var campaignId = Guid.NewGuid();
            var campaign = new Campaign
            {
                Id = campaignId,
                CampaignName = "Test Campaign",
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(10),
                Budget = 1000000,
                Status = Core.Enums.Marketing.CampaignStatus.ACTIVE
            };

            _mockCampaignRepository
                .Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            var dto = new TrackImpressionDto
            {
                SessionId = "", // Empty session ID
                Page = "home"
            };

            // Track impression with empty session ID
            var result = await _analyticsService.TrackImpressionAsync(campaignId, dto);

            // Verify failure
            Assert.False(result.Success, "Impression tracking should fail for empty session ID");
        }

        /// <summary>
        /// Property 23 Edge Case: Empty Page
        /// For any impression with empty page, tracking should fail with validation error.
        /// **Validates: Requirements 7.1**
        /// </summary>
        [Fact]
        public async Task Property_23_EdgeCase_EmptyPage()
        {
            // **Feature: campaign-promotion-api, Property 23: Impression Tracking Records Events (Edge Case)**
            // **Validates: Requirements 7.1**

            var campaignId = Guid.NewGuid();
            var campaign = new Campaign
            {
                Id = campaignId,
                CampaignName = "Test Campaign",
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(10),
                Budget = 1000000,
                Status = Core.Enums.Marketing.CampaignStatus.ACTIVE
            };

            _mockCampaignRepository
                .Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            var dto = new TrackImpressionDto
            {
                SessionId = "session-123",
                Page = "" // Empty page
            };

            // Track impression with empty page
            var result = await _analyticsService.TrackImpressionAsync(campaignId, dto);

            // Verify failure
            Assert.False(result.Success, "Impression tracking should fail for empty page");
        }
    }
}
