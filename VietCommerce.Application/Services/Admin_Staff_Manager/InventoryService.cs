using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Application.Services.Admin_Staff_Manager.Interfaces;
using VietCommerce.Application.Services.Services;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.DTOs.Inventory;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Enums.Products;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;
using IInventoryService = VietCommerce.Application.Services.Admin_Staff_Manager.Interfaces.IInventoryService;

namespace VietCommerce.Application.Services.Admin_Staff_Manager;
    public class InventoryService : BaseService, IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<InventoryService> _logger;


    private const string CACHE_PREFIX_INVENTORY = "inv";
    private const string CACHE_PREFIX_LOW_STOCK = "low_stock";
    private const int CACHE_TTL_MINUTES = 15;

    public InventoryService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUser currentUser,
        ILogger<InventoryService> logger,
        ICacheService? cacheService = null)
        : base(logger, cacheService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _logger = logger;
    }

    #region Cache Helper
    /// <summary>
    /// Invalidate inventory cache cho một product ở một store
    /// </summary>
    private async Task InvalidateInventoryCacheAsync(Guid storeId, Guid productId)
    {
        await InvalidateCacheAsync(CreateCacheKey(CACHE_PREFIX_INVENTORY, storeId, productId));
        await InvalidateCacheAsync(CreateCacheKey(CACHE_PREFIX_LOW_STOCK, storeId));
    }
    #endregion

    #region Reserve/Confirm/Release Flow

    /// <summary>
    /// Reserve (giữ chỗ) tồn kho khi tạo đơn hàng
    /// Tăng QuantityReserved, kiểm tra không oversell
    /// </summary>
    public async Task<ApiResponse<Inventory>> ReserveAsync(Guid productId, Guid storeId, int quantity, Guid orderId)
    {
        return await ExecuteAsApiResponseAsync<Inventory>(
            async () =>
            {
                ValidateId(productId, nameof(productId));
                ValidateId(storeId, nameof(storeId));
                ValidateId(orderId, nameof(orderId));
                ThrowIf(quantity <= 0, "Số lượng phải > 0");

                var inventory = await _unitOfWork.Inventories
                    .GetByStoreAndProductAsync(storeId, productId);

                ThrowIfNot(inventory != null, $"Sản phẩm không tồn tại trong kho #{storeId}");

                var quantityActual = inventory!.QuantityAvailable - inventory.QuantityReserved;
                ThrowIfNot(quantityActual >= quantity, $"Không đủ hàng. Có sẵn: {quantityActual}");

                // Transaction để đảm bảo atomicity
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var quantityBefore = inventory.QuantityReserved;
                    inventory.QuantityReserved += quantity;

                    // Ghi nhận movement
                    var movement = new InventoryMovement
                    {
                        InventoryId = inventory.Id,
                        ChangeAmount = -quantity,
                        MovementType = InventoryMovementType.PURCHASE,
                        QuantityBefore = quantityBefore,
                        QuantityAfter = inventory.QuantityReserved,
                        Reason = $"Reserve cho đơn hàng {orderId}",
                        OrderId = orderId,
                        PerformedById = _currentUser.UserId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _currentUser.UserId
                    };

                    await _unitOfWork.InventoryMovements.AddAsync(movement);
                    _unitOfWork.Inventories.Update(inventory);
                    await _unitOfWork.CommitTransactionAsync();

                    // Invalidate cache
                    await InvalidateInventoryCacheAsync(storeId, productId);

                    LogInfo($"✅ Reserved {quantity} units for order {orderId}");
                    return inventory;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    LogError($"❌ Reserve failed for order {orderId}", ex);
                    throw;
                }
            },
            $"ReserveStock({productId}, {storeId}, {quantity}, {orderId})"
        ).ConfigureAwait(false);
    }

    /// <summary>
    /// Confirm (xác nhận) khi thanh toán thành công
    /// Giảm QuantityAvailable, QuantityReserved
    /// </summary>
    public async Task<ApiResponse<Inventory>> ConfirmAsync(Guid productId, Guid storeId, int quantity, Guid orderId)
    {
        return await ExecuteAsApiResponseAsync<Inventory>(
            async () =>
            {
                ValidateId(productId, nameof(productId));
                ValidateId(storeId, nameof(storeId));
                ValidateId(orderId, nameof(orderId));
                ThrowIf(quantity <= 0, "Số lượng phải > 0");

                var inventory = await _unitOfWork.Inventories
                    .GetByStoreAndProductAsync(storeId, productId);

                ThrowIfNot(inventory != null, $"Sản phẩm không tồn tại trong kho #{storeId}");
                ThrowIfNot(inventory!.QuantityReserved >= quantity,
                    $"Số lượng reserved không đủ. Reserved: {inventory.QuantityReserved}");

                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var availableBefore = inventory.QuantityAvailable;
                    var reservedBefore = inventory.QuantityReserved;

                    // Giảm cả available và reserved
                    inventory.QuantityAvailable -= quantity;
                    inventory.QuantityReserved -= quantity;

                    ThrowIfNot(inventory.QuantityAvailable >= 0, "Số lượng available không thể âm");

                    // Ghi nhận movement
                    var movement = new InventoryMovement
                    {
                        InventoryId = inventory.Id,
                        ChangeAmount = -quantity,
                        MovementType = InventoryMovementType.SALE,
                        QuantityBefore = availableBefore,
                        QuantityAfter = inventory.QuantityAvailable,
                        Reason = $"Confirm order {orderId}",
                        OrderId = orderId,
                        PerformedById = _currentUser.UserId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _currentUser.UserId
                    };

                    await _unitOfWork.InventoryMovements.AddAsync(movement);
                    _unitOfWork.Inventories.Update(inventory);
                    await _unitOfWork.CommitTransactionAsync();

                    // Invalidate cache
                    await InvalidateInventoryCacheAsync(storeId, productId);

                    LogInfo($"✅ Confirmed {quantity} units for order {orderId}");
                    return inventory;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    LogError($"❌ Confirm failed for order {orderId}", ex);
                    throw;
                }
            },
            $"ConfirmStock({productId}, {storeId}, {quantity}, {orderId})"
        ).ConfigureAwait(false);
    }

    /// <summary>
    /// Release (trả lại) tồn kho đã reserve khi cancel/timeout
    /// Giảm QuantityReserved
    /// </summary>
    public async Task<ApiResponse<Inventory>> ReleaseAsync(Guid productId, Guid storeId, int quantity, Guid orderId)
    {
        return await ExecuteAsApiResponseAsync<Inventory>(
            async () =>
            {
                ValidateId(productId, nameof(productId));
                ValidateId(storeId, nameof(storeId));
                ValidateId(orderId, nameof(orderId));
                ThrowIf(quantity <= 0, "Số lượng phải > 0");

                var inventory = await _unitOfWork.Inventories
                    .GetByStoreAndProductAsync(storeId, productId);

                ThrowIfNot(inventory != null, $"Sản phẩm không tồn tại trong kho #{storeId}");
                ThrowIfNot(inventory!.QuantityReserved >= quantity,
                    $"Số lượng reserved không đủ để release. Reserved: {inventory.QuantityReserved}");

                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var quantityBefore = inventory.QuantityReserved;
                    inventory.QuantityReserved -= quantity;

                    var movement = new InventoryMovement
                    {
                        InventoryId = inventory.Id,
                        ChangeAmount = quantity,
                        MovementType = InventoryMovementType.RETURN,
                        QuantityBefore = quantityBefore,
                        QuantityAfter = inventory.QuantityReserved,
                        Reason = $"Release order {orderId}",
                        OrderId = orderId,
                        PerformedById = _currentUser.UserId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _currentUser.UserId
                    };

                    await _unitOfWork.InventoryMovements.AddAsync(movement);
                    _unitOfWork.Inventories.Update(inventory);
                    await _unitOfWork.CommitTransactionAsync();

                    await InvalidateInventoryCacheAsync(storeId, productId);

                    LogInfo($"✅ Released {quantity} units for order {orderId}");
                    return inventory;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    LogError($"❌ Release failed for order {orderId}", ex);
                    throw;
                }
            },
            $"ReleaseStock({productId}, {storeId}, {quantity}, {orderId})"
        ).ConfigureAwait(false);
    }

    #endregion

    #region Queries

    public async Task<int> GetAvailableQuantityAsync(Guid productId, Guid storeId)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(productId, nameof(productId));
            ValidateId(storeId, nameof(storeId));

            var cacheKey = CreateCacheKey(CACHE_PREFIX_INVENTORY, "qty", storeId, productId);
            return await GetFromCacheOrExecuteAsync(
                cacheKey,
                async () =>
                {
                    var quantity = await _unitOfWork.Inventories
                        .GetAvailableQuantityAsync(storeId, productId);
                    LogDebug($"📦 Available quantity for {productId}: {quantity}");
                    return quantity;
                },
                TimeSpan.FromMinutes(CACHE_TTL_MINUTES)
            );
        }, $"GetAvailableQuantity({productId}, {storeId})");
    }

    public async Task<List<Inventory>> GetLowStockProductsAsync(Guid storeId)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(storeId, nameof(storeId));

            var cacheKey = CreateCacheKey(CACHE_PREFIX_LOW_STOCK, storeId);
            return await GetFromCacheOrExecuteAsync(
                cacheKey,
                async () =>
                {
                    var items = await _unitOfWork.Inventories
                        .GetLowStockByStoreAsync(storeId);
                    LogDebug($"⚠️ Found {items.Count} low stock items in store {storeId}");
                    return items;
                },
                TimeSpan.FromMinutes(CACHE_TTL_MINUTES)
            );
        }, $"GetLowStockProducts({storeId})");
    }

    public async Task<CheckStockResponse> CheckStockAsync(Guid storeId, Guid productId, int requiredQuantity)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(storeId, nameof(storeId));
            ValidateId(productId, nameof(productId));
            ThrowIf(requiredQuantity <= 0, "RequiredQuantity phải > 0");

            var inventory = await _unitOfWork.Inventories
                .GetByStoreAndProductAsync(storeId, productId);

            ThrowIfNot(inventory != null, $"Sản phẩm không tồn tại trong kho #{storeId}");

            var quantityActual = inventory!.QuantityAvailable - inventory.QuantityReserved;
            var isAvailable = quantityActual >= requiredQuantity;

            return new CheckStockResponse
            {
                ProductId = productId,
                IsAvailable = isAvailable,
                QuantityAvailable = inventory.QuantityAvailable,
                QuantityReserved = inventory.QuantityReserved,
                QuantityActual = quantityActual,
                IsLowStock = quantityActual <= inventory.ReorderLevel,
                Message = isAvailable
                    ? $"Có sẵn {quantityActual} sản phẩm"
                    : $"Thiếu {requiredQuantity - quantityActual} sản phẩm"
            };
        }, $"CheckStock({storeId}, {productId}, {requiredQuantity})");
    }

    public async Task<BulkCheckStockResponse> BulkCheckStockAsync(Guid storeId, List<Guid> productIds)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(storeId, nameof(storeId));
            ValidateNotEmpty(productIds, nameof(productIds));

            // ✅ FIX: Dùng GetByStoreAndProductsAsync (repository method mới)
            var inventories = await _unitOfWork.Inventories
                .GetByStoreAndProductsAsync(storeId, productIds);

            var results = new List<CheckStockResponse>();

            foreach (var productId in productIds)
            {
                var inventory = inventories.FirstOrDefault(x => x.ProductId == productId);

                if (inventory == null)
                {
                    results.Add(new CheckStockResponse
                    {
                        ProductId = productId,
                        IsAvailable = false,
                        Message = "Sản phẩm không tồn tại trong kho"
                    });
                    continue;
                }

                var quantityActual = inventory.QuantityAvailable - inventory.QuantityReserved;
                results.Add(new CheckStockResponse
                {
                    ProductId = productId,
                    IsAvailable = true,
                    QuantityAvailable = inventory.QuantityAvailable,
                    QuantityReserved = inventory.QuantityReserved,
                    QuantityActual = quantityActual,
                    IsLowStock = quantityActual <= inventory.ReorderLevel,
                    Message = $"Có sẵn {quantityActual} sản phẩm"
                });
            }

            LogDebug($"📦 Checked {productIds.Count} products, {results.Count(x => x.IsLowStock)} low stock");
            return new BulkCheckStockResponse
            {
                Results = results,
                TotalLowStockItems = results.Count(x => x.IsLowStock)
            };
        }, $"BulkCheckStock({storeId})");
    }

    public async Task<InventoryDto> GetInventoryAsync(Guid storeId, Guid productId)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(storeId, nameof(storeId));
            ValidateId(productId, nameof(productId));

            var cacheKey = CreateCacheKey(CACHE_PREFIX_INVENTORY, storeId, productId);
            var inventory = await GetFromCacheOrExecuteAsync(
                cacheKey,
                async () =>
                {
                    var inv = await _unitOfWork.Inventories
                        .GetByStoreAndProductAsync(storeId, productId);
                    ThrowIfNot(inv != null, "Tồn kho không tìm thấy");
                    return inv!;
                },
                TimeSpan.FromMinutes(CACHE_TTL_MINUTES)
            );

            return _mapper.Map<InventoryDto>(inventory);
        }, $"GetInventory({storeId}, {productId})");
    }

    #endregion

    #region Admin Operations

    public async Task<ApiResponse<Inventory>> AddStockAsync(
        Guid productId, Guid storeId, int quantity, string reason)
    {
        return await ExecuteAsApiResponseAsync<Inventory>(
            async () =>
            {
                ValidateId(productId, nameof(productId));
                ValidateId(storeId, nameof(storeId));
                ThrowIf(quantity <= 0, "Số lượng phải > 0");
                ValidateNotEmpty(reason, nameof(reason));

                var inventory = await _unitOfWork.Inventories
                    .GetByStoreAndProductAsync(storeId, productId);

                ThrowIfNot(inventory != null, "Tồn kho không tìm thấy");

                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var quantityBefore = inventory!.QuantityAvailable;
                    inventory.QuantityAvailable += quantity;

                    var movement = new InventoryMovement
                    {
                        InventoryId = inventory.Id,
                        ChangeAmount = quantity,
                        MovementType = InventoryMovementType.PURCHASE,
                        QuantityBefore = quantityBefore,
                        QuantityAfter = inventory.QuantityAvailable,
                        Reason = reason,
                        PerformedById = _currentUser.UserId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _currentUser.UserId
                    };

                    await _unitOfWork.InventoryMovements.AddAsync(movement);
                    _unitOfWork.Inventories.Update(inventory);
                    await _unitOfWork.CommitTransactionAsync();

                    await InvalidateInventoryCacheAsync(storeId, productId);

                    LogInfo($"✅ Added {quantity} units stock ({reason})");
                    return inventory;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    LogError($"❌ AddStock failed", ex);
                    throw;
                }
            },
            $"AddStock({productId}, {storeId}, {quantity})"
        ).ConfigureAwait(false);
    }

    public async Task<ApiResponse<InventoryDto>> AdjustInventoryAsync(
        Guid storeId, AdjustInventoryRequest request)
    {
        return await ExecuteAsApiResponseAsync<InventoryDto>(
            async () =>
            {
                ValidateId(storeId, nameof(storeId));
                ValidateNotNull(request, nameof(request));
                ValidateId(request.ProductId, nameof(request.ProductId));
                ValidateNotEmpty(request.Reason, nameof(request.Reason));

                var inventory = await _unitOfWork.Inventories
                    .GetByStoreAndProductAsync(storeId, request.ProductId);

                ThrowIfNot(inventory != null, "Tồn kho không tìm thấy");

                // Transaction để đảm bảo atomicity
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var quantityBefore = inventory!.QuantityAvailable;
                    inventory.QuantityAvailable += request.AdjustmentAmount;

                    ThrowIfNot(inventory.QuantityAvailable >= 0, "Số lượng sau điều chỉnh không thể âm");

                    var movement = new InventoryMovement
                    {
                        InventoryId = inventory.Id,
                        ChangeAmount = request.AdjustmentAmount,
                        MovementType = request.AdjustmentAmount > 0
                            ? InventoryMovementType.ADJUSTMENT
                            : InventoryMovementType.ADJUSTMENT,
                        QuantityBefore = quantityBefore,
                        QuantityAfter = inventory.QuantityAvailable,
                        Reason = request.Reason,
                        OrderId = request.OrderId,
                        TransferId = request.TransferId,
                        PerformedById = _currentUser.UserId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _currentUser.UserId
                    };

                    await _unitOfWork.InventoryMovements.AddAsync(movement);
                    _unitOfWork.Inventories.Update(inventory);
                    await _unitOfWork.CommitTransactionAsync();

                    await InvalidateInventoryCacheAsync(storeId, request.ProductId);

                    LogInfo($"✅ Adjusted inventory by {request.AdjustmentAmount}");
                    return _mapper.Map<InventoryDto>(inventory);
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    LogError($"❌ AdjustInventory failed", ex);
                    throw;
                }
            },
            $"AdjustInventory({storeId}, {request.ProductId})"
        ).ConfigureAwait(false);
    }

    public async Task<List<InventoryMovementDto>> GetInventoryMovementsAsync(
        Guid inventoryId, int pageNumber = 1, int pageSize = 10)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(inventoryId, nameof(inventoryId));
            ThrowIfNot(pageNumber > 0, "PageNumber phải > 0");
            ThrowIfNot(pageSize > 0, "PageSize phải > 0");

            // ✅ FIX: Dùng GetByInventoryIdAsync thay vì AsQueryable
            var movements = await _unitOfWork.InventoryMovements
                .GetByInventoryIdAsync(inventoryId);

            // Manual paging after getting all
            var pagedMovements = movements
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            LogDebug($"📋 Retrieved {pagedMovements.Count} movements for inventory {inventoryId}");
            return _mapper.Map<List<InventoryMovementDto>>(pagedMovements);
        }, $"GetInventoryMovements({inventoryId}, page {pageNumber})");
    }

    public async Task<List<InventoryMovementDto>> GetMovementsByOrderIdAsync(Guid orderId)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(orderId, nameof(orderId));
            var movements = await _unitOfWork.InventoryMovements
                .GetByOrderIdAsync(orderId);
            LogDebug($"📋 Retrieved {movements.Count} movements for order {orderId}");
            return _mapper.Map<List<InventoryMovementDto>>(movements);
        }, $"GetMovementsByOrderId({orderId})");
    }

    public async Task<List<InventoryMovementDto>> GetMovementsByTransferIdAsync(Guid transferId)
    {
        return await ExecuteAsync(async () =>
        {
            ValidateId(transferId, nameof(transferId));
            var movements = await _unitOfWork.InventoryMovements
                .GetByTransferIdAsync(transferId);
            LogDebug($"📋 Retrieved {movements.Count} movements for transfer {transferId}");
            return _mapper.Map<List<InventoryMovementDto>>(movements);
        }, $"GetMovementsByTransferId({transferId})");
    }

    public async Task<List<InventoryMovementDto>> GetMovementsInDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await ExecuteAsync(async () =>
        {
            ThrowIfNot(startDate < endDate, "StartDate phải < EndDate");
            var movements = await _unitOfWork.InventoryMovements
                .GetMovementsInDateRangeAsync(startDate, endDate);
            LogDebug($"📋 Retrieved {movements.Count} movements in range {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
            return _mapper.Map<List<InventoryMovementDto>>(movements);
        }, $"GetMovementsInDateRange({startDate:yyyy-MM-dd}, {endDate:yyyy-MM-dd})");
    }

    #endregion
}
