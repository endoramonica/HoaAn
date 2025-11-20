using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Logistics;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services.Interfaces
{
    public interface IStockTransferService
    {
        Task<PaginatedResult<StockTransferDto>> GetStockTransfersAsync(
            PaginationParams pagination,
            StockTransferFilters filters);
        Task<StockTransferDto> GetStockTransferByIdAsync(Guid id);
        Task<StockTransferDto> CreateStockTransferAsync(CreateStockTransferRequest request);
        Task<StockTransferDto> UpdateStockTransferStatusAsync(Guid id, UpdateStockTransferStatusRequest request);
        Task CancelStockTransferAsync(Guid id, CancelStockTransferRequest request);
        Task<StockTransferSummaryDto> GetTransferSummaryAsync(StockTransferFilters? filters = null);
    }
}
