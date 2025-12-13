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
using static VietCommerce.Core.Enums.Marketing.PromotionType;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Unit tests for PromotionService CRUD operations
    /// Validates: Requirements 2.1, 2.2, 2.3, 2.4, 2.5
    /// </summary>
    public class PromotionServiceCrudTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPromotionRepository> _mockPromotionRepository;
        private readonly Mock<ICampaignRepository> _mockCampaignRepository;
        private readonly Mock<ILogger<PromotionService>> _mockLogger;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly IMapper _mapper;
        private readonly PromotionService _promotionService;

        public PromotionServiceCrudTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPromotionRepository = new Mock<IPromotionRepository>();
            _mockCampaignRepository = new Mock<ICampaignRepository>();
            _mockLogger = new Mock<ILogger<PromotionService>>();
            _mockCacheService = new Mock<ICacheService>();

            // Setup mapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PromotionMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            // Setup UnitOfWork
            _mockUnitOfWork.Setup(u => u.Promotions).Returns(_mockPromotionRepository.Object);
            _mockUnitOfWork.Setup(u => u.Campaigns).Returns(_mockCampaignRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Setup cache service
            _mockCacheService.DefaultValue = DefaultValue.Mock;

            _promotionService = new PromotionService(
                _mockUnitOfWork.Object,
                _mapper,
                _mockLogger.Object,
                _mockCacheService.Object
            );
        }

        #region CreatePromotionAsync Tests

        /// <summary>
        /// Test: Create promotion for DRAFT campaign succeeds
        /// Requirement: 2.1
        /// </summary>
        [Fact]
        public async Task CreatePromotionAsync_WithValidDataForDraftCampaign_ShouldSucceed()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var campaign = new Campaign
            {
                Id = campaignId,
                CampaignName = "Test Campaign",
                Status = CampaignStatus.DRAFT,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 1000
            };

            var createDto = new CreatePromotionDto
            {
                PromotionName = "Test Promotion",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 20,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(15)
            };

            _mockCampaignRepository.Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            _mockPromotionRepository.Setup(r => r.AddAsync(It.IsAny<Promotion>()))
                .ReturnsAsync((Promotion p) => p);

            // Act
            var result = await _promotionService.CreatePromotionAsync(campaignId, createDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(createDto.PromotionName, result.Data.PromotionName);
            Assert.Equal(createDto.DiscountValue, result.Data.DiscountValue);
            Assert.Equal(PromotionStatus.ACTIVE, result.Data.Status);
        }

        /// <summary>
        /// Test: Create promotion for non-DRAFT campaign fails
        /// Requirement: 2.2
        /// </summary>
        [Fact]
        public async Task CreatePromotionAsync_WithNonDraftCampaign_ShouldFail()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var campaign = new Campaign
            {
                Id = campaignId,
                CampaignName = "Test Campaign",
                Status = CampaignStatus.ACTIVE,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 1000
            };

            var createDto = new CreatePromotionDto
            {
                PromotionName = "Test Promotion",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 20,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(15)
            };

            _mockCampaignRepository.Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            // Act
            var result = await _promotionService.CreatePromotionAsync(campaignId, createDto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Contains("DRAFT", result.Message);
        }

        /// <summary>
        /// Test: Create promotion with invalid discount value fails
        /// Requirement: 2.3
        /// </summary>
        [Fact]
        public async Task CreatePromotionAsync_WithInvalidDiscountValue_ShouldFail()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var campaign = new Campaign
            {
                Id = campaignId,
                CampaignName = "Test Campaign",
                Status = CampaignStatus.DRAFT,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 1000
            };

            var createDto = new CreatePromotionDto
            {
                PromotionName = "Test Promotion",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 0, // Invalid
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(15)
            };

            _mockCampaignRepository.Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            // Act
            var result = await _promotionService.CreatePromotionAsync(campaignId, createDto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Contains("DiscountValue", result.Message);
        }

        /// <summary>
        /// Test: Create promotion with invalid date range fails
        /// Requirement: 2.4
        /// </summary>
        [Fact]
        public async Task CreatePromotionAsync_WithInvalidDateRange_ShouldFail()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var campaign = new Campaign
            {
                Id = campaignId,
                CampaignName = "Test Campaign",
                Status = CampaignStatus.DRAFT,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Budget = 1000
            };

            var createDto = new CreatePromotionDto
            {
                PromotionName = "Test Promotion",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 20,
                StartDate = DateTime.UtcNow.AddDays(15),
                EndDate = DateTime.UtcNow // EndDate <= StartDate
            };

            _mockCampaignRepository.Setup(r => r.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            // Act
            var result = await _promotionService.CreatePromotionAsync(campaignId, createDto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Contains("EndDate", result.Message);
        }

        #endregion

        #region UpdatePromotionAsync Tests

        /// <summary>
        /// Test: Update promotion succeeds
        /// </summary>
        [Fact]
        public async Task UpdatePromotionAsync_WithValidData_ShouldSucceed()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var existingPromotion = new Promotion
            {
                Id = promotionId,
                CampaignId = Guid.NewGuid(),
                PromotionName = "Old Name",
                DiscountValue = 20,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(15),
                Status = PromotionStatus.ACTIVE
            };

            var updateDto = new UpdatePromotionDto
            {
                PromotionName = "New Name",
                DiscountValue = 30
            };

            _mockPromotionRepository.Setup(r => r.GetByIdAsync(promotionId))
                .ReturnsAsync(existingPromotion);

            _mockPromotionRepository.Setup(r => r.Update(It.IsAny<Promotion>()));

            // Act
            var result = await _promotionService.UpdatePromotionAsync(promotionId, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("New Name", result.Data.PromotionName);
            Assert.Equal(30, result.Data.DiscountValue);
        }

        #endregion

        #region DeletePromotionAsync Tests

        /// <summary>
        /// Test: Delete promotion succeeds (soft delete)
        /// </summary>
        [Fact]
        public async Task DeletePromotionAsync_WithValidId_ShouldSucceed()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var promotion = new Promotion
            {
                Id = promotionId,
                CampaignId = Guid.NewGuid(),
                PromotionName = "Test Promotion",
                DiscountValue = 20,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(15),
                Status = PromotionStatus.ACTIVE,
                IsDeleted = false
            };

            _mockPromotionRepository.Setup(r => r.GetByIdAsync(promotionId))
                .ReturnsAsync(promotion);

            _mockPromotionRepository.Setup(r => r.Update(It.IsAny<Promotion>()));

            // Act
            var result = await _promotionService.DeletePromotionAsync(promotionId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.True(promotion.IsDeleted);
            Assert.NotNull(promotion.DeletedAt);
        }

        #endregion

        #region LinkProductsAsync Tests

        /// <summary>
        /// Test: Link products to promotion succeeds
        /// </summary>
        [Fact]
        public async Task LinkProductsAsync_WithValidProductIds_ShouldSucceed()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var productIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            var promotion = new Promotion
            {
                Id = promotionId,
                CampaignId = Guid.NewGuid(),
                PromotionName = "Test Promotion",
                PromotionProducts = new List<PromotionProduct>()
            };

            var linkDto = new LinkProductsDto
            {
                ProductIds = productIds,
                DiscountOverride = null
            };

            _mockPromotionRepository.Setup(r => r.GetByIdAsync(promotionId))
                .ReturnsAsync(promotion);

            _mockUnitOfWork.Setup(u => u.Products.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Core.Entities.Products.Product { Id = Guid.NewGuid() });

            _mockPromotionRepository.Setup(r => r.Update(It.IsAny<Promotion>()));

            // Act
            var result = await _promotionService.LinkProductsAsync(promotionId, linkDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.Data);
        }

        #endregion
    }
}
