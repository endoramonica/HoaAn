using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.Helpers;
using Xunit;

namespace VietCommerce.Tests.Helpers;

/// <summary>
/// Unit tests for PriceCalculationHelper
/// Tests price calculation for package products with customizations
/// Validates: Requirements 3.3
/// </summary>
public class PriceCalculationHelperTests
{
    #region CalculateFinalPrice Tests

    [Fact]
    public void CalculateFinalPrice_WithNullCustomizations_ShouldReturnBasePrice()
    {
        // Arrange
        decimal basePrice = 3500000m;

        // Act
        var finalPrice = PriceCalculationHelper.CalculateFinalPrice(basePrice, null);

        // Assert
        Assert.Equal(basePrice, finalPrice);
    }

    [Fact]
    public void CalculateFinalPrice_WithEmptyCustomizations_ShouldReturnBasePrice()
    {
        // Arrange
        decimal basePrice = 3500000m;
        var customizations = new List<CartItemCustomizationDto>();

        // Act
        var finalPrice = PriceCalculationHelper.CalculateFinalPrice(basePrice, customizations);

        // Assert
        Assert.Equal(basePrice, finalPrice);
    }

    [Fact]
    public void CalculateFinalPrice_WithSingleCustomization_ShouldCalculateCorrectly()
    {
        // Arrange
        decimal basePrice = 3500000m;
        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-xoi",
                Quantity = 10,
                UnitPrice = 45000m,
                TotalPrice = 450000m
            }
        };

        // Act
        var finalPrice = PriceCalculationHelper.CalculateFinalPrice(basePrice, customizations);

        // Assert
        // Expected: 3500000 + (10 * 45000) = 3500000 + 450000 = 3950000
        Assert.Equal(3950000m, finalPrice);
    }

    [Fact]
    public void CalculateFinalPrice_WithMultipleCustomizations_ShouldSumAllSurcharges()
    {
        // Arrange
        decimal basePrice = 3500000m;
        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-xoi",
                Quantity = 10,
                UnitPrice = 45000m,
                TotalPrice = 450000m
            },
            new()
            {
                OptionId = "opt-che",
                Quantity = 5,
                UnitPrice = 35000m,
                TotalPrice = 175000m
            },
            new()
            {
                OptionId = "opt-ruou",
                Quantity = 2,
                UnitPrice = 150000m,
                TotalPrice = 300000m
            }
        };

        // Act
        var finalPrice = PriceCalculationHelper.CalculateFinalPrice(basePrice, customizations);

        // Assert
        // Expected: 3500000 + (10*45000 + 5*35000 + 2*150000) = 3500000 + 925000 = 4425000
        Assert.Equal(4425000m, finalPrice);
    }

    [Fact]
    public void CalculateFinalPrice_WithZeroQuantityCustomization_ShouldHandleCorrectly()
    {
        // Arrange
        decimal basePrice = 3500000m;
        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-xoi",
                Quantity = 0,
                UnitPrice = 45000m,
                TotalPrice = 0m
            }
        };

        // Act
        var finalPrice = PriceCalculationHelper.CalculateFinalPrice(basePrice, customizations);

        // Assert
        // Expected: 3500000 + (0 * 45000) = 3500000
        Assert.Equal(basePrice, finalPrice);
    }

    [Fact]
    public void CalculateFinalPrice_WithZeroUnitPrice_ShouldHandleCorrectly()
    {
        // Arrange
        decimal basePrice = 3500000m;
        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-free",
                Quantity = 10,
                UnitPrice = 0m,
                TotalPrice = 0m
            }
        };

        // Act
        var finalPrice = PriceCalculationHelper.CalculateFinalPrice(basePrice, customizations);

        // Assert
        // Expected: 3500000 + (10 * 0) = 3500000
        Assert.Equal(basePrice, finalPrice);
    }

    [Fact]
    public void CalculateFinalPrice_WithLargeQuantities_ShouldCalculateCorrectly()
    {
        // Arrange
        decimal basePrice = 1000000m;
        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-bulk",
                Quantity = 1000,
                UnitPrice = 50000m,
                TotalPrice = 50000000m
            }
        };

        // Act
        var finalPrice = PriceCalculationHelper.CalculateFinalPrice(basePrice, customizations);

        // Assert
        // Expected: 1000000 + (1000 * 50000) = 1000000 + 50000000 = 51000000
        Assert.Equal(51000000m, finalPrice);
    }

    [Fact]
    public void CalculateFinalPrice_WithDecimalPrices_ShouldCalculateCorrectly()
    {
        // Arrange
        decimal basePrice = 3500000.50m;
        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-xoi",
                Quantity = 10,
                UnitPrice = 45000.75m,
                TotalPrice = 450007.50m
            }
        };

        // Act
        var finalPrice = PriceCalculationHelper.CalculateFinalPrice(basePrice, customizations);

        // Assert
        // Expected: 3500000.50 + (10 * 45000.75) = 3500000.50 + 450007.50 = 3950008.00
        Assert.Equal(3950008.00m, finalPrice);
    }

    [Fact]
    public void CalculateFinalPrice_WithZeroBasePrice_ShouldCalculateCorrectly()
    {
        // Arrange
        decimal basePrice = 0m;
        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-xoi",
                Quantity = 10,
                UnitPrice = 45000m,
                TotalPrice = 450000m
            }
        };

        // Act
        var finalPrice = PriceCalculationHelper.CalculateFinalPrice(basePrice, customizations);

        // Assert
        // Expected: 0 + (10 * 45000) = 450000
        Assert.Equal(450000m, finalPrice);
    }

    [Fact]
    public void CalculateFinalPrice_WithNegativeBasePrice_ShouldCalculateCorrectly()
    {
        // Arrange - This is an edge case that shouldn't happen in practice, but we test it
        decimal basePrice = -1000m;
        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-xoi",
                Quantity = 10,
                UnitPrice = 45000m,
                TotalPrice = 450000m
            }
        };

        // Act
        var finalPrice = PriceCalculationHelper.CalculateFinalPrice(basePrice, customizations);

        // Assert
        // Expected: -1000 + (10 * 45000) = 449000
        Assert.Equal(449000m, finalPrice);
    }

    #endregion
}
