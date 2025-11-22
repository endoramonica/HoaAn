using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IUserRepository? _users;
    private IRefreshTokenRepository? _refreshTokens;
    private IRoleRepository? _roles;
    private IPermissionRepository? _permissions;
    private IUserRoleRepository? _userRoles;
    private IRolePermissionRepository? _rolePermissions;
    private IProductRepository? _products;
    private ICategoryRepository? _categories;
    private ICartRepository? _cartRepository;
    private IOrderRepository? _orders;
    private IOrderItemRepository? _orderItems;
    private readonly ILogger<OrderRepository> _logger;
    private IOrderShippingRepository? _orderShippings;
    private IOrderStatusHistoryRepository? _orderStatusHistories;
    private ICustomerRepository? _customer;
    private IProductImageRepository? _productImages;
    private IInventoryRepository? _inventories;
    private IInventoryMovementRepository? _inventoryMovements;
    private IProductFavoriteRepository _productFavorites;
    private ICustomerAddressRepository? _customerAddressRepository;
    private ICRMInteractionRepository? _crmInteractionRepository;
    private IPaymentRepository? _payments;
    private IPaymentMethodRepository? _paymentMethods;
    private ISupplierRepository? _supplierRepository;
    private IStockTransferRepository? _stockTransfers;
    private ITransferItemRepository? _transferItems;
    private ILeaveRequestRepository? _leaveRequests;
    private IEmployeeRepository? _employee;
    private IWorkScheduleRepository? _workSchedule;
    private IShiftRepository? _shift;
    public INotificationRepository? _notifications;
    public INotificationTemplateRepository? _notificationTemplates;
    public ITaskRepository? _tasks;



    private IDbContextTransaction? _transaction;
    public UnitOfWork(AppDbContext context , ILogger<OrderRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);
    public IRoleRepository Roles => _roles ??= new RoleRepository(_context);
    public IPermissionRepository Permissions => _permissions ??= new PermissionRepository(_context);
    public IUserRoleRepository UserRoles => _userRoles ??= new UserRoleRepository(_context);
    public IRolePermissionRepository RolePermissions => _rolePermissions ??= new RolePermissionRepository(_context);
    public IProductRepository Products => _products ??= new ProductRepository(_context);
    public IProductFavoriteRepository ProductFavorites =>
        _productFavorites ??= new ProductFavoriteRepository(_context);
    public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
    public ICartRepository Carts
    {
        get => _cartRepository ??= new CartRepository(_context);
    }

    public IGenericRepository<CartItem> CartItems => throw new NotImplementedException();
    public IOrderRepository Orders => _orders ??= new OrderRepository(_context , _logger);
    public IOrderItemRepository OrderItems => _orderItems ??= new OrderItemRepository(_context);
    private IOrderShippingRepository _orderShipping;
    public IOrderShippingRepository OrderShipping => _orderShippings ??= new OrderShippingRepository(_context, null!);
    public IOrderStatusHistoryRepository OrderStatusHistories => _orderStatusHistories ??= new OrderStatusHistoryRepository(_context, null!);
    public IProductImageRepository ProductImages => _productImages ??= new ProductImageRepository(_context);
    public IInventoryRepository Inventories => _inventories ??= new InventoryRepository(_context);
    public IInventoryMovementRepository InventoryMovements => _inventoryMovements ??= new InventoryMovementRepository(_context);
    public ICustomerRepository Customers => _customer ??= new CustomerRepository(_context);
    public ICustomerAddressRepository CustomerAddresses => _customerAddressRepository ??= new CustomerAddressRepository(_context);
    public IPaymentRepository Payments =>
            _payments ??= new PaymentRepository(_context);

    public IPaymentMethodRepository PaymentMethods =>
        _paymentMethods ??= new PaymentMethodRepository(_context);

    public ICRMInteractionRepository CRMInteractions => 
    _crmInteractionRepository ??= new CRMInteractionRepository(_context);
    public ISupplierRepository Suppliers => 
    _supplierRepository ??= new SupplierRepository(_context);
    public IStockTransferRepository StockTransfers => 
    _stockTransfers ??= new StockTransferRepository (_context);
    public ITransferItemRepository TransferItems => 
    _transferItems ??= new TransferItemRepository(_context);
    public IEmployeeRepository Employees => 
    _employee ??= new EmployeeRepository(_context);
    public IWorkScheduleRepository WorkSchedules => 
    _workSchedule ??= new WorkScheduleRepository(_context);
    public IShiftRepository Shifts => 
    _shift ??= new ShiftRepository(_context);
    public ILeaveRequestRepository LeaveRequests => 
    _leaveRequests ??= new LeaveRequestRepository(_context);

    public INotificationRepository Notifications =>
        _notifications ??= new NotificationRepository(_context);
    public INotificationTemplateRepository NotificationTemplates =>
        _notificationTemplates ??= new NotificationTemplateRepository(_context);
    public ITaskRepository Tasks =>
        _tasks ??= new TaskRepository(_context);




    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
        return _transaction;
    }


    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            await _transaction?.CommitAsync()!;
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            await _transaction?.RollbackAsync()!;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context?.Dispose();
    }
}

