using VietCommerce.Core.Entities;
using VietCommerce.Core.Entities.Common;
using VietCommerce.Core.Models;

namespace VietCommerce.Data.Repositories.Generic;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<PaginatedResult<T>> GetPaginatedAsync(int pageNumber, int pageSize);
    Task<int> GetTotalCountAsync();
    Task<T> CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}