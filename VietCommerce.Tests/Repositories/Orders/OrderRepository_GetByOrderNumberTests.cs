using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Data.Tests.Base;
using Xunit;

namespace VietCommerce.Tests.Repositories.Orders

{
    public class OrderRepository_GetByOrderNumberTests : BaseTestClass
    {
        [Fact]
        public async Task GetByOrderNumberAsync_WithValidOrderNumber_ShouldReturnOrder()
        {
            // Arrange
            var orderNumber = "ORD20250125-ABC123";
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = orderNumber,
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Pending,
                TotalAmount = 100m
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetByOrderNumberAsync(orderNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderNumber, result.OrderNumber);
            Assert.Equal(order.Id, result.Id);
            Assert.Equal(OrderStatus.Pending, result.Status);
        }

        [Fact]
        public async Task GetByOrderNumberAsync_WithNonExistentOrderNumber_ShouldReturnNull()
        {
            // Act
            var result = await _orderRepository.GetByOrderNumberAsync("ORD99999999-NONEXISTENT");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByOrderNumberAsync_WithDeletedOrder_ShouldReturnNull()
        {
            // Arrange
            var orderNumber = "ORD20250125-DELETED";
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = orderNumber,
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Pending,
                TotalAmount = 100m,
                IsDeleted = true
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetByOrderNumberAsync(orderNumber);

            // Assert
            Assert.Null(result);
        }
    }
}