using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Enums;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Models;

namespace VietCommerce.Core.Services.Orders;

public interface IOrderService
{
    Task<PaginatedResult<OrderListDTO>> GetPaginatedAsync(int pageNumber = 1, int pageSize = 10);
    Task<OrderListDTO> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(OrderCreateDTO dto);
    Task UpdateAsync(Guid id, OrderUpdateDTO dto);
    Task DeleteAsync(Guid id);
    Task UpdateOrderStatusAsync(Guid id, OrderStatus status, string notes);
}