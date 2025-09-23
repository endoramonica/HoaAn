using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using VietCommerce.Core.Common.Exceptions;
using VietCommerce.Core.DTOs.Orders;
using VietCommerce.Core.Enums;
using VietCommerce.Core.Models;
using VietCommerce.Core.Services.Notifications;
using VietCommerce.Core.Services.Orders;
using VietCommerce.Data.Repositories.Orders;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Enums.Orders;

namespace VietCommerce.Core.Services.Orders;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusHistoryRepository _historyRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly IValidator<OrderCreateDTO> _createValidator;
    private readonly IValidator<OrderUpdateDTO> _updateValidator;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository orderRepository, IOrderStatusHistoryRepository historyRepository, INotificationService notificationService, IMapper mapper, IValidator<OrderCreateDTO> createValidator, IValidator<OrderUpdateDTO> updateValidator, ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _historyRepository = historyRepository;
        _notificationService = notificationService;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<PaginatedResult<OrderListDTO>> GetPaginatedAsync(int pageNumber = 1, int pageSize = 10)
    {
        var orders = await _orderRepository.GetPaginatedAsync(pageNumber, pageSize);
        var total = await _orderRepository.GetTotalCountAsync();
        var dtos = _mapper.Map<IEnumerable<OrderListDTO>>(orders);
        return new PaginatedResult<OrderListDTO>(dtos, pageNumber, pageSize, total);
    }

    public async Task<OrderListDTO> GetByIdAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            throw new BusinessException("Order not found");
        }
        return _mapper.Map<OrderListDTO>(order);
    }

    public async Task<Guid> CreateAsync(OrderCreateDTO dto)
    {
        await _createValidator.ValidateAndThrowAsync(dto);

        var order = _mapper.Map<Order>(dto);
        order.Id = Guid.NewGuid();
        order.Status = OrderStatus.Pending;
        order.CreatedDate = DateTime.UtcNow;

        order.OrderItems = _mapper.Map<List<OrderItem>>(dto.Items);
        if (dto.Shipping != null)
        {
            var shipping = _mapper.Map<OrderShipping>(dto.Shipping);
            shipping.Id = Guid.NewGuid();
            shipping.OrderId = order.Id;
            order.OrderShipping = shipping;
        }

        await _orderRepository.CreateAsync(order);
        _logger.LogInformation("Order created: {Id}", order.Id);
        return order.Id;
    }

    public async Task UpdateAsync(Guid id, OrderUpdateDTO dto)
    {
        await _updateValidator.ValidateAndThrowAsync(dto);
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            throw new BusinessException("Order not found");
        }

        _mapper.Map(dto, order);
        order.UpdatedDate = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order);
        _logger.LogInformation("Order updated: {Id}", id);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _orderRepository.DeleteAsync(id);
        _logger.LogInformation("Order deleted: {Id}", id);
    }

    public async Task UpdateOrderStatusAsync(Guid id, OrderStatus status, string notes)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            throw new BusinessException("Order not found");
        }

        order.Status = status;
        order.UpdatedDate = DateTime.UtcNow;

        var history = new OrderStatusHistory
        {
            Id = Guid.NewGuid(),
            OrderId = id,
            Status = status,
            Notes = notes,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = Guid.Empty // Set current user ID
        };

        await _historyRepository.CreateAsync(history);
        await _orderRepository.UpdateAsync(order);

        await _notificationService.SendOrderStatusNotificationAsync(order.CustomerId, status, notes);

        _logger.LogInformation("Order status updated: {Id} to {Status}", id, status);
    }
}