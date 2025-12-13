using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Data.Tests.Base;
using Xunit;

namespace VietCommerce.Data.Tests.Repositories
{
    public class OrderRepository_UpdateOrderStatusTests : BaseTestClass
    {
        [Fact]
        public async Task UpdateOrderStatusAsync_WithValidData_ShouldUpdateStatusAndCreateHistory()
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

            var changedBy = Guid.NewGuid();
            var notes = "Order confirmed";

            // Act
            var result = await _orderRepository.UpdateOrderStatusAsync(
                order.Id,
                OrderStatus.Confirmed,
                changedBy,
                notes);

            // Assert
            Assert.True(result);

            var updatedOrder = await _context.Orders.FindAsync(order.Id);
            Assert.Equal(OrderStatus.Confirmed, updatedOrder.Status);
            Assert.Equal(changedBy, updatedOrder.UpdatedBy);

            var history = await _context.OrderStatusHistories
                .Where(h => h.OrderId == order.Id)
                .ToListAsync();
            Assert.Single(history);
            Assert.Equal(OrderStatus.Confirmed, history.First().NewStatus);
            Assert.Equal(notes, history.First().Notes);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_WithNonExistentOrder_ShouldReturnFalse()
        {
            // Act
            var result = await _orderRepository.UpdateOrderStatusAsync(
                Guid.NewGuid(),
                OrderStatus.Confirmed,
                Guid.NewGuid(),
                "test");

            // Assert
            Assert.False(result);
        }
    }
}