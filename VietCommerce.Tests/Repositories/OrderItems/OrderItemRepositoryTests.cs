using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Core.Enums;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Enums.Products;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories;
using VietCommerce.Data.Tests.Base;
using Xunit;

namespace VietCommerce.Data.Tests.Repositories
{
    public class OrderItemRepositoryTests : BaseTestClass
    {
        private readonly AppDbContext _context;
        private readonly OrderItemRepository _orderItemRepository;

        // Test data IDs
        private readonly Guid _storeId = Guid.NewGuid();
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _productId1 = Guid.NewGuid();
        private readonly Guid _productId2 = Guid.NewGuid();
        private readonly Guid _orderId = Guid.NewGuid();

        public OrderItemRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new AppDbContext(options);
            _orderItemRepository = new OrderItemRepository(_context);
        }

        public async Task InitializeAsync()
        {
            await SeedTestDataAsync();
        }

        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        #region Test Data Seeding

        private async Task SeedTestDataAsync()
        {
            // Create test store
            var store = new Store
            {
                Id = _storeId,
                Name = "Test Store",
                Phone = "0123456789",
                Address = "123 Test St",
                IsActive = true
            };

            // Create test customer
            var customer = new Customer
            {
                Id = _customerId,
                Name = "Test Customer",
                Email = "customer@test.com",
                Phone = "0987654321",
                StoreId = _storeId,
                IsActive = true
            };

            // Create test user
            var user = new User
            {
                Id = _userId,
                Email = "user@test.com",
                Name = "Test User",
                PasswordHash = "hash",
                IsActive = true
            };

            // Create test products
            var product1 = new Product
            {
                Id = _productId1,
                Name = "Test Product 1",
                Code = "PROD-001",
                Slug = "test-product-1",
                SKU = "TEST-SKU-001",
                StoreId = _storeId,
                CategoryId = Guid.NewGuid(),
                Stock = 100,
                IsActive = true,
                ViewCount = 0,
                FavoriteCount = 0,
                PurchaseCount = 0,
                ReviewCount = 0,
                AvgRating = 0
            };

            var product2 = new Product
            {
                Id = _productId2,
                Name = "Test Product 2",
                Code = "PROD-002",
                Slug = "test-product-2",
                SKU = "TEST-SKU-002",
                StoreId = _storeId,
                CategoryId = Guid.NewGuid(),
                Stock = 50,
                IsActive = true,
                ViewCount = 0,
                FavoriteCount = 0,
                PurchaseCount = 0,
                ReviewCount = 0,
                AvgRating = 0
            };

            // Create product prices
            var price1 = new ProductPrice
            {
                Id = Guid.NewGuid(),
                ProductId = _productId1,
                PriceType = PriceType.REGULAR,
                Price = 100m,
                EffectiveFrom = DateTime.UtcNow,
                EffectiveTo = null,
                IsActive = true,
                CreatedBy = _userId,
                UpdatedBy = _userId
            };

            var price2 = new ProductPrice
            {
                Id = Guid.NewGuid(),
                ProductId = _productId2,
                PriceType = PriceType.REGULAR,
                Price = 200m,
                EffectiveFrom = DateTime.UtcNow,
                EffectiveTo = null,
                IsActive = true,
                CreatedBy = _userId,
                UpdatedBy = _userId
            };

            _context.Stores.Add(store);
            _context.Customers.Add(customer);
            _context.Users.Add(user);
            _context.Products.AddRange(product1, product2);
            _context.ProductPrices.AddRange(price1, price2);

            await _context.SaveChangesAsync();
        }

        #endregion

        #region GetByOrderIdAsync Tests

        [Fact]
        public async Task GetByOrderIdAsync_WithValidOrderId_ShouldReturnAllOrderItems()
        {
            // Arrange
            var order = new Order
            {
                Id = _orderId,
                OrderNumber = "ORD20250125-001",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Pending,
                TotalAmount = 500m
            };

            var item1 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = _orderId,
                ProductId = _productId1,
                Quantity = 2,
                UnitPrice = 100m
            };

            var item2 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = _orderId,
                ProductId = _productId2,
                Quantity = 1,
                UnitPrice = 200m
            };

            _context.Orders.Add(order);
            _context.OrderItems.AddRange(item1, item2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderItemRepository.GetByOrderIdAsync(_orderId);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(_orderId, item.OrderId));
        }

        [Fact]
        public async Task GetByOrderIdAsync_WithNonExistentOrderId_ShouldReturnEmpty()
        {
            // Act
            var result = await _orderItemRepository.GetByOrderIdAsync(Guid.NewGuid());

            // Assert
            Assert.Empty(result);
        }

        
        [Fact]
        public async Task GetByOrderIdAsync_ShouldIncludeProductDetails()
        {
            // Arrange
            await SeedTestDataAsync();

            var order = new Order
            {
                Id = _orderId,
                OrderNumber = "ORD20250125-002",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Pending,
                TotalAmount = 200m
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var item = new OrderItem
            {
                Id = Guid.NewGuid(),
                Order = order, // 👈 EF tự gán OrderId
                ProductId = _productId1,
                Quantity = 2,
                UnitPrice = 100m
            };
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderItemRepository.GetByOrderIdAsync(order.Id);
            Assert.True(result.Any(), $"No OrderItems found for order {_orderId}");

            // Assert
            Assert.NotEmpty(result);
            var orderItem = result.First();
            Assert.NotNull(orderItem.Product);
            Assert.Equal(_productId1, orderItem.Product.Id);
        }


        #endregion

        #region GetByProductIdAsync Tests

        [Fact]
        public async Task GetByProductIdAsync_ShouldReturnAllItemsForProduct()
        {
            // Arrange
            var order1 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD001",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Completed,
                TotalAmount = 100m
            };

            var order2 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD002",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Completed,
                TotalAmount = 200m
            };

            var item1 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order1.Id,
                ProductId = _productId1,
                Quantity = 2,
                UnitPrice = 50m
            };

            var item2 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order2.Id,
                ProductId = _productId1,
                Quantity = 3,
                UnitPrice = 50m
            };

            _context.Orders.AddRange(order1, order2);
            _context.OrderItems.AddRange(item1, item2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderItemRepository.GetByProductIdAsync(_productId1);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(_productId1, item.ProductId));
        }

        [Fact]
        public async Task GetByProductIdAsync_WithNonExistentProductId_ShouldReturnEmpty()
        {
            // Act
            var result = await _orderItemRepository.GetByProductIdAsync(Guid.NewGuid());

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetTotalQuantitySoldAsync Tests

        [Fact]
        public async Task GetTotalQuantitySoldAsync_ShouldOnlyCountCompletedAndShippedOrders()
        {
            // Arrange
            var completedOrder = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD001",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Completed,
                TotalAmount = 100m
            };

            var shippedOrder = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD002",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Shipped,
                TotalAmount = 100m
            };

            var pendingOrder = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD003",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Pending,
                TotalAmount = 100m
            };

            var item1 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = completedOrder.Id,
                ProductId = _productId1,
                Quantity = 5,
                UnitPrice = 20m
            };

            var item2 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = shippedOrder.Id,
                ProductId = _productId1,
                Quantity = 3,
                UnitPrice = 20m
            };

            var item3 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = pendingOrder.Id,
                ProductId = _productId1,
                Quantity = 10,
                UnitPrice = 20m
            };

            _context.Orders.AddRange(completedOrder, shippedOrder, pendingOrder);
            _context.OrderItems.AddRange(item1, item2, item3);
            await _context.SaveChangesAsync();

            // Act
            var totalQuantity = await _orderItemRepository.GetTotalQuantitySoldAsync(_productId1);

            // Assert
            Assert.Equal(8, totalQuantity); // 5 + 3, excluding pending order
        }

        [Fact]
        public async Task GetTotalQuantitySoldAsync_WithDateRangeFilter_ShouldReturnFilteredQuantity()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var yesterday = now.AddDays(-1);
            var tomorrow = now.AddDays(1);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD001",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Completed,
                TotalAmount = 100m,
                CreatedAt = now
            };

            var item = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = _productId1,
                Quantity = 5,
                UnitPrice = 20m,
                CreatedAt = now
            };

            _context.Orders.Add(order);
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();

            // Act
            var totalQuantity = await _orderItemRepository.GetTotalQuantitySoldAsync(
                _productId1,
                fromDate: yesterday,
                toDate: tomorrow);

            // Assert
            Assert.Equal(5, totalQuantity);
        }

        [Fact]
        public async Task GetTotalQuantitySoldAsync_WithNoSales_ShouldReturnZero()
        {
            // Act
            var totalQuantity = await _orderItemRepository.GetTotalQuantitySoldAsync(Guid.NewGuid());

            // Assert
            Assert.Equal(0, totalQuantity);
        }

        #endregion

        #region GetTopSellingProductsAsync Tests

        [Fact]
        public async Task GetTopSellingProductsAsync_ShouldReturnProductsByTotalQuantity()
        {
            // Arrange
            var order1 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD001",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Completed,
                TotalAmount = 100m
            };

            var order2 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD002",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Completed,
                TotalAmount = 200m
            };

            // Product 1: 10 units total
            var item1 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order1.Id,
                ProductId = _productId1,
                Quantity = 7,
                UnitPrice = 100m
            };

            var item2 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order2.Id,
                ProductId = _productId1,
                Quantity = 3,
                UnitPrice = 100m
            };

            // Product 2: 5 units total
            var item3 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order1.Id,
                ProductId = _productId2,
                Quantity = 5,
                UnitPrice = 200m
            };

            _context.Orders.AddRange(order1, order2);
            _context.OrderItems.AddRange(item1, item2, item3);
            await _context.SaveChangesAsync();

            // Act
            var topProducts = await _orderItemRepository.GetTopSellingProductsAsync(top: 10);

            // Assert
            Assert.NotEmpty(topProducts);
            Assert.Equal(2, topProducts.Count);

            var sorted = topProducts.OrderByDescending(x => x.Value).ToList();
            Assert.Equal(_productId1, sorted[0].Key); // 10 units
            Assert.Equal(10, sorted[0].Value);
            Assert.Equal(_productId2, sorted[1].Key); // 5 units
            Assert.Equal(5, sorted[1].Value);
        }

        [Fact]
        public async Task GetTopSellingProductsAsync_WithTopLimit_ShouldReturnOnlyTopN()
        {
            // Arrange
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD001",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Completed,
                TotalAmount = 100m
            };

            var productIds = Enumerable.Range(1, 15)
                .Select(_ => Guid.NewGuid())
                .ToList();

            var items = productIds
                .Select((productId, index) => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = productId,
                    Quantity = 15 - index, // Descending quantities
                    UnitPrice = 100m
                })
                .ToList();

            _context.Orders.Add(order);
            _context.OrderItems.AddRange(items);
            await _context.SaveChangesAsync();

            // Act
            var topProducts = await _orderItemRepository.GetTopSellingProductsAsync(top: 5);

            // Assert
            Assert.Equal(5, topProducts.Count);
        }

        [Fact]
        public async Task GetTopSellingProductsAsync_WithStoreFilter_ShouldReturnOnlyStoreProducts()
        {
            // Arrange
            var otherStoreId = Guid.NewGuid();

            // Create other store product
            var otherProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Other Store Product",
                Code = "PROD-003",
                Slug = "other-store-product",
                SKU = "OTHER-SKU-001",
                StoreId = otherStoreId,
                CategoryId = Guid.NewGuid(),
                Stock = 100,
                IsActive = true,
                ViewCount = 0,
                FavoriteCount = 0,
                PurchaseCount = 0,
                ReviewCount = 0,
                AvgRating = 0
            };

            var otherProductPrice = new ProductPrice
            {
                Id = Guid.NewGuid(),
                ProductId = otherProduct.Id,
                PriceType = PriceType.REGULAR,
                Price = 100m,
                EffectiveFrom = DateTime.UtcNow,
                EffectiveTo = null,
                IsActive = true,
                CreatedBy = _userId,
                UpdatedBy = _userId
            };

            var order1 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD001",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Completed,
                TotalAmount = 100m
            };

            var order2 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD002",
                StoreId = otherStoreId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Completed,
                TotalAmount = 100m
            };

            var item1 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order1.Id,
                ProductId = _productId1,
                Quantity = 10,
                UnitPrice = 100m
            };

            var item2 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order2.Id,
                ProductId = otherProduct.Id,
                Quantity = 20, // More quantity, but different store
                UnitPrice = 100m
            };

            _context.Products.Add(otherProduct);
            _context.ProductPrices.Add(otherProductPrice);
            _context.Orders.AddRange(order1, order2);
            _context.OrderItems.AddRange(item1, item2);
            await _context.SaveChangesAsync();

            // Act
            var topProducts = await _orderItemRepository.GetTopSellingProductsAsync(
                top: 10,
                storeId: _storeId);

            // Assert
            Assert.Single(topProducts);
            Assert.Equal(_productId1, topProducts.First().Key);
            Assert.Equal(10, topProducts.First().Value);
        }

        [Fact]
        public async Task GetTopSellingProductsAsync_WithDateFilter_ShouldReturnFilteredResults()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var yesterday = now.AddDays(-1);
            var tomorrow = now.AddDays(1);
            var weekAgo = now.AddDays(-7);

            var recentOrder = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD001",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Completed,
                TotalAmount = 100m,
                CreatedAt = now
            };

            var oldOrder = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD002",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Completed,
                TotalAmount = 100m,
                CreatedAt = weekAgo
            };

            var recentItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = recentOrder.Id,
                ProductId = _productId1,
                Quantity = 5,
                UnitPrice = 100m,
                CreatedAt = now
            };

            var oldItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = oldOrder.Id,
                ProductId = _productId1,
                Quantity = 20,
                UnitPrice = 100m,
                CreatedAt = weekAgo
            };

            _context.Orders.AddRange(recentOrder, oldOrder);
            _context.OrderItems.AddRange(recentItem, oldItem);
            await _context.SaveChangesAsync();

            // Act - Only get recent (last 2 days)
            var topProducts = await _orderItemRepository.GetTopSellingProductsAsync(
                top: 10,
                fromDate: yesterday);

            // Assert
            Assert.Single(topProducts);
            Assert.Equal(_productId1, topProducts.First().Key);
            Assert.Equal(5, topProducts.First().Value); // Only recent quantity
        }

        [Fact]
        public async Task GetTopSellingProductsAsync_WithNoSales_ShouldReturnEmpty()
        {
            // Act
            var topProducts = await _orderItemRepository.GetTopSellingProductsAsync(top: 10);

            // Assert
            Assert.Empty(topProducts);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task MultipleItemsFromDifferentOrders_ShouldBeAccessibleSeparately()
        {
            // Arrange
            var order1 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD001",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Completed,
                TotalAmount = 100m
            };

            var order2 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD002",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Completed,
                TotalAmount = 200m
            };

            var items = new List<OrderItem>
            {
                new() { Id = Guid.NewGuid(), OrderId = order1.Id, ProductId = _productId1, Quantity = 2, UnitPrice = 50m },
                new() { Id = Guid.NewGuid(), OrderId = order1.Id, ProductId = _productId2, Quantity = 1, UnitPrice = 100m },
                new() { Id = Guid.NewGuid(), OrderId = order2.Id, ProductId = _productId1, Quantity = 3, UnitPrice = 50m },
                new() { Id = Guid.NewGuid(), OrderId = order2.Id, ProductId = _productId2, Quantity = 2, UnitPrice = 100m }
            };

            _context.Orders.AddRange(order1, order2);
            _context.OrderItems.AddRange(items);
            await _context.SaveChangesAsync();

            // Act & Assert
            var order1Items = await _orderItemRepository.GetByOrderIdAsync(order1.Id);
            Assert.Equal(2, order1Items.Count());

            var order2Items = await _orderItemRepository.GetByOrderIdAsync(order2.Id);
            Assert.Equal(2, order2Items.Count());

            var product1Sales = await _orderItemRepository.GetTotalQuantitySoldAsync(_productId1);
            Assert.Equal(5, product1Sales); // 2 + 3

            var product2Sales = await _orderItemRepository.GetTotalQuantitySoldAsync(_productId2);
            Assert.Equal(3, product2Sales); // 1 + 2
        }

        #endregion
    }
}