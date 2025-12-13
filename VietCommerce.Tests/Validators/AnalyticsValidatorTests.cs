using FluentValidation;
using VietCommerce.Application.Validators.Marketing;
using VietCommerce.Core.DTOs.Marketing;
using Xunit;

namespace VietCommerce.Tests.Validators;

/// <summary>
/// Unit tests for Analytics validators
/// Validates: Requirements 7.1, 7.2, 7.4, 7.5
/// </summary>
public class AnalyticsValidatorTests
{
    private readonly TrackImpressionDtoValidator _impressionValidator = new();
    private readonly TrackClickDtoValidator _clickValidator = new();
    private readonly GetCampaignStatsQueryDtoValidator _statsValidator = new();

    #region TrackImpressionDtoValidator - Valid Cases

    [Fact]
    public async Task TrackImpression_WithValidData_ShouldPass()
    {
        // Arrange
        var dto = new TrackImpressionDto
        {
            SessionId = "SESSION123",
            Page = "home"
            // RecordedAt is optional, so we don't set it
        };

        // Act
        var result = await _impressionValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task TrackImpression_WithoutRecordedAt_ShouldPass()
    {
        // Arrange
        var dto = new TrackImpressionDto
        {
            SessionId = "SESSION123",
            Page = "products"
        };

        // Act
        var result = await _impressionValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task TrackImpression_WithPastRecordedAt_ShouldPass()
    {
        // Arrange
        var dto = new TrackImpressionDto
        {
            SessionId = "SESSION123",
            Page = "checkout",
            RecordedAt = DateTime.UtcNow.AddMinutes(-5)
        };

        // Act
        var result = await _impressionValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region TrackImpressionDtoValidator - Invalid Cases

    [Fact]
    public async Task TrackImpression_WithEmptySessionId_ShouldFail()
    {
        // Arrange
        var dto = new TrackImpressionDto
        {
            SessionId = string.Empty,
            Page = "home"
        };

        // Act
        var result = await _impressionValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "SessionId");
    }

    [Fact]
    public async Task TrackImpression_WithEmptyPage_ShouldFail()
    {
        // Arrange
        var dto = new TrackImpressionDto
        {
            SessionId = "SESSION123",
            Page = string.Empty
        };

        // Act
        var result = await _impressionValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Page");
    }

    [Fact]
    public async Task TrackImpression_WithFutureRecordedAt_ShouldFail()
    {
        // Arrange
        var dto = new TrackImpressionDto
        {
            SessionId = "SESSION123",
            Page = "home",
            RecordedAt = DateTime.UtcNow.AddMinutes(5)
        };

        // Act
        var result = await _impressionValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "RecordedAt");
    }

    [Fact]
    public async Task TrackImpression_WithSessionIdExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var dto = new TrackImpressionDto
        {
            SessionId = new string('a', 101),
            Page = "home"
        };

        // Act
        var result = await _impressionValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "SessionId");
    }

    [Fact]
    public async Task TrackImpression_WithPageExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var dto = new TrackImpressionDto
        {
            SessionId = "SESSION123",
            Page = new string('a', 101)
        };

        // Act
        var result = await _impressionValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Page");
    }

    #endregion

    #region TrackClickDtoValidator - Valid Cases

    [Fact]
    public async Task TrackClick_WithValidData_ShouldPass()
    {
        // Arrange
        var dto = new TrackClickDto
        {
            SessionId = "SESSION123",
            Page = "home"
            // RecordedAt is optional, so we don't set it
        };

        // Act
        var result = await _clickValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task TrackClick_WithoutRecordedAt_ShouldPass()
    {
        // Arrange
        var dto = new TrackClickDto
        {
            SessionId = "SESSION123",
            Page = "products"
        };

        // Act
        var result = await _clickValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region TrackClickDtoValidator - Invalid Cases

    [Fact]
    public async Task TrackClick_WithEmptySessionId_ShouldFail()
    {
        // Arrange
        var dto = new TrackClickDto
        {
            SessionId = string.Empty,
            Page = "home"
        };

        // Act
        var result = await _clickValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "SessionId");
    }

    [Fact]
    public async Task TrackClick_WithEmptyPage_ShouldFail()
    {
        // Arrange
        var dto = new TrackClickDto
        {
            SessionId = "SESSION123",
            Page = string.Empty
        };

        // Act
        var result = await _clickValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Page");
    }

    [Fact]
    public async Task TrackClick_WithFutureRecordedAt_ShouldFail()
    {
        // Arrange
        var dto = new TrackClickDto
        {
            SessionId = "SESSION123",
            Page = "home",
            RecordedAt = DateTime.UtcNow.AddMinutes(5)
        };

        // Act
        var result = await _clickValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "RecordedAt");
    }

    #endregion

    #region GetCampaignStatsQueryDtoValidator - Valid Cases

    [Fact]
    public async Task GetCampaignStats_WithValidDateRange_ShouldPass()
    {
        // Arrange
        var dto = new GetCampaignStatsQueryDto
        {
            FromDate = DateTime.UtcNow.AddDays(-30),
            ToDate = DateTime.UtcNow.AddDays(-1) // Yesterday
        };

        // Act
        var result = await _statsValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task GetCampaignStats_WithoutDateRange_ShouldPass()
    {
        // Arrange
        var dto = new GetCampaignStatsQueryDto();

        // Act
        var result = await _statsValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task GetCampaignStats_WithOnlyFromDate_ShouldPass()
    {
        // Arrange
        var dto = new GetCampaignStatsQueryDto
        {
            FromDate = DateTime.UtcNow.AddDays(-30)
        };

        // Act
        var result = await _statsValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task GetCampaignStats_WithOnlyToDate_ShouldPass()
    {
        // Arrange
        var dto = new GetCampaignStatsQueryDto
        {
            ToDate = DateTime.UtcNow.AddDays(-1) // Yesterday
        };

        // Act
        var result = await _statsValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region GetCampaignStatsQueryDtoValidator - Invalid Cases

    [Fact]
    public async Task GetCampaignStats_WithToDateBeforeFromDate_ShouldFail()
    {
        // Arrange
        var dto = new GetCampaignStatsQueryDto
        {
            FromDate = DateTime.UtcNow.AddDays(-10),
            ToDate = DateTime.UtcNow.AddDays(-30)
        };

        // Act
        var result = await _statsValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ToDate");
    }

    [Fact]
    public async Task GetCampaignStats_WithToDateEqualToFromDate_ShouldFail()
    {
        // Arrange
        var date = DateTime.UtcNow.AddDays(-10);
        var dto = new GetCampaignStatsQueryDto
        {
            FromDate = date,
            ToDate = date
        };

        // Act
        var result = await _statsValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ToDate");
    }

    [Fact]
    public async Task GetCampaignStats_WithFutureFromDate_ShouldFail()
    {
        // Arrange
        var dto = new GetCampaignStatsQueryDto
        {
            FromDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var result = await _statsValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FromDate");
    }

    [Fact]
    public async Task GetCampaignStats_WithFutureToDate_ShouldFail()
    {
        // Arrange
        var dto = new GetCampaignStatsQueryDto
        {
            ToDate = DateTime.UtcNow.AddDays(10)
        };

        // Act
        var result = await _statsValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ToDate");
    }

    #endregion
}
