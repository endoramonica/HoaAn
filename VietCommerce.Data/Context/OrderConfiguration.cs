using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietCommerce.Core.Entities.Orders;

namespace VietCommerce.Data.Context;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        // Decimal column - precision & scale
        builder.Property(o => o.TaxAmount)
               .HasPrecision(18, 2); // hoặc .HasColumnType("decimal(18,2)")

        // Các property khác
        builder.Property(o => o.TotalAmount)
               .HasPrecision(18, 2);

        builder.Property(o => o.SubTotal)
               .HasPrecision(18, 2);

        // Các quan hệ (nếu có)
        builder.HasOne(o => o.Customer)
               .WithMany(c => c.Orders)
               .HasForeignKey(o => o.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        // Query Filter / Indexes nếu cần
    }
}
