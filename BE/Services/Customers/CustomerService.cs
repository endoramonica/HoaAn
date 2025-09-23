using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using VietCommerce.Core.Common.Exceptions;
using VietCommerce.Core.DTOs.Customers;
using VietCommerce.Core.Models;
using VietCommerce.Core.Services.Customers;
using VietCommerce.Data.Repositories.Customers;
using VietCommerce.Core.Entities.Customers;

namespace VietCommerce.Core.Services.Customers;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CustomerCreateDTO> _createValidator;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(ICustomerRepository repository, IMapper mapper, IValidator<CustomerCreateDTO> createValidator, ILogger<CustomerService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _logger = logger;
    }

    public async Task<PaginatedResult<CustomerListDTO>> GetPaginatedAsync(int pageNumber = 1, int pageSize = 10)
    {
        var customers = await _repository.GetPaginatedAsync(pageNumber, pageSize);
        var total = await _repository.GetTotalCountAsync();
        var dtos = _mapper.Map<IEnumerable<CustomerListDTO>>(customers);
        return new PaginatedResult<CustomerListDTO>(dtos, pageNumber, pageSize, total);
    }

    public async Task<CustomerListDTO> GetByIdAsync(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);
        if (customer == null)
        {
            throw new BusinessException("Customer not found");
        }
        return _mapper.Map<CustomerListDTO>(customer);
    }

    public async Task<Guid> CreateAsync(CustomerCreateDTO dto)
    {
        await _createValidator.ValidateAndThrowAsync(dto);

        var customer = _mapper.Map<Customer>(dto);
        customer.Id = Guid.NewGuid();
        customer.CreatedDate = DateTime.UtcNow;

        await _repository.CreateAsync(customer);
        _logger.LogInformation("Customer created: {Id}", customer.Id);
        return customer.Id;
    }

    public async Task UpdateAsync(Guid id, CustomerUpdateDTO dto)
    {
        var customer = await _repository.GetByIdAsync(id);
        if (customer == null)
        {
            throw new BusinessException("Customer not found");
        }

        _mapper.Map(dto, customer);
        customer.UpdatedDate = DateTime.UtcNow;
        await _repository.UpdateAsync(customer);
        _logger.LogInformation("Customer updated: {Id}", id);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        _logger.LogInformation("Customer deleted: {Id}", id);
    }
}