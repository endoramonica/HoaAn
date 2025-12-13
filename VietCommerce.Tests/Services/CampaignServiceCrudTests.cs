using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using VietCommerce.Application.Mappings;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Enums.Marketing;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Unit tests for CampaignService CRUD operations
    /// Validates: Requirements 1.1, 1.2, 1.3, 1.4, 1.5
    /// </summary>
    public class CampaignServiceCrudTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICampaignRepository> _mockCampaignRepository;
        private readonly Mock<ILogger<CampaignService>> _mockLogger;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly IMapper _mapper;
        private readonly CampaignService _campaignService;

        public CampaignServiceCrudTests()
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

            // Setup cache service - use loose mock to avoid optional parameter issues
            _mockCacheService.DefaultValue = DefaultValue.Mock;

            _campaignService = new CampaignService(
                _mockUnitOfWork.Object,
                _mapper,
                _mockLogger.Object,
                _mockCacheService.Object
            );
        }

        #region CreateCampaignAsync Tests

        [Fact]
        public async Task CreateCampaignAsync_WithValidData_ShouldCreateCampaign()
        {
            // Arrange
            var storeId = Guid.NewGuid();
            var createDto = new CreateCampaignDto
            {
                StoreId = storeId,
                CampaignName = "Summer Sale 2025",
                Description = "Summer promotional campaign",
                CampaignType = CampaignType.SEASONAL,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 10000000,
                TargetingRules = "{}"
            };

            var createdCampaign = new Campaign
            {
                Id = Guid.NewGuid(),
                StoreId = storeId,
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

            // Act
            var result = await _campaignService.CreateCampaignAsync(createDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(createDto.CampaignName, result.Data.CampaignName);
            Assert.Equal(CampaignStatus.DRAFT, result.Data.Status);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateCampaignAsync_WithInvalidDateRange_ShouldReturnError()
        {
            // Arrange
            var createDto = new CreateCampaignDto
            {
                StoreId = Guid.NewGuid(),
                CampaignName = "Invalid Campaign",
                CampaignType = CampaignType.SEASONAL,
                StartDate = DateTime.UtcNow.AddDays(30),
                EndDate = DateTime.UtcNow.AddDays(1), // EndDate < StartDate
                Budget = 10000000
            };

            // Act
            var result = await _campaignService.CreateCampaignAsync(createDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("EndDate must be greater than StartDate", result.Message);
        }

        [Fact]
        public async Task CreateCampaignAsync_WithNegativeBudget_ShouldReturnError()
        {
            // Arrange
            var createDto = new CreateCampaignDto
            {
                StoreId = Guid.NewGuid(),
                CampaignName = "Invalid Budget Campaign",
                CampaignType = CampaignType.SEASONAL,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = -1000 // Negative budget
            };

            // Act
            var result = await _campaignService.CreateCampaignAsync(createDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Budget must be non-negative", result.Message);
        }

        #endregion

        #region GetCampaignsAsync Tests

        [Fact]
        public async Task GetCampaignsAsync_WithValidQuery_ShouldReturnPaginatedCampaigns()
        {
            // Arrange
            var query = new GetCampaignsQueryDto
            {
                PageNumber = 1,
                PageSize = 10,
                Status = CampaignStatus.ACTIVE
            };

            var campaigns = new List<Campaign>
            {
                new Campaign
                {
                    Id = Guid.NewGuid(),
                    StoreId = Guid.Empty, // Service returns Guid.Empty from GetCurrentStoreId()
                    CampaignName = "Campaign 1",
                    Status = CampaignStatus.ACTIVE,
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddDays(20)
                }
            };

            var paginatedResult = new PaginatedResult<Campaign>
            {
                Items = campaigns,
                PageNumber = 1,
                PageSize = 10,
                TotalItems = 1,
                TotalPages = 1
            };

            _mockCampaignRepository.Setup(r => r.GetCampaignsByStoreAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CampaignStatus?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
            )).ReturnsAsync(paginatedResult);

            // Act
            var result = await _campaignService.GetCampaignsAsync(query);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data.Items);
            Assert.Equal(1, result.Data.TotalItems);
        }

        [Fact]
        public async Task GetCampaignsAsync_WithInvalidPageNumber_ShouldReturnError()
        {
            // Arrange
            var query = new GetCampaignsQueryDto
            {
                PageNumber = 0, // Invalid page number
                PageSize = 10
            };

            // Act
            var result = await _campaignService.GetCampaignsAsync(query);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Page number must be greater than 0", result.Message);
        }

        #endregion

        #region GetCampaignByIdAsync Tests

        [Fact]
        public async Task GetCampaignByIdAsync_WithValidId_ShouldReturnCampaign()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var campaign = new Campaign
            {
                Id = campaignId,
                StoreId = Guid.NewGuid(),
                CampaignName = "Test Campaign",
                Status = CampaignStatus.DRAFT,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            _mockCampaignRepository
                .Setup(r => r.GetCampaignWithDetailsAsync(campaignId))
                .ReturnsAsync(campaign);

            // Act
            var result = await _campaignService.GetCampaignByIdAsync(campaignId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(campaign.CampaignName, result.Data.CampaignName);
        }

        [Fact]
        public async Task GetCampaignByIdAsync_WithInvalidId_ShouldReturnError()
        {
            // Arrange
            var invalidId = Guid.Empty;

            // Act
            var result = await _campaignService.GetCampaignByIdAsync(invalidId);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public async Task GetCampaignByIdAsync_WithNonExistentId_ShouldReturnError()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            _mockCampaignRepository
                .Setup(r => r.GetCampaignWithDetailsAsync(campaignId))
                .ReturnsAsync((Campaign)null);

            // Act
            var result = await _campaignService.GetCampaignByIdAsync(campaignId);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("not found", result.Message);
        }

        #endregion

        #region UpdateCampaignAsync Tests

        [Fact]
        public async Task UpdateCampaignAsync_WithValidData_ShouldUpdateCampaign()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var storeId = Guid.NewGuid();
            var existingCampaign = new Campaign
            {
                Id = campaignId,
                StoreId = storeId,
                CampaignName = "Old Name",
                Description = "Old Description",
                Status = CampaignStatus.DRAFT,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 5000000
            };

            var updateDto = new UpdateCampaignDto
            {
                CampaignName = "New Name",
                Description = "New Description",
                Budget = 10000000
            };

            _mockCampaignRepository
                .Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(existingCampaign);

            // Act
            var result = await _campaignService.UpdateCampaignAsync(campaignId, updateDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            _mockCampaignRepository.Verify(r => r.Update(It.IsAny<Campaign>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCampaignAsync_WithInvalidDateRange_ShouldReturnError()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var existingCampaign = new Campaign
            {
                Id = campaignId,
                StoreId = Guid.NewGuid(),
                CampaignName = "Test Campaign",
                Status = CampaignStatus.DRAFT,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            var updateDto = new UpdateCampaignDto
            {
                StartDate = DateTime.UtcNow.AddDays(30),
                EndDate = DateTime.UtcNow.AddDays(1) // Invalid: EndDate < StartDate
            };

            _mockCampaignRepository
                .Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(existingCampaign);

            // Act
            var result = await _campaignService.UpdateCampaignAsync(campaignId, updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("EndDate must be greater than StartDate", result.Message);
        }

        #endregion

        #region DeleteCampaignAsync Tests

        [Fact]
        public async Task DeleteCampaignAsync_WithValidId_ShouldDeleteCampaign()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var campaign = new Campaign
            {
                Id = campaignId,
                StoreId = Guid.NewGuid(),
                CampaignName = "Test Campaign",
                Status = CampaignStatus.DRAFT
            };

            _mockCampaignRepository
                .Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            // Act
            var result = await _campaignService.DeleteCampaignAsync(campaignId);

            // Assert
            Assert.True(result.Success);
            Assert.True(result.Data);
            _mockCampaignRepository.Verify(r => r.Delete(It.IsAny<Campaign>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCampaignAsync_WithNonExistentId_ShouldReturnError()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            _mockCampaignRepository
                .Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync((Campaign)null);

            // Act
            var result = await _campaignService.DeleteCampaignAsync(campaignId);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("not found", result.Message);
        }

        #endregion

        #region ChangeCampaignStatusAsync Tests

        [Fact]
        public async Task ChangeCampaignStatusAsync_FromDraftToActive_ShouldChangeStatus()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var campaign = new Campaign
            {
                Id = campaignId,
                StoreId = Guid.NewGuid(),
                CampaignName = "Test Campaign",
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

            // Assert
            Assert.True(result.Success);
            Assert.Equal(CampaignStatus.ACTIVE, result.Data.Status);
            _mockCampaignRepository.Verify(r => r.Update(It.IsAny<Campaign>()), Times.Once);
        }

        [Fact]
        public async Task ChangeCampaignStatusAsync_ToActiveWithMissingFields_ShouldReturnError()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var campaign = new Campaign
            {
                Id = campaignId,
                StoreId = Guid.NewGuid(),
                CampaignName = "", // Missing campaign name
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

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Cannot activate campaign", result.Message);
        }

        #endregion
    }
}

