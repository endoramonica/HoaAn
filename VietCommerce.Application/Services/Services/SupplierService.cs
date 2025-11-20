using AutoMapper;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Application.Services.Services.Interfaces.Identities;
using VietCommerce.Core.DTOs.Suppliers;
using VietCommerce.Core.Entities.Logistics;
using VietCommerce.Core.Models;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Application.Services.Services;

/// <summary>
/// Service implementation for managing suppliers
/// Implements business logic with validation, caching, relationship management AND Permission Checks
/// 
/// FEATURES:
/// ✅ CRUD operations with validation
/// ✅ Permission Checks (RBAC)
/// ✅ Duplicate checking (Name, Email)
/// ✅ Relationship validation (Products, StockTransfers)
/// ✅ Redis caching with TTL
/// ✅ Pagination and filtering
/// ✅ Audit logging
/// </summary>
public class SupplierService : BaseService, ISupplierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUser _currentUser;

    // Cache settings
    private const string CACHE_PREFIX_SINGLE = "supplier";
    private const string CACHE_PREFIX_LIST = "suppliers:list";
    private const string CACHE_PREFIX_STATS = "supplier:stats";
    private static readonly TimeSpan CACHE_DURATION_SINGLE = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan CACHE_DURATION_LIST = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan CACHE_DURATION_STATS = TimeSpan.FromMinutes(15);

    // Permission constants
    private const string PERMISSION_MANAGE_SUPPLIERS = "lrm.manage_suppliers";

    public SupplierService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<SupplierService> logger,
        ICacheService? cacheService,
        IPermissionService permissionService,
        ICurrentUser currentUser)
        : base(logger, cacheService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    #region Public Methods

    /// <summary>
    /// Get paginated list of suppliers with filtering and sorting
    /// </summary>
    public async Task<PaginatedResult<SupplierDto>> GetSuppliersAsync(
        PaginationParams pagination,
        SupplierFilters filters)
    {
        return await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_MANAGE_SUPPLIERS);

            ValidateNotNull(pagination, nameof(pagination));
            ValidateNotNull(filters, nameof(filters));

            LogInfo("📋 Lấy danh sách suppliers - Page: {Page}, PageSize: {PageSize}",
                pagination.Page, pagination.PageSize);

            // Generate cache key based on pagination and filters
            var cacheKey = GenerateListCacheKey(pagination, filters);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var (items, totalCount) = await _unitOfWork.Suppliers
                    .GetSuppliersPagedAsync(pagination, filters);

                // Map to DTOs and enrich with counts
                var dtos = new List<SupplierDto>();
                foreach (var supplier in items)
                {
                    var dto = _mapper.Map<SupplierDto>(supplier);
                    dto.ProductCount = await _unitOfWork.Suppliers.GetProductCountAsync(supplier.Id);
                    dto.StockTransferCount = await _unitOfWork.Suppliers.GetStockTransferCountAsync(supplier.Id);
                    dtos.Add(dto);
                }

                var result = new PaginatedResult<SupplierDto>(
                    dtos,
                    pagination.Page,
                    pagination.PageSize,
                    totalCount
                );

                LogInfo("✅ Tìm thấy {Count} suppliers (Total: {Total})", dtos.Count, totalCount);
                return result;
            }, CACHE_DURATION_LIST);

        }, "GetSuppliersAsync");
    }

    /// <summary>
    /// Get supplier details by ID
    /// </summary>
    public async Task<SupplierDto> GetSupplierByIdAsync(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_MANAGE_SUPPLIERS);

            ValidateId(id, nameof(id));

            LogInfo("🔍 Lấy thông tin supplier - ID: {SupplierId}", id);

            var cacheKey = CreateCacheKey(CACHE_PREFIX_SINGLE, id);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var supplier = await _unitOfWork.Suppliers.GetSupplierWithDetailsAsync(id);
                ThrowIf(supplier == null, $"Không tìm thấy supplier với ID: {id}");

                var dto = _mapper.Map<SupplierDto>(supplier);
                dto.ProductCount = supplier!.Products?.Count ?? 0;
                dto.StockTransferCount = supplier.StockTransfers?.Count ?? 0;

                LogInfo("✅ Tìm thấy supplier: {Name}", dto.Name);
                return dto;
            }, CACHE_DURATION_SINGLE);

        }, "GetSupplierByIdAsync");
    }

    /// <summary>
    /// Create a new supplier with validation
    /// </summary>
    public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_MANAGE_SUPPLIERS);

            ValidateNotNull(request, nameof(request));
            ValidateEmail(request.Email);

            LogInfo("➕ Tạo supplier mới - Name: {Name}, Email: {Email}",
                request.Name, request.Email);

            // Check duplicate name
            var nameExists = await _unitOfWork.Suppliers
                .IsNameExistsAsync(request.Name);
            ThrowIf(nameExists, $"Tên supplier '{request.Name}' đã tồn tại");

            // Check duplicate email
            var emailExists = await _unitOfWork.Suppliers
                .IsEmailExistsAsync(request.Email);
            ThrowIf(emailExists, $"Email '{request.Email}' đã được sử dụng");

            // Map and create
            var supplier = _mapper.Map<Supplier>(request);
            supplier.CreatedAt = DateTime.UtcNow;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.SaveChangesAsync();

            // Map to DTO
            var dto = _mapper.Map<SupplierDto>(supplier);
            dto.ProductCount = 0;
            dto.StockTransferCount = 0;

            // Invalidate list cache
            await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX_LIST}:*");

            LogInfo("✅ Tạo supplier thành công - ID: {SupplierId}, Name: {Name}",
                dto.Id, dto.Name);

            return dto;

        }, "CreateSupplierAsync");
    }

    /// <summary>
    /// Update an existing supplier (partial update supported)
    /// </summary>
    public async Task<SupplierDto> UpdateSupplierAsync(Guid id, UpdateSupplierRequest request)
    {
        return await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_MANAGE_SUPPLIERS);

            ValidateId(id, nameof(id));
            ValidateNotNull(request, nameof(request));

            LogInfo("✏️ Cập nhật supplier - ID: {SupplierId}", id);

            // Check if supplier exists
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            ThrowIf(supplier == null, $"Không tìm thấy supplier với ID: {id}");

            // Validate email if provided
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                ValidateEmail(request.Email);
            }

            // Check duplicate name if changed
            if (!string.IsNullOrWhiteSpace(request.Name) &&
                request.Name != supplier!.Name)
            {
                var nameExists = await _unitOfWork.Suppliers
                    .IsNameExistsAsync(request.Name, id);
                ThrowIf(nameExists, $"Tên supplier '{request.Name}' đã tồn tại");
            }

            // Check duplicate email if changed
            if (!string.IsNullOrWhiteSpace(request.Email) &&
                request.Email != supplier!.Email)
            {
                var emailExists = await _unitOfWork.Suppliers
                    .IsEmailExistsAsync(request.Email, id);
                ThrowIf(emailExists, $"Email '{request.Email}' đã được sử dụng");
            }

            // Apply partial updates
            if (!string.IsNullOrWhiteSpace(request.Name))
                supplier!.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Email))
                supplier!.Email = request.Email;

            if (!string.IsNullOrWhiteSpace(request.Phone))
                supplier!.Phone = request.Phone;

            if (!string.IsNullOrWhiteSpace(request.Address))
                supplier!.Address = request.Address;

            if (request.Status.HasValue)
                supplier!.Status = request.Status.Value;

            if (!string.IsNullOrWhiteSpace(request.PaymentTerms))
                supplier!.PaymentTerms = request.PaymentTerms;

            if (!string.IsNullOrWhiteSpace(request.DeliverySchedule))
                supplier!.DeliverySchedule = request.DeliverySchedule;

            supplier!.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.SaveChangesAsync();

            // Map to DTO
            var dto = _mapper.Map<SupplierDto>(supplier);
            dto.ProductCount = await _unitOfWork.Suppliers.GetProductCountAsync(id);
            dto.StockTransferCount = await _unitOfWork.Suppliers.GetStockTransferCountAsync(id);

            // Invalidate caches
            await InvalidateMultipleCachesAsync(
                CreateCacheKey(CACHE_PREFIX_SINGLE, id),
                CreateCacheKey(CACHE_PREFIX_STATS, id)
            );
            await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX_LIST}:*");

            LogInfo("✅ Cập nhật supplier thành công - ID: {SupplierId}", id);

            return dto;

        }, "UpdateSupplierAsync");
    }

    /// <summary>
    /// Delete a supplier (checks for relationships first)
    /// </summary>
    public async Task DeleteSupplierAsync(Guid id)
    {
        await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_MANAGE_SUPPLIERS);

            ValidateId(id, nameof(id));

            LogInfo("🗑️ Xóa supplier - ID: {SupplierId}", id);

            // Check if supplier exists
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            ThrowIf(supplier == null, $"Không tìm thấy supplier với ID: {id}");

            // Check for relationships
            var hasProducts = await _unitOfWork.Suppliers.HasProductsAsync(id);
            ThrowIf(hasProducts,
                $"Không thể xóa supplier '{supplier!.Name}' vì đang có sản phẩm liên kết");

            var hasStockTransfers = await _unitOfWork.Suppliers.HasStockTransfersAsync(id);
            ThrowIf(hasStockTransfers,
                $"Không thể xóa supplier '{supplier.Name}' vì đang có phiếu chuyển kho liên kết");

            // Delete supplier
            _unitOfWork.Suppliers.Delete(supplier);
            await _unitOfWork.SaveChangesAsync();

            // Invalidate caches
            await InvalidateMultipleCachesAsync(
                CreateCacheKey(CACHE_PREFIX_SINGLE, id),
                CreateCacheKey(CACHE_PREFIX_STATS, id)
            );
            await InvalidateCacheByPrefixAsync($"{CACHE_PREFIX_LIST}:*");

            LogInfo("✅ Xóa supplier thành công - ID: {SupplierId}, Name: {Name}",
                id, supplier.Name);

        }, "DeleteSupplierAsync");
    }

    /// <summary>
    /// Check if supplier can be deleted (no active relationships)
    /// </summary>
    public async Task<bool> CanDeleteSupplierAsync(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission (Optional for simple check, but good for security)
            await EnsurePermissionAsync(PERMISSION_MANAGE_SUPPLIERS);

            ValidateId(id, nameof(id));

            var hasProducts = await _unitOfWork.Suppliers.HasProductsAsync(id);
            var hasStockTransfers = await _unitOfWork.Suppliers.HasStockTransfersAsync(id);

            return !hasProducts && !hasStockTransfers;

        }, "CanDeleteSupplierAsync");
    }

    /// <summary>
    /// Get supplier statistics (product count, stock transfer count)
    /// </summary>
    public async Task<Dictionary<string, int>> GetSupplierStatsAsync(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            // 🔒 Check Permission
            await EnsurePermissionAsync(PERMISSION_MANAGE_SUPPLIERS);

            ValidateId(id, nameof(id));

            var cacheKey = CreateCacheKey(CACHE_PREFIX_STATS, id);

            return await GetFromCacheOrExecuteAsync(cacheKey, async () =>
            {
                var productCount = await _unitOfWork.Suppliers.GetProductCountAsync(id);
                var stockTransferCount = await _unitOfWork.Suppliers.GetStockTransferCountAsync(id);

                return new Dictionary<string, int>
                {
                    { "ProductCount", productCount },
                    { "StockTransferCount", stockTransferCount }
                };

            }, CACHE_DURATION_STATS);

        }, "GetSupplierStatsAsync");
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Helper to check user permissions
    /// </summary>
    private async Task EnsurePermissionAsync(string permissionName)
    {
        var userId = _currentUser.UserId;

        // Kiểm tra user có hợp lệ không
        ThrowIf(userId == Guid.Empty, "Không xác định được người dùng hiện tại (Unauthorized)");

        var hasPermission = await _permissionService.CheckUserPermissionAsync(userId, permissionName);

        // Nếu không có quyền thì throw
        ThrowIf(!hasPermission, $"Bạn không có quyền thực hiện hành động này ({permissionName})");

    }

    /// <summary>
    /// Generate cache key for supplier list based on pagination and filters
    /// </summary>
    private string GenerateListCacheKey(PaginationParams pagination, SupplierFilters filters)
    {
        var parts = new List<string>
        {
            pagination.Page.ToString(),
            pagination.PageSize.ToString(),
            pagination.SortBy,
            pagination.SortDescending.ToString()
        };

        // Add filter values to cache key
        if (!string.IsNullOrWhiteSpace(filters.Name))
            parts.Add($"name:{filters.Name}");

        if (!string.IsNullOrWhiteSpace(filters.Email))
            parts.Add($"email:{filters.Email}");

        if (filters.Status.HasValue)
            parts.Add($"status:{filters.Status.Value}");

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
            parts.Add($"search:{filters.SearchTerm}");

        return CreateCacheKey(CACHE_PREFIX_LIST, string.Join(":", parts));
    }

    #endregion
}