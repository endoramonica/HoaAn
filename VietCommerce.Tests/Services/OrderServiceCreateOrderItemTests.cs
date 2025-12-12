using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using VietCommerce.Application.Mappings;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Tests for OrderService CreateOrderItemFromCartItemAsync method.
    /// Validates: Requirements 5.1, 5.2, 5.3
    /// Feature: package-customizable-products, Property 4: Order item snapshot
    /// </summary>
    public class OrderServiceCreateOrderItemTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<OrderService>> _mockLogger;
        private readonly Mock<ICurrentUser> _mockCurrentUser;
        private readonly OrderService _orderService;

        public OrderServiceCreateOrderItemTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<OrderService>>();
            _mockCurrentUser = new Mock<ICurrentUser>();

            // Setup UnitOfWork
            _mockUnitOfWork.Setup(u => u.Orders).Returns(_mockOrderRepository.Object);

            // Setup current user
            _mockCurrentUser.Setup(c => c.UserId).Returns(Guid.NewGuid());
            _mockCurrentUser.Setup(c => c.IsAdmin).Returns(false);

            _orderService = new OrderService(
                _mockOrderRepository.Object,
                _mockUnitOfWork.Object,
                _mockMapper.Object,
                _mockLogger.Object,
                _mockCurrentUser.Object
            );
        }

        [Fact]
        public async Task CreateOrderItemFromCartItemAsync_WithCustomizations_ShouldSnapshotAllData()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var cartItemId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Mâm Cúng Khai Trương",
                Code = "SKU-001",
                Stock = 100,
                Slug = "mam-cung-khai-truong",
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
                CartId = Guid.NewGuid(),
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

            _mockCurrentUser.Setup(c => c.UserId).Returns(userId);

            // Act
            var result = await _orderService.CreateOrderItemFromCartItemAsync(cartItem, orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal("Mâm Cúng Khai Trương", result.ProductName);
            Assert.Equal("SKU-001", result.ProductCode);
            Assert.Equal(1, result.Quantity);

            // Verify snapshot data
            Assert.Equal(3500000, result.BasePrice);
            Assert.Equal(450000, result.CustomizationPrice);
            Assert.Equal(3950000, result.TotalPrice);
            Assert.Equal(customizationsJson, result.CustomizationsJson);

            // Verify audit fields
            Assert.Equal(userId, result.CreatedBy);
            Assert.True(result.IsActive);
            Assert.False(result.IsDeleted);
        }

        [Fact]
        public async Task CreateOrderItemFromCartItemAsync_WithoutCustomizations_ShouldSnapshotBasePrice()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var cartItemId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Mâm Cúng Cơ Bản",
                Code = "SKU-002",
                Stock = 100,
                Slug = "mam-cung-co-ban",
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
                CartId = Guid.NewGuid(),
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

            _mockCurrentUser.Setup(c => c.UserId).Returns(userId);

            // Act
            var result = await _orderService.CreateOrderItemFromCartItemAsync(cartItem, orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal("Mâm Cúng Cơ Bản", result.ProductName);
            Assert.Equal("SKU-002", result.ProductCode);
            Assert.Equal(1, result.Quantity);

            // Verify snapshot data
            Assert.Equal(2500000, result.BasePrice);
            Assert.Equal(0, result.CustomizationPrice);
            Assert.Equal(2500000, result.TotalPrice);
            Assert.Null(result.CustomizationsJson);

            // Verify audit fields
            Assert.Equal(userId, result.CreatedBy);
            Assert.True(result.IsActive);
            Assert.False(result.IsDeleted);
        }

        [Fact]
        public async Task CreateOrderItemFromCartItemAsync_WithMultipleCustomizations_ShouldPreserveAllData()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var cartItemId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Mâm Cúng Đầy Đủ",
                Code = "SKU-003",
                Stock = 100,
                Slug = "mam-cung-day-du",
                IsActive = true,
                PurchaseCount = 20,
                AvgRating = 4.8m,
                ReviewCount = 10,
                Prices = new List<ProductPrice>
                {
                    new ProductPrice
                    {
                        Price = 5000000,
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow.AddDays(-1),
                        EffectiveTo = null
                    }
                },
                Images = new List<ProductImage>()
            };

            var customizationsJson = "[" +
                "{\"optionId\":\"opt-xoi\",\"quantity\":10,\"unitPrice\":45000,\"totalPrice\":450000}," +
                "{\"optionId\":\"opt-che\",\"quantity\":5,\"unitPrice\":35000,\"totalPrice\":175000}" +
                "]";

            var cartItem = new CartItem
            {
                Id = cartItemId,
                CartId = Guid.NewGuid(),
                ProductId = productId,
                Product = product,
                Quantity = 2,
                BasePrice = 5000000,
                CustomizationPrice = 625000,
                FinalPrice = 5625000,
                CustomizationsJson = customizationsJson,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockCurrentUser.Setup(c => c.UserId).Returns(userId);

            // Act
            var result = await _orderService.CreateOrderItemFromCartItemAsync(cartItem, orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal("Mâm Cúng Đầy Đủ", result.ProductName);
            Assert.Equal("SKU-003", result.ProductCode);
            Assert.Equal(2, result.Quantity);

            // Verify snapshot data
            Assert.Equal(5000000, result.BasePrice);
            Assert.Equal(625000, result.CustomizationPrice);
            Assert.Equal(5625000, result.TotalPrice);
            Assert.Equal(customizationsJson, result.CustomizationsJson);

            // Verify audit fields
            Assert.Equal(userId, result.CreatedBy);
            Assert.True(result.IsActive);
            Assert.False(result.IsDeleted);
        }

        [Fact]
        public async Task CreateOrderItemFromCartItemAsync_WithNullProductName_ShouldUseDefaultValue()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var cartItemId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var cartItem = new CartItem
            {
                Id = cartItemId,
                CartId = Guid.NewGuid(),
                ProductId = productId,
                Product = null!, // Product is null
                Quantity = 1,
                BasePrice = 1000000,
                CustomizationPrice = 0,
                FinalPrice = 1000000,
                CustomizationsJson = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockCurrentUser.Setup(c => c.UserId).Returns(userId);

            // Act
            var result = await _orderService.CreateOrderItemFromCartItemAsync(cartItem, orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Unknown Product", result.ProductName);
            Assert.Equal("UNKNOWN", result.ProductCode);
        }

        [Fact]
        public async Task CreateOrderItemFromCartItemAsync_ShouldPreserveHistoricalAccuracy()
        {
            // Arrange - Simulate a scenario where product price changes after cart addition
            var orderId = Guid.NewGuid();
            var cartItemId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                Name = "Mâm Cúng",
                Code = "SKU-001",
                Stock = 100,
                Slug = "mam-cung",
                IsActive = true,
                PurchaseCount = 10,
                AvgRating = 4.5m,
                ReviewCount = 5,
                Prices = new List<ProductPrice>
                {
                    new ProductPrice
                    {
                        Price = 4000000, // Current price is higher
                        IsActive = true,
                        EffectiveFrom = DateTime.UtcNow,
                        EffectiveTo = null
                    }
                },
                Images = new List<ProductImage>()
            };

            // CartItem has the old price (3500000) from when it was added
            var cartItem = new CartItem
            {
                Id = cartItemId,
                CartId = Guid.NewGuid(),
                ProductId = productId,
                Product = product,
                Quantity = 1,
                BasePrice = 3500000, // Old price
                CustomizationPrice = 450000,
                FinalPrice = 3950000,
                CustomizationsJson = "[{\"optionId\":\"opt-xoi\",\"quantity\":10,\"unitPrice\":45000,\"totalPrice\":450000}]",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockCurrentUser.Setup(c => c.UserId).Returns(userId);

            // Act
            var result = await _orderService.CreateOrderItemFromCartItemAsync(cartItem, orderId);

            // Assert - OrderItem should preserve the old price from CartItem, not the current product price
            Assert.NotNull(result);
            Assert.Equal(3500000, result.BasePrice); // Should be old price, not 4000000
            Assert.Equal(3950000, result.TotalPrice); // Should be old total, not new total
        }
    }
}
