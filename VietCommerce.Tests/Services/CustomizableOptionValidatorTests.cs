using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Validators.Products;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Models;
using Xunit;

namespace VietCommerce.Tests.Services;

/// <summary>
/// Unit tests for CustomizableOptionValidator service
/// Validates: Requirements 1.2, 1.3, 7.2
/// </summary>
public class CustomizableOptionValidatorTests
{
    private readonly Mock<ILogger<CustomizableOptionValidator>> _mockLogger;
    private readonly IValidator<CustomizableOptionDto> _optionValidator;
    private readonly CustomizableOptionValidator _validator;

    public CustomizableOptionValidatorTests()
    {
        _mockLogger = new Mock<ILogger<CustomizableOptionValidator>>();
        _optionValidator = new CustomizableOptionDtoValidator();
        _validator = new CustomizableOptionValidator(_mockLogger.Object, _optionValidator);
    }

    #region Valid Cases

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithValidOptions_ShouldPass()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-xoi",
                Name = "Xôi gấc đậu xanh",
                BaseQuantity = 5,
                UnitPrice = 45000,
                MinQuantity = 5,
                MaxQuantity = 100,
                Unit = "dĩa"
            },
            new()
            {
                Id = "opt-che",
                Name = "Chè trôi nước",
                BaseQuantity = 5,
                UnitPrice = 35000,
                MinQuantity = 5,
                MaxQuantity = null,
                Unit = "chén"
            }
        };

        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(options);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithSingleValidOption_ShouldPass()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-xoi",
                Name = "Xôi gấc đậu xanh",
                BaseQuantity = 5,
                UnitPrice = 45000,
                MinQuantity = 5,
                MaxQuantity = 100,
                Unit = "dĩa"
            }
        };

        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(options);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithNullMaxQuantity_ShouldPass()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-che",
                Name = "Chè trôi nước",
                BaseQuantity = 5,
                UnitPrice = 35000,
                MinQuantity = 5,
                MaxQuantity = null,
                Unit = "chén"
            }
        };

        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(options);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    #endregion

    #region Invalid Cases - Null/Empty Input

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithNullList_ShouldFail()
    {
        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(null!);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("cannot be null", result.Errors[0]);
    }

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithEmptyList_ShouldFail()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>();

        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(options);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("cannot be empty", result.Errors[0]);
    }

    #endregion

    #region Invalid Cases - Field Validation

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithEmptyId_ShouldFail()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = string.Empty,
                Name = "Xôi gấc đậu xanh",
                BaseQuantity = 5,
                UnitPrice = 45000,
                MinQuantity = 5,
                MaxQuantity = 100,
                Unit = "dĩa"
            }
        };

        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(options);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Id", result.Errors[0]);
    }

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithEmptyName_ShouldFail()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-xoi",
                Name = string.Empty,
                BaseQuantity = 5,
                UnitPrice = 45000,
                MinQuantity = 5,
                MaxQuantity = 100,
                Unit = "dĩa"
            }
        };

        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(options);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Name", result.Errors[0]);
    }

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithZeroUnitPrice_ShouldFail()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-xoi",
                Name = "Xôi gấc đậu xanh",
                BaseQuantity = 5,
                UnitPrice = 0,
                MinQuantity = 5,
                MaxQuantity = 100,
                Unit = "dĩa"
            }
        };

        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(options);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("UnitPrice", result.Errors[0]);
    }

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithZeroMinQuantity_ShouldFail()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-xoi",
                Name = "Xôi gấc đậu xanh",
                BaseQuantity = 5,
                UnitPrice = 45000,
                MinQuantity = 0,
                MaxQuantity = 100,
                Unit = "dĩa"
            }
        };

        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(options);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("MinQuantity", result.Errors[0]);
    }

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithMinGreaterThanMax_ShouldFail()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-xoi",
                Name = "Xôi gấc đậu xanh",
                BaseQuantity = 5,
                UnitPrice = 45000,
                MinQuantity = 100,
                MaxQuantity = 50,
                Unit = "dĩa"
            }
        };

        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(options);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Minimum quantity must be less than or equal to maximum quantity", result.Errors[0]);
    }

    #endregion

    #region Invalid Cases - Duplicate IDs

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithDuplicateIds_ShouldFail()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-xoi",
                Name = "Xôi gấc đậu xanh",
                BaseQuantity = 5,
                UnitPrice = 45000,
                MinQuantity = 5,
                MaxQuantity = 100,
                Unit = "dĩa"
            },
            new()
            {
                Id = "opt-xoi", // Duplicate ID
                Name = "Xôi khác",
                BaseQuantity = 5,
                UnitPrice = 50000,
                MinQuantity = 5,
                MaxQuantity = 100,
                Unit = "dĩa"
            }
        };

        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(options);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Duplicate option IDs found", result.Errors.Last());
    }

    #endregion

    #region Invalid Cases - Multiple Errors

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithMultipleInvalidOptions_ShouldFail()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = string.Empty, // Invalid
                Name = "Xôi gấc đậu xanh",
                BaseQuantity = 5,
                UnitPrice = 45000,
                MinQuantity = 5,
                MaxQuantity = 100,
                Unit = "dĩa"
            },
            new()
            {
                Id = "opt-che",
                Name = string.Empty, // Invalid
                BaseQuantity = 5,
                UnitPrice = 35000,
                MinQuantity = 5,
                MaxQuantity = null,
                Unit = "chén"
            }
        };

        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(options);

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors.Length >= 2);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithMinEqualToMax_ShouldPass()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-fixed",
                Name = "Fixed Option",
                BaseQuantity = 10,
                UnitPrice = 50000,
                MinQuantity = 10,
                MaxQuantity = 10,
                Unit = "bộ"
            }
        };

        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(options);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithLargeQuantityValues_ShouldPass()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-bulk",
                Name = "Bulk Option",
                BaseQuantity = 1000,
                UnitPrice = 1000,
                MinQuantity = 100,
                MaxQuantity = 10000,
                Unit = "cái"
            }
        };

        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(options);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateCustomizableOptionsAsync_WithDecimalUnitPrice_ShouldPass()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-decimal",
                Name = "Decimal Price Option",
                BaseQuantity = 5,
                UnitPrice = 45000.50m,
                MinQuantity = 5,
                MaxQuantity = 100,
                Unit = "dĩa"
            }
        };

        // Act
        var result = await _validator.ValidateCustomizableOptionsAsync(options);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    #endregion
}
