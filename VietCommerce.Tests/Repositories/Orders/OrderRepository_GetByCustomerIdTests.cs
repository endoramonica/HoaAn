using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Data.Tests.Base;
using Xunit;

namespace VietCommerce.Data.Tests.Repositories.Orders
{
    public class OrderRepository_GetByCustomerIdTests : BaseTestClass
    {
        [Fact]
        public async Task GetByCustomerIdAsync_ShouldReturnOnlyCustomerOrders()
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
                CustomerId = Guid.NewGuid(),
                CreatedById = _userId,
                Status = OrderStatus.Pending,
                TotalAmount = 200m
            };

            _context.Orders.AddRange(order1, order2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetByCustomerIdAsync(_customerId);

            // Assert
            Assert.NotEmpty(result);
            Assert.Single(result);
            Assert.Equal(order1.Id, result.First().Id);
        }

        [Fact]
        public async Task GetByCustomerIdAsync_WithNoOrders_ShouldReturnEmpty()
        {
            // Act
            var result = await _orderRepository.GetByCustomerIdAsync(Guid.NewGuid());

            // Assert
            Assert.Empty(result);
        }
    }
}