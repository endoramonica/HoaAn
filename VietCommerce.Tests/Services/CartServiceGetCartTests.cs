using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using VietCommerce.Application.Mappings;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Cart;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Tests for CartService GetCartAsync with customization deserialization.
    /// Validates: Requirements 8.1, 8.2
    /// </summary>
    public class CartServiceGetCartTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPermissionService> _mockPermissionService;
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly Mock<ILogger<CartService>> _mockLogger;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly IMapper _mapper;
        private readonly CartService _cartService;

        public CartServiceGetCartTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPermissionService = new Mock<IPermissionService>();
            _mockProductService = new Mock<IProductService>();
            _mockCartRepository = new Mock<ICartRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLogger = new Mock<ILogger<CartService>>();
            _mockCacheService = new Mock<ICacheService>();

            // Setup mapper
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CartMappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            // Setup UnitOfWork
            _mockUnitOfWork.Setup(u => u.Carts).Returns(_mockCartRepository.Object);

            // Setup cache service
            _mockCacheService.Setup(c => c.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            _cartService = new CartService(
                _mockUnitOfWork.Object,
                _mockPermissionService.Object,
                _mockProductService.Object,
                _mapper,
                _mockHttpContextAccessor.Object,
                _mockLogger.Object,
                _mockCacheService.Object
            );
        }

        [Fact]
        public async Task GetCartAsync_WithCustomizations_ShouldDeserializeAndReturnCustomizations()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cartId = Guid.NewGuid();
            var cartItemId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Mâm Cúng Khai Trương",
                Stock = 100,
                Slug = "mam-cung-khai-truong",
                Code = "SKU-001",
                IsActive = true,
                PurchaseCount = 10,
                AvgRating = 4.5m,
                ReviewCount = 5,
                Prices = new List<ProductPrice>
                {
                    new ProductPrice
                    {
                        Price = 3500000,
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow.AddDays(-1),
                        EffectiveTo = null
                    }
                },
                Images = new List<ProductImage>()
            };

            var customizationsJson = "[{\"optionId\":\"opt-xoi\",\"quantity\":10,\"unitPrice\":45000,\"totalPrice\":450000}]";

            var cartItem = new CartItem
            {
                Id = cartItemId,
                CartId = cartId,
                ProductId = productId,
                Product = product,
                Quantity = 1,
                BasePrice = 3500000,
                CustomizationPrice = 450000,
                FinalPrice = 3950000,
                CustomizationsJson = customizationsJson,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var cart = new Cart
            {
                Id = cartId,
                UserId = userId,
                CartItems = new List<CartItem> { cartItem },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Setup mocks
            _mockPermissionService
                .Setup(p => p.CheckUserPermissionAsync(userId, "cart.view"))
                .ReturnsAsync(true);

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                .ReturnsAsync(new User { Id = userId });

            _mockCartRepository.Setup(r => r.GetUserCartWithItemsAsync(userId))
                .ReturnsAsync(cart);

            _mockCacheService.Setup(c => c.GetAsync<Cart>(It.IsAny<string>()))
                .ReturnsAsync((Cart)null);

            _mockCacheService.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<Cart>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cartService.GetCartAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(cartId, result.Data.CartId);
            Assert.Single(result.Data.Items);

            var item = result.Data.Items.First();
            Assert.Equal(cartItemId, item.CartItemId);
            Assert.Equal(productId, item.ProductId);
            Assert.Equal("Mâm Cúng Khai Trương", item.ProductName);
            Assert.Equal(3500000, item.BasePrice);
            Assert.Equal(450000, item.CustomizationPrice);
            Assert.Equal(3950000, item.FinalPrice);
            Assert.NotNull(item.Customizations);
            Assert.Single(item.Customizations);
            Assert.Equal("opt-xoi", item.Customizations.First().OptionId);
            Assert.Equal(10, item.Customizations.First().Quantity);
            Assert.Equal(45000, item.Customizations.First().UnitPrice);
            Assert.Equal(450000, item.Customizations.First().TotalPrice);
        }

        [Fact]
        public async Task GetCartAsync_WithoutCustomizations_ShouldReturnNullCustomizations()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cartId = Guid.NewGuid();
            var cartItemId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Mâm Cúng Cơ Bản",
                Stock = 100,
                Slug = "mam-cung-co-ban",
                Code = "SKU-002",
                IsActive = true,
                PurchaseCount = 5,
                AvgRating = 4.0m,
                ReviewCount = 3,
                Prices = new List<ProductPrice>
                {
                    new ProductPrice
                    {
                        Price = 2500000,
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow.AddDays(-1),
                        EffectiveTo = null
                    }
                },
                Images = new List<ProductImage>()
            };

            var cartItem = new CartItem
            {
                Id = cartItemId,
                CartId = cartId,
                ProductId = productId,
                Product = product,
                Quantity = 1,
                BasePrice = 2500000,
                CustomizationPrice = 0,
                FinalPrice = 2500000,
                CustomizationsJson = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var cart = new Cart
            {
                Id = cartId,
                UserId = userId,
                CartItems = new List<CartItem> { cartItem },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Setup mocks
            _mockPermissionService
                .Setup(p => p.CheckUserPermissionAsync(userId, "cart.view"))
                .ReturnsAsync(true);

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                .ReturnsAsync(new User { Id = userId });

            _mockCartRepository.Setup(r => r.GetUserCartWithItemsAsync(userId))
                .ReturnsAsync(cart);

            _mockCacheService.Setup(c => c.GetAsync<Cart>(It.IsAny<string>()))
                .ReturnsAsync((Cart)null);

            _mockCacheService.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<Cart>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cartService.GetCartAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data.Items);

            var item = result.Data.Items.First();
            Assert.Equal(2500000, item.BasePrice);
            Assert.Equal(0, item.CustomizationPrice);
            Assert.Equal(2500000, item.FinalPrice);
            Assert.Null(item.Customizations);
        }

        [Fact]
        public async Task GetCartAsync_WithMultipleItems_ShouldReturnAllItemsWithCustomizations()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cartId = Guid.NewGuid();

            var product1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Mâm Cúng 1",
                Stock = 100,
                Slug = "mam-cung-1",
                Code = "SKU-001",
                IsActive = true,
                PurchaseCount = 10,
                AvgRating = 4.5m,
                ReviewCount = 5,
                Prices = new List<ProductPrice>
                {
                    new ProductPrice
                    {
                        Price = 3500000,
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow.AddDays(-1),
                        EffectiveTo = null
                    }
                },
                Images = new List<ProductImage>()
            };

            var product2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Mâm Cúng 2",
                Stock = 100,
                Slug = "mam-cung-2",
                Code = "SKU-002",
                IsActive = true,
                PurchaseCount = 5,
                AvgRating = 4.0m,
                ReviewCount = 3,
                Prices = new List<ProductPrice>
                {
                    new ProductPrice
                    {
                        Price = 2500000,
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow.AddDays(-1),
                        EffectiveTo = null
                    }
                },
                Images = new List<ProductImage>()
            };

            var cartItem1 = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cartId,
                ProductId = product1.Id,
                Product = product1,
                Quantity = 1,
                BasePrice = 3500000,
                CustomizationPrice = 450000,
                FinalPrice = 3950000,
                CustomizationsJson = "[{\"optionId\":\"opt-xoi\",\"quantity\":10,\"unitPrice\":45000,\"totalPrice\":450000}]",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var cartItem2 = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cartId,
                ProductId = product2.Id,
                Product = product2,
                Quantity = 1,
                BasePrice = 2500000,
                CustomizationPrice = 0,
                FinalPrice = 2500000,
                CustomizationsJson = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var cart = new Cart
            {
                Id = cartId,
                UserId = userId,
                CartItems = new List<CartItem> { cartItem1, cartItem2 },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Setup mocks
            _mockPermissionService
                .Setup(p => p.CheckUserPermissionAsync(userId, "cart.view"))
                .ReturnsAsync(true);

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(userId))
                .ReturnsAsync(new User { Id = userId });

            _mockCartRepository.Setup(r => r.GetUserCartWithItemsAsync(userId))
                .ReturnsAsync(cart);

            _mockCacheService.Setup(c => c.GetAsync<Cart>(It.IsAny<string>()))
                .ReturnsAsync((Cart)null);

            _mockCacheService.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<Cart>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cartService.GetCartAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Items.Count);

            // Check first item with customizations
            var item1 = result.Data.Items.First();
            Assert.Equal(3500000, item1.BasePrice);
            Assert.Equal(450000, item1.CustomizationPrice);
            Assert.Equal(3950000, item1.FinalPrice);
            Assert.NotNull(item1.Customizations);
            Assert.Single(item1.Customizations);

            // Check second item without customizations
            var item2 = result.Data.Items.Last();
            Assert.Equal(2500000, item2.BasePrice);
            Assert.Equal(0, item2.CustomizationPrice);
            Assert.Equal(2500000, item2.FinalPrice);
            Assert.Null(item2.Customizations);

            // Check total calculation
            Assert.Equal(6450000, result.Data.SubTotal);
        }
    }
}
