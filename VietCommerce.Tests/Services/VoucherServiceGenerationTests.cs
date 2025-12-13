using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using VietCommerce.Application.Mappings;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Unit tests for VoucherService voucher generation
    /// **Feature: campaign-promotion-api, Property 11: Voucher Codes Generated Uniquely**
    /// **Validates: Requirements 3.1**
    /// </summary>
    public class VoucherServiceGenerationTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IVoucherRepository> _mockVoucherRepository;
        private readonly Mock<IPromotionRepository> _mockPromotionRepository;
        private readonly Mock<ILogger<VoucherService>> _mockLogger;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly IMapper _mapper;
        private readonly VoucherService _voucherService;

        public VoucherServiceGenerationTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockVoucherRepository = new Mock<IVoucherRepository>();
            _mockPromotionRepository = new Mock<IPromotionRepository>();
            _mockLogger = new Mock<ILogger<VoucherService>>();
            _mockCacheService = new Mock<ICacheService>();

            // Setup mapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<VoucherMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            // Setup UnitOfWork
            _mockUnitOfWork.Setup(u => u.Vouchers).Returns(_mockVoucherRepository.Object);
            _mockUnitOfWork.Setup(u => u.Promotions).Returns(_mockPromotionRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Setup cache service
            _mockCacheService.DefaultValue = DefaultValue.Mock;

            _voucherService = new VoucherService(
                _mockUnitOfWork.Object,
                _mapper,
                _mockLogger.Object,
                _mockCacheService.Object
            );
        }

        #region GenerateVouchersAsync Tests

        /// <summary>
        /// Test: Generate vouchers with valid data succeeds
        /// Requirement: 3.1
        /// </summary>
        [Fact]
        public async Task GenerateVouchersAsync_WithValidData_ShouldSucceed()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var promotion = new Promotion
            {
                Id = promotionId,
                PromotionName = "Test Promotion",
                DiscountValue = 100,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            var generateDto = new GenerateVouchersDto
            {
                Quantity = 5,
                Prefix = "TEST",
                ExpiryDate = DateTime.UtcNow.AddDays(60)
            };

            _mockPromotionRepository
                .Setup(r => r.GetByIdAsync(promotionId))
                .ReturnsAsync(promotion);

            _mockVoucherRepository
                .Setup(r => r.CodeExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _mockVoucherRepository
                .Setup(r => r.BulkInsertAsync(It.IsAny<IEnumerable<Voucher>>()))
                .ReturnsAsync((IEnumerable<Voucher> vouchers) => vouchers);

            // Act
            var result = await _voucherService.GenerateVouchersAsync(promotionId, generateDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(5, result.Data.GeneratedCount);
            Assert.Equal(5, result.Data.VoucherCodes.Count);
            Assert.Equal(promotionId, result.Data.PromotionId);

            // Verify all codes start with prefix
            foreach (var code in result.Data.VoucherCodes)
            {
                Assert.StartsWith("TEST-", code);
            }

            // Verify all codes are unique
            Assert.Equal(result.Data.VoucherCodes.Count, result.Data.VoucherCodes.Distinct().Count());
        }

        /// <summary>
        /// Test: Generate vouchers with invalid quantity fails
        /// Requirement: 3.1
        /// </summary>
        [Fact]
        public async Task GenerateVouchersAsync_WithInvalidQuantity_ShouldFail()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var generateDto = new GenerateVouchersDto
            {
                Quantity = 0,
                Prefix = "TEST",
                ExpiryDate = DateTime.UtcNow.AddDays(60)
            };

            // Act
            var result = await _voucherService.GenerateVouchersAsync(promotionId, generateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Quantity must be greater than 0", result.Message);
        }

        /// <summary>
        /// Test: Generate vouchers with expired date fails
        /// Requirement: 3.1
        /// </summary>
        [Fact]
        public async Task GenerateVouchersAsync_WithExpiredDate_ShouldFail()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var generateDto = new GenerateVouchersDto
            {
                Quantity = 5,
                Prefix = "TEST",
                ExpiryDate = DateTime.UtcNow.AddDays(-1) // Past date
            };

            // Act
            var result = await _voucherService.GenerateVouchersAsync(promotionId, generateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("ExpiryDate must be in the future", result.Message);
        }

        /// <summary>
        /// Test: Generate vouchers for non-existent promotion fails
        /// Requirement: 3.1
        /// </summary>
        [Fact]
        public async Task GenerateVouchersAsync_WithNonExistentPromotion_ShouldFail()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var generateDto = new GenerateVouchersDto
            {
                Quantity = 5,
                Prefix = "TEST",
                ExpiryDate = DateTime.UtcNow.AddDays(60)
            };

            _mockPromotionRepository
                .Setup(r => r.GetByIdAsync(promotionId))
                .ReturnsAsync((Promotion)null!);

            // Act
            var result = await _voucherService.GenerateVouchersAsync(promotionId, generateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Promotion with ID", result.Message);
        }

        /// <summary>
        /// Test: Generated voucher codes are unique
        /// Requirement: 3.1
        /// </summary>
        [Fact]
        public async Task GenerateVouchersAsync_GeneratedCodes_ShouldBeUnique()
        {
            // Arrange
            var promotionId = Guid.NewGuid();
            var promotion = new Promotion
            {
                Id = promotionId,
                PromotionName = "Test Promotion",
                DiscountValue = 100,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            var generateDto = new GenerateVouchersDto
            {
                Quantity = 100,
                Prefix = "UNIQUE",
                ExpiryDate = DateTime.UtcNow.AddDays(60)
            };

            _mockPromotionRepository
                .Setup(r => r.GetByIdAsync(promotionId))
                .ReturnsAsync(promotion);

            _mockVoucherRepository
                .Setup(r => r.CodeExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            _mockVoucherRepository
                .Setup(r => r.BulkInsertAsync(It.IsAny<IEnumerable<Voucher>>()))
                .ReturnsAsync((IEnumerable<Voucher> vouchers) => vouchers);

            // Act
            var result = await _voucherService.GenerateVouchersAsync(promotionId, generateDto);

            // Assert
            Assert.NotNull(result.Data);
            var codes = result.Data.VoucherCodes;
            var uniqueCodes = codes.Distinct().ToList();
            Assert.Equal(codes.Count, uniqueCodes.Count);
        }

        #endregion

        #region ValidateVoucherAsync Tests

        /// <summary>
        /// Test: Validate valid voucher succeeds
        /// Requirement: 3.2, 3.4
        /// </summary>
        [Fact]
        public async Task ValidateVoucherAsync_WithValidCode_ShouldSucceed()
        {
            // Arrange
            var code = "TEST-001";
            var promotionId = Guid.NewGuid();
            var promotion = new Promotion
            {
                Id = promotionId,
                PromotionName = "Test Promotion",
                DiscountValue = 100,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Status = VietCommerce.Core.Enums.Marketing.PromotionStatus.ACTIVE,
                UsageLimit = null
            };

            var voucher = new Voucher
            {
                Id = Guid.NewGuid(),
                Code = code,
                PromotionId = promotionId,
                ExpiryDate = DateTime.UtcNow.AddDays(60),
                IsActive = true,
                IsDeleted = false,
                UsageCount = 0,
                Promotion = promotion
            };

            _mockVoucherRepository
                .Setup(r => r.ValidateVoucherDetailedAsync(code))
                .ReturnsAsync((true, null));

            _mockVoucherRepository
                .Setup(r => r.GetByCodeAsync(code))
                .ReturnsAsync(voucher);

            // Act
            var result = await _voucherService.ValidateVoucherAsync(code);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(code, result.Data.Code);
        }

        /// <summary>
        /// Test: Validate invalid voucher fails
        /// Requirement: 3.2, 3.4
        /// </summary>
        [Fact]
        public async Task ValidateVoucherAsync_WithInvalidCode_ShouldFail()
        {
            // Arrange
            var code = "INVALID-001";

            _mockVoucherRepository
                .Setup(r => r.ValidateVoucherDetailedAsync(code))
                .ReturnsAsync((false, "Voucher code not found"));

            _mockVoucherRepository
                .Setup(r => r.GetByCodeAsync(code))
                .ReturnsAsync((Voucher?)null);

            // Act
            var result = await _voucherService.ValidateVoucherAsync(code);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Voucher code not found", result.Message);
        }

        /// <summary>
        /// Test: Validate expired voucher fails
        /// Requirement: 3.4
        /// </summary>
        [Fact]
        public async Task ValidateVoucherAsync_WithExpiredCode_ShouldFail()
        {
            // Arrange
            var code = "EXPIRED-001";

            _mockVoucherRepository
                .Setup(r => r.ValidateVoucherDetailedAsync(code))
                .ReturnsAsync((false, "Voucher has expired"));

            _mockVoucherRepository
                .Setup(r => r.GetByCodeAsync(code))
                .ReturnsAsync((Voucher?)null);

            // Act
            var result = await _voucherService.ValidateVoucherAsync(code);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Voucher has expired", result.Message);
        }

        /// <summary>
        /// Test: Validate voucher with usage limit exceeded fails
        /// Requirement: 3.5
        /// </summary>
        [Fact]
        public async Task ValidateVoucherAsync_WithUsageLimitExceeded_ShouldFail()
        {
            // Arrange
            var code = "LIMIT-001";

            _mockVoucherRepository
                .Setup(r => r.ValidateVoucherDetailedAsync(code))
                .ReturnsAsync((false, "Voucher usage limit has been reached"));

            _mockVoucherRepository
                .Setup(r => r.GetByCodeAsync(code))
                .ReturnsAsync((Voucher?)null);

            // Act
            var result = await _voucherService.ValidateVoucherAsync(code);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Voucher usage limit has been reached", result.Message);
        }

        /// <summary>
        /// Test: Validate voucher with inactive promotion fails
        /// Requirement: 3.2
        /// </summary>
        [Fact]
        public async Task ValidateVoucherAsync_WithInactivePromotion_ShouldFail()
        {
            // Arrange
            var code = "INACTIVE-001";

            _mockVoucherRepository
                .Setup(r => r.ValidateVoucherDetailedAsync(code))
                .ReturnsAsync((false, "Associated promotion is not active"));

            _mockVoucherRepository
                .Setup(r => r.GetByCodeAsync(code))
                .ReturnsAsync((Voucher?)null);

            // Act
            var result = await _voucherService.ValidateVoucherAsync(code);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Associated promotion is not active", result.Message);
        }

        #endregion

        #region UpdateVoucherUsageAsync Tests

        /// <summary>
        /// Test: Update voucher usage succeeds
        /// Requirement: 3.5
        /// </summary>
        [Fact]
        public async Task UpdateVoucherUsageAsync_WithValidCode_ShouldSucceed()
        {
            // Arrange
            var code = "TEST-001";
            var userId = Guid.NewGuid();
            var voucher = new Voucher
            {
                Id = Guid.NewGuid(),
                Code = code,
                PromotionId = Guid.NewGuid(),
                ExpiryDate = DateTime.UtcNow.AddDays(60),
                UsageCount = 0,
                IsActive = true,
                IsDeleted = false
            };

            _mockVoucherRepository
                .Setup(r => r.GetByCodeAsync(code))
                .ReturnsAsync(voucher);

            _mockVoucherRepository
                .Setup(r => r.Update(It.IsAny<Voucher>()));

            // Act
            var result = await _voucherService.UpdateVoucherUsageAsync(code, userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.Data);

            // Verify usage was incremented
            _mockVoucherRepository.Verify(r => r.Update(It.Is<Voucher>(v =>
                v.UsageCount == 1 &&
                v.LastUsedBy == userId &&
                v.LastUsedAt != null
            )), Times.Once);
        }

        #endregion

        #region RemoveVoucherAsync Tests

        /// <summary>
        /// Test: Remove voucher from cart succeeds
        /// Requirement: 3.2
        /// </summary>
        [Fact]
        public async Task RemoveVoucherAsync_WithValidCart_ShouldSucceed()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var cart = new VietCommerce.Core.Entities.Orders.Cart
            {
                Id = cartId,
                UserId = Guid.NewGuid(),
                AppliedVoucherId = Guid.NewGuid(),
                AppliedVoucherCode = "TEST-001",
                DiscountAmount = 100,
                IsActive = true,
                IsDeleted = false
            };

            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository
                .Setup(r => r.GetByIdAsync(cartId))
                .ReturnsAsync(cart);

            mockCartRepository
                .Setup(r => r.Update(It.IsAny<VietCommerce.Core.Entities.Orders.Cart>()));

            _mockUnitOfWork.Setup(u => u.Carts).Returns(mockCartRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _voucherService.RemoveVoucherAsync(cartId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.Data);

            // Verify cart was updated
            mockCartRepository.Verify(r => r.Update(It.Is<VietCommerce.Core.Entities.Orders.Cart>(c =>
                c.AppliedVoucherId == null &&
                c.AppliedVoucherCode == null &&
                c.DiscountAmount == 0
            )), Times.Once);
        }

        /// <summary>
        /// Test: Remove voucher from non-existent cart fails
        /// Requirement: 3.2
        /// </summary>
        [Fact]
        public async Task RemoveVoucherAsync_WithNonExistentCart_ShouldFail()
        {
            // Arrange
            var cartId = Guid.NewGuid();

            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository
                .Setup(r => r.GetByIdAsync(cartId))
                .ReturnsAsync((VietCommerce.Core.Entities.Orders.Cart)null!);

            _mockUnitOfWork.Setup(u => u.Carts).Returns(mockCartRepository.Object);

            // Act
            var result = await _voucherService.RemoveVoucherAsync(cartId);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Cart with ID", result.Message);
        }

        /// <summary>
        /// Test: Remove voucher resets discount amount to zero
        /// Requirement: 3.2
        /// </summary>
        [Fact]
        public async Task RemoveVoucherAsync_ShouldResetDiscountAmount()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var cart = new VietCommerce.Core.Entities.Orders.Cart
            {
                Id = cartId,
                UserId = Guid.NewGuid(),
                AppliedVoucherId = Guid.NewGuid(),
                AppliedVoucherCode = "DISCOUNT-50",
                DiscountAmount = 500000, // 500k VND discount
                IsActive = true,
                IsDeleted = false
            };

            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository
                .Setup(r => r.GetByIdAsync(cartId))
                .ReturnsAsync(cart);

            mockCartRepository
                .Setup(r => r.Update(It.IsAny<VietCommerce.Core.Entities.Orders.Cart>()));

            _mockUnitOfWork.Setup(u => u.Carts).Returns(mockCartRepository.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _voucherService.RemoveVoucherAsync(cartId);

            // Assert
            Assert.True(result.Success);
            mockCartRepository.Verify(r => r.Update(It.Is<VietCommerce.Core.Entities.Orders.Cart>(c =>
                c.DiscountAmount == 0
            )), Times.Once);
        }

        #endregion
    }
}
