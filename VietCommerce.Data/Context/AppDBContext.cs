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
using VietCommerce.Core.Entities.CRM;
using VietCommerce.Core.Entities.Logistics;
using VietCommerce.Core.Entities.HRM;
using VietCommerce.Core.Entities.Tasks;

namespace VietCommerce.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    #region DbSets

    // Core
    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<Store> Stores { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Permission> Permissions { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    // Customers
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<UserAddress> UserAddresses { get; set; } = null!;
    public DbSet<CustomerAddress> CustomerAddresses { get; set; } = null!;

    // Products
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductImage> ProductImages { get; set; } = null!;
    public DbSet<ProductPrice> ProductPrices { get; set; } = null!;
    public DbSet<Inventory> Inventories { get; set; } = null!;
    public DbSet<InventoryMovement> InventoryMovements { get; set; } = null!;
    public DbSet<ProductFavorite> ProductFavorites { get; set; } = null!;
    public DbSet<ProductView> ProductViews { get; set; } = null!;
    public DbSet<ProductReview> ProductReviews { get; set; } = null!;

    // Orders
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; } = null!;
    public DbSet<OrderShipping> OrderShippings { get; set; } = null!;

    // Payments
    public DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; } = null!;

    // Notifications
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; } = null!;

    // Audit
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    // Cart
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;

    // Marketing
    public DbSet<Campaign> Campaigns { get; set; } = null!;
    public DbSet<Promotion> Promotions { get; set; } = null!;
    public DbSet<PromotionProduct> PromotionProducts { get; set; } = null!;

    // CRM
    public DbSet<CRMInteraction> CRMInteractions { get; set; } = null!;

    // Logistics
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<StockTransfer> StockTransfers { get; set; } = null!;
    public DbSet<TransferItem> TransferItems { get; set; } = null!;

    // HRM
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<PerformanceMetric> PerformanceMetrics { get; set; } = null!;
    public DbSet<LeaveRequest> LeaveRequests { get; set; } = null!;
    public DbSet<WorkSchedule> WorkSchedules { get; set; } = null!;
    public DbSet<Shift> Shifts { get; set; } = null!;

    // Tasks
    public DbSet<WorkTask> Tasks { get; set; } = null!;

    #endregion

    #region OnModelCreating

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply global soft delete filter
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(ISoftDelete).IsAssignableFrom(t.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .HasQueryFilter(GetSoftDeleteFilter(entityType.ClrType));
        }

        // Configure relationships
        ConfigureTenantEntities(modelBuilder);
        ConfigureStoreEntities(modelBuilder);
        ConfigureUserEntities(modelBuilder);
        ConfigureRolePermissionEntities(modelBuilder);
        ConfigureCustomerEntities(modelBuilder);
        ConfigureCategoryEntities(modelBuilder);
        ConfigureProductEntities(modelBuilder);
        ConfigureInventoryEntities(modelBuilder);
        ConfigureOrderEntities(modelBuilder);
        ConfigurePaymentEntities(modelBuilder);
        ConfigureCartEntities(modelBuilder);
        ConfigureMarketingEntities(modelBuilder);
        ConfigureNotificationEntities(modelBuilder);
        ConfigureCRMEntities(modelBuilder);
        ConfigureLogisticsEntities(modelBuilder);
        ConfigureHRMEntities(modelBuilder);
        ConfigureShiftEntities(modelBuilder);
        ConfigureTaskEntities(modelBuilder);
        ConfigureAuditEntities(modelBuilder);

        // Composite Keys
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                 .IsRequired(false); // 

            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        });

        modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
        modelBuilder.Entity<PromotionProduct>().HasKey(pp => new { pp.PromotionId, pp.ProductId });

        // Configure many-to-many relationships
        ConfigureManyToManyRelationships(modelBuilder);

        // Precision configurations
        ConfigurePrecisions(modelBuilder);

        // Performance indexes
        ConfigurePerformanceIndexes(modelBuilder);

        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Order>().Ignore(o => o.CreatedBy);
    }

    #endregion

    #region Configuration Methods

    private void ConfigureTenantEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            // ========== STORES ==========
            entity.HasMany(t => t.Stores)
                .WithOne(s => s.Tenant)
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== CUSTOMERS ==========
            entity.HasMany(t => t.Customers)
                .WithOne(c => c.Tenant)
                .HasForeignKey(c => c.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureStoreEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Store>(entity =>
        {
            // ========== INDEXES ==========
            entity.HasIndex(s => s.Id).IsUnique();
            entity.HasIndex(s => s.Name);

            // ========== TENANT ==========
            entity.HasOne(s => s.Tenant)
                .WithMany(t => t.Stores)
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== USERS ==========
            entity.HasMany(s => s.Users)
                .WithOne(u => u.Store)
                .HasForeignKey(u => u.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== CUSTOMERS ==========
            entity.HasMany(s => s.Customers)
                .WithOne(c => c.Store)
                .HasForeignKey(c => c.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== CATEGORIES ==========
            entity.HasMany(s => s.Categories)
                .WithOne(c => c.Store)
                .HasForeignKey(c => c.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== PRODUCTS ==========
            entity.HasMany(s => s.Products)
                .WithOne(p => p.Store)
                .HasForeignKey(p => p.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== INVENTORIES ==========
            entity.HasMany(s => s.Inventories)
                .WithOne(i => i.Store)
                .HasForeignKey(i => i.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== ORDERS ==========
            entity.HasMany(s => s.Orders)
                .WithOne(o => o.Store)
                .HasForeignKey(o => o.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== CAMPAIGNS ==========
            entity.HasMany(s => s.Campaigns)
                .WithOne(c => c.Store)
                .HasForeignKey(c => c.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            // ========== EMPLOYEES ==========
            entity.HasMany(s => s.Employees)
                .WithOne(e => e.Store)
                .HasForeignKey(e => e.StoreId)
                .OnDelete(DeleteBehavior.SetNull);

            // ========== SHIFTS ==========
            entity.HasMany(s => s.Shifts)
                .WithOne(sh => sh.Store)
                .HasForeignKey(sh => sh.StoreId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureUserEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email)
                .IsUnique()
                .HasFilter("IsDeleted = 0");

            // Self-referencing relationship for Manager
            entity.HasOne(u => u.Manager)
                .WithMany(u => u.Subordinates)
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.UserAddresses)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Carts)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.CreatedOrders)
                .WithOne(o => o.CreatedByUser)
                .HasForeignKey(o => o.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.InventoryMovements)
                .WithOne(im => im.PerformedBy)
                .HasForeignKey(im => im.PerformedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.AuditLogs)
                .WithOne(al => al.User)
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.CreatedPrices)
                .WithOne(pp => pp.Creator)
                .HasForeignKey(pp => pp.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.CreatedCampaigns)
                .WithOne(c => c.Creator)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.CRMInteractions)
                .WithOne(ci => ci.CreatedByUser)
                .HasForeignKey(ci => ci.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureRolePermissionEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasMany(r => r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.RolePermissions)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Permission>()
            .HasMany(p => p.RolePermissions)
            .WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureCustomerEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            // ========== INDEXES ==========
            entity.HasIndex(c => c.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL AND [IsDeleted] = 0");

            entity.HasIndex(c => c.Phone);

            entity.HasIndex(c => new { c.StoreId, c.CreatedAt }); 
            // ========== TENANT RELATIONSHIP (REQUIRED) ==========
            entity.HasOne(c => c.Tenant)
                .WithMany(t => t.Customers)
                .HasForeignKey(c => c.TenantId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            // ========== STORE RELATIONSHIP (REQUIRED) ==========
            entity.HasOne(c => c.Store)
                .WithMany(s => s.Customers)
                .HasForeignKey(c => c.StoreId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            // ========== USER RELATIONSHIP (OPTIONAL) ==========
            entity.HasOne(c => c.User)
                .WithMany() // User không có navigation collection về Customer
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
            // ========== CHILD COLLECTIONS ==========
            entity.HasMany(c => c.Addresses)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(c => c.Carts)
                .WithOne(cart => cart.Customer)
                .HasForeignKey(cart => cart.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(c => c.Interactions)
                .WithOne(i => i.Customer)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        });

        // ========== CUSTOMER ADDRESS ==========
        modelBuilder.Entity<CustomerAddress>(entity =>
        {
            entity.HasOne(ca => ca.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(ca => ca.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureCategoryEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasMany(c => c.SubCategories)
                .WithOne(c => c.ParentCategory)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureProductEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(p => p.Code).IsUnique();
            entity.HasIndex(p => p.Slug).IsUnique();
            entity.HasIndex(p => new { p.CategoryId, p.IsDeleted });
            entity.HasIndex(p => p.SKU)
                .IsUnique()
                .HasFilter("IsDeleted = 0");

            entity.HasMany(p => p.Images)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.Inventories)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.Prices)
                .WithOne(pp => pp.Product)
                .HasForeignKey(pp => pp.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.PromotionProducts)
                .WithOne(pp => pp.Product)
                .HasForeignKey(pp => pp.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.TransferItems)
                .WithOne(ti => ti.Product)
                .HasForeignKey(ti => ti.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private void ConfigureInventoryEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasIndex(i => new { i.ProductId, i.StoreId })
                .IsUnique()
                .HasFilter("IsDeleted = 0");

            entity.HasMany(i => i.InventoryMovements)
                .WithOne(im => im.Inventory)
                .HasForeignKey(im => im.InventoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.HasOne(im => im.Order)
                .WithMany(o => o.InventoryMovements)
                .HasForeignKey(im => im.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(im => im.Transfer)
                .WithMany(t => t.InventoryMovements)
                .HasForeignKey(im => im.TransferId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private void ConfigureOrderEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            // ========== INDEXES ==========
            entity.HasIndex(o => o.OrderNumber)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            entity.HasIndex(o => new { o.StoreId, o.CreatedAt });
            entity.HasIndex(o => new { o.CustomerId, o.IsDeleted });

            // ========== STORE RELATIONSHIP (REQUIRED) ==========
            entity.HasOne(o => o.Store)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StoreId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // ========== CUSTOMER RELATIONSHIP (OPTIONAL) ==========
            entity.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // ========== CREATED BY USER (OPTIONAL) ==========
            entity.HasOne(o => o.CreatedByUser)
                .WithMany(u => u.CreatedOrders)
                .HasForeignKey(o => o.CreatedById)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // ========== CHILD COLLECTIONS ==========
            entity.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(o => o.Payments)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(o => o.OrderStatusHistories)
                .WithOne(osh => osh.Order)
                .HasForeignKey(osh => osh.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(o => o.InventoryMovements)
                .WithOne(im => im.Order)
                .HasForeignKey(im => im.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            // ========== ONE-TO-ONE: ORDER SHIPPING ==========
            entity.HasOne(o => o.OrderShipping)
                .WithOne(os => os.Order)
                .HasForeignKey<OrderShipping>(os => os.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigurePaymentEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity
                .HasIndex(p => new { p.OrderId, p.Status });
                
            entity.HasOne(p => p.PaymentMethod)
                .WithMany(pm => pm.Payments)
                .HasForeignKey(p => p.MethodId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.PaymentTransactions)
                .WithOne(pt => pt.Payment)
                .HasForeignKey(pt => pt.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureCartEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Carts");

            // User relationship (Authenticated users)
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Customer relationship (Optional)
            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // CartItems relationship
            entity.HasMany(e => e.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Properties
            entity.Property(e => e.SessionId)
                .HasMaxLength(450)
                .IsRequired(false);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

        });
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("CartItems");

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            entity.Property(e => e.Quantity)
                .IsRequired();

           
        });


    }

    private void ConfigureMarketingEntities(ModelBuilder modelBuilder)
    {
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
    }

    private void ConfigureNotificationEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            
            entity.HasIndex(n => new { n.UserId, n.Read });
        });

        modelBuilder.Entity<NotificationTemplate>()
            .HasMany(nt => nt.Notifications)
            .WithOne(n => n.Template)
            .HasForeignKey(n => n.TemplateId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureCRMEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CRMInteraction>(entity =>
        {
            entity.HasOne(i => i.Customer)
                .WithMany(c => c.Interactions)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
        });
    }

    private void ConfigureLogisticsEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockTransfer>(entity =>
        {
            // 1️ StockTransfer ↔ TransferItem (1-nhiều)
            entity.HasMany(st => st.TransferItems)
                .WithOne(ti => ti.StockTransfer)
                .HasForeignKey(ti => ti.StockTransferId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2️ StockTransfer.RequestedByUser ↔ User.RequestedTransfers (1-nhiều)
            entity.HasOne(st => st.RequestedByUser)
                .WithMany(u => u.RequestedTransfers)
                .HasForeignKey(st => st.RequestedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // 3️1 StockTransfer.ApprovedByUser ↔ User.ApprovedTransfers (1-nhiều)
            entity.HasOne(st => st.ApprovedByUser)
                .WithMany(u => u.ApprovedTransfers)
                .HasForeignKey(st => st.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // 4️⃣ StockTransfer.Supplier ↔ Supplier.StockTransfers (1-nhiều)
            entity.HasOne(st => st.Supplier)
                .WithMany(s => s.StockTransfers)
                .HasForeignKey(st => st.SupplierId)
                .OnDelete(DeleteBehavior.SetNull);
        });

    }

    private void ConfigureHRMEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

           // Self-referencing relationship for Manager
            entity.HasOne(e => e.Manager)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1–1 Employee ↔ User
            entity.HasOne(e => e.User)
                .WithOne(u => u.EmployeeProfile)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Performance)
                .WithOne(pm => pm.Employee)
                .HasForeignKey(pm => pm.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.LeaveRequests)
                .WithOne(lr => lr.Employee)
                .HasForeignKey(lr => lr.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.WorkSchedules)
                .WithOne(ws => ws.Employee)
                .HasForeignKey(ws => ws.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PerformanceMetric>()
            .HasIndex(pm => new { pm.EmployeeId, pm.Period });

        modelBuilder.Entity<LeaveRequest>()
            .HasOne(lr => lr.ApprovedByEmployee)
            .WithMany()
            .HasForeignKey(lr => lr.ApprovedBy)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<WorkSchedule>()
            .HasIndex(ws => new { ws.EmployeeId, ws.Date });
    }

    private void ConfigureShiftEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasIndex(s => new { s.UserId, s.StartTime });

            entity.HasOne(s => s.Staff)
                .WithMany(u => u.Shifts)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureTaskEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkTask>(entity =>
        {
            

            entity.HasOne(t => t.AssignedToUser)
                 .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedTo)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.AssignedByUser)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.AssignedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureAuditEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>()
            .HasIndex(al => new { al.UserId, al.Timestamp });
    }

    private void ConfigureManyToManyRelationships(ModelBuilder modelBuilder)
    {
        // Product-Supplier many-to-many relationship
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Suppliers)
            .WithMany(s => s.Products)
            .UsingEntity<Dictionary<string, object>>(
                "ProductSuppliers",
                j => j.HasOne<Supplier>().WithMany().HasForeignKey("SupplierId"),
                j => j.HasOne<Product>().WithMany().HasForeignKey("ProductId"),
                j =>
                {
                    j.HasKey("ProductId", "SupplierId");
                    j.ToTable("ProductSuppliers");
                });
    }

    private void ConfigurePrecisions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductPrice>()
            .Property(pp => pp.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PromotionProduct>()
            .Property(pp => pp.DiscountOverride)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
            entity.Property(oi => oi.TotalPrice).HasPrecision(18, 2);
        });

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
    }

    private void ConfigurePerformanceIndexes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasIndex(p => new { p.StoreId, p.IsDeleted });

        modelBuilder.Entity<Order>()
            .HasIndex(o => new { o.CustomerId, o.IsDeleted });

        modelBuilder.Entity<Inventory>()
            .HasIndex(i => new { i.StoreId, i.IsDeleted });
        // Product
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Name); // Tìm kiếm theo tên

        // Customer  
        modelBuilder.Entity<Customer>()
            .HasIndex(c => new { c.StoreId, c.CreatedAt }); // Báo cáo khách hàng mới

        // Employee
        modelBuilder.Entity<Employee>()
            .HasIndex(e => new { e.StoreId, e.Status }); // Lọc nhân viên theo trạng thái
    // Cần bổ sung:
modelBuilder.Entity<Product>()
    .HasIndex(p => new { p.CategoryId, p.IsDeleted });

modelBuilder.Entity<Payment>()
    .HasIndex(p => new { p.OrderId, p.Status });

modelBuilder.Entity<InventoryMovement>()
    .HasIndex(im => new { im.InventoryId, im.CreatedAt });

modelBuilder.Entity<OrderStatusHistory>()
    .HasIndex(osh => new { osh.OrderId, osh.CreatedAt });
    }

    #endregion

    #region SoftDelete Filter

    private static LambdaExpression GetSoftDeleteFilter(Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "e");
        var prop = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
        var body = Expression.Equal(prop, Expression.Constant(false));
        return Expression.Lambda(body, parameter);
    }

    public override int SaveChanges()
    {
        ProcessSoftDelete();
        UpdateOrderItemTotalPrices();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ProcessSoftDelete();
        await UpdateOrderItemTotalPricesAsync();
        await GenerateOrderNumbersAsync(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ProcessSoftDelete()
    {
        foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
            }
        }
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

    foreach (var order in newOrders)
    {
        var currentTime = DateTime.UtcNow;
        var datePart = currentTime.ToString("yyyyMMdd");

        string orderNumber;
        bool exists;
        do
        {
            var uniqueId = Guid.NewGuid().ToString("N")[..8].ToUpper();
            orderNumber = $"ORD{datePart}-{uniqueId}";
            exists = await Orders.AnyAsync(o => o.OrderNumber == orderNumber, cancellationToken);
        } while (exists);

        order.OrderNumber = orderNumber;
    }
}
    #endregion
}