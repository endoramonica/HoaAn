using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using VietCommerce.Application.Mappings;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Validators.Products;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Helpers;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;
using Xunit;

namespace VietCommerce.Tests.Services;

/// <summary>
/// Unit tests for ProductService.ValidateCustomizationsAsync
/// Validates: Requirements 3.1, 7.3
/// </summary>
public class ProductServiceValidateCustomizationsTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IPermissionService> _mockPermissionService;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly IMapper _mapper;
    private readonly CustomizableOptionValidator _customizableOptionValidator;
    private readonly ProductService _productService;

    public ProductServiceValidateCustomizationsTests()
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

    #region ValidateCustomizationsAsync - Valid Cases

    [Fact]
    public async Task ValidateCustomizationsAsync_WithValidCustomizations_ShouldReturnValid()
    {
        // Arrange
        var productId = Guid.NewGuid();

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

        var product = new Product
        {
            Id = productId,
            Name = "Mâm Cúng Khai Trương",
            Code = "MAM-KT-001",
            Slug = "mam-cung-khai-truong",
            Stock = 50,
            IsActive = true,
            StoreId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            CustomizableOptionsJson = JsonSerializationHelper.SerializeCustomizableOptions(customizableOptions),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-xoi",
                Quantity = 10,
                UnitPrice = 45000,
                TotalPrice = 450000
            },
            new()
            {
                OptionId = "opt-che",
                Quantity = 5,
                UnitPrice = 35000,
                TotalPrice = 175000
            }
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateCustomizationsAsync_WithMinimumQuantities_ShouldReturnValid()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var customizableOptions = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-1",
                Name = "Option 1",
                BaseQuantity = 5,
                UnitPrice = 50000,
                MinQuantity = 5,
                MaxQuantity = 100,
                Unit = "cái"
            }
        };

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Code = "TEST-001",
            Slug = "test-product",
            Stock = 50,
            IsActive = true,
            StoreId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            CustomizableOptionsJson = JsonSerializationHelper.SerializeCustomizableOptions(customizableOptions),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-1",
                Quantity = 5,
                UnitPrice = 50000,
                TotalPrice = 250000
            }
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateCustomizationsAsync_WithMaximumQuantities_ShouldReturnValid()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var customizableOptions = new List<CustomizableOptionDto>
        {
            new()
            {
                Id = "opt-1",
                Name = "Option 1",
                BaseQuantity = 5,
                UnitPrice = 50000,
                MinQuantity = 5,
                MaxQuantity = 100,
                Unit = "cái"
            }
        };

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Code = "TEST-001",
            Slug = "test-product",
            Stock = 50,
            IsActive = true,
            StoreId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            CustomizableOptionsJson = JsonSerializationHelper.SerializeCustomizableOptions(customizableOptions),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-1",
                Quantity = 100,
                UnitPrice = 50000,
                TotalPrice = 5000000
            }
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateCustomizationsAsync_WithUnlimitedMaxQuantity_ShouldReturnValid()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var customizableOptions = new List<CustomizableOptionDto>
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

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Code = "TEST-001",
            Slug = "test-product",
            Stock = 50,
            IsActive = true,
            StoreId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            CustomizableOptionsJson = JsonSerializationHelper.SerializeCustomizableOptions(customizableOptions),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-che",
                Quantity = 1000,
                UnitPrice = 35000,
                TotalPrice = 35000000
            }
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    #endregion

    #region ValidateCustomizationsAsync - Invalid Cases

    [Fact]
    public async Task ValidateCustomizationsAsync_WithNullCustomizations_ShouldReturnInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, null!);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Customizations list cannot be null or empty", result.Errors);
    }

    [Fact]
    public async Task ValidateCustomizationsAsync_WithEmptyCustomizations_ShouldReturnInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var customizations = new List<CartItemCustomizationDto>();

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Customizations list cannot be null or empty", result.Errors);
    }

    [Fact]
    public async Task ValidateCustomizationsAsync_WithProductNotFound_ShouldReturnInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-1",
                Quantity = 10,
                UnitPrice = 50000,
                TotalPrice = 500000
            }
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync((Product)null!);

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Product not found", result.Errors);
    }

    [Fact]
    public async Task ValidateCustomizationsAsync_WithProductHavingNoOptions_ShouldReturnInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            Name = "Simple Product",
            Code = "SIMPLE-001",
            Slug = "simple-product",
            Stock = 50,
            IsActive = true,
            StoreId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            CustomizableOptionsJson = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-1",
                Quantity = 10,
                UnitPrice = 50000,
                TotalPrice = 500000
            }
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Product does not have customizable options", result.Errors);
    }

    [Fact]
    public async Task ValidateCustomizationsAsync_WithInvalidOptionId_ShouldReturnInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();

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
            }
        };

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Code = "TEST-001",
            Slug = "test-product",
            Stock = 50,
            IsActive = true,
            StoreId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            CustomizableOptionsJson = JsonSerializationHelper.SerializeCustomizableOptions(customizableOptions),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-invalid",
                Quantity = 10,
                UnitPrice = 45000,
                TotalPrice = 450000
            }
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Option ID 'opt-invalid' not found in product", result.Errors[0]);
    }

    [Fact]
    public async Task ValidateCustomizationsAsync_WithQuantityBelowMinimum_ShouldReturnInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();

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
            }
        };

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Code = "TEST-001",
            Slug = "test-product",
            Stock = 50,
            IsActive = true,
            StoreId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            CustomizableOptionsJson = JsonSerializationHelper.SerializeCustomizableOptions(customizableOptions),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-xoi",
                Quantity = 3,
                UnitPrice = 45000,
                TotalPrice = 135000
            }
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Quantity 3 is below minimum 5", result.Errors[0]);
    }

    [Fact]
    public async Task ValidateCustomizationsAsync_WithQuantityAboveMaximum_ShouldReturnInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();

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
            }
        };

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Code = "TEST-001",
            Slug = "test-product",
            Stock = 50,
            IsActive = true,
            StoreId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            CustomizableOptionsJson = JsonSerializationHelper.SerializeCustomizableOptions(customizableOptions),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-xoi",
                Quantity = 150,
                UnitPrice = 45000,
                TotalPrice = 6750000
            }
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Quantity 150 exceeds maximum 100", result.Errors[0]);
    }

    [Fact]
    public async Task ValidateCustomizationsAsync_WithMultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var productId = Guid.NewGuid();

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
                MaxQuantity = 50,
                Unit = "chén"
            }
        };

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Code = "TEST-001",
            Slug = "test-product",
            Stock = 50,
            IsActive = true,
            StoreId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            CustomizableOptionsJson = JsonSerializationHelper.SerializeCustomizableOptions(customizableOptions),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-xoi",
                Quantity = 3,
                UnitPrice = 45000,
                TotalPrice = 135000
            },
            new()
            {
                OptionId = "opt-invalid",
                Quantity = 10,
                UnitPrice = 50000,
                TotalPrice = 500000
            },
            new()
            {
                OptionId = "opt-che",
                Quantity = 100,
                UnitPrice = 35000,
                TotalPrice = 3500000
            }
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Equal(3, result.Errors.Length);
        Assert.Contains("Quantity 3 is below minimum 5", result.Errors[0]);
        Assert.Contains("Option ID 'opt-invalid' not found in product", result.Errors[1]);
        Assert.Contains("Quantity 100 exceeds maximum 50", result.Errors[2]);
    }

    #endregion

    #region ValidateCustomizationsAsync - Edge Cases

    [Fact]
    public async Task ValidateCustomizationsAsync_WithMalformedCustomizableOptionsJson_ShouldReturnInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Code = "TEST-001",
            Slug = "test-product",
            Stock = 50,
            IsActive = true,
            StoreId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            CustomizableOptionsJson = "{invalid json",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-1",
                Quantity = 10,
                UnitPrice = 50000,
                TotalPrice = 500000
            }
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Product does not have customizable options", result.Errors);
    }

    [Fact]
    public async Task ValidateCustomizationsAsync_WithEmptyCustomizableOptionsJson_ShouldReturnInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Code = "TEST-001",
            Slug = "test-product",
            Stock = 50,
            IsActive = true,
            StoreId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            CustomizableOptionsJson = string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-1",
                Quantity = 10,
                UnitPrice = 50000,
                TotalPrice = 500000
            }
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Product does not have customizable options", result.Errors);
    }

    [Fact]
    public async Task ValidateCustomizationsAsync_WithZeroQuantity_ShouldReturnInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();

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
            }
        };

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Code = "TEST-001",
            Slug = "test-product",
            Stock = 50,
            IsActive = true,
            StoreId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            CustomizableOptionsJson = JsonSerializationHelper.SerializeCustomizableOptions(customizableOptions),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-xoi",
                Quantity = 0,
                UnitPrice = 45000,
                TotalPrice = 0
            }
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Quantity 0 is below minimum 5", result.Errors[0]);
    }

    [Fact]
    public async Task ValidateCustomizationsAsync_WithNegativeQuantity_ShouldReturnInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();

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
            }
        };

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Code = "TEST-001",
            Slug = "test-product",
            Stock = 50,
            IsActive = true,
            StoreId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            CustomizableOptionsJson = JsonSerializationHelper.SerializeCustomizableOptions(customizableOptions),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            UpdatedBy = Guid.NewGuid()
        };

        var customizations = new List<CartItemCustomizationDto>
        {
            new()
            {
                OptionId = "opt-xoi",
                Quantity = -5,
                UnitPrice = 45000,
                TotalPrice = -225000
            }
        };

        _mockUnitOfWork.Setup(x => x.Products.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.ValidateCustomizationsAsync(productId, customizations);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
        Assert.Contains("Quantity -5 is below minimum 5", result.Errors[0]);
    }

    #endregion
}
