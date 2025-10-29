using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Data.Tests.Base;
using Xunit;

namespace VietCommerce.Data.Tests.Repositories.Orders
{
    public class OrderRepository_ExistsByOrderNumberTests : BaseTestClass
    {
        [Fact]
        public async Task ExistsByOrderNumberAsync_WithExistingOrderNumber_ShouldReturnTrue()
        {
            // Arrange
            var orderNumber = "ORD20250125-EXISTS";
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
            var exists = await _orderRepository.ExistsByOrderNumberAsync(orderNumber);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsByOrderNumberAsync_WithNonExistentOrderNumber_ShouldReturnFalse()
        {
            // Act
            var exists = await _orderRepository.ExistsByOrderNumberAsync("ORD99999999-FAKE");

            // Assert
            Assert.False(exists);
        }
    }
}