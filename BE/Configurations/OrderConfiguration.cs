using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietCommerce.Core.Entities.Orders;

namespace VietCommerce.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Subtotal)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.TaxAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.ShippingAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.Notes)
            .HasMaxLength(500);

        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Store)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.StoreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.OrderShipping)
            .WithOne(os => os.Order)
            .HasForeignKey<OrderShipping>(os => os.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}