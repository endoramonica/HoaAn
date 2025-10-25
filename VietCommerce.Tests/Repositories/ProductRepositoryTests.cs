using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Diagnostics;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Data.Repositories;
using VietCommerce.Tests.Base;
using Xunit;

namespace VietCommerce.Tests.Repositories;

/// <summary>
/// Kiểm thử cho ProductRepository
/// Dùng DbContext InMemory + TestDataSeeder.
/// </summary>
public class ProductRepositoryTests : TestBase
{
    [Fact]
    public async Task GetBySlugAsync_ShouldReturn_CorrectProduct()
    {
        // Arrange
        var slug = "bat-huong-men-ran";

        // Act
        var product = await _productRepository.GetBySlugAsync(slug);

        // Assert
        Assert.NotNull(product);
        Assert.Equal("Bát hương men rạn", product!.Name);
    }

    [Fact]
    public async Task GetByCodeAsync_ShouldReturn_CorrectProduct()
    {
        // Arrange
        var code = "BT01";

        // Act
        var product = await _productRepository.GetByCodeAsync(code);

        // Assert
        Assert.NotNull(product);
        Assert.Equal("Bàn thờ gỗ mít", product!.Name);
    }

    [Fact]
    public async Task GetPaginatedAsync_ShouldReturn_PagedResults()
    {
        // Act
        var result = await _productRepository.GetPaginatedAsync(pageNumber: 1, pageSize: 10);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Items.Any());
        Assert.Equal(2, result.TotalItems); // vì TestDataSeeder có 2 sản phẩm
    }

    [Fact]
    public async Task GetByCategoryIdAsync_ShouldReturn_FilteredProducts()
    {
        // Arrange
        var categoryId = Guid.Parse("00000000-0000-0000-0000-000000000010");

        // Act
        var products = await _productRepository.GetByCategoryIdAsync(categoryId);

        // Assert
        Assert.Single(products);
        Assert.Contains(products, p => p.Slug == "bat-huong-men-ran");
    }

    [Fact]
    public async Task ExistsByCodeAsync_ShouldReturn_True_WhenExists()
    {
        // Act
        var exists = await _productRepository.ExistsByCodeAsync("BH01");

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsByCodeAsync_ShouldReturn_False_WhenNotExists()
    {
        // Act
        var exists = await _productRepository.ExistsByCodeAsync("NOT_EXIST");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task GetCurrentPriceAsync_ShouldReturn_CorrectPrice()
    {
        // Arrange
        var product = (await _productRepository.GetByCodeAsync("BH01"))!;

        // Act
        var price = await _productRepository.GetCurrentPriceAsync(product.Id);

        // Assert
        Assert.NotNull(price);
        Assert.Equal(500000m, price);
    }

    [Fact]
    public async Task UpdateStockAsync_ShouldDecrease_StockCorrectly()
    {
        // Arrange
        var product = (await _productRepository.GetByCodeAsync("BH01"))!;
        var oldStock = product.Stock;
        var quantityToSubtract = 5;

        // Act
        var success = await _productRepository.UpdateStockAsync(product.Id, -quantityToSubtract);
        var updated = await _productRepository.GetByCodeAsync("BH01");

        // Assert
        Assert.True(success);
        Assert.Equal(oldStock - quantityToSubtract, updated!.Stock);
        Debug.WriteLine($"ProductId = {product.Id}");

    }

    [Fact]
    public async Task IncrementViewCountAsync_ShouldIncrease_ViewCount()
    {
        // Arrange
        var product = (await _productRepository.GetByCodeAsync("BT01"))!;
        var oldViewCount = product.ViewCount;

        // Act
        var success = await _productRepository.IncrementViewCountAsync(product.Id);
        var updated = await _productRepository.GetByCodeAsync("BT01");

        // Assert
        Assert.True(success);
        Assert.Equal(oldViewCount + 1, updated!.ViewCount);
    }
}
