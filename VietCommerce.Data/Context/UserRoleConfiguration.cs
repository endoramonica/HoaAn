// ================================================================
// FILE: UserRoleConfiguration.cs
// Author: VietCommerce Team
// Purpose: Cấu hình Entity UserRole theo convention
// ================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VietCommerce.Core.Entities.Users;

namespace VietCommerce.Data.Context
{
    /// <summary>
    /// Cấu hình quan hệ và ràng buộc cho UserRole
    /// </summary>
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            // Composite Primary Key
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            // UserId → FK tới User
            builder.HasOne(ur => ur.User)
                   .WithMany(u => u.UserRoles)
                   .HasForeignKey(ur => ur.UserId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            // RoleId → FK tới Role
            builder.HasOne(ur => ur.Role)
                   .WithMany(r => r.UserRoles)
                   .HasForeignKey(ur => ur.RoleId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();

            // TenantId (nếu có) - Index để query nhanh
            builder.HasIndex(ur => ur.TenantId)
                   .HasDatabaseName("IX_UserRoles_TenantId");

            // Table name (tùy chọn - nếu muốn đổi tên bảng)
            // builder.ToTable("UserRoles");

            // Property configurations
            builder.Property(ur => ur.UserId)
                   .HasColumnName("UserId")
                   .IsRequired();

            builder.Property(ur => ur.RoleId)
                   .HasColumnName("RoleId")
                   .IsRequired();

            builder.Property(ur => ur.TenantId)
                   .HasColumnName("TenantId")
                   .IsRequired(false); // nullable
        }
    }
}