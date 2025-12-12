using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using VietCommerce.Application.Services.Services;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Data.Context;

namespace VietCommerce.Tests.Services
{
    /// <summary>
    /// Tests for CustomizationAnalyticsService.
    /// Validates: Requirements 9.1, 9.2, 9.3
    /// Feature: package-customizable-products
    /// </summary>
    public class CustomizationAnalyticsServiceTests
    {
        private readonly Mock<ILogger<CustomizationAnalyticsService>> _mockLogger;
        private readonly AppDbContext _context;
        private readonly CustomizationAnalyticsService _service;

        public CustomizationAnalyticsServiceTests()
        {
            _mockLogger = new Mock<ILogger<CustomizationAnalyticsService>>();

            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _service = new CustomizationAnalyticsService(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task GetMostPopularOptionsAsync_WithNoCustomizations_ShouldReturnEmptyDictionary()
        {
            // Act
            var result = await _service.GetMostPopularOptionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetMostPopularOptionsAsync_WithSingleCustomization_ShouldReturnCorrectAggregation()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var order = new Order
            {
                Id = orderId,
                OrderNumber = "ORD-001",
                CustomerId = Guid.NewGuid(),
                StoreId = Guid.NewGuid(),
                Status = Core.Enums.Orders.OrderStatus.Completed,
                TotalAmount = 3950000,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsDeleted = false
            };

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = productId,
                ProductName = "Mâm Cúng",
                ProductCode = "SKU-001",
                UnitPrice = 3500000,
                Quantity = 1,
                TotalPrice = 3950000,
                CustomizationsJson = "[{\"optionId\":\"opt-xoi\",\"quantity\":10,\"unitPrice\":45000,\"totalPrice\":450000}]",
                BasePrice = 3500000,
                CustomizationPrice = 450000,
                IsActive = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetMostPopularOptionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.True(result.ContainsKey("opt-xoi"));
            Assert.Equal(10, result["opt-xoi"]);
        }

        [Fact]
        public async Task GetMostPopularOptionsAsync_WithMultipleCustomizations_ShouldAggregateCorrectly()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            var orderId2 = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var orders = new[]
            {
                new Order
                {
                    Id = orderId1,
                    OrderNumber = "ORD-001",
                    CustomerId = Guid.NewGuid(),
                    StoreId = Guid.NewGuid(),
                    Status = Core.Enums.Orders.OrderStatus.Completed,
                    TotalAmount = 4125000,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                },
                new Order
                {
                    Id = orderId2,
                    OrderNumber = "ORD-002",
                    CustomerId = Guid.NewGuid(),
                    StoreId = Guid.NewGuid(),
                    Status = Core.Enums.Orders.OrderStatus.Completed,
                    TotalAmount = 3950000,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                }
            };

            var orderItems = new[]
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId1,
                    ProductId = productId,
                    ProductName = "Mâm Cúng",
                    ProductCode = "SKU-001",
                    UnitPrice = 3500000,
                    Quantity = 1,
                    TotalPrice = 4125000,
                    CustomizationsJson = "[{\"optionId\":\"opt-xoi\",\"quantity\":10,\"unitPrice\":45000,\"totalPrice\":450000},{\"optionId\":\"opt-che\",\"quantity\":5,\"unitPrice\":35000,\"totalPrice\":175000}]",
                    BasePrice = 3500000,
                    CustomizationPrice = 625000,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId2,
                    ProductId = productId,
                    ProductName = "Mâm Cúng",
                    ProductCode = "SKU-001",
                    UnitPrice = 3500000,
                    Quantity = 1,
                    TotalPrice = 3950000,
                    CustomizationsJson = "[{\"optionId\":\"opt-xoi\",\"quantity\":8,\"unitPrice\":45000,\"totalPrice\":360000}]",
                    BasePrice = 3500000,
                    CustomizationPrice = 450000,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.Orders.AddRange(orders);
            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetMostPopularOptionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey("opt-xoi"));
            Assert.True(result.ContainsKey("opt-che"));
            Assert.Equal(18, result["opt-xoi"]); // 10 + 8
            Assert.Equal(5, result["opt-che"]);  // 5
        }

        [Fact]
        public async Task GetMostPopularOptionsAsync_WithNullCustomizations_ShouldSkipAndContinue()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            var orderId2 = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var orders = new[]
            {
                new Order
                {
                    Id = orderId1,
                    OrderNumber = "ORD-001",
                    CustomerId = Guid.NewGuid(),
                    StoreId = Guid.NewGuid(),
                    Status = Core.Enums.Orders.OrderStatus.Completed,
                    TotalAmount = 3500000,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                },
                new Order
                {
                    Id = orderId2,
                    OrderNumber = "ORD-002",
                    CustomerId = Guid.NewGuid(),
                    StoreId = Guid.NewGuid(),
                    Status = Core.Enums.Orders.OrderStatus.Completed,
                    TotalAmount = 3950000,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                }
            };

            var orderItems = new[]
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId1,
                    ProductId = productId,
                    ProductName = "Mâm Cúng",
                    ProductCode = "SKU-001",
                    UnitPrice = 3500000,
                    Quantity = 1,
                    TotalPrice = 3500000,
                    CustomizationsJson = null,
                    BasePrice = 3500000,
                    CustomizationPrice = 0,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId2,
                    ProductId = productId,
                    ProductName = "Mâm Cúng",
                    ProductCode = "SKU-001",
                    UnitPrice = 3500000,
                    Quantity = 1,
                    TotalPrice = 3950000,
                    CustomizationsJson = "[{\"optionId\":\"opt-xoi\",\"quantity\":10,\"unitPrice\":45000,\"totalPrice\":450000}]",
                    BasePrice = 3500000,
                    CustomizationPrice = 450000,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.Orders.AddRange(orders);
            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetMostPopularOptionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.True(result.ContainsKey("opt-xoi"));
            Assert.Equal(10, result["opt-xoi"]);
        }

        [Fact]
        public async Task GetCustomizationAnalyticsAsync_WithNoCustomizations_ShouldReturnEmptyList()
        {
            // Act
            var result = await _service.GetCustomizationAnalyticsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetCustomizationAnalyticsAsync_WithCustomizations_ShouldCalculatePercentagesCorrectly()
        {
            // Arrange
            var orderId1 = Guid.NewGuid();
            var orderId2 = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var orders = new[]
            {
                new Order
                {
                    Id = orderId1,
                    OrderNumber = "ORD-001",
                    CustomerId = Guid.NewGuid(),
                    StoreId = Guid.NewGuid(),
                    Status = Core.Enums.Orders.OrderStatus.Completed,
                    TotalAmount = 4125000,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                },
                new Order
                {
                    Id = orderId2,
                    OrderNumber = "ORD-002",
                    CustomerId = Guid.NewGuid(),
                    StoreId = Guid.NewGuid(),
                    Status = Core.Enums.Orders.OrderStatus.Completed,
                    TotalAmount = 3950000,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                }
            };

            var orderItems = new[]
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId1,
                    ProductId = productId,
                    ProductName = "Mâm Cúng",
                    ProductCode = "SKU-001",
                    UnitPrice = 3500000,
                    Quantity = 1,
                    TotalPrice = 4125000,
                    CustomizationsJson = "[{\"optionId\":\"opt-xoi\",\"quantity\":10,\"unitPrice\":45000,\"totalPrice\":450000},{\"optionId\":\"opt-che\",\"quantity\":5,\"unitPrice\":35000,\"totalPrice\":175000}]",
                    BasePrice = 3500000,
                    CustomizationPrice = 625000,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                },
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId2,
                    ProductId = productId,
                    ProductName = "Mâm Cúng",
                    ProductCode = "SKU-001",
                    UnitPrice = 3500000,
                    Quantity = 1,
                    TotalPrice = 3950000,
                    CustomizationsJson = "[{\"optionId\":\"opt-xoi\",\"quantity\":8,\"unitPrice\":45000,\"totalPrice\":360000}]",
                    BasePrice = 3500000,
                    CustomizationPrice = 450000,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.Orders.AddRange(orders);
            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetCustomizationAnalyticsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            var xoiAnalytics = result.FirstOrDefault(a => a.OptionId == "opt-xoi");
            Assert.NotNull(xoiAnalytics);
            Assert.Equal(18, xoiAnalytics.TotalQuantity);
            Assert.Equal(100, xoiAnalytics.PercentageOfOrders);

            var cheAnalytics = result.FirstOrDefault(a => a.OptionId == "opt-che");
            Assert.NotNull(cheAnalytics);
            Assert.Equal(5, cheAnalytics.TotalQuantity);
            Assert.Equal(50, cheAnalytics.PercentageOfOrders);
        }
    }
}
