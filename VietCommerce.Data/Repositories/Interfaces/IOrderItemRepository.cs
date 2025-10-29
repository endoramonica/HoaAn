using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Models;

namespace VietCommerce.Data.Repositories.Interfaces;

public interface IOrderItemRepository : IGenericRepository<OrderItem>
{
    // Custom query methods
    Task<IEnumerable<OrderItem>> GetByOrderIdAsync(Guid orderId);
    Task<IEnumerable<OrderItem>> GetByProductIdAsync(Guid productId);
}