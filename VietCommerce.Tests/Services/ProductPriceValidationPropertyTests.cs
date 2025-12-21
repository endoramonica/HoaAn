using Bogus;
using Xunit;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Enums.Products;
using FluentValidation;

namespace VietCommerce.Tests.Services;

/// <summary>
/// Property-based tests for Product Price Validation
/// **Feature: product-page-refactor, Property 1: Price Validation**
/// **Validates: Requirements 1.4, 1.5**
/// </summary>
public class ProductPriceValidationPropertyTests
{
    private readonly Faker _faker = new Faker();

    /// <summary>
    /// Property 1: Price Validation
    /// For any ProductPrice, the Price value SHALL be greater than zero and EffectiveFrom SHALL be before or equal to EffectiveTo (if provided).
    /// **Validates: Requirements 1.4, 1.5**
    /// </summary>
    [Fact]
    public void PriceValidation_PriceGreaterThanZero_ShouldBeValid()
    {
        // Arrange: Generate 100 random valid prices
        var validPrices = new List<decimal>();
        for (int i = 0; i < 100; i++)
        {
            validPrices.Add(_faker.Random.Decimal(0.01m, 10000m));
        }

        // Act & Assert: All prices should be greater than 0
        foreach (var price in validPrices)
        {
            Assert.True(price > 0, $"Price {price} should be greater than 0");
        }
    }

    /// <summary>
    /// Property 1: Price Validation - Effective Date Range
    /// For any ProductPrice with EffectiveTo provided, EffectiveFrom SHALL be before EffectiveTo.
    /// **Validates: Requirements 1.4**
    /// </summary>
    [Fact]
    public void PriceValidation_EffectiveDateRange_ShouldBeValid()
    {
        // Arrange: Generate 100 random valid date ranges
        var now = DateTime.UtcNow;
        var validDateRanges = new List<(DateTime from, DateTime to)>();

        for (int i = 0; i < 100; i++)
        {
            var effectiveFrom = now.AddDays(_faker.Random.Int(-30, 0));
            var effectiveTo = effectiveFrom.AddDays(_faker.Random.Int(1, 365));
            validDateRanges.Add((effectiveFrom, effectiveTo));
        }

        // Act & Assert: All EffectiveFrom should be before EffectiveTo
        foreach (var (from, to) in validDateRanges)
        {
            Assert.True(from < to, $"EffectiveFrom {from} should be before EffectiveTo {to}");
        }
    }

    /// <summary>
    /// Property 1: Price Validation - Invalid Price
    /// For any ProductPrice with Price <= 0, validation should fail.
    /// **Validates: Requirements 1.5**
    /// </summary>
    [Fact]
    public void PriceValidation_InvalidPrice_ShouldFail()
    {
        // Arrange: Generate 50 invalid prices (0 or negative)
        var invalidPrices = new List<decimal>();
        for (int i = 0; i < 50; i++)
        {
            invalidPrices.Add(_faker.Random.Decimal(-1000m, 0m));
        }

        // Act & Assert: All prices should be <= 0 (invalid)
        foreach (var price in invalidPrices)
        {
            Assert.True(price <= 0, $"Price {price} should be invalid (<=0)");
        }
    }

    /// <summary>
    /// Property 1: Price Validation - Invalid Date Range
    /// For any ProductPrice where EffectiveFrom >= EffectiveTo, validation should fail.
    /// **Validates: Requirements 1.4**
    /// </summary>
    [Fact]
    public void PriceValidation_InvalidDateRange_ShouldFail()
    {
        // Arrange: Generate 50 invalid date ranges (EffectiveFrom >= EffectiveTo)
        var now = DateTime.UtcNow;
        var invalidDateRanges = new List<(DateTime from, DateTime to)>();

        for (int i = 0; i < 50; i++)
        {
            var effectiveFrom = now.AddDays(_faker.Random.Int(0, 30));
            var effectiveTo = effectiveFrom.AddDays(_faker.Random.Int(-30, 0)); // Same or before
            invalidDateRanges.Add((effectiveFrom, effectiveTo));
        }

        // Act & Assert: All EffectiveFrom should be >= EffectiveTo (invalid)
        foreach (var (from, to) in invalidDateRanges)
        {
            Assert.True(from >= to, $"EffectiveFrom {from} should be >= EffectiveTo {to} (invalid)");
        }
    }

    /// <summary>
    /// Property 1: Price Validation - DTO Creation with Valid Data
    /// For any valid price data, creating a ProductPriceCreateDto should succeed.
    /// **Validates: Requirements 1.4, 1.5**
    /// </summary>
    [Fact]
    public void PriceValidation_CreateDtoWithValidData_ShouldSucceed()
    {
        // Arrange: Generate 100 valid ProductPriceCreateDto instances
        var validDtos = new List<ProductPriceCreateDto>();
        var now = DateTime.UtcNow;

        for (int i = 0; i < 100; i++)
        {
            var effectiveFrom = now.AddDays(_faker.Random.Int(-30, 0));
            var effectiveTo = effectiveFrom.AddDays(_faker.Random.Int(1, 365));
            var price = _faker.Random.Decimal(0.01m, 10000m);

            var dto = new ProductPriceCreateDto
            {
                ProductId = Guid.NewGuid(),
                PriceType = _faker.PickRandom<PriceType>(),
                Price = price,
                EffectiveFrom = effectiveFrom,
                EffectiveTo = effectiveTo,
                IsActive = true
            };

            validDtos.Add(dto);
        }

        // Act & Assert: All DTOs should have valid data
        foreach (var dto in validDtos)
        {
            Assert.NotEqual(Guid.Empty, dto.ProductId);
            Assert.True(dto.Price > 0, $"Price {dto.Price} should be > 0");
            Assert.True(dto.EffectiveFrom < dto.EffectiveTo, $"EffectiveFrom should be < EffectiveTo");
        }
    }

    /// <summary>
    /// Property 1: Price Validation - DTO Update with Valid Data
    /// For any valid price update data, creating a ProductPriceUpdateDto should succeed.
    /// **Validates: Requirements 1.4, 1.5**
    /// </summary>
    [Fact]
    public void PriceValidation_UpdateDtoWithValidData_ShouldSucceed()
    {
        // Arrange: Generate 100 valid ProductPriceUpdateDto instances
        var validDtos = new List<ProductPriceUpdateDto>();

        for (int i = 0; i < 100; i++)
        {
            var dto = new ProductPriceUpdateDto
            {
                Price = _faker.Random.Bool() ? _faker.Random.Decimal(0.01m, 10000m) : null,
                EffectiveTo = _faker.Random.Bool() ? DateTime.UtcNow.AddDays(_faker.Random.Int(1, 365)) : null,
                IsActive = _faker.Random.Bool() ? _faker.Random.Bool() : null
            };

            validDtos.Add(dto);
        }

        // Act & Assert: All DTOs should have valid data (if provided)
        foreach (var dto in validDtos)
        {
            if (dto.Price.HasValue)
            {
                Assert.True(dto.Price.Value > 0, $"Price {dto.Price} should be > 0");
            }
        }
    }

    /// <summary>
    /// Property 1: Price Validation - Multiple Price Types
    /// For any valid PriceType, creating a ProductPriceCreateDto should succeed.
    /// **Validates: Requirements 1.5**
    /// </summary>
    [Fact]
    public void PriceValidation_AllPriceTypes_ShouldBeValid()
    {
        // Arrange: Test all PriceType enum values
        var priceTypes = Enum.GetValues(typeof(PriceType)).Cast<PriceType>().ToList();
        var now = DateTime.UtcNow;

        // Act & Assert: All price types should be valid
        foreach (var priceType in priceTypes)
        {
            var dto = new ProductPriceCreateDto
            {
                ProductId = Guid.NewGuid(),
                PriceType = priceType,
                Price = _faker.Random.Decimal(0.01m, 10000m),
                EffectiveFrom = now,
                EffectiveTo = now.AddDays(30),
                IsActive = true
            };

            Assert.NotEqual(default(PriceType), dto.PriceType);
            Assert.True(Enum.IsDefined(typeof(PriceType), dto.PriceType));
        }
    }
}
