using System;
using VietCommerce.Core.Entities.Products;
namespace VietCommerce.Api.Services.Interfaces;

/// Service quản lý inventory với race condition handling
/// Flow: Reserve → Confirm → Release

public interface IInventoryService
{
    
    /// Reserve (giữ) số lượng tồn kho khi user tạo đơn hàng
    /// Tăng QuantityReserved, đảm bảo không oversell
    
    /// <param name="productId">ID sản phẩm</param>
    /// <param name="storeId">ID cửa hàng</param>
    /// <param name="quantity">Số lượng cần reserve</param>
    /// <param name="orderId">ID đơn hàng (để track)</param>
    /// <returns>Success hoặc error message</returns>
    Task<InventoryOperationResult> ReserveAsync(Guid productId, Guid storeId, int quantity, Guid orderId);

    
    /// Confirm (xác nhận) khi thanh toán thành công
    /// Giảm QuantityAvailable, QuantityReserved; tăng QuantitySold
    
    /// <param name="productId">ID sản phẩm</param>
    /// <param name="storeId">ID cửa hàng</param>
    /// <param name="quantity">Số lượng confirm</param>
    /// <param name="orderId">ID đơn hàng</param>
    /// <returns>Success hoặc error message</returns>
    Task<InventoryOperationResult> ConfirmAsync(Guid productId, Guid storeId, int quantity, Guid orderId);

    
    /// Release (trả lại) số lượng đã reserve khi cancel/timeout
    /// Giảm QuantityReserved
    
    /// <param name="productId">ID sản phẩm</param>
    /// <param name="storeId">ID cửa hàng</param>
    /// <param name="quantity">Số lượng release</param>
    /// <param name="orderId">ID đơn hàng</param>
    /// <returns>Success hoặc error message</returns>
    Task<InventoryOperationResult> ReleaseAsync(Guid productId, Guid storeId, int quantity, Guid orderId);

    
    /// Kiểm tra tồn kho available (không bao gồm reserved)
    
    Task<int> GetAvailableQuantityAsync(Guid productId, Guid storeId);

    
    /// Thêm stock vào kho (nhập hàng)
    
    Task<InventoryOperationResult> AddStockAsync(Guid productId, Guid storeId, int quantity, Guid performedBy, string reason);

    
    /// Kiểm tra các sản phẩm có tồn kho thấp (cần reorder)
    
    Task<List<Inventory>> GetLowStockProductsAsync(Guid storeId);
}


/// Result object cho inventory operations

public class InventoryOperationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Inventory? Inventory { get; set; }

    public static InventoryOperationResult SuccessResult(Inventory inventory)
    {
        return new InventoryOperationResult
        {
            Success = true,
            Inventory = inventory
        };
    }

    public static InventoryOperationResult FailureResult(string errorMessage)
    {
        return new InventoryOperationResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}