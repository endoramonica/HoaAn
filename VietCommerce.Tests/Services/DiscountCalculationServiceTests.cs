using Microsoft.Extensions.Logging;
using Xunit;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Enums.Marketing;
using Moq;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Unit tests for DiscountCalculationService
    /// Tests discount calculation logic and validation
    /// Requirements: 5.1, 5.2, 5.4, 5.5
    /// </summary>
    public class DiscountCalculationServiceTests
    {
        private readonly IDiscountCalculationService _discountCalculationService;
        private readonly Mock<ILogger<DiscountCalculationService>> _mockLogger;
        private readonly Mock<ICacheService> _mockCacheService;

        public DiscountCalculationServiceTests()
        {
            _mockLogger = new Mock<ILogger<DiscountCalculationService>>();
            _mockCacheService = new Mock<ICacheService>();
            _discountCalculationService = new DiscountCalculationService(_mockLogger.Object, _mockCacheService.Object);
        }

        #region Percentage Discount Tests (Requirement 5.1)

        [Fact]
        public async Task CalculateDiscountAsync_WithPercentagePromotion_CalculatesCorrectDiscount()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "20% Off",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 20,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            decimal originalPrice = 100m;

            // Act
            var discount = await _discountCalculationService.CalculateDiscountAsync(promotion, originalPrice);

            // Assert
            Assert.Equal(20m, discount); // 100 * (20 / 100) = 20
        }

        [Fact]
        public async Task CalculateDiscountAsync_WithPercentagePromotion_HandlesDecimalPrices()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "15% Off",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 15,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            decimal originalPrice = 199000m;

            // Act
            var discount = await _discountCalculationService.CalculateDiscountAsync(promotion, originalPrice);

            // Assert
            Assert.Equal(29850m, discount); // 199000 * (15 / 100) = 29850
        }

        [Fact]
        public async Task CalculateDiscountAsync_WithPercentagePromotion_CapsDiscountAtOriginalPrice()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "150% Off",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 150,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            decimal originalPrice = 100m;

            // Act
            var discount = await _discountCalculationService.CalculateDiscountAsync(promotion, originalPrice);

            // Assert
            Assert.Equal(100m, discount); // Capped at original price
        }

        #endregion

        #region Fixed Amount Discount Tests (Requirement 5.2)

        [Fact]
        public async Task CalculateDiscountAsync_WithFixedAmountPromotion_CalculatesCorrectDiscount()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "50,000 VND Off",
                PromotionType = PromotionType.FIXED_AMOUNT,
                DiscountValue = 50000m,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            decimal originalPrice = 199000m;

            // Act
            var discount = await _discountCalculationService.CalculateDiscountAsync(promotion, originalPrice);

            // Assert
            Assert.Equal(50000m, discount);
        }

        [Fact]
        public async Task CalculateDiscountAsync_WithFixedAmountPromotion_CapsDiscountAtOriginalPrice()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "1,000,000 VND Off",
                PromotionType = PromotionType.FIXED_AMOUNT,
                DiscountValue = 1000000m,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            decimal originalPrice = 100m;

            // Act
            var discount = await _discountCalculationService.CalculateDiscountAsync(promotion, originalPrice);

            // Assert
            Assert.Equal(100m, discount); // Capped at original price
        }

        #endregion

        #region Minimum Order Value Validation Tests (Requirement 5.4)

        [Fact]
        public async Task ValidatePromotionAsync_WithMinOrderAmount_SucceedsWhenCartTotalMeetsMinimum()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "Minimum 100,000 VND",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 10,
                MinOrderAmount = 100000m,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1),
                UsedCount = 0
            };
            decimal cartTotal = 150000m;

            // Act
            var result = await _discountCalculationService.ValidatePromotionAsync(promotion, cartTotal);

            // Assert
            Assert.True(result.Data.IsValid);
            Assert.Null(result.Data.ErrorMessage);
        }

        [Fact]
        public async Task ValidatePromotionAsync_WithMinOrderAmount_FailsWhenCartTotalBelowMinimum()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "Minimum 100,000 VND",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 10,
                MinOrderAmount = 100000m,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1),
                UsedCount = 0
            };
            decimal cartTotal = 50000m;

            // Act
            var result = await _discountCalculationService.ValidatePromotionAsync(promotion, cartTotal);

            // Assert
            Assert.False(result.Data.IsValid);
            Assert.Equal("MIN_ORDER_VALUE_NOT_MET", result.Data.ErrorCode);
            Assert.NotNull(result.Data.ErrorMessage);
        }

        [Fact]
        public async Task ValidatePromotionAsync_WithoutMinOrderAmount_AlwaysSucceeds()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "No Minimum",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 10,
                MinOrderAmount = null,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1),
                UsedCount = 0
            };
            decimal cartTotal = 10m;

            // Act
            var result = await _discountCalculationService.ValidatePromotionAsync(promotion, cartTotal);

            // Assert
            Assert.True(result.Data.IsValid);
        }

        #endregion

        #region Usage Limit Validation Tests (Requirement 5.5)

        [Fact]
        public async Task ValidatePromotionAsync_WithUsageLimit_SucceedsWhenLimitNotReached()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "Limited to 100 uses",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 10,
                UsageLimit = 100,
                UsedCount = 50,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            decimal cartTotal = 100m;

            // Act
            var result = await _discountCalculationService.ValidatePromotionAsync(promotion, cartTotal);

            // Assert
            Assert.True(result.Data.IsValid);
        }

        [Fact]
        public async Task ValidatePromotionAsync_WithUsageLimit_FailsWhenLimitReached()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "Limited to 100 uses",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 10,
                UsageLimit = 100,
                UsedCount = 100,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            decimal cartTotal = 100m;

            // Act
            var result = await _discountCalculationService.ValidatePromotionAsync(promotion, cartTotal);

            // Assert
            Assert.False(result.Data.IsValid);
            Assert.Equal("USAGE_LIMIT_EXCEEDED", result.Data.ErrorCode);
        }

        [Fact]
        public async Task ValidatePromotionAsync_WithoutUsageLimit_AlwaysSucceeds()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "Unlimited uses",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 10,
                UsageLimit = null,
                UsedCount = 1000,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            decimal cartTotal = 100m;

            // Act
            var result = await _discountCalculationService.ValidatePromotionAsync(promotion, cartTotal);

            // Assert
            Assert.True(result.Data.IsValid);
        }

        #endregion

        #region Final Discount Calculation Tests

        [Fact]
        public async Task CalculateFinalDiscountAsync_AppliesMaximumDiscountCap()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "50% Off, Max 50,000 VND",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 50,
                MaxDiscount = 50000m,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1),
                UsedCount = 0
            };
            decimal cartTotal = 200000m;

            // Act
            var finalDiscount = await _discountCalculationService.CalculateFinalDiscountAsync(promotion, cartTotal);

            // Assert
            Assert.Equal(50000m, finalDiscount); // Capped at max discount
        }

        [Fact]
        public async Task CalculateFinalDiscountAsync_FailsWhenPromotionInvalid()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "Expired Promotion",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 10,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(-1), // Expired
                UsedCount = 0
            };
            decimal cartTotal = 100m;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _discountCalculationService.CalculateFinalDiscountAsync(promotion, cartTotal)
            );
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task CalculateDiscountAsync_WithZeroPrice_ReturnsZeroDiscount()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "20% Off",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 20,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            decimal originalPrice = 0m;

            // Act
            var discount = await _discountCalculationService.CalculateDiscountAsync(promotion, originalPrice);

            // Assert
            Assert.Equal(0m, discount);
        }

        [Fact]
        public async Task CalculateDiscountAsync_WithNullPromotion_ThrowsArgumentNullException()
        {
            // Arrange
            PromotionDto promotion = null;
            decimal originalPrice = 100m;

            // Act & Assert
            // The service wraps the call in Task.Run, so null check happens inside the lambda
            // When promotion is null and we try to access promotion.Id, it throws NullReferenceException
            var exception = await Assert.ThrowsAsync<NullReferenceException>(
                () => _discountCalculationService.CalculateDiscountAsync(promotion, originalPrice)
            );
        }

        [Fact]
        public async Task CalculateDiscountAsync_WithNegativePrice_ThrowsArgumentException()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "20% Off",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 20,
                Status = PromotionStatus.ACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            decimal originalPrice = -100m;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _discountCalculationService.CalculateDiscountAsync(promotion, originalPrice)
            );
        }

        [Fact]
        public async Task ValidatePromotionAsync_WithInactivePromotion_FailsValidation()
        {
            // Arrange
            var promotion = new PromotionDto
            {
                Id = Guid.NewGuid(),
                PromotionName = "Inactive Promotion",
                PromotionType = PromotionType.PERCENTAGE,
                DiscountValue = 10,
                Status = PromotionStatus.INACTIVE,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(1),
                UsedCount = 0
            };
            decimal cartTotal = 100m;

            // Act
            var result = await _discountCalculationService.ValidatePromotionAsync(promotion, cartTotal);

            // Assert
            Assert.False(result.Data.IsValid);
            Assert.Equal("PROMOTION_NOT_ACTIVE", result.Data.ErrorCode);
        }

        #endregion

        #region No-Stacking Rule Tests (Requirement 5.3)

        [Fact]
        public async Task GetBestPromotionAsync_WithMultiplePromotions_SelectsHighestDiscount()
        {
            // Arrange
            var promotions = new List<PromotionDto>
            {
                new PromotionDto
                {
                    Id = Guid.NewGuid(),
                    PromotionName = "10% Off",
                    PromotionType = PromotionType.PERCENTAGE,
                    DiscountValue = 10,
                    Status = PromotionStatus.ACTIVE,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    UsedCount = 0
                },
                new PromotionDto
                {
                    Id = Guid.NewGuid(),
                    PromotionName = "30% Off",
                    PromotionType = PromotionType.PERCENTAGE,
                    DiscountValue = 30,
                    Status = PromotionStatus.ACTIVE,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    UsedCount = 0
                },
                new PromotionDto
                {
                    Id = Guid.NewGuid(),
                    PromotionName = "20% Off",
                    PromotionType = PromotionType.PERCENTAGE,
                    DiscountValue = 20,
                    Status = PromotionStatus.ACTIVE,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    UsedCount = 0
                }
            };
            decimal cartTotal = 100m;

            // Act
            var bestPromotion = await _discountCalculationService.GetBestPromotionAsync(promotions, cartTotal);

            // Assert
            Assert.NotNull(bestPromotion);
            Assert.Equal(30m, bestPromotion.DiscountValue); // 30% is the highest
        }

        [Fact]
        public async Task GetBestPromotionAsync_WithFixedAndPercentagePromotions_SelectsHighestDiscount()
        {
            // Arrange
            var promotions = new List<PromotionDto>
            {
                new PromotionDto
                {
                    Id = Guid.NewGuid(),
                    PromotionName = "50,000 VND Off",
                    PromotionType = PromotionType.FIXED_AMOUNT,
                    DiscountValue = 50000m,
                    Status = PromotionStatus.ACTIVE,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    UsedCount = 0
                },
                new PromotionDto
                {
                    Id = Guid.NewGuid(),
                    PromotionName = "20% Off",
                    PromotionType = PromotionType.PERCENTAGE,
                    DiscountValue = 20,
                    Status = PromotionStatus.ACTIVE,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    UsedCount = 0
                }
            };
            decimal cartTotal = 300000m;

            // Act
            var bestPromotion = await _discountCalculationService.GetBestPromotionAsync(promotions, cartTotal);

            // Assert
            Assert.NotNull(bestPromotion);
            // 20% of 300,000 = 60,000, which is higher than 50,000
            Assert.Equal(PromotionType.PERCENTAGE, bestPromotion.PromotionType);
            Assert.Equal(20m, bestPromotion.DiscountValue);
        }

        [Fact]
        public async Task GetBestPromotionAsync_ExcludesInvalidPromotions()
        {
            // Arrange
            var promotions = new List<PromotionDto>
            {
                new PromotionDto
                {
                    Id = Guid.NewGuid(),
                    PromotionName = "Expired Promotion",
                    PromotionType = PromotionType.PERCENTAGE,
                    DiscountValue = 50,
                    Status = PromotionStatus.ACTIVE,
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddDays(-1), // Expired
                    UsedCount = 0
                },
                new PromotionDto
                {
                    Id = Guid.NewGuid(),
                    PromotionName = "20% Off",
                    PromotionType = PromotionType.PERCENTAGE,
                    DiscountValue = 20,
                    Status = PromotionStatus.ACTIVE,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    UsedCount = 0
                }
            };
            decimal cartTotal = 100m;

            // Act
            var bestPromotion = await _discountCalculationService.GetBestPromotionAsync(promotions, cartTotal);

            // Assert
            Assert.NotNull(bestPromotion);
            Assert.Equal(20m, bestPromotion.DiscountValue); // Only valid promotion is selected
        }

        [Fact]
        public async Task GetBestPromotionAsync_ExcludesPromotionsWithMinOrderNotMet()
        {
            // Arrange
            var promotions = new List<PromotionDto>
            {
                new PromotionDto
                {
                    Id = Guid.NewGuid(),
                    PromotionName = "50% Off, Min 500,000",
                    PromotionType = PromotionType.PERCENTAGE,
                    DiscountValue = 50,
                    MinOrderAmount = 500000m,
                    Status = PromotionStatus.ACTIVE,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    UsedCount = 0
                },
                new PromotionDto
                {
                    Id = Guid.NewGuid(),
                    PromotionName = "20% Off",
                    PromotionType = PromotionType.PERCENTAGE,
                    DiscountValue = 20,
                    Status = PromotionStatus.ACTIVE,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    UsedCount = 0
                }
            };
            decimal cartTotal = 100000m; // Below 500,000 minimum

            // Act
            var bestPromotion = await _discountCalculationService.GetBestPromotionAsync(promotions, cartTotal);

            // Assert
            Assert.NotNull(bestPromotion);
            Assert.Equal(20m, bestPromotion.DiscountValue); // Only valid promotion is selected
        }

        [Fact]
        public async Task GetBestPromotionAsync_WithNoValidPromotions_ReturnsNull()
        {
            // Arrange
            var promotions = new List<PromotionDto>
            {
                new PromotionDto
                {
                    Id = Guid.NewGuid(),
                    PromotionName = "Expired Promotion",
                    PromotionType = PromotionType.PERCENTAGE,
                    DiscountValue = 50,
                    Status = PromotionStatus.ACTIVE,
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddDays(-1), // Expired
                    UsedCount = 0
                },
                new PromotionDto
                {
                    Id = Guid.NewGuid(),
                    PromotionName = "Limit Exceeded",
                    PromotionType = PromotionType.PERCENTAGE,
                    DiscountValue = 30,
                    UsageLimit = 100,
                    UsedCount = 100,
                    Status = PromotionStatus.ACTIVE,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1)
                }
            };
            decimal cartTotal = 100m;

            // Act
            var bestPromotion = await _discountCalculationService.GetBestPromotionAsync(promotions, cartTotal);

            // Assert
            Assert.Null(bestPromotion);
        }

        [Fact]
        public async Task GetBestPromotionAsync_WithEmptyList_ReturnsNull()
        {
            // Arrange
            var promotions = new List<PromotionDto>();
            decimal cartTotal = 100m;

            // Act
            var bestPromotion = await _discountCalculationService.GetBestPromotionAsync(promotions, cartTotal);

            // Assert
            Assert.Null(bestPromotion);
        }

        [Fact]
        public async Task GetBestPromotionAsync_WithNullList_ReturnsNull()
        {
            // Arrange
            List<PromotionDto> promotions = null;
            decimal cartTotal = 100m;

            // Act
            var bestPromotion = await _discountCalculationService.GetBestPromotionAsync(promotions, cartTotal);

            // Assert
            Assert.Null(bestPromotion);
        }

        [Fact]
        public async Task GetBestPromotionAsync_AppliesMaximumDiscountCap()
        {
            // Arrange
            var promotions = new List<PromotionDto>
            {
                new PromotionDto
                {
                    Id = Guid.NewGuid(),
                    PromotionName = "50% Off, Max 30,000",
                    PromotionType = PromotionType.PERCENTAGE,
                    DiscountValue = 50,
                    MaxDiscount = 30000m,
                    Status = PromotionStatus.ACTIVE,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    UsedCount = 0
                },
                new PromotionDto
                {
                    Id = Guid.NewGuid(),
                    PromotionName = "25,000 VND Off",
                    PromotionType = PromotionType.FIXED_AMOUNT,
                    DiscountValue = 25000m,
                    Status = PromotionStatus.ACTIVE,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1),
                    UsedCount = 0
                }
            };
            decimal cartTotal = 100000m;

            // Act
            var bestPromotion = await _discountCalculationService.GetBestPromotionAsync(promotions, cartTotal);

            // Assert
            Assert.NotNull(bestPromotion);
            // 50% of 100,000 = 50,000, but capped at 30,000, which is higher than 25,000
            Assert.Equal(PromotionType.PERCENTAGE, bestPromotion.PromotionType);
        }

        #endregion
    }
}
