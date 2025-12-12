using FluentValidation;
using VietCommerce.Application.Validators.Products;
using VietCommerce.Core.DTOs.Products;
using Xunit;

namespace VietCommerce.Tests.Validators;

/// <summary>
/// Unit tests for CustomizableOptionDtoValidator
/// Validates: Requirements 1.2, 1.3
/// </summary>
public class CustomizableOptionDtoValidatorTests
{
    private readonly CustomizableOptionDtoValidator _validator = new();

    #region Valid Cases

    [Fact]
    public async Task Validate_WithValidOption_ShouldPass()
    {
        // Arrange
        var option = new CustomizableOptionDto
        {
            Id = "opt-xoi",
            Name = "Xôi gấc đậu xanh",
            BaseQuantity = 5,
            UnitPrice = 45000,
            MinQuantity = 5,
            MaxQuantity = 100,
            Unit = "dĩa"
        };

        // Act
        var result = await _validator.ValidateAsync(option);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_WithNullMaxQuantity_ShouldPass()
    {
        // Arrange
        var option = new CustomizableOptionDto
        {
            Id = "opt-che",
            Name = "Chè trôi nước",
            BaseQuantity = 5,
            UnitPrice = 35000,
            MinQuantity = 5,
            MaxQuantity = null,
            Unit = "chén"
        };

        // Act
        var result = await _validator.ValidateAsync(option);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_WithMinEqualToMax_ShouldPass()
    {
        // Arrange
        var option = new CustomizableOptionDto
        {
            Id = "opt-fixed",
            Name = "Fixed Option",
            BaseQuantity = 10,
            UnitPrice = 50000,
            MinQuantity = 10,
            MaxQuantity = 10,
            Unit = "bộ"
        };

        // Act
        var result = await _validator.ValidateAsync(option);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    #endregion

    #region Invalid Id Cases

    [Fact]
    public async Task Validate_WithEmptyId_ShouldFail()
    {
        // Arrange
        var option = new CustomizableOptionDto
        {
            Id = string.Empty,
            Name = "Xôi gấc đậu xanh",
            BaseQuantity = 5,
            UnitPrice = 45000,
            MinQuantity = 5,
            MaxQuantity = 100,
            Unit = "dĩa"
        };

        // Act
        var result = await _validator.ValidateAsync(option);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task Validate_WithNullId_ShouldFail()
    {
        // Arrange
        var option = new CustomizableOptionDto
        {
            Id = null!,
            Name = "Xôi gấc đậu xanh",
            BaseQuantity = 5,
            UnitPrice = 45000,
            MinQuantity = 5,
            MaxQuantity = 100,
            Unit = "dĩa"
        };

        // Act
        var result = await _validator.ValidateAsync(option);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    #endregion

    #region Invalid Name Cases

    [Fact]
    public async Task Validate_WithEmptyName_ShouldFail()
    {
        // Arrange
        var option = new CustomizableOptionDto
        {
            Id = "opt-xoi",
            Name = string.Empty,
            BaseQuantity = 5,
            UnitPrice = 45000,
            MinQuantity = 5,
            MaxQuantity = 100,
            Unit = "dĩa"
        };

        // Act
        var result = await _validator.ValidateAsync(option);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithNullName_ShouldFail()
    {
        // Arrange
        var option = new CustomizableOptionDto
        {
            Id = "opt-xoi",
            Name = null!,
            BaseQuantity = 5,
            UnitPrice = 45000,
            MinQuantity = 5,
            MaxQuantity = 100,
            Unit = "dĩa"
        };

        // Act
        var result = await _validator.ValidateAsync(option);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    #endregion

    #region Invalid UnitPrice Cases

    [Fact]
    public async Task Validate_WithZeroUnitPrice_ShouldFail()
    {
        // Arrange
        var option = new CustomizableOptionDto
        {
            Id = "opt-xoi",
            Name = "Xôi gấc đậu xanh",
            BaseQuantity = 5,
            UnitPrice = 0,
            MinQuantity = 5,
            MaxQuantity = 100,
            Unit = "dĩa"
        };

        // Act
        var result = await _validator.ValidateAsync(option);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "UnitPrice");
    }

    [Fact]
    public async Task Validate_WithNegativeUnitPrice_ShouldFail()
    {
        // Arrange
        var option = new CustomizableOptionDto
        {
            Id = "opt-xoi",
            Name = "Xôi gấc đậu xanh",
            BaseQuantity = 5,
            UnitPrice = -1000,
            MinQuantity = 5,
            MaxQuantity = 100,
            Unit = "dĩa"
        };

        // Act
        var result = await _validator.ValidateAsync(option);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "UnitPrice");
    }

    #endregion

    #region Invalid MinQuantity Cases

    [Fact]
    public async Task Validate_WithZeroMinQuantity_ShouldFail()
    {
        // Arrange
        var option = new CustomizableOptionDto
        {
            Id = "opt-xoi",
            Name = "Xôi gấc đậu xanh",
            BaseQuantity = 5,
            UnitPrice = 45000,
            MinQuantity = 0,
            MaxQuantity = 100,
            Unit = "dĩa"
        };

        // Act
        var result = await _validator.ValidateAsync(option);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MinQuantity");
    }

    [Fact]
    public async Task Validate_WithNegativeMinQuantity_ShouldFail()
    {
        // Arrange
        var option = new CustomizableOptionDto
        {
            Id = "opt-xoi",
            Name = "Xôi gấc đậu xanh",
            BaseQuantity = 5,
            UnitPrice = 45000,
            MinQuantity = -5,
            MaxQuantity = 100,
            Unit = "dĩa"
        };

        // Act
        var result = await _validator.ValidateAsync(option);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MinQuantity");
    }

    #endregion

    #region Invalid Quantity Range Cases

    [Fact]
    public async Task Validate_WithMinGreaterThanMax_ShouldFail()
    {
        // Arrange
        var option = new CustomizableOptionDto
        {
            Id = "opt-xoi",
            Name = "Xôi gấc đậu xanh",
            BaseQuantity = 5,
            UnitPrice = 45000,
            MinQuantity = 100,
            MaxQuantity = 50,
            Unit = "dĩa"
        };

        // Act
        var result = await _validator.ValidateAsync(option);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Minimum quantity must be less than or equal to maximum quantity"));
    }

    #endregion
}
