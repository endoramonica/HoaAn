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
    public class OrderRepository_GetTotalOrdersCountTests : BaseTestClass
    {
        [Fact]
        public async Task GetTotalOrdersCountAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            var orders = Enumerable.Range(1, 5)
                .Select(i => new Order
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = $"ORD{i:D3}",
                    StoreId = _storeId,
                    CustomerId = _customerId,
                    CreatedById = _userId,
                    Status = OrderStatus.Pending,
                    TotalAmount = 100m
                })
                .ToList();

            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();

            // Act
            var count = await _orderRepository.GetTotalOrdersCountAsync(storeId: _storeId);

            // Assert
            Assert.Equal(5, count);
        }
    }
}