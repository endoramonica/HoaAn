using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using VietCommerce.Application.Mappings;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Validators.Products;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Helpers;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;
using Xunit;

namespace VietCommerce.Tests.Services;

/// <summary>
/// Unit tests for ProductService GetProductByIdAsync with customizations
/// Validates: Requirements 2.1, 2.2, 2.4
/// </summary>
public class ProductServiceGetPackageTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IPermissionService> _mockPermissionService;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly IMapper _mapper;
    private readonly CustomizableOptionValidator _customizableOptionValidator;
    private readonly ProductService _productService;

    public ProductServiceGetPackageTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPermissionService = new Mock<IPermissionService>();
        _mockLogger = new Mock<ILogger<ProductService>>();
        _mockCacheService = new Mock<ICacheService>();

        // Setup AutoMapper
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductMappingProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        // Setup CustomizableOptionValidator
        var optionValidator = new CustomizableOptionDtoValidator();
        var validatorLogger = new Mock<ILogger<CustomizableOptionValidator>>();
        _customizableOptionValidator = new CustomizableOptionValidator(validatorLogger.Object, optionValidator);

        // Setup ProductService
        _productService = new ProductService(
            _mockUnitOfWork.Object,
            _mockPermissionService.Object,
            _mockLogger.Object,
            _mapper,
            _mockCacheService.Object,
            _customizableOptionValidator
        );
    }

    #region GetProductByIdAsync - With Customizations

    [Fact]
    public async Task GetProductByIdAsync_WithCustomizableOptions_ShouldDeserializeAndReturn()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var storeId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var customizableOptions = new List<CustomizableOptionDto>
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

        var details = new List<string>
        {
            "Cá chép giấy (3 con)",
            "Mũ giấy (3 cái)",
            "Vàng mã (1 bộ)"
        };

        var product = new Product
        {
            Id = productId,
            Name = "Mâm Cúng Khai Trương",
            Code = "MAM-KT-001",
            Slug = "mam-cung-khai-truong",
            Stock = 50,
            IsActive = true,
            StoreId = storeId,
            CategoryId = categoryId,
            DetailsJson = JsonSerializationHelper.SerializeDetails(details),
            CustomizableOptionsJson = JsonSerializationHelper.SerializeCustomizableOptions(customizableOptions),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid(),
            ViewCount = 10,
            FavoriteCount = 5,
            ReviewCount = 2,
            AvgRating = 4.5m
        };

        _mockCacheService
            .Setup(x => x.GetAsync<Product>(It.IsAny<string>()))
            .ReturnsAsync((Product)null!);

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);

        var productDto = result.Data;
        Assert.Equal(productId, productDto.Id);
        Assert.Equal("Mâm Cúng Khai Trương", productDto.Name);

        // Verify Details are deserialized
        Assert.NotNull(productDto.Details);
        Assert.Equal(3, productDto.Details.Count);
        Assert.Contains("Cá chép giấy (3 con)", productDto.Details);
        Assert.Contains("Mũ giấy (3 cái)", productDto.Details);
        Assert.Contains("Vàng mã (1 bộ)", productDto.Details);

        // Verify CustomizableOptions are deserialized
        Assert.NotNull(productDto.CustomizableOptions);
        Assert.Equal(2, productDto.CustomizableOptions.Count);

        var xoiOption = productDto.CustomizableOptions.First(o => o.Id == "opt-xoi");
        Assert.Equal("Xôi gấc đậu xanh", xoiOption.Name);
        Assert.Equal(45000, xoiOption.UnitPrice);
        Assert.Equal(5, xoiOption.MinQuantity);
        Assert.Equal(100, xoiOption.MaxQuantity);
        Assert.Equal("dĩa", xoiOption.Unit);

        var cheOption = productDto.CustomizableOptions.First(o => o.Id == "opt-che");
        Assert.Equal("Chè trôi nước", cheOption.Name);
        Assert.Equal(35000, cheOption.UnitPrice);
        Assert.Equal(5, cheOption.MinQuantity);
        Assert.Null(cheOption.MaxQuantity);
        Assert.Equal("chén", cheOption.Unit);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithNullCustomizableOptions_ShouldReturnEmptyList()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var storeId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            Name = "Simple Product",
            Code = "SIMPLE-001",
            Slug = "simple-product",
            Stock = 50,
            IsActive = true,
            StoreId = storeId,
            CategoryId = categoryId,
            DetailsJson = null,
            CustomizableOptionsJson = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid(),
            ViewCount = 0,
            FavoriteCount = 0,
            ReviewCount = 0,
            AvgRating = 0
        };

        _mockCacheService
            .Setup(x => x.GetAsync<Product>(It.IsAny<string>()))
            .ReturnsAsync((Product)null!);

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);

        var productDto = result.Data;
        Assert.NotNull(productDto.Details);
        Assert.Empty(productDto.Details);
        Assert.NotNull(productDto.CustomizableOptions);
        Assert.Empty(productDto.CustomizableOptions);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithEmptyJsonStrings_ShouldReturnEmptyLists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var storeId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            Name = "Product With Empty JSON",
            Code = "EMPTY-JSON-001",
            Slug = "product-with-empty-json",
            Stock = 50,
            IsActive = true,
            StoreId = storeId,
            CategoryId = categoryId,
            DetailsJson = string.Empty,
            CustomizableOptionsJson = string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid(),
            ViewCount = 0,
            FavoriteCount = 0,
            ReviewCount = 0,
            AvgRating = 0
        };

        _mockCacheService
            .Setup(x => x.GetAsync<Product>(It.IsAny<string>()))
            .ReturnsAsync((Product)null!);

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);

        var productDto = result.Data;
        Assert.NotNull(productDto.Details);
        Assert.Empty(productDto.Details);
        Assert.NotNull(productDto.CustomizableOptions);
        Assert.Empty(productDto.CustomizableOptions);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithOnlyDetails_ShouldReturnDetailsAndEmptyOptions()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var storeId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var details = new List<string>
        {
            "Item 1",
            "Item 2",
            "Item 3"
        };

        var product = new Product
        {
            Id = productId,
            Name = "Product With Details Only",
            Code = "DETAILS-ONLY-001",
            Slug = "product-with-details-only",
            Stock = 50,
            IsActive = true,
            StoreId = storeId,
            CategoryId = categoryId,
            DetailsJson = JsonSerializationHelper.SerializeDetails(details),
            CustomizableOptionsJson = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid(),
            ViewCount = 0,
            FavoriteCount = 0,
            ReviewCount = 0,
            AvgRating = 0
        };

        _mockCacheService
            .Setup(x => x.GetAsync<Product>(It.IsAny<string>()))
            .ReturnsAsync((Product)null!);

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);

        var productDto = result.Data;
        Assert.NotNull(productDto.Details);
        Assert.Equal(3, productDto.Details.Count);
        Assert.NotNull(productDto.CustomizableOptions);
        Assert.Empty(productDto.CustomizableOptions);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithInactiveProduct_ShouldRequirePermission()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var storeId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            Name = "Inactive Product",
            Code = "INACTIVE-001",
            Slug = "inactive-product",
            Stock = 50,
            IsActive = false,
            StoreId = storeId,
            CategoryId = categoryId,
            DetailsJson = null,
            CustomizableOptionsJson = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid(),
            ViewCount = 0,
            FavoriteCount = 0,
            ReviewCount = 0,
            AvgRating = 0
        };

        _mockCacheService
            .Setup(x => x.GetAsync<Product>(It.IsAny<string>()))
            .ReturnsAsync((Product)null!);

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _mockPermissionService.Setup(x => x.CheckUserPermissionAsync(userId, "product.view"))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _productService.GetProductByIdAsync(productId, userId)
        );

        Assert.Contains("Product not available", exception.Message);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithInactiveProductAndPermission_ShouldReturn()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var storeId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            Name = "Inactive Product",
            Code = "INACTIVE-001",
            Slug = "inactive-product",
            Stock = 50,
            IsActive = false,
            StoreId = storeId,
            CategoryId = categoryId,
            DetailsJson = null,
            CustomizableOptionsJson = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid(),
            ViewCount = 0,
            FavoriteCount = 0,
            ReviewCount = 0,
            AvgRating = 0
        };

        _mockCacheService
            .Setup(x => x.GetAsync<Product>(It.IsAny<string>()))
            .ReturnsAsync((Product)null!);

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _mockPermissionService.Setup(x => x.CheckUserPermissionAsync(userId, "product.view"))
            .ReturnsAsync(true);

        // Act
        var result = await _productService.GetProductByIdAsync(productId, userId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Inactive Product", result.Data.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithProductNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mockCacheService
            .Setup(x => x.GetAsync<Product>(It.IsAny<string>()))
            .ReturnsAsync((Product)null!);

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync((Product)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _productService.GetProductByIdAsync(productId)
        );

        Assert.Contains("Product not found", exception.Message);
    }

    #endregion

    #region GetProductByIdAsync - Deserialization Edge Cases

    [Fact]
    public async Task GetProductByIdAsync_WithMalformedJson_ShouldReturnEmptyLists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var storeId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            Name = "Product With Malformed JSON",
            Code = "MALFORMED-001",
            Slug = "product-with-malformed-json",
            Stock = 50,
            IsActive = true,
            StoreId = storeId,
            CategoryId = categoryId,
            DetailsJson = "{invalid json",
            CustomizableOptionsJson = "[{invalid}]",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid(),
            ViewCount = 0,
            FavoriteCount = 0,
            ReviewCount = 0,
            AvgRating = 0
        };

        _mockCacheService
            .Setup(x => x.GetAsync<Product>(It.IsAny<string>()))
            .ReturnsAsync((Product)null!);

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);

        var productDto = result.Data;
        // Malformed JSON should be handled gracefully and return empty lists
        Assert.NotNull(productDto.Details);
        Assert.Empty(productDto.Details);
        Assert.NotNull(productDto.CustomizableOptions);
        Assert.Empty(productDto.CustomizableOptions);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithComplexCustomizableOptions_ShouldDeserializeCorrectly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var storeId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var customizableOptions = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-1",
                Name = "Option 1",
                BaseQuantity = 1,
                UnitPrice = 10000.50m,
                MinQuantity = 1,
                MaxQuantity = 1000,
                Unit = "cái"
            },
            new()
            {
                Id = "opt-2",
                Name = "Option 2",
                BaseQuantity = 5,
                UnitPrice = 50000,
                MinQuantity = 5,
                MaxQuantity = null,
                Unit = "bộ"
            },
            new()
            {
                Id = "opt-3",
                Name = "Option 3",
                BaseQuantity = 10,
                UnitPrice = 100000,
                MinQuantity = 10,
                MaxQuantity = 10,
                Unit = "set"
            }
        };

        var product = new Product
        {
            Id = productId,
            Name = "Complex Package",
            Code = "COMPLEX-001",
            Slug = "complex-package",
            Stock = 50,
            IsActive = true,
            StoreId = storeId,
            CategoryId = categoryId,
            DetailsJson = null,
            CustomizableOptionsJson = JsonSerializationHelper.SerializeCustomizableOptions(customizableOptions),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid(),
            ViewCount = 0,
            FavoriteCount = 0,
            ReviewCount = 0,
            AvgRating = 0
        };

        _mockCacheService
            .Setup(x => x.GetAsync<Product>(It.IsAny<string>()))
            .ReturnsAsync((Product)null!);

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.Data);

        var productDto = result.Data;
        Assert.NotNull(productDto.CustomizableOptions);
        Assert.Equal(3, productDto.CustomizableOptions.Count);

        // Verify all options are deserialized correctly
        var opt1 = productDto.CustomizableOptions.First(o => o.Id == "opt-1");
        Assert.Equal(10000.50m, opt1.UnitPrice);
        Assert.Equal(1000, opt1.MaxQuantity);

        var opt2 = productDto.CustomizableOptions.First(o => o.Id == "opt-2");
        Assert.Null(opt2.MaxQuantity);

        var opt3 = productDto.CustomizableOptions.First(o => o.Id == "opt-3");
        Assert.Equal(10, opt3.MinQuantity);
        Assert.Equal(10, opt3.MaxQuantity);
    }

    #endregion
}
