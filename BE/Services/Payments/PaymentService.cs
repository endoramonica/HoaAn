using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using VietCommerce.Core.Common.Exceptions;
using VietCommerce.Core.DTOs.Payments;
using VietCommerce.Core.Entities.Payments;
using VietCommerce.Core.Enums.Payments;
using VietCommerce.Core.Models;

namespace VietCommerce.Core.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repository;
    private readonly IPaymentTransactionRepository _transactionRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<PaymentCreateDTO> _createValidator;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IPaymentRepository repository, IPaymentTransactionRepository transactionRepository, IMapper mapper, IValidator<PaymentCreateDTO> createValidator, ILogger<PaymentService> logger)
    {
        _repository = repository;
        _transactionRepository = transactionRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _logger = logger;
    }

    public async Task<PaginatedResult<PaymentListDTO>> GetPaginatedAsync(int pageNumber = 1, int pageSize = 10)
    {
        var payments = await _repository.GetPaginatedAsync(pageNumber, pageSize);
        var total = await _repository.GetTotalCountAsync();
        var dtos = _mapper.Map<IEnumerable<PaymentListDTO>>(payments);
        return new PaginatedResult<PaymentListDTO>(dtos, pageNumber, pageSize, total);
    }

    public async Task<PaymentListDTO> GetByIdAsync(Guid id)
    {
        var payment = await _repository.GetByIdAsync(id);
        if (payment == null)
        {
            throw new BusinessException("Payment not found");
        }
        return _mapper.Map<PaymentListDTO>(payment);
    }

    public async Task<Guid> CreateAsync(PaymentCreateDTO dto)
    {
        await _createValidator.ValidateAndThrowAsync(dto);

        var payment = _mapper.Map<Payment>(dto);
        payment.Id = Guid.NewGuid();
        payment.Status = PaymentMethodType.PENDING;
        payment.CreatedAt = DateTime.UtcNow;

        await _repository.CreateAsync(payment);
        _logger.LogInformation("Payment created: {Id}", payment.Id);
        return payment.Id;
    }

    public async Task UpdateAsync(Guid id, PaymentUpdateDTO dto)
    {
        var payment = await _repository.GetByIdAsync(id);
        if (payment == null)
        {
            throw new BusinessException("Payment not found");
        }

        _mapper.Map(dto, payment);
        payment.UpdatedDate = DateTime.UtcNow;
        await _repository.UpdateAsync(payment);
        _logger.LogInformation("Payment updated: {Id}", id);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        _logger.LogInformation("Payment deleted: {Id}", id);
    }

    public async Task ProcessPaymentAsync(Guid paymentId, PaymentMethodType status, string transactionId)
    {
        var payment = await _repository.GetByIdAsync(paymentId);
        if (payment == null)
        {
            throw new BusinessException("Payment not found");
        }

        payment.Status = status;
        payment.UpdatedDate = DateTime.UtcNow;

        var transaction = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            PaymentId = paymentId,
            TransactionId = transactionId,
            Status = status,
            GatewayResponse = "Processed via gateway",
            CreatedAt = DateTime.UtcNow
        };

        await _transactionRepository.CreateAsync(transaction);
        await _repository.UpdateAsync(payment);

        _logger.LogInformation("Payment processed: {Id}, Status: {Status}", paymentId, status);
    }
}