using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VietCommerce.Core.Common;
using VietCommerce.Core.Entities.Audit;
using VietCommerce.Core.Entities.Customers;
using VietCommerce.Core.Entities.Marketing;
using VietCommerce.Core.Entities.Notifications;
using VietCommerce.Core.Entities.Orders;
using VietCommerce.Core.Entities.Organization;
using VietCommerce.Core.Entities.Payments;
using VietCommerce.Core.Entities.Products;
using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<Store> Stores { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<UserAddress> UserAddresses { get; set; } = null!;
    public DbSet<CustomerAddress> CustomerAddresses { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductImage> ProductImages { get; set; } = null!;
    public DbSet<ProductPrice> ProductPrices { get; set; } = null!;
    public DbSet<Inventory> Inventories { get; set; } = null!;
    public DbSet<InventoryMovement> InventoryMovements { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; } = null!;
    public DbSet<OrderShipping> OrderShippings { get; set; } = null!;
    public DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<Campaign> Campaigns { get; set; } = null!;
    public DbSet<Promotion> Promotions { get; set; } = null!;
    public DbSet<PromotionProduct> PromotionProducts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply global query filter for soft delete to all entities implementing ISoftDelete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(ISoftDelete).IsAssignableFrom(t.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(GetSoftDeleteFilter(entityType.ClrType));
        }

        // -----------------
        // TENANT RELATIONSHIPS
        // -----------------
        modelBuilder.Entity<Tenant>()
            .HasMany(t => t.Stores)
            .WithOne(s => s.Tenant)
            .HasForeignKey(s => s.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // NOTE: removed Tenant <-> Users / Tenant <-> Products / Tenant <-> Orders many-to-many mappings
        // because these are redundant (Store -> Tenant, Product -> Store, Order -> Store). Keeping them
        // can create cycles and redundant data. If you truly need direct Tenant<->User mapping, add
        // an explicit entity like TenantUser and navigation properties on both sides.

       

       

       

        // -----------------
        // STORE RELATIONSHIPS
        // -----------------
        modelBuilder.Entity<Store>()
            .HasMany(s => s.Users)
            .WithOne(u => u.Store)
            .HasForeignKey(u => u.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Store>()
            .HasMany(s => s.Customers)
            .WithOne(c => c.Store)
            .HasForeignKey(c => c.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Store>()
            .HasMany(s => s.Categories)
            .WithOne(c => c.Store)
            .HasForeignKey(c => c.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Store>()
            .HasMany(s => s.Products)
            .WithOne(p => p.Store)
            .HasForeignKey(p => p.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Store>()
            .HasMany(s => s.Inventories)
            .WithOne(i => i.Store)
            .HasForeignKey(i => i.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Store>()
            .HasMany(s => s.Orders)
            .WithOne(o => o.Store)
            .HasForeignKey(o => o.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        // Stores often should not cascade-delete addresses across the DB because that can lead
        // to unwanted deletes in multi-tenant scenarios; use Restrict so admin explicitly handles removal.
        modelBuilder.Entity<Store>()
            .HasMany(s => s.Addresses)
            .WithOne(a => a.Store)
            .HasForeignKey(a => a.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Store>()
            .HasMany(s => s.Campaigns)
            .WithOne(c => c.Store)
            .HasForeignKey(c => c.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        // -----------------
        // USER RELATIONSHIPS
        // -----------------
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("IsDeleted = 0");

        modelBuilder.Entity<User>()
            .HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Keep single Addresses navigation to avoid duplication. If your entity class is named
        // UserAddress the DbSet can remain, but navigation property on User should be Addresses.
        modelBuilder.Entity<User>()
            .HasMany(u => u.Addresses)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Notifications)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Carts)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.CreatedOrders)
            .WithOne(o => o.CreatedByUser)
            .HasForeignKey(o => o.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.InventoryMovements)
            .WithOne(im => im.PerformedBy)
            .HasForeignKey(im => im.PerformedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.AuditLogs)
            .WithOne(al => al.User)
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.CreatedPrices)
            .WithOne(pp => pp.Creator)
            .HasForeignKey(pp => pp.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.CreatedCampaigns)
            .WithOne(c => c.Creator)
            .HasForeignKey(c => c.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // -----------------
        // ROLE & PERMISSION RELATIONSHIPS
        // -----------------
        modelBuilder.Entity<Role>()
            .HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Role>()
            .HasMany(r => r.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Permission>()
            .HasMany(p => p.RolePermissions)
            .WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        // -----------------
        // COMPOSITE KEYS
        // -----------------
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<PromotionProduct>()
            .HasKey(pp => new { pp.PromotionId, pp.ProductId });

        // -----------------
        // CUSTOMER RELATIONSHIPS
        // -----------------
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Email)
            .IsUnique()
            .HasFilter("Email IS NOT NULL AND IsDeleted = 0");

        modelBuilder.Entity<Customer>()
            .HasOne(c => c.User)
            .WithMany() // No direct navigation from User to Customer
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Addresses)
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Carts)
            .WithOne(c => c.Customer)
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // CUSTOMER ADDRESS RELATIONSHIPS
        modelBuilder.Entity<CustomerAddress>()
            .HasOne(ca => ca.Customer)
            .WithMany(c => c.Addresses)
            .HasForeignKey(ca => ca.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Tenant navigation here should not point to Tenant.Customers (that is a different semantic)
        modelBuilder.Entity<CustomerAddress>()
            .HasOne(ca => ca.Tenant)
            .WithMany() // changed from .WithMany(t => t.Customers) to avoid mismatched navigation
            .HasForeignKey(ca => ca.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // -----------------
        // CATEGORY RELATIONSHIPS
        // -----------------
        modelBuilder.Entity<Category>()
            .HasMany(c => c.SubCategories)
            .WithOne(c => c.ParentCategory)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // -----------------
        // PRODUCT RELATIONSHIPS
        // -----------------
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SKU)
            .IsUnique()
            .HasFilter("IsDeleted = 0");

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Images)
            .WithOne(pi => pi.Product)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Change to Restrict to avoid multiple cascade paths (Product -> Inventory -> InventoryMovements)
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Inventories)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.OrderItems)
            .WithOne(oi => oi.Product)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.CartItems)
            .WithOne(ci => ci.Product)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Prices)
            .WithOne(pp => pp.Product)
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.PromotionProducts)
            .WithOne(pp => pp.Product)
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // We deliberately do NOT map Product -> InventoryMovements to avoid multiple cascade paths.

        // -----------------
        // INVENTORY RELATIONSHIPS
        // -----------------
        modelBuilder.Entity<Inventory>()
            .HasMany(i => i.InventoryMovements)
            .WithOne(im => im.Inventory)
            .HasForeignKey(im => im.InventoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Inventory>()
            .HasIndex(i => new { i.ProductId, i.StoreId })
            .IsUnique()
            .HasFilter("IsDeleted = 0");

        // -----------------
        // ORDER RELATIONSHIPS
        // -----------------
        modelBuilder.Entity<Order>()
            .HasIndex(o => o.OrderNumber)
            .IsUnique()
            .HasFilter("IsDeleted = 0");

        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Payments)
            .WithOne(p => p.Order)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderStatusHistories)
            .WithOne(osh => osh.Order)
            .HasForeignKey(osh => osh.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.OrderShipping)
            .WithOne(os => os.Order)
            .HasForeignKey<OrderShipping>(os => os.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // -----------------
        // PAYMENT RELATIONSHIPS
        // -----------------
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.PaymentMethod)
            .WithMany(pm => pm.Payments)
            .HasForeignKey(p => p.MethodId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasMany(p => p.PaymentTransactions)
            .WithOne(pt => pt.Payment)
            .HasForeignKey(pt => pt.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        // -----------------
        // CART RELATIONSHIPS
        // -----------------
        modelBuilder.Entity<Cart>()
            .HasMany(c => c.CartItems)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Cart>()
            .HasIndex(c => c.UserId)
            .IsUnique()
            .HasFilter("UserId IS NOT NULL AND IsDeleted = 0");

        modelBuilder.Entity<Cart>()
            .HasIndex(c => c.CustomerId)
            .IsUnique()
            .HasFilter("CustomerId IS NOT NULL AND IsDeleted = 0");

        // -----------------
        // MARKETING RELATIONSHIPS
        // -----------------
        modelBuilder.Entity<Campaign>()
            .HasMany(c => c.Promotions)
            .WithOne(p => p.Campaign)
            .HasForeignKey(p => p.CampaignId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Promotion>()
            .HasMany(p => p.PromotionProducts)
            .WithOne(pp => pp.Promotion)
            .HasForeignKey(pp => pp.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);

        // -----------------
        // NOTIFICATION RELATIONSHIPS
        // -----------------
        modelBuilder.Entity<NotificationTemplate>()
            .HasMany(nt => nt.Notifications)
            .WithOne(n => n.Template)
            .HasForeignKey(n => n.TemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        // -----------------
        // PRODUCT PRICE RELATIONSHIPS
        // -----------------
        modelBuilder.Entity<ProductPrice>()
            .HasOne(pp => pp.Creator)
            .WithMany(u => u.CreatedPrices)
            .HasForeignKey(pp => pp.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // -----------------
        // PRECISION CONFIGURATIONS
        // -----------------
        modelBuilder.Entity<ProductPrice>()
            .Property(pp => pp.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PromotionProduct>()
            .Property(pp => pp.DiscountOverride)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.TotalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PaymentTransaction>()
            .Property(pt => pt.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderShipping>()
            .Property(os => os.ShippingCost)
            .HasPrecision(18, 2);

        // Additional price/amount fields you may have should be configured similarly.

        // -----------------
        // INDEXES FOR PERFORMANCE
        // -----------------
        modelBuilder.Entity<Product>()
            .HasIndex(p => new { p.StoreId, p.IsDeleted });

        modelBuilder.Entity<Order>()
            .HasIndex(o => new { o.StoreId, o.CreatedAt, o.IsDeleted });

        modelBuilder.Entity<Order>()
            .HasIndex(o => new { o.CustomerId, o.IsDeleted });

        modelBuilder.Entity<AuditLog>()
            .HasIndex(al => new { al.UserId, al.Timestamp });

        modelBuilder.Entity<Inventory>()
            .HasIndex(i => new { i.StoreId, i.IsDeleted });

        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.UserId, n.Read });

        base.OnModelCreating(modelBuilder);
    }

    private static LambdaExpression GetSoftDeleteFilter(Type type)
    {
        var parameter = Expression.Parameter(type, "e");
        var property = Expression.Property(parameter, "IsDeleted");
        var constant = Expression.Constant(false);
        var body = Expression.Equal(property, constant);
        return Expression.Lambda(body, parameter);
    }

    public override int SaveChanges()
    {
        UpdateOrderItemTotalPrices();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await UpdateOrderItemTotalPricesAsync();
        await GenerateOrderNumbersAsync(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateOrderItemTotalPrices()
    {
        var entries = ChangeTracker.Entries<OrderItem>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var item = entry.Entity;
            if (item.Quantity <= 0)
                throw new InvalidOperationException($"OrderItem Quantity must be greater than 0. Current value: {item.Quantity}");
            if (item.UnitPrice < 0)
                throw new InvalidOperationException($"OrderItem UnitPrice cannot be negative. Current value: {item.UnitPrice}");
            item.TotalPrice = item.Quantity * item.UnitPrice;
        }
    }

    private async Task UpdateOrderItemTotalPricesAsync()
    {
        var entries = ChangeTracker.Entries<OrderItem>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var item = entry.Entity;
            if (item.Quantity <= 0)
                throw new InvalidOperationException($"OrderItem Quantity must be greater than 0. Current value: {item.Quantity}");
            if (item.UnitPrice < 0)
                throw new InvalidOperationException($"OrderItem UnitPrice cannot be negative. Current value: {item.UnitPrice}");
            item.TotalPrice = item.Quantity * item.UnitPrice;
        }
        await Task.CompletedTask;
    }

    private async Task GenerateOrderNumbersAsync(CancellationToken cancellationToken)
    {
        var newOrders = ChangeTracker.Entries<Order>()
            .Where(e => e.State == EntityState.Added && string.IsNullOrEmpty(e.Entity.OrderNumber))
            .Select(e => e.Entity)
            .ToList();

        if (!newOrders.Any()) return;

        var currentTime = DateTime.UtcNow;
        var datePart = currentTime.ToString("yyyyMMdd");
        var timePart = currentTime.ToString("HHmmss");
        var todayStart = currentTime.Date;
        var todayEnd = todayStart.AddDays(1);

        var existingOrdersToday = await Orders
            .Where(o => o.CreatedAt >= todayStart && o.CreatedAt < todayEnd)
            .CountAsync(cancellationToken);

        for (int i = 0; i < newOrders.Count; i++)
        {
            var order = newOrders[i];
            var sequenceNumber = existingOrdersToday + i + 1;
            var randomSuffix = Random.Shared.Next(100, 999);
            order.OrderNumber = $"ORD{datePart}-{timePart}-{sequenceNumber:D3}-{randomSuffix}";
        }
    }
}
