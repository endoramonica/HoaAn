using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Data.Tests.Base;
using Xunit;

namespace VietCommerce.Data.Tests.Repositories.Orders
{
    public class OrderRepository_GetTotalRevenueTests : BaseTestClass
    {
        [Fact]
        public async Task GetTotalRevenueAsync_ShouldOnlyCountCompletedAndShippedOrders()
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
                TotalAmount = 200m
            };

            var pendingOrder = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD003",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Pending,
                TotalAmount = 150m
            };

            _context.Orders.AddRange(completedOrder, shippedOrder, pendingOrder);
            await _context.SaveChangesAsync();

            // Act
            var revenue = await _orderRepository.GetTotalRevenueAsync(storeId: _storeId);

            // Assert
            Assert.Equal(300m, revenue);
        }
    }
}