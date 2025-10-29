using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Data.Tests.Base;
using Xunit;

namespace VietCommerce.Data.Tests.Repositories
{
    public class OrderRepository_CustomerHasOrdersTests : BaseTestClass
    {
        [Fact]
        public async Task CustomerHasOrdersAsync_WithExistingOrders_ShouldReturnTrue()
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
            var hasOrders = await _orderRepository.CustomerHasOrdersAsync(_customerId);

            // Assert
            Assert.True(hasOrders);
        }

        [Fact]
        public async Task CustomerHasOrdersAsync_WithNoOrders_ShouldReturnFalse()
        {
            // Act
            var hasOrders = await _orderRepository.CustomerHasOrdersAsync(Guid.NewGuid());

            // Assert
            Assert.False(hasOrders);
        }
    }
}