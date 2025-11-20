using VietCommerce.Core.Entities.Logistics;

namespace VietCommerce.Data.Repositories.Interfaces
{
    public interface ITransferItemRepository : IGenericRepository<TransferItem>
    {
        Task<IEnumerable<TransferItem>> GetByStockTransferIdAsync(Guid stockTransferId);
        Task<IEnumerable<TransferItem>> GetByProductIdAsync(Guid productId);
        Task<TransferItem?> GetByIdWithDetailsAsync(Guid id);
    }
}
