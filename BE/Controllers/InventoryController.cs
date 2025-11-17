using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Admin_Staff_Manager.Interfaces;
using VietCommerce.Core.DTOs.Inventory;

namespace VietCommerce.AdminAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// Kiểm tra tồn kho cho một sản phẩm
        /// </summary>
        [HttpGet("check-stock")]
        [ProducesResponseType(typeof(CheckStockResponse), 200)]
        public async Task<IActionResult> CheckStock(
            [FromQuery] Guid storeId,
            [FromQuery] Guid productId,
            [FromQuery] int requiredQuantity)
        {
            try
            {
                var result = await _inventoryService.CheckStockAsync(storeId, productId, requiredQuantity);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Kiểm tra tồn kho cho nhiều sản phẩm cùng lúc
        /// </summary>
        [HttpPost("bulk-check-stock")]
        [ProducesResponseType(typeof(BulkCheckStockResponse), 200)]
        public async Task<IActionResult> BulkCheckStock(
            [FromQuery] Guid storeId,
            [FromBody] List<Guid> productIds)
        {
            try
            {
                var result = await _inventoryService.BulkCheckStockAsync(storeId, productIds);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin tồn kho của một sản phẩm
        /// </summary>
        [HttpGet("store/{storeId}/product/{productId}")]
        [ProducesResponseType(typeof(InventoryDto), 200)]
        public async Task<IActionResult> GetInventory(Guid storeId, Guid productId)
        {
            try
            {
                var result = await _inventoryService.GetInventoryAsync(storeId, productId);

                if (result == null)
                    return NotFound(new { message = "Không tìm thấy thông tin tồn kho" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Điều chỉnh số lượng tồn kho (nhập/xuất kho)
        /// </summary>
        [HttpPost("store/{storeId}/adjust")]
        [ProducesResponseType(typeof(InventoryDto), 200)]
        public async Task<IActionResult> AdjustInventory(
            Guid storeId,
            [FromBody] AdjustInventoryRequest request)
        {
            try
            {
                // ✅ Service return ApiResponse<InventoryDto>
                var response = await _inventoryService.AdjustInventoryAsync(storeId, request);

                if (!response.Success)
                    return BadRequest(new { message = response.Message });

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Dự trữ stock (khi khách đặt hàng)
        /// </summary>
        [HttpPost("store/{storeId}/reserve")]
        [ProducesResponseType(typeof(InventoryDto), 200)]
        public async Task<IActionResult> ReserveStock(
            Guid storeId,
            [FromBody] ReserveStockRequest request)
        {
            try
            {
                // ✅ Gọi method cấp thấp ReserveAsync
                var response = await _inventoryService.ReserveAsync(
                    request.ProductId,
                    storeId,
                    request.Quantity,
                    request.OrderId
                );

                if (!response.Success)
                    return BadRequest(new { message = response.Message });

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Giải phóng stock đã dự trữ (khi hủy đơn hoặc thanh toán thành công)
        /// </summary>
        [HttpPost("store/{storeId}/release-reserved")]
        [ProducesResponseType(typeof(InventoryDto), 200)]
        public async Task<IActionResult> ReleaseReservedStock(
            Guid storeId,
            [FromBody] ReleaseReservedStockRequest request)
        {
            try
            {
                // ✅ Gọi method ReleaseAsync
                var response = await _inventoryService.ReleaseAsync(
                    request.ProductId,
                    storeId,
                    request.Quantity,
                    request.OrderId
                );

                if (!response.Success)
                    return BadRequest(new { message = response.Message });

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy lịch sử chuyển động kho
        /// </summary>
        [HttpGet("{inventoryId}/movements")]
        [ProducesResponseType(typeof(List<InventoryMovementDto>), 200)]
        public async Task<IActionResult> GetInventoryMovements(
            Guid inventoryId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _inventoryService.GetInventoryMovementsAsync(inventoryId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách sản phẩm tồn kho thấp
        /// </summary>
        [HttpGet("store/{storeId}/low-stock")]
        [ProducesResponseType(typeof(List<InventoryDto>), 200)]
        public async Task<IActionResult> GetLowStockItems(Guid storeId)
        {
            try
            {
                var result = await _inventoryService.GetLowStockProductsAsync(storeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}