using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using VietCommerce.Core.Entities.Orders;
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
    private ICartRepository? _cartRepository;
    private IOrderRepository? _orders;
    private IOrderItemRepository? _orderItems;
    private readonly ILogger<OrderRepository> _logger;
    private IOrderShippingRepository? _orderShippings;
    private IOrderStatusHistoryRepository? _orderStatusHistories;
    private ICustomerRepository? _customer;

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

    public ICustomerRepository Customers => _customer ??= new CustomerRepository(_context);

    

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

