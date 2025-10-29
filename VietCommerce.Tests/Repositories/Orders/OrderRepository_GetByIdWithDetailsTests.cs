using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Data.Tests.Base;
using Xunit;

namespace VietCommerce.Tests.Repositories.Orders 
{
    public class OrderRepository_GetByIdWithDetailsTests : BaseTestClass
    {
        [Fact]
        public async Task GetByIdWithDetailsAsync_WithValidId_ShouldReturnOrderWithDetails()
        {
            // Arrange
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = "ORD20250125-DETAIL",
                StoreId = _storeId,
                CustomerId = _customerId,
                CreatedById = _userId,
                Status = OrderStatus.Pending,
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
            var result = await _orderRepository.GetByIdWithDetailsAsync(order.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
            Assert.NotNull(result.OrderItems);
            Assert.Single(result.OrderItems);
            Assert.NotNull(result.OrderShipping);
        }

        [Fact]
        public async Task GetByIdWithDetailsAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Act
            var result = await _orderRepository.GetByIdWithDetailsAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }
    }
}