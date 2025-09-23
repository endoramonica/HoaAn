using VietCommerce.Core.DTOs.Products;
using VietCommerce.Core.Models;

namespace VietCommerce.Core.Services.Products;

public interface IProductService
{
    Task<PaginatedResult<ProductListDTO>> GetPaginatedAsync(int pageNumber = 1, int pageSize = 10);
    Task<ProductListDTO> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(ProductCreateDTO dto);
    Task UpdateAsync(Guid id, ProductUpdateDTO dto);
    Task DeleteAsync(Guid id);
}