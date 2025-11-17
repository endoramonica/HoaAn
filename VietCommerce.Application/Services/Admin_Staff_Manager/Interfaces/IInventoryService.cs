using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VietCommerce.Core.DTOs.Inventory;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Models; // ✅ THÊM để có ApiResponse

namespace VietCommerce.Application.Services.Admin_Staff_Manager.Interfaces
{
    public interface IInventoryService
    {
        // ✅ Kiểm tra tồn kho (không wrap ApiResponse)
        Task<CheckStockResponse> CheckStockAsync(Guid storeId, Guid productId, int requiredQuantity);
        Task<BulkCheckStockResponse> BulkCheckStockAsync(Guid storeId, List<Guid> productIds);

        // ✅ Quản lý tồn kho
        Task<InventoryDto> GetInventoryAsync(Guid storeId, Guid productId);
        Task<ApiResponse<InventoryDto>> AdjustInventoryAsync(Guid storeId, AdjustInventoryRequest request); // ✅ Không cần userId param (dùng ICurrentUser)

        // ✅ Dự trữ & Giải phóng stock - các method cấp thấp
        Task<ApiResponse<Inventory>> ReserveAsync(Guid productId, Guid storeId, int quantity, Guid orderId);
        Task<ApiResponse<Inventory>> ConfirmAsync(Guid productId, Guid storeId, int quantity, Guid orderId);
        Task<ApiResponse<Inventory>> ReleaseAsync(Guid productId, Guid storeId, int quantity, Guid orderId);

        // ✅ Admin operations
        Task<ApiResponse<Inventory>> AddStockAsync(Guid productId, Guid storeId, int quantity, string reason);

        // ✅ Queries (thêm method còn thiếu)
        Task<int> GetAvailableQuantityAsync(Guid productId, Guid storeId);

        // ✅ Lịch sử chuyển động
        Task<List<InventoryMovementDto>> GetInventoryMovementsAsync(Guid inventoryId, int pageNumber = 1, int pageSize = 10);
        Task<List<InventoryMovementDto>> GetMovementsByOrderIdAsync(Guid orderId);
        Task<List<InventoryMovementDto>> GetMovementsByTransferIdAsync(Guid transferId);
        Task<List<InventoryMovementDto>> GetMovementsInDateRangeAsync(DateTime startDate, DateTime endDate);

        // ✅ Danh sách tồn kho thấp
        Task<List<Inventory>> GetLowStockProductsAsync(Guid storeId);
    }
}