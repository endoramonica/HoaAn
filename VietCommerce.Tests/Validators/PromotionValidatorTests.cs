using FluentValidation;
using VietCommerce.Application.Validators.Marketing;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Enums.Marketing;
using Xunit;

namespace VietCommerce.Tests.Validators;

/// <summary>
/// Unit tests for Promotion validators
/// Validates: Requirements 2.1, 2.2, 2.3, 2.4, 2.5
/// </summary>
public class PromotionValidatorTests
{
    private readonly CreatePromotionDtoValidator _createValidator = new();
    private readonly UpdatePromotionDtoValidator _updateValidator = new();

    #region CreatePromotionDtoValidator - Valid Cases

    [Fact]
    public async Task CreatePromotion_WithValidPercentageDiscount_ShouldPass()
    {
        // Arrange
        var dto = new CreatePromotionDto
        {
            PromotionName = "Summer Discount",
            PromotionType = PromotionType.PERCENTAGE,
            DiscountValue = 20,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task CreatePromotion_WithValidFixedDiscount_ShouldPass()
    {
        // Arrange
        var dto = new CreatePromotionDto
        {
            PromotionName = "Fixed Discount",
            PromotionType = PromotionType.FIXED_AMOUNT,
            DiscountValue = 100000,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreatePromotion_WithMinOrderAmount_ShouldPass()
    {
        // Arrange
        var dto = new CreatePromotionDto
        {
            PromotionName = "Promotion with Min Order",
            PromotionType = PromotionType.PERCENTAGE,
            DiscountValue = 15,
            MinOrderAmount = 500000,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreatePromotion_WithUsageLimit_ShouldPass()
    {
        // Arrange
        var dto = new CreatePromotionDto
        {
            PromotionName = "Limited Promotion",
            PromotionType = PromotionType.PERCENTAGE,
            DiscountValue = 25,
            UsageLimit = 100,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreatePromotion_WithAllOptionalFields_ShouldPass()
    {
        // Arrange
        var dto = new CreatePromotionDto
        {
            PromotionName = "Complete Promotion",
            PromotionType = PromotionType.PERCENTAGE,
            DiscountValue = 30,
            MinOrderAmount = 1000000,
            MaxDiscount = 500000,
            UsageLimit = 50,
            Conditions = "Valid for selected products only",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region CreatePromotionDtoValidator - Invalid Cases

    [Fact]
    public async Task CreatePromotion_WithEmptyPromotionName_ShouldFail()
    {
        // Arrange
        var dto = new CreatePromotionDto
        {
            PromotionName = string.Empty,
            PromotionType = PromotionType.PERCENTAGE,
            DiscountValue = 20,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PromotionName");
    }

    [Fact]
    public async Task CreatePromotion_WithZeroDiscountValue_ShouldFail()
    {
        // Arrange
        var dto = new CreatePromotionDto
        {
            PromotionName = "Invalid Promotion",
            PromotionType = PromotionType.PERCENTAGE,
            DiscountValue = 0,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "DiscountValue");
    }

    [Fact]
    public async Task CreatePromotion_WithNegativeDiscountValue_ShouldFail()
    {
        // Arrange
        var dto = new CreatePromotionDto
        {
            PromotionName = "Invalid Promotion",
            PromotionType = PromotionType.PERCENTAGE,
            DiscountValue = -10,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "DiscountValue");
    }

    [Fact]
    public async Task CreatePromotion_WithEndDateEqualToStartDate_ShouldFail()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var dto = new CreatePromotionDto
        {
            PromotionName = "Invalid Promotion",
            PromotionType = PromotionType.PERCENTAGE,
            DiscountValue = 20,
            StartDate = startDate,
            EndDate = startDate
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EndDate");
    }

    [Fact]
    public async Task CreatePromotion_WithEndDateBeforeStartDate_ShouldFail()
    {
        // Arrange
        var dto = new CreatePromotionDto
        {
            PromotionName = "Invalid Promotion",
            PromotionType = PromotionType.PERCENTAGE,
            DiscountValue = 20,
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EndDate");
    }

    [Fact]
    public async Task CreatePromotion_WithNegativeMinOrderAmount_ShouldFail()
    {
        // Arrange
        var dto = new CreatePromotionDto
        {
            PromotionName = "Invalid Promotion",
            PromotionType = PromotionType.PERCENTAGE,
            DiscountValue = 20,
            MinOrderAmount = -100000,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MinOrderAmount");
    }

    [Fact]
    public async Task CreatePromotion_WithZeroUsageLimit_ShouldFail()
    {
        // Arrange
        var dto = new CreatePromotionDto
        {
            PromotionName = "Invalid Promotion",
            PromotionType = PromotionType.PERCENTAGE,
            DiscountValue = 20,
            UsageLimit = 0,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "UsageLimit");
    }

    [Fact]
    public async Task CreatePromotion_WithPromotionNameExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var dto = new CreatePromotionDto
        {
            PromotionName = new string('a', 256),
            PromotionType = PromotionType.PERCENTAGE,
            DiscountValue = 20,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PromotionName");
    }

    #endregion

    #region UpdatePromotionDtoValidator - Valid Cases

    [Fact]
    public async Task UpdatePromotion_WithValidData_ShouldPass()
    {
        // Arrange
        var dto = new UpdatePromotionDto
        {
            PromotionName = "Updated Promotion",
            DiscountValue = 25,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task UpdatePromotion_WithOnlyNameUpdate_ShouldPass()
    {
        // Arrange
        var dto = new UpdatePromotionDto
        {
            PromotionName = "New Name"
        };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region UpdatePromotionDtoValidator - Invalid Cases

    [Fact]
    public async Task UpdatePromotion_WithZeroDiscountValue_ShouldFail()
    {
        // Arrange
        var dto = new UpdatePromotionDto
        {
            DiscountValue = 0
        };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "DiscountValue");
    }

    [Fact]
    public async Task UpdatePromotion_WithEndDateBeforeStartDate_ShouldFail()
    {
        // Arrange
        var dto = new UpdatePromotionDto
        {
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EndDate");
    }

    #endregion
}
