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
    public class OrderRepository_GetRecentOrdersTests : BaseTestClass
    {
        [Fact]
        public async Task GetRecentOrdersAsync_ShouldReturnLastNOrders()
        {
            // Arrange
            var orders = Enumerable.Range(1, 15)
                .Select(i => new Order
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = $"ORD{i:D3}",
                    StoreId = _storeId,
                    CustomerId = _customerId,
                    CreatedById = _userId,
                    Status = OrderStatus.Pending,
                    TotalAmount = 100m * i,
                    CreatedAt = DateTime.UtcNow.AddDays(-i)
                })
                .ToList();

            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderRepository.GetRecentOrdersAsync(count: 10);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(10, result.Count());
        }
    }
}