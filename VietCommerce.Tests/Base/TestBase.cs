using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories;
using VietCommerce.Data.Repositories.Interfaces;
using VietCommerce.Tests.SeedData;

namespace VietCommerce.Tests.Base;

/// <summary>
/// Base class cho tất cả repository test.
/// Tạo DbContext InMemory và seed dữ liệu mẫu.
/// </summary>
public abstract class TestBase : IDisposable
{
    protected readonly AppDbContext _context;
    protected readonly IProductRepository _productRepository;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly ILogger<OrderRepository> _logger;

    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // mỗi test có DB riêng
            .Options;

        _context = new AppDbContext(options);
        _productRepository = new ProductRepository(_context);
        
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        
        _logger = serviceProvider.GetRequiredService<ILogger<OrderRepository>>();
        _unitOfWork = new UnitOfWork(_context, _logger);

        // ✅ Seed dữ liệu mẫu
        TestDataSeeder.SeedAsync(_context).Wait();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
