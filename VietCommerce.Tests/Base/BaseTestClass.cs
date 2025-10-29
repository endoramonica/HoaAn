using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Users;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories;
using Xunit;

namespace VietCommerce.Data.Tests.Base
{
    public abstract class BaseTestClass : IAsyncLifetime
    {
        protected readonly AppDbContext _context;
        protected readonly OrderRepository _orderRepository;
        protected readonly IServiceProvider _serviceProvider;

        // Test data IDs
        protected readonly Guid _storeId = Guid.NewGuid();
        protected readonly Guid _customerId = Guid.NewGuid();
        protected readonly Guid _userId = Guid.NewGuid();
        protected readonly Guid _productId = Guid.NewGuid();
        protected readonly Guid _orderId = Guid.NewGuid();
        protected readonly Guid _productId1 = Guid.NewGuid();


        protected BaseTestClass()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new AppDbContext(options);
            _orderRepository = new OrderRepository(_context);
            _serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();
        }

        public virtual async Task InitializeAsync()
        {
            await SeedTestDataAsync();
        }

        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        protected async Task SeedTestDataAsync()
        {
            var store = new Store
            {
                Id = _storeId,
                Name = "Test Store",
                Phone = "0123456789",
                Address = "123 Test St",
                IsActive = true
            };

            var customer = new Customer
            {
                Id = _customerId,
                Name = "Test Customer",
                Email = "customer@test.com",
                Phone = "0987654321",
                StoreId = store.Id,
                IsActive = true
            };

            var user = new User
            {
                Id = _userId,
                Email = "user@test.com",
                Name = "Test User",
                PasswordHash = "hash",
                IsActive = true
            };

            var product = new Product
            {
                Id = _productId,
                Name = "Test Product",
                Code = "PROD-001",
                Slug = "test-product",
                SKU = "TEST-SKU-001",
                StoreId = _storeId,
                CategoryId = Guid.NewGuid(),
                Stock = 100,
                IsActive = true,
                ViewCount = 0,
                FavoriteCount = 0,
                PurchaseCount = 0,
                ReviewCount = 0,
                AvgRating = 0
            };

            var price = new ProductPrice
            {
                Id = Guid.NewGuid(),
                ProductId = _productId,
                PriceType = VietCommerce.Core.Enums.Products.PriceType.REGULAR,
                Price = 199000m,
                EffectiveFrom = DateTime.UtcNow,
                EffectiveTo = null,
                IsActive = true,
                CreatedBy = _userId,
                UpdatedBy = _userId
            };

            _context.Stores.Add(store);
            _context.Customers.Add(customer);
            _context.Users.Add(user);
            _context.Products.Add(product);
            _context.ProductPrices.Add(price);

            await _context.SaveChangesAsync();
        }

    }
}