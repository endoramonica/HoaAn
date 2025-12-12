using FluentValidation;
using VietCommerce.Application.Validators.Products;
using VietCommerce.Core.DTOs.Products;
using Xunit;

namespace VietCommerce.Tests.Validators;

/// <summary>
/// Unit tests for ProductCreateDtoValidator
/// Validates: Requirements 1.1, 1.2
/// </summary>
public class ProductCreateDtoValidatorTests
{
    private readonly ProductCreateDtoValidator _validator = new();

    #region Valid Cases

    [Fact]
    public async Task Validate_WithValidProductWithoutPackage_ShouldPass()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Regular Product",
            Code = "PROD001",
            Price = 100000,
            StockQuantity = 10,
            IsActive = true
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_WithValidPackageProductWithDetails_ShouldPass()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Mâm Cúng Ông Công Ông Táo",
            Code = "PACKAGE001",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string>
            {
                "Cá chép giấy (3 con)",
                "Mũ giấy (3 cái)",
                "Vàng mã (1 bộ)"
            }
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_WithValidPackageProductWithCustomizableOptions_ShouldPass()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Mâm Cúng Khai Trương",
            Code = "PACKAGE002",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string>
            {
                "Cá chép giấy (3 con)",
                "Mũ giấy (3 cái)"
            },
            CustomizableOptions = new List<CustomizableOptionDto>
            {
                new CustomizableOptionDto
                {
                    Id = "opt-xoi",
                    Name = "Xôi gấc đậu xanh",
                    BaseQuantity = 5,
                    UnitPrice = 45000,
                    MinQuantity = 5,
                    MaxQuantity = 100,
                    Unit = "dĩa"
                },
                new CustomizableOptionDto
                {
                    Id = "opt-che",
                    Name = "Chè trôi nước",
                    BaseQuantity = 5,
                    UnitPrice = 35000,
                    MinQuantity = 5,
                    MaxQuantity = null,
                    Unit = "chén"
                }
            }
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_WithNullDetails_ShouldPass()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Product Without Details",
            Code = "PROD002",
            Price = 100000,
            StockQuantity = 10,
            IsActive = true,
            Details = null
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_WithNullCustomizableOptions_ShouldPass()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Product Without Options",
            Code = "PROD003",
            Price = 100000,
            StockQuantity = 10,
            IsActive = true,
            CustomizableOptions = null
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_WithEmptyCustomizableOptionsList_ShouldPass()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Product With Empty Options",
            Code = "PROD004",
            Price = 100000,
            StockQuantity = 10,
            IsActive = true,
            CustomizableOptions = new List<CustomizableOptionDto>()
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    #endregion

    #region Invalid Details Cases

    [Fact]
    public async Task Validate_WithEmptyDetailsList_ShouldFail()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Package Product",
            Code = "PACKAGE003",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string>()
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Details");
    }

    [Fact]
    public async Task Validate_WithDetailsContainingEmptyString_ShouldFail()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Package Product",
            Code = "PACKAGE004",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string>
            {
                "Cá chép giấy (3 con)",
                string.Empty,
                "Vàng mã (1 bộ)"
            }
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Details");
    }

    [Fact]
    public async Task Validate_WithDetailsContainingWhitespaceOnly_ShouldFail()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Package Product",
            Code = "PACKAGE005",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string>
            {
                "Cá chép giấy (3 con)",
                "   ",
                "Vàng mã (1 bộ)"
            }
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Details");
    }

    #endregion

    #region Invalid CustomizableOptions Cases

    [Fact]
    public async Task Validate_WithInvalidCustomizableOption_ShouldFail()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Mâm Cúng Khai Trương",
            Code = "PACKAGE006",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string> { "Item 1" },
            CustomizableOptions = new List<CustomizableOptionDto>
            {
                new CustomizableOptionDto
                {
                    Id = string.Empty, // Invalid: empty ID
                    Name = "Xôi gấc đậu xanh",
                    BaseQuantity = 5,
                    UnitPrice = 45000,
                    MinQuantity = 5,
                    MaxQuantity = 100,
                    Unit = "dĩa"
                }
            }
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains("CustomizableOptions"));
    }

    [Fact]
    public async Task Validate_WithInvalidOptionUnitPrice_ShouldFail()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Mâm Cúng Khai Trương",
            Code = "PACKAGE007",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string> { "Item 1" },
            CustomizableOptions = new List<CustomizableOptionDto>
            {
                new CustomizableOptionDto
                {
                    Id = "opt-xoi",
                    Name = "Xôi gấc đậu xanh",
                    BaseQuantity = 5,
                    UnitPrice = 0, // Invalid: zero price
                    MinQuantity = 5,
                    MaxQuantity = 100,
                    Unit = "dĩa"
                }
            }
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains("CustomizableOptions"));
    }

    [Fact]
    public async Task Validate_WithInvalidOptionQuantityRange_ShouldFail()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Mâm Cúng Khai Trương",
            Code = "PACKAGE008",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string> { "Item 1" },
            CustomizableOptions = new List<CustomizableOptionDto>
            {
                new CustomizableOptionDto
                {
                    Id = "opt-xoi",
                    Name = "Xôi gấc đậu xanh",
                    BaseQuantity = 5,
                    UnitPrice = 45000,
                    MinQuantity = 100, // Invalid: min > max
                    MaxQuantity = 50,
                    Unit = "dĩa"
                }
            }
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains("CustomizableOptions"));
    }

    [Fact]
    public async Task Validate_WithDuplicateOptionIds_ShouldFail()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Mâm Cúng Khai Trương",
            Code = "PACKAGE009",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string> { "Item 1" },
            CustomizableOptions = new List<CustomizableOptionDto>
            {
                new CustomizableOptionDto
                {
                    Id = "opt-xoi",
                    Name = "Xôi gấc đậu xanh",
                    BaseQuantity = 5,
                    UnitPrice = 45000,
                    MinQuantity = 5,
                    MaxQuantity = 100,
                    Unit = "dĩa"
                },
                new CustomizableOptionDto
                {
                    Id = "opt-xoi", // Duplicate ID
                    Name = "Xôi khác",
                    BaseQuantity = 5,
                    UnitPrice = 50000,
                    MinQuantity = 5,
                    MaxQuantity = 100,
                    Unit = "dĩa"
                }
            }
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CustomizableOptions");
    }

    [Fact]
    public async Task Validate_WithMultipleInvalidOptions_ShouldFail()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Mâm Cúng Khai Trương",
            Code = "PACKAGE010",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string> { "Item 1" },
            CustomizableOptions = new List<CustomizableOptionDto>
            {
                new CustomizableOptionDto
                {
                    Id = string.Empty, // Invalid
                    Name = "Xôi gấc đậu xanh",
                    BaseQuantity = 5,
                    UnitPrice = 45000,
                    MinQuantity = 5,
                    MaxQuantity = 100,
                    Unit = "dĩa"
                },
                new CustomizableOptionDto
                {
                    Id = "opt-che",
                    Name = "Chè trôi nước",
                    BaseQuantity = 5,
                    UnitPrice = 0, // Invalid
                    MinQuantity = 5,
                    MaxQuantity = 100,
                    Unit = "chén"
                }
            }
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 2);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task Validate_WithSingleDetailItem_ShouldPass()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Package Product",
            Code = "PACKAGE011",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string> { "Single Item" }
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_WithSingleCustomizableOption_ShouldPass()
    {
        // Arrange
        var dto = new ProductCreateDto
        {
            Name = "Mâm Cúng Khai Trương",
            Code = "PACKAGE012",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string> { "Item 1" },
            CustomizableOptions = new List<CustomizableOptionDto>
            {
                new CustomizableOptionDto
                {
                    Id = "opt-xoi",
                    Name = "Xôi gấc đậu xanh",
                    BaseQuantity = 5,
                    UnitPrice = 45000,
                    MinQuantity = 5,
                    MaxQuantity = 100,
                    Unit = "dĩa"
                }
            }
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_WithManyCustomizableOptions_ShouldPass()
    {
        // Arrange
        var options = new List<CustomizableOptionDto>();
        for (int i = 0; i < 10; i++)
        {
            options.Add(new CustomizableOptionDto
            {
                Id = $"opt-{i}",
                Name = $"Option {i}",
                BaseQuantity = 5,
                UnitPrice = 45000 + (i * 1000),
                MinQuantity = 5,
                MaxQuantity = 100,
                Unit = "dĩa"
            });
        }

        var dto = new ProductCreateDto
        {
            Name = "Mâm Cúng Khai Trương",
            Code = "PACKAGE013",
            Price = 3500000,
            StockQuantity = 5,
            IsActive = true,
            Details = new List<string> { "Item 1" },
            CustomizableOptions = options
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    #endregion
}
