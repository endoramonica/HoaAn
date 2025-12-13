using AutoMapper;
using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using VietCommerce.Application.Mappings;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Property-based tests for Campaign Status Lifecycle
    /// Validates: Requirements 6.1, 6.2, 6.3, 6.4, 6.5
    /// </summary>
    public class CampaignStatusLifecyclePropertyTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICampaignRepository> _mockCampaignRepository;
        private readonly Mock<ILogger<CampaignService>> _mockLogger;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly IMapper _mapper;
        private readonly CampaignService _campaignService;

        public CampaignStatusLifecyclePropertyTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCampaignRepository = new Mock<ICampaignRepository>();
            _mockLogger = new Mock<ILogger<CampaignService>>();
            _mockCacheService = new Mock<ICacheService>();

            // Setup mapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CampaignMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            // Setup UnitOfWork
            _mockUnitOfWork.Setup(u => u.Campaigns).Returns(_mockCampaignRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Setup cache service
            _mockCacheService.DefaultValue = DefaultValue.Mock;

            _campaignService = new CampaignService(
                _mockUnitOfWork.Object,
                _mapper,
                _mockLogger.Object,
                _mockCacheService.Object
            );
        }

        /// <summary>
        /// Property 18: Campaign Initial Status is DRAFT
        /// **Feature: campaign-promotion-api, Property 18: Campaign Initial Status is DRAFT**
        /// **Validates: Requirements 6.1**
        /// 
        /// For any newly created campaign, the initial status should be DRAFT.
        /// </summary>
        [Fact]
        public async Task Property_18_CampaignInitialStatusIsDraft()
        {
            // Arrange
            var faker = new Faker<CreateCampaignDto>();
            faker
                .RuleFor(x => x.StoreId, f => Guid.NewGuid())
                .RuleFor(x => x.CampaignName, f => f.Commerce.ProductName())
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.CampaignType, f => f.PickRandom<CampaignType>())
                .RuleFor(x => x.StartDate, f => f.Date.Future(1).ToUniversalTime())
                .RuleFor(x => x.EndDate, (f, dto) => dto.StartDate.AddDays(f.Random.Int(1, 30)))
                .RuleFor(x => x.Budget, f => f.Random.Decimal(1000, 100000000))
                .RuleFor(x => x.TargetingRules, f => "{}");

            // Act & Assert - Run 100 iterations
            for (int i = 0; i < 100; i++)
            {
                var createDto = faker.Generate();

                var createdCampaign = new Campaign
                {
                    Id = Guid.NewGuid(),
                    StoreId = createDto.StoreId,
                    CampaignName = createDto.CampaignName,
                    Description = createDto.Description,
                    CampaignType = createDto.CampaignType,
                    StartDate = createDto.StartDate,
                    EndDate = createDto.EndDate,
                    Budget = createDto.Budget,
                    Status = CampaignStatus.DRAFT,
                    CreatedAt = DateTime.UtcNow
                };

                _mockCampaignRepository
                    .Setup(r => r.AddAsync(It.IsAny<Campaign>()))
                    .ReturnsAsync(createdCampaign);

                var result = await _campaignService.CreateCampaignAsync(createDto);

                // Verify initial status is DRAFT
                Assert.True(result.Success);
                Assert.NotNull(result.Data);
                Assert.Equal(CampaignStatus.DRAFT, result.Data.Status);
            }
        }

        /// <summary>
        /// Property 20: Status Transition Validation
        /// **Feature: campaign-promotion-api, Property 20: Status Transition Validation**
        /// **Validates: Requirements 6.3**
        /// 
        /// For any campaign transitioning from DRAFT to ACTIVE, all required fields must be set 
        /// or transition should fail with HTTP 400 error.
        /// </summary>
        [Fact]
        public async Task Property_20_StatusTransitionValidation()
        {
            // Test 1: Valid transition from DRAFT to ACTIVE with all required fields
            for (int i = 0; i < 50; i++)
            {
                // Arrange - Valid campaign with all required fields
                var campaignId = Guid.NewGuid();
                var campaign = new Campaign
                {
                    Id = campaignId,
                    StoreId = Guid.NewGuid(),
                    CampaignName = $"Valid Campaign {i}",
                    Status = CampaignStatus.DRAFT,
                    StartDate = DateTime.UtcNow.AddDays(1),
                    EndDate = DateTime.UtcNow.AddDays(30),
                    Budget = 5000000
                };

                _mockCampaignRepository
                    .Setup(r => r.GetByIdAsync(campaignId))
                    .ReturnsAsync(campaign);

                // Act
                var result = await _campaignService.ChangeCampaignStatusAsync(campaignId, (int)CampaignStatus.ACTIVE);

                // Assert - Should succeed
                Assert.True(result.Success);
                Assert.Equal(CampaignStatus.ACTIVE, result.Data.Status);
            }

            // Test 2: Invalid transition from DRAFT to ACTIVE with missing campaign name
            for (int i = 0; i < 50; i++)
            {
                // Arrange - Campaign with missing name
                var campaignId = Guid.NewGuid();
                var campaign = new Campaign
                {
                    Id = campaignId,
                    StoreId = Guid.NewGuid(),
                    CampaignName = "", // Missing name
                    Status = CampaignStatus.DRAFT,
                    StartDate = DateTime.UtcNow.AddDays(1),
                    EndDate = DateTime.UtcNow.AddDays(30),
                    Budget = 5000000
                };

                _mockCampaignRepository
                    .Setup(r => r.GetByIdAsync(campaignId))
                    .ReturnsAsync(campaign);

                // Act
                var result = await _campaignService.ChangeCampaignStatusAsync(campaignId, (int)CampaignStatus.ACTIVE);

                // Assert - Should fail
                Assert.False(result.Success);
                Assert.Contains("Cannot activate campaign", result.Message);
            }

            // Test 3: Invalid transition from DRAFT to ACTIVE with negative budget
            for (int i = 0; i < 50; i++)
            {
                // Arrange - Campaign with negative budget
                var campaignId = Guid.NewGuid();
                var campaign = new Campaign
                {
                    Id = campaignId,
                    StoreId = Guid.NewGuid(),
                    CampaignName = $"Campaign {i}",
                    Status = CampaignStatus.DRAFT,
                    StartDate = DateTime.UtcNow.AddDays(1),
                    EndDate = DateTime.UtcNow.AddDays(30),
                    Budget = -1000 // Negative budget
                };

                _mockCampaignRepository
                    .Setup(r => r.GetByIdAsync(campaignId))
                    .ReturnsAsync(campaign);

                // Act
                var result = await _campaignService.ChangeCampaignStatusAsync(campaignId, (int)CampaignStatus.ACTIVE);

                // Assert - Should fail
                Assert.False(result.Success);
                Assert.Contains("Cannot activate campaign", result.Message);
            }

            // Test 4: Invalid transition from DRAFT to ACTIVE with invalid date range
            for (int i = 0; i < 50; i++)
            {
                // Arrange - Campaign with invalid date range
                var campaignId = Guid.NewGuid();
                var campaign = new Campaign
                {
                    Id = campaignId,
                    StoreId = Guid.NewGuid(),
                    CampaignName = $"Campaign {i}",
                    Status = CampaignStatus.DRAFT,
                    StartDate = DateTime.UtcNow.AddDays(30),
                    EndDate = DateTime.UtcNow.AddDays(1), // EndDate < StartDate
                    Budget = 5000000
                };

                _mockCampaignRepository
                    .Setup(r => r.GetByIdAsync(campaignId))
                    .ReturnsAsync(campaign);

                // Act
                var result = await _campaignService.ChangeCampaignStatusAsync(campaignId, (int)CampaignStatus.ACTIVE);

                // Assert - Should fail
                Assert.False(result.Success);
                Assert.Contains("Cannot activate campaign", result.Message);
            }
        }

        /// <summary>
        /// Property 22: Automatic Status Completion
        /// **Feature: campaign-promotion-api, Property 22: Automatic Status Completion**
        /// **Validates: Requirements 6.5**
        /// 
        /// For any campaign with EndDate in the past, the status should be automatically 
        /// transitioned to COMPLETED.
        /// 
        /// Note: This property tests the business logic that should be implemented in a 
        /// background job or scheduled task. For now, we test that campaigns with past 
        /// end dates can be manually transitioned to COMPLETED.
        /// </summary>
        [Fact]
        public async Task Property_22_AutomaticStatusCompletion()
        {
            // Arrange - Generate campaigns with past end dates
            var faker = new Faker<Campaign>();
            faker
                .RuleFor(x => x.Id, f => Guid.NewGuid())
                .RuleFor(x => x.StoreId, f => Guid.NewGuid())
                .RuleFor(x => x.CampaignName, f => f.Commerce.ProductName())
                .RuleFor(x => x.Status, f => CampaignStatus.ACTIVE)
                .RuleFor(x => x.StartDate, f => f.Date.Past(60).ToUniversalTime())
                .RuleFor(x => x.EndDate, (f, c) => c.StartDate.AddDays(f.Random.Int(1, 30)))
                .RuleFor(x => x.Budget, f => f.Random.Decimal(1000, 100000000))
                .RuleFor(x => x.CreatedAt, f => f.Date.Past(90).ToUniversalTime());

            // Act & Assert - Run 100 iterations
            for (int i = 0; i < 100; i++)
            {
                var campaign = faker.Generate();
                
                // Ensure EndDate is in the past
                campaign.EndDate = DateTime.UtcNow.AddDays(-1);

                _mockCampaignRepository
                    .Setup(r => r.GetByIdAsync(campaign.Id))
                    .ReturnsAsync(campaign);

                // Transition to COMPLETED
                var result = await _campaignService.ChangeCampaignStatusAsync(campaign.Id, (int)CampaignStatus.COMPLETED);

                // Verify status is COMPLETED
                Assert.True(result.Success);
                Assert.Equal(CampaignStatus.COMPLETED, result.Data.Status);
            }
        }
    }
}
