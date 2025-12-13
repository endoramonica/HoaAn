using FluentValidation;
using VietCommerce.Application.Validators.Marketing;
using VietCommerce.Core.DTOs.Marketing;
using VietCommerce.Core.Enums.Marketing;
using Xunit;

namespace VietCommerce.Tests.Validators;

/// <summary>
/// Unit tests for Campaign validators
/// Validates: Requirements 1.1, 1.2, 1.3, 1.4
/// </summary>
public class CampaignValidatorTests
{
    private readonly CreateCampaignDtoValidator _createValidator = new();
    private readonly UpdateCampaignDtoValidator _updateValidator = new();

    #region CreateCampaignDtoValidator - Valid Cases

    [Fact]
    public async Task CreateCampaign_WithValidData_ShouldPass()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            StoreId = Guid.NewGuid(),
            CampaignName = "Summer Sale",
            Description = "Summer promotion campaign",
            CampaignType = CampaignType.DISCOUNT,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30),
            Budget = 1000000
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task CreateCampaign_WithZeroBudget_ShouldPass()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            StoreId = Guid.NewGuid(),
            CampaignName = "Free Campaign",
            CampaignType = CampaignType.PROMOTION,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30),
            Budget = 0
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreateCampaign_WithValidJsonTargetingRules_ShouldPass()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            StoreId = Guid.NewGuid(),
            CampaignName = "Targeted Campaign",
            CampaignType = CampaignType.DISCOUNT,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30),
            Budget = 500000,
            TargetingRules = "{\"pages\": [\"home\", \"products\"], \"frequency\": \"once-per-session\"}"
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region CreateCampaignDtoValidator - Invalid Cases

    [Fact]
    public async Task CreateCampaign_WithEmptyCampaignName_ShouldFail()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            StoreId = Guid.NewGuid(),
            CampaignName = string.Empty,
            CampaignType = CampaignType.DISCOUNT,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30),
            Budget = 1000000
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CampaignName");
    }

    [Fact]
    public async Task CreateCampaign_WithEmptyStoreId_ShouldFail()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            StoreId = Guid.Empty,
            CampaignName = "Campaign",
            CampaignType = CampaignType.DISCOUNT,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30),
            Budget = 1000000
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "StoreId");
    }

    [Fact]
    public async Task CreateCampaign_WithEndDateEqualToStartDate_ShouldFail()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var dto = new CreateCampaignDto
        {
            StoreId = Guid.NewGuid(),
            CampaignName = "Campaign",
            CampaignType = CampaignType.DISCOUNT,
            StartDate = startDate,
            EndDate = startDate, // Same as start date
            Budget = 1000000
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EndDate");
    }

    [Fact]
    public async Task CreateCampaign_WithEndDateBeforeStartDate_ShouldFail()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            StoreId = Guid.NewGuid(),
            CampaignName = "Campaign",
            CampaignType = CampaignType.DISCOUNT,
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(1), // Before start date
            Budget = 1000000
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EndDate");
    }

    [Fact]
    public async Task CreateCampaign_WithNegativeBudget_ShouldFail()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            StoreId = Guid.NewGuid(),
            CampaignName = "Campaign",
            CampaignType = CampaignType.DISCOUNT,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30),
            Budget = -1000
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Budget");
    }

    [Fact]
    public async Task CreateCampaign_WithInvalidJsonTargetingRules_ShouldFail()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            StoreId = Guid.NewGuid(),
            CampaignName = "Campaign",
            CampaignType = CampaignType.DISCOUNT,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30),
            Budget = 1000000,
            TargetingRules = "{ invalid json }"
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TargetingRules");
    }

    [Fact]
    public async Task CreateCampaign_WithCampaignNameExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var dto = new CreateCampaignDto
        {
            StoreId = Guid.NewGuid(),
            CampaignName = new string('a', 256), // Exceeds 255 character limit
            CampaignType = CampaignType.DISCOUNT,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30),
            Budget = 1000000
        };

        // Act
        var result = await _createValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CampaignName");
    }

    #endregion

    #region UpdateCampaignDtoValidator - Valid Cases

    [Fact]
    public async Task UpdateCampaign_WithValidData_ShouldPass()
    {
        // Arrange
        var dto = new UpdateCampaignDto
        {
            CampaignName = "Updated Campaign",
            Budget = 2000000,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task UpdateCampaign_WithOnlyNameUpdate_ShouldPass()
    {
        // Arrange
        var dto = new UpdateCampaignDto
        {
            CampaignName = "New Name"
        };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task UpdateCampaign_WithOnlyBudgetUpdate_ShouldPass()
    {
        // Arrange
        var dto = new UpdateCampaignDto
        {
            Budget = 5000000
        };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region UpdateCampaignDtoValidator - Invalid Cases

    [Fact]
    public async Task UpdateCampaign_WithEndDateBeforeStartDate_ShouldFail()
    {
        // Arrange
        var dto = new UpdateCampaignDto
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

    [Fact]
    public async Task UpdateCampaign_WithNegativeBudget_ShouldFail()
    {
        // Arrange
        var dto = new UpdateCampaignDto
        {
            Budget = -500
        };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Budget");
    }

    [Fact]
    public async Task UpdateCampaign_WithInvalidJsonTargetingRules_ShouldFail()
    {
        // Arrange
        var dto = new UpdateCampaignDto
        {
            TargetingRules = "{ not valid json }"
        };

        // Act
        var result = await _updateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TargetingRules");
    }

    #endregion
}
