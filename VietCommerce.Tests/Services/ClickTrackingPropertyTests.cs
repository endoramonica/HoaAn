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
    /// Property-based tests for Campaign Click Tracking
    /// **Feature: campaign-promotion-api, Property 24: Click Tracking Records Events**
    /// **Validates: Requirements 7.2**
    /// </summary>
    public class ClickTrackingPropertyTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICampaignRepository> _mockCampaignRepository;
        private readonly Mock<ICampaignClickRepository> _mockClickRepository;
        private readonly Mock<ILogger<AnalyticsService>> _mockLogger;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly AnalyticsService _analyticsService;

        public ClickTrackingPropertyTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCampaignRepository = new Mock<ICampaignRepository>();
            _mockClickRepository = new Mock<ICampaignClickRepository>();
            _mockLogger = new Mock<ILogger<AnalyticsService>>();
            _mockCacheService = new Mock<ICacheService>();

            // Setup UnitOfWork
            _mockUnitOfWork.Setup(u => u.Campaigns).Returns(_mockCampaignRepository.Object);
            _mockUnitOfWork.Setup(u => u.CampaignClicks).Returns(_mockClickRepository.Object);
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
        /// Property 24: Click Tracking Records Events
        /// For any active campaign, tracking a click should result in a CampaignClick record being created.
        /// **Validates: Requirements 7.2**
        /// </summary>
        [Fact]
        public async Task Property_24_ClickTrackingRecordsEvents()
        {
            // **Feature: campaign-promotion-api, Property 24: Click Tracking Records Events**
            // **Validates: Requirements 7.2**

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
                // Generate random click data
                var sessionId = faker.Random.Guid().ToString();
                var page = faker.Internet.Url();
                var recordedAt = DateTime.UtcNow.AddSeconds(-faker.Random.Int(0, 3600));

                var dto = new TrackClickDto
                {
                    SessionId = sessionId,
                    Page = page,
                    RecordedAt = recordedAt
                };

                // Track click
                var result = await _analyticsService.TrackClickAsync(campaignId, dto);

                // Verify success
                Assert.True(result.Success, $"Iteration {i}: Click tracking should succeed");
                Assert.True(result.Data, $"Iteration {i}: Result data should be true");

                // Verify click was added to repository
                _mockClickRepository.Verify(
                    r => r.AddAsync(It.Is<CampaignClick>(x =>
                        x.CampaignId == campaignId &&
                        x.SessionId == sessionId &&
                        x.Page == page &&
                        x.RecordedAt == recordedAt
                    )),
                    Times.Once,
                    $"Iteration {i}: Click should be added to repository"
                );

                // Reset mocks for next iteration
                _mockClickRepository.Reset();
                _mockUnitOfWork.Reset();
                _mockUnitOfWork.Setup(u => u.Campaigns).Returns(_mockCampaignRepository.Object);
                _mockUnitOfWork.Setup(u => u.CampaignClicks).Returns(_mockClickRepository.Object);
                _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
                _mockCampaignRepository.Setup(r => r.GetByIdAsync(campaignId)).ReturnsAsync(campaign);
            }
        }

        /// <summary>
        /// Property 24 Edge Case: Invalid Campaign ID
        /// For any invalid campaign ID, tracking a click should fail with KeyNotFoundException.
        /// **Validates: Requirements 7.2**
        /// </summary>
        [Fact]
        public async Task Property_24_EdgeCase_InvalidCampaignId()
        {
            // **Feature: campaign-promotion-api, Property 24: Click Tracking Records Events (Edge Case)**
            // **Validates: Requirements 7.2**

            var invalidCampaignId = Guid.NewGuid();

            _mockCampaignRepository
                .Setup(r => r.GetByIdAsync(invalidCampaignId))
                .ReturnsAsync((Campaign)null);

            var dto = new TrackClickDto
            {
                SessionId = "session-123",
                Page = "home"
            };

            // Track click with invalid campaign
            var result = await _analyticsService.TrackClickAsync(invalidCampaignId, dto);

            // Verify failure
            Assert.False(result.Success, "Click tracking should fail for invalid campaign");
            Assert.Contains("not found", result.Message.ToLower(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Property 24 Edge Case: Empty Session ID
        /// For any click with empty session ID, tracking should fail with validation error.
        /// **Validates: Requirements 7.2**
        /// </summary>
        [Fact]
        public async Task Property_24_EdgeCase_EmptySessionId()
        {
            // **Feature: campaign-promotion-api, Property 24: Click Tracking Records Events (Edge Case)**
            // **Validates: Requirements 7.2**

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

            var dto = new TrackClickDto
            {
                SessionId = "", // Empty session ID
                Page = "home"
            };

            // Track click with empty session ID
            var result = await _analyticsService.TrackClickAsync(campaignId, dto);

            // Verify failure
            Assert.False(result.Success, "Click tracking should fail for empty session ID");
        }

        /// <summary>
        /// Property 24 Edge Case: Empty Page
        /// For any click with empty page, tracking should fail with validation error.
        /// **Validates: Requirements 7.2**
        /// </summary>
        [Fact]
        public async Task Property_24_EdgeCase_EmptyPage()
        {
            // **Feature: campaign-promotion-api, Property 24: Click Tracking Records Events (Edge Case)**
            // **Validates: Requirements 7.2**

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

            var dto = new TrackClickDto
            {
                SessionId = "session-123",
                Page = "" // Empty page
            };

            // Track click with empty page
            var result = await _analyticsService.TrackClickAsync(campaignId, dto);

            // Verify failure
            Assert.False(result.Success, "Click tracking should fail for empty page");
        }
    }
}
