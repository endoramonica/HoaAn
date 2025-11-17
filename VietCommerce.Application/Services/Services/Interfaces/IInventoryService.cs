// ==================== Service Interface ====================
namespace VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.DTOs.Inventory;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Models;

public interface IInventoryService
{
    // ============ Reserve/Confirm/Release Flow ============
    /// <summary>
    /// Giữ chỗ tồn kho khi tạo đơn hàng (tăng QuantityReserved)
    /// </summary>
    Task<ApiResponse<Inventory>> ReserveAsync(Guid productId, Guid storeId, int quantity, Guid orderId);

    /// <summary>
    /// Xác nhận khi thanh toán thành công (giảm QuantityAvailable & QuantityReserved)
    /// </summary>
    Task<ApiResponse<Inventory>> ConfirmAsync(Guid productId, Guid storeId, int quantity, Guid orderId);

    /// <summary>
    /// Trả lại tồn kho khi cancel/timeout (giảm QuantityReserved)
    /// </summary>
    Task<ApiResponse<Inventory>> ReleaseAsync(Guid productId, Guid storeId, int quantity, Guid orderId);

    // ============ Query Methods ============
    /// <summary>
    /// Lấy số lượng sẵn có (available - reserved) với caching
    /// </summary>
    Task<int> GetAvailableQuantityAsync(Guid productId, Guid storeId);

    /// <summary>
    /// Lấy danh sách sản phẩm tồn kho thấp với caching
    /// </summary>
    Task<List<Inventory>> GetLowStockProductsAsync(Guid storeId);

    /// <summary>
    /// Kiểm tra đủ hàng cho một sản phẩm
    /// </summary>
    Task<CheckStockResponse> CheckStockAsync(Guid storeId, Guid productId, int requiredQuantity);

    /// <summary>
    /// Kiểm tra đủ hàng cho nhiều sản phẩm cùng lúc
    /// </summary>
    Task<BulkCheckStockResponse> BulkCheckStockAsync(Guid storeId, List<Guid> productIds);

    /// <summary>
    /// Lấy chi tiết tồn kho của một sản phẩm
    /// </summary>
    Task<InventoryDto> GetInventoryAsync(Guid storeId, Guid productId);

    // ============ Admin Operations ============
    /// <summary>
    /// Cộng thêm hàng nhập kho (lấy UserId từ CurrentUser)
    /// Xóa param performedBy, dùng _currentUser.UserId thay thế
    /// </summary>
    Task<ApiResponse<Inventory>> AddStockAsync(Guid productId, Guid storeId, int quantity, string reason);

    /// <summary>
    /// Điều chỉnh tồn kho thủ công (lấy UserId từ CurrentUser)
    /// Xóa param userId, dùng _currentUser.UserId thay thế
    /// </summary>
    Task<ApiResponse<InventoryDto>> AdjustInventoryAsync(Guid storeId, AdjustInventoryRequest request);

    /// <summary>
    /// Lấy lịch sử thay đổi tồn kho của một inventory record
    /// </summary>
    Task<List<InventoryMovementDto>> GetInventoryMovementsAsync(Guid inventoryId, int pageNumber = 1, int pageSize = 10);

    // ============ Movement Query Methods (NEW) ============
    /// <summary>
    /// Lấy lịch sử thay đổi tồn kho theo OrderId (dùng repository method)
    /// </summary>
    Task<List<InventoryMovementDto>> GetMovementsByOrderIdAsync(Guid orderId);

    /// <summary>
    /// Lấy lịch sử thay đổi tồn kho theo TransferId (dùng repository method)
    /// </summary>
    Task<List<InventoryMovementDto>> GetMovementsByTransferIdAsync(Guid transferId);

    /// <summary>
    /// Lấy lịch sử thay đổi tồn kho trong khoảng thời gian (dùng repository method)
    /// </summary>
    Task<List<InventoryMovementDto>> GetMovementsInDateRangeAsync(DateTime startDate, DateTime endDate);
}