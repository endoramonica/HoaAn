using FluentValidation;
using VietCommerce.Application.Validators.Marketing;
using VietCommerce.Core.DTOs.Marketing;
using Xunit;

namespace VietCommerce.Tests.Validators;

/// <summary>
/// Unit tests for Voucher validators
/// Validates: Requirements 3.1, 3.2, 3.4, 3.5
/// </summary>
public class VoucherValidatorTests
{
    private readonly GenerateVouchersDtoValidator _generateValidator = new();
    private readonly ApplyVoucherDtoValidator _applyValidator = new();

    #region GenerateVouchersDtoValidator - Valid Cases

    [Fact]
    public async Task GenerateVouchers_WithValidData_ShouldPass()
    {
        // Arrange
        var dto = new GenerateVouchersDto
        {
            Quantity = 100,
            Prefix = "SUMMER",
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _generateValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task GenerateVouchers_WithoutPrefix_ShouldPass()
    {
        // Arrange
        var dto = new GenerateVouchersDto
        {
            Quantity = 50,
            ExpiryDate = DateTime.UtcNow.AddDays(60)
        };

        // Act
        var result = await _generateValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task GenerateVouchers_WithSingleQuantity_ShouldPass()
    {
        // Arrange
        var dto = new GenerateVouchersDto
        {
            Quantity = 1,
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _generateValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task GenerateVouchers_WithMaxQuantity_ShouldPass()
    {
        // Arrange
        var dto = new GenerateVouchersDto
        {
            Quantity = 10000,
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _generateValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task GenerateVouchers_WithValidPrefixFormat_ShouldPass()
    {
        // Arrange
        var dto = new GenerateVouchersDto
        {
            Quantity = 100,
            Prefix = "PROMO_2024",
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _generateValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region GenerateVouchersDtoValidator - Invalid Cases

    [Fact]
    public async Task GenerateVouchers_WithZeroQuantity_ShouldFail()
    {
        // Arrange
        var dto = new GenerateVouchersDto
        {
            Quantity = 0,
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _generateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Quantity");
    }

    [Fact]
    public async Task GenerateVouchers_WithNegativeQuantity_ShouldFail()
    {
        // Arrange
        var dto = new GenerateVouchersDto
        {
            Quantity = -10,
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _generateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Quantity");
    }

    [Fact]
    public async Task GenerateVouchers_WithQuantityExceedingMax_ShouldFail()
    {
        // Arrange
        var dto = new GenerateVouchersDto
        {
            Quantity = 10001,
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _generateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Quantity");
    }

    [Fact]
    public async Task GenerateVouchers_WithExpiryDateInPast_ShouldFail()
    {
        // Arrange
        var dto = new GenerateVouchersDto
        {
            Quantity = 100,
            ExpiryDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = await _generateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ExpiryDate");
    }

    [Fact]
    public async Task GenerateVouchers_WithExpiryDateInPastByOneSecond_ShouldFail()
    {
        // Arrange
        var dto = new GenerateVouchersDto
        {
            Quantity = 100,
            ExpiryDate = DateTime.UtcNow.AddSeconds(-2) // Clearly in the past
        };

        // Act
        var result = await _generateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ExpiryDate");
    }

    [Fact]
    public async Task GenerateVouchers_WithInvalidPrefixFormat_ShouldFail()
    {
        // Arrange
        var dto = new GenerateVouchersDto
        {
            Quantity = 100,
            Prefix = "promo-2024", // Lowercase not allowed
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _generateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Prefix");
    }

    [Fact]
    public async Task GenerateVouchers_WithPrefixContainingSpecialCharacters_ShouldFail()
    {
        // Arrange
        var dto = new GenerateVouchersDto
        {
            Quantity = 100,
            Prefix = "PROMO@2024", // @ not allowed
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _generateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Prefix");
    }

    [Fact]
    public async Task GenerateVouchers_WithPrefixExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var dto = new GenerateVouchersDto
        {
            Quantity = 100,
            Prefix = new string('A', 51),
            ExpiryDate = DateTime.UtcNow.AddDays(30)
        };

        // Act
        var result = await _generateValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Prefix");
    }

    #endregion

    #region ApplyVoucherDtoValidator - Valid Cases

    [Fact]
    public async Task ApplyVoucher_WithValidCode_ShouldPass()
    {
        // Arrange
        var dto = new ApplyVoucherDto
        {
            VoucherCode = "SUMMER2024"
        };

        // Act
        var result = await _applyValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ApplyVoucher_WithCodeContainingNumbers_ShouldPass()
    {
        // Arrange
        var dto = new ApplyVoucherDto
        {
            VoucherCode = "PROMO123"
        };

        // Act
        var result = await _applyValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ApplyVoucher_WithCodeContainingHyphens_ShouldPass()
    {
        // Arrange
        var dto = new ApplyVoucherDto
        {
            VoucherCode = "PROMO-2024"
        };

        // Act
        var result = await _applyValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ApplyVoucher_WithCodeContainingUnderscores_ShouldPass()
    {
        // Arrange
        var dto = new ApplyVoucherDto
        {
            VoucherCode = "PROMO_2024"
        };

        // Act
        var result = await _applyValidator.ValidateAsync(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    #endregion

    #region ApplyVoucherDtoValidator - Invalid Cases

    [Fact]
    public async Task ApplyVoucher_WithEmptyCode_ShouldFail()
    {
        // Arrange
        var dto = new ApplyVoucherDto
        {
            VoucherCode = string.Empty
        };

        // Act
        var result = await _applyValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VoucherCode");
    }

    [Fact]
    public async Task ApplyVoucher_WithWhitespaceCode_ShouldFail()
    {
        // Arrange
        var dto = new ApplyVoucherDto
        {
            VoucherCode = "   "
        };

        // Act
        var result = await _applyValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VoucherCode");
    }

    [Fact]
    public async Task ApplyVoucher_WithLowercaseCode_ShouldFail()
    {
        // Arrange
        var dto = new ApplyVoucherDto
        {
            VoucherCode = "promo2024"
        };

        // Act
        var result = await _applyValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VoucherCode");
    }

    [Fact]
    public async Task ApplyVoucher_WithCodeContainingSpecialCharacters_ShouldFail()
    {
        // Arrange
        var dto = new ApplyVoucherDto
        {
            VoucherCode = "PROMO@2024"
        };

        // Act
        var result = await _applyValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VoucherCode");
    }

    [Fact]
    public async Task ApplyVoucher_WithCodeExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var dto = new ApplyVoucherDto
        {
            VoucherCode = new string('A', 101)
        };

        // Act
        var result = await _applyValidator.ValidateAsync(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VoucherCode");
    }

    #endregion
}
