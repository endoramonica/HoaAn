using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Data.Tests.Base;
using Xunit;

namespace VietCommerce.Data.Tests.Repositories.Orders
{
    public class OrderRepository_GetPaginatedTests : BaseTestClass
    {
        [Fact]
        public async Task GetPaginatedAsync_WithDefaultFilter_ShouldReturnFirstPage()
        {
            // Arrange
            var orders = Enumerable.Range(1, 25)
                .Select(i => new Order
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = $"ORD{i:D3}",
                    StoreId = _storeId,
                    CustomerId = _customerId,
                    CreatedById = _userId,
                    Status = OrderStatus.Pending,
                    TotalAmount = 100m * i
                })
                .ToList();

            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();

            var filter = new OrderFilterDTO
            {
                Page = 1,
                PageSize = 20
            };

            // Act
            var result = await _orderRepository.GetPaginatedAsync(filter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(20, result.PageSize);
            Assert.Equal(25, result.TotalItems);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal(20, result.Items.Count());
        }

        [Fact]
        public async Task GetPaginatedAsync_WithStatusFilter_ShouldReturnFilteredOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new() { Id = Guid.NewGuid(), OrderNumber = "ORD001", StoreId = _storeId, CustomerId = _customerId, CreatedById = _userId, Status = OrderStatus.Pending, TotalAmount = 100m },
                new() { Id = Guid.NewGuid(), OrderNumber = "ORD002", StoreId = _storeId, CustomerId = _customerId, CreatedById = _userId, Status = OrderStatus.Confirmed, TotalAmount = 200m }
            };

            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();

            var filter = new OrderFilterDTO
            {
                Page = 1,
                PageSize = 20,
                Status = OrderStatus.Confirmed
            };

            // Act
            var result = await _orderRepository.GetPaginatedAsync(filter);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal(OrderStatus.Confirmed, result.Items.First().Status);
        }

        [Fact]
        public async Task GetPaginatedAsync_WithDateRangeFilter_ShouldReturnOrdersInRange()
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
                Status = OrderStatus.Pending,
                TotalAmount = 100m,
                CreatedAt = now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var filter = new OrderFilterDTO
            {
                Page = 1,
                PageSize = 20,
                FromDate = yesterday,
                ToDate = tomorrow
            };

            // Act
            var result = await _orderRepository.GetPaginatedAsync(filter);

            // Assert
            Assert.Single(result.Items);
        }

        [Fact]
        public async Task GetPaginatedAsync_WithAmountFilter_ShouldReturnFilteredOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new() { Id = Guid.NewGuid(), OrderNumber = "ORD001", StoreId = _storeId, CustomerId = _customerId, CreatedById = _userId, Status = OrderStatus.Pending, TotalAmount = 50m },
                new() { Id = Guid.NewGuid(), OrderNumber = "ORD002", StoreId = _storeId, CustomerId = _customerId, CreatedById = _userId, Status = OrderStatus.Pending, TotalAmount = 150m },
                new() { Id = Guid.NewGuid(), OrderNumber = "ORD003", StoreId = _storeId, CustomerId = _customerId, CreatedById = _userId, Status = OrderStatus.Pending, TotalAmount = 250m }
            };

            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();

            var filter = new OrderFilterDTO
            {
                Page = 1,
                PageSize = 20,
                MinAmount = 100m,
                MaxAmount = 200m
            };

            // Act
            var result = await _orderRepository.GetPaginatedAsync(filter);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal(150m, result.Items.First().TotalAmount);
        }

        [Fact]
        public async Task GetPaginatedAsync_WithRestrictToCustomerId_ShouldOnlyReturnCustomerOrders()
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

            var otherCustomerId = Guid.NewGuid();
            var order2 = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD002",
                StoreId = _storeId,
                CustomerId = otherCustomerId,
                CreatedById = _userId,
                Status = OrderStatus.Pending,
                TotalAmount = 200m
            };

            _context.Orders.AddRange(order1, order2);
            await _context.SaveChangesAsync();

            var filter = new OrderFilterDTO { Page = 1, PageSize = 20 };

            // Act
            var result = await _orderRepository.GetPaginatedAsync(filter, restrictToCustomerId: _customerId);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal(_customerId, result.Items.First().CustomerId);
        }
    }
}