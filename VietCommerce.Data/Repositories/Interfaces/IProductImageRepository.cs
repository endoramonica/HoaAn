using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Products;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface IProductImageRepository : IGenericRepository<ProductImage>
    {
        Task<List<ProductImage>> GetByProductIdAsync(Guid productId, bool onlyImages = true);
        Task<ProductImage?> GetMainImageAsync(Guid productId);
        Task SetMainImageAsync(Guid productId, Guid imageId);
        Task<int> GetNextDisplayOrderAsync(Guid productId);
        Task DeleteAsync(Guid imageId);
    }
}
