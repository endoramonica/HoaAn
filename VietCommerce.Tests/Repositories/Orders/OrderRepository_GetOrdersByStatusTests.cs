using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Data.Tests.Base;
using Xunit;

namespace VietCommerce.Data.Tests.Repositories.Orders
{
    public class OrderRepository_GetOrdersByStatusTests : BaseTestClass
    {
        [Fact]
        public async Task GetOrdersByStatusAsync_WithValidStatus_ShouldReturnOrdersAsList()
        {
            // Arrange
            var order1 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD001",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Pending,
                TotalAmount = 100m
            };

            var order2 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD002",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Pending,
                TotalAmount = 200m
            };

            var order3 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD003",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Confirmed,
                TotalAmount = 300m
            };

            _context.Orders.AddRange(order1, order2, order3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetOrdersByStatusAsync(OrderStatus.Pending);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Order>>(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, order => Assert.Equal(OrderStatus.Pending, order.Status));
        }

        [Fact]
        public async Task GetOrdersByStatusAsync_ShouldReturnOrdersOrderedByCreatedAtDescending()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var order1 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD001",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Shipped,
                TotalAmount = 100m,
                CreatedAt = now.AddDays(-2)
            };

            var order2 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD002",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Shipped,
                TotalAmount = 200m,
                CreatedAt = now
            };

            var order3 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD003",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Shipped,
                TotalAmount = 300m,
                CreatedAt = now.AddDays(-1)
            };

            _context.Orders.AddRange(order1, order2, order3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetOrdersByStatusAsync(OrderStatus.Shipped);

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(order2.Id, result[0].Id);
            Assert.Equal(order3.Id, result[1].Id);
            Assert.Equal(order1.Id, result[2].Id);
        }

        [Fact]
        public async Task GetOrdersByStatusAsync_ShouldIncludeRelatedEntities()
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

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = _productId,
                Quantity = 2,
                UnitPrice = 50m
            };

            var orderShipping = new OrderShipping
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Address = "Test Address",
                City = "Test City"
            };

            _context.Orders.Add(order);
            _context.OrderItems.Add(orderItem);
            _context.OrderShippings.Add(orderShipping);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetOrdersByStatusAsync(OrderStatus.Completed);

            // Assert
            var returnedOrder = result.First();
            Assert.NotNull(returnedOrder.Customer);
            Assert.NotNull(returnedOrder.Store);
            Assert.NotNull(returnedOrder.OrderItems);
            Assert.NotEmpty(returnedOrder.OrderItems);
            Assert.NotNull(returnedOrder.OrderShipping);
        }

        [Fact]
        public async Task GetOrdersByStatusAsync_WithDeletedOrder_ShouldNotIncludeInResults()
        {
            // Arrange
            var activeOrder = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD001",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Pending,
                TotalAmount = 100m,
                IsDeleted = false
            };

            var deletedOrder = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD002",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Pending,
                TotalAmount = 200m,
                IsDeleted = true
            };

            _context.Orders.AddRange(activeOrder, deletedOrder);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetOrdersByStatusAsync(OrderStatus.Pending);

            // Assert
            Assert.Single(result);
            Assert.Equal(activeOrder.Id, result.First().Id);
            Assert.DoesNotContain(result, o => o.Id == deletedOrder.Id);
        }

        [Fact]
        public async Task GetOrdersByStatusAsync_WithNoOrdersOfStatus_ShouldReturnEmptyList()
        {
            // Arrange
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD001",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Pending,
                TotalAmount = 100m
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetOrdersByStatusAsync(OrderStatus.Completed);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(OrderStatus.Pending)]
        [InlineData(OrderStatus.Confirmed)]
        [InlineData(OrderStatus.Shipped)]
        [InlineData(OrderStatus.Completed)]
        [InlineData(OrderStatus.Cancelled)]
        public async Task GetOrdersByStatusAsync_WithEachStatus_ShouldReturnCorrectOrders(OrderStatus status)
        {
            // Arrange
            var orders = new List<Order>
            {
                new() { Id = Guid.NewGuid(), OrderNumber = "ORD001", StoreId = _storeId, CustomerId = _customerId, CreatedById = _userId, Status = OrderStatus.Pending, TotalAmount = 100m },
                new() { Id = Guid.NewGuid(), OrderNumber = "ORD002", StoreId = _storeId, CustomerId = _customerId, CreatedById = _userId, Status = OrderStatus.Confirmed, TotalAmount = 200m },
                new() { Id = Guid.NewGuid(), OrderNumber = "ORD003", StoreId = _storeId, CustomerId = _customerId, CreatedById = _userId, Status = OrderStatus.Shipped, TotalAmount = 300m },
                new() { Id = Guid.NewGuid(), OrderNumber = "ORD004", StoreId = _storeId, CustomerId = _customerId, CreatedById = _userId, Status = OrderStatus.Completed, TotalAmount = 400m },
                new() { Id = Guid.NewGuid(), OrderNumber = "ORD005", StoreId = _storeId, CustomerId = _customerId, CreatedById = _userId, Status = OrderStatus.Cancelled, TotalAmount = 500m }
            };

            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetOrdersByStatusAsync(status);

            // Assert
            Assert.Single(result);
            Assert.Equal(status, result.First().Status);
        }

        [Fact]
        public async Task GetOrdersByStatusAsync_ShouldUseAsNoTracking_ForReadOnlyPerformance()
        {
            // Arrange
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD001",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Pending,
                TotalAmount = 100m
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetOrdersByStatusAsync(OrderStatus.Pending);
            var returnedOrder = result.First();

            // Assert
            var entry = _context.Entry(returnedOrder);
            Assert.Equal(EntityState.Detached, entry.State);
        }

        [Fact]
        public async Task GetOrdersByStatusAsync_WithMultipleOrders_ShouldReturnAllOrdersAsList()
        {
            // Arrange
            var orderCount = 10;
            var orders = Enumerable.Range(1, orderCount)
                .Select(i => new Order
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = $"ORD{i:D3}",
                    StoreId = _storeId,
                    CustomerId = _customerId,
                    CreatedById = _userId,
                    Status = OrderStatus.Confirmed,
                    TotalAmount = 100m * i
                })
                .ToList();

            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetOrdersByStatusAsync(OrderStatus.Confirmed);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Order>>(result);
            Assert.Equal(orderCount, result.Count);
            Assert.All(result, order => Assert.Equal(OrderStatus.Confirmed, order.Status));
        }
    }
}