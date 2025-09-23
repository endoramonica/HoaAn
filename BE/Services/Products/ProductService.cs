using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using VietCommerce.Core.Common.Exceptions;
using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Models;
using VietCommerce.Core.Services.Products;
using VietCommerce.Data.Repositories.Products;
using VietCommerce.Core.Entities.Products;

namespace VietCommerce.Core.Services.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<ProductCreateDTO> _createValidator;
    private readonly IValidator<ProductUpdateDTO> _updateValidator;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, IMapper mapper, IValidator<ProductCreateDTO> createValidator, IValidator<ProductUpdateDTO> updateValidator, ILogger<ProductService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<PaginatedResult<ProductListDTO>> GetPaginatedAsync(int pageNumber = 1, int pageSize = 10)
    {
        var products = await _repository.GetPaginatedAsync(pageNumber, pageSize);
        var total = await _repository.GetTotalCountAsync();
        var dtos = _mapper.Map<IEnumerable<ProductListDTO>>(products);
        return new PaginatedResult<ProductListDTO>(dtos, pageNumber, pageSize, total);
    }

    public async Task<ProductListDTO> GetByIdAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            throw new BusinessException("Product not found");
        }
        return _mapper.Map<ProductListDTO>(product);
    }

    public async Task<Guid> CreateAsync(ProductCreateDTO dto)
    {
        await _createValidator.ValidateAndThrowAsync(dto);

        var product = _mapper.Map<Product>(dto);
        product.Id = Guid.NewGuid();
        product.CreatedDate = DateTime.UtcNow;
        product.IsActive = true;

        await _repository.CreateAsync(product);
        _logger.LogInformation("Product created: {Id}", product.Id);
        return product.Id;
    }

    public async Task UpdateAsync(Guid id, ProductUpdateDTO dto)
    {
        await _updateValidator.ValidateAndThrowAsync(dto);
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            throw new BusinessException("Product not found");
        }

        _mapper.Map(dto, product);
        product.UpdatedDate = DateTime.UtcNow;
        await _repository.UpdateAsync(product);
        _logger.LogInformation("Product updated: {Id}", id);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        _logger.LogInformation("Product deleted: {Id}", id);
    }
}