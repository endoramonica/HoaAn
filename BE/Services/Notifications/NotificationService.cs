using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using VietCommerce.Core.Common.Exceptions;
using VietCommerce.Core.DTOs.Notifications;
using VietCommerce.Core.Models;
using VietCommerce.Core.Entities.Notifications;
using VietCommerce.Core.Enums.Orders;
using VietCommerce.Core.Enums.Notifications;

namespace VietCommerce.Core.Services.Notifications;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repository;
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<NotificationCreateDTO> _createValidator;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(INotificationRepository repository, INotificationTemplateRepository templateRepository, IMapper mapper, IValidator<NotificationCreateDTO> createValidator, ILogger<NotificationService> logger)
    {
        _repository = repository;
        _templateRepository = templateRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _logger = logger;
    }

    public async Task<PaginatedResult<NotificationListDTO>> GetPaginatedAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
    {
        var notifications = await _repository.GetPaginatedByUserAsync(userId, pageNumber, pageSize);
        var total = await _repository.GetTotalCountByUserAsync(userId);
        var dtos = _mapper.Map<IEnumerable<NotificationListDTO>>(notifications);
        return new PaginatedResult<NotificationListDTO>(dtos, pageNumber, pageSize, total);
    }

    public async Task<NotificationListDTO> GetByIdAsync(Guid id)
    {
        var notification = await _repository.GetByIdAsync(id);
        if (notification == null)
        {
            throw new BusinessException("Notification not found");
        }
        return _mapper.Map<NotificationListDTO>(notification);
    }

    public async Task<Guid> CreateAsync(NotificationCreateDTO dto)
    {
        await _createValidator.ValidateAndThrowAsync(dto);

        var notification = _mapper.Map<Notification>(dto);
        notification.Id = Guid.NewGuid();
        notification.CreatedAt = DateTime.UtcNow;

        await _repository.CreateAsync(notification);
        _logger.LogInformation("Notification created: {Id}", notification.Id);
        return notification.Id;
    }

    public async Task UpdateAsync(Guid id, NotificationUpdateDTO dto)
    {
        var notification = await _repository.GetByIdAsync(id);
        if (notification == null)
        {
            throw new BusinessException("Notification not found");
        }

        _mapper.Map(dto, notification);
        notification.UpdatedDate = DateTime.UtcNow;
        await _repository.UpdateAsync(notification);
        _logger.LogInformation("Notification updated: {Id}", id);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        _logger.LogInformation("Notification deleted: {Id}", id);
    }

    public async Task SendOrderStatusNotificationAsync(Guid userId, OrderStatus status, string notes)
    {
        var template = await _templateRepository.GetByTypeAsync(NotificationType.OrderUpdate);
        if (template == null)
        {
            _logger.LogWarning("No template found for OrderUpdate");
            return;
        }

        var message = template.Body.Replace("{Status}", status.ToString()).Replace("{Notes}", notes);
        var dto = new NotificationCreateDTO
        {
            UserId = userId,
            Type = NotificationType.OrderUpdate,
            Title = template.Subject.Replace("{Status}", status.ToString()),
            Message = message
        };

        await CreateAsync(dto);
        _logger.LogInformation("Order status notification sent to user {UserId}", userId);
    }
}