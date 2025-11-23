using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VietCommerce.Core.Entities.Notifications;

namespace VietCommerce.Data.Context.Configurations.Notifications
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            // ===== TABLE =====
            builder.ToTable("Notifications");
            builder.HasKey(n => n.Id);

            // ===== RECIPIENTS =====
            // Customer notifications (social + buyer)
            builder.HasOne(n => n.Customer)
                .WithMany()
                .HasForeignKey(n => n.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // User notifications (admin/employee)
            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== ACTOR =====
            builder.HasOne(n => n.ActorCustomer)
                .WithMany()
                .HasForeignKey(n => n.ActorCustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== RELATED ENTITIES =====
            builder.HasOne(n => n.Post)
                .WithMany()
                .HasForeignKey(n => n.PostId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(n => n.Order)
                .WithMany()
                .HasForeignKey(n => n.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(n => n.Template)
                .WithMany(t => t.Notifications)
                .HasForeignKey(n => n.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== PROPERTIES =====
            builder.Property(n => n.Title)
                .HasMaxLength(200);

            builder.Property(n => n.Message)
                .HasMaxLength(1000);

            builder.Property(n => n.Link)
                .HasMaxLength(500);

            builder.Property(n => n.Data)
                .HasColumnType("nvarchar(max)");

            builder.Property(n => n.NotifiedOn)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(n => n.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(n => n.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(n => n.SentViaInApp)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(n => n.SentViaEmail)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(n => n.SentViaSms)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(n => n.SentViaPush)
                .IsRequired()
                .HasDefaultValue(false);

            // ===== INDEXES =====
            // Unread notifications for customers
            builder.HasIndex(n => new { n.CustomerId, n.IsRead, n.NotifiedOn })
                .HasDatabaseName("IX_Notifications_Customer_Unread")
                .HasFilter("[CustomerId] IS NOT NULL");

            // Unread notifications for users (admin)
            builder.HasIndex(n => new { n.UserId, n.IsRead, n.NotifiedOn })
                .HasDatabaseName("IX_Notifications_User_Unread")
                .HasFilter("[UserId] IS NOT NULL");

            // By type for filtering
            builder.HasIndex(n => new { n.CustomerId, n.Type, n.NotifiedOn })
                .HasDatabaseName("IX_Notifications_Customer_Type")
                .HasFilter("[CustomerId] IS NOT NULL");

            // By actor (to show "you and 5 others liked this post")
            builder.HasIndex(n => new { n.PostId, n.Type, n.ActorCustomerId })
                .HasDatabaseName("IX_Notifications_Post_Type_Actor")
                .HasFilter("[PostId] IS NOT NULL");

            // By order
            builder.HasIndex(n => n.OrderId)
                .HasDatabaseName("IX_Notifications_OrderId")
                .HasFilter("[OrderId] IS NOT NULL");

            // ===== CONSTRAINTS =====
            // Phải có ít nhất 1 recipient
            builder.HasCheckConstraint(
                "CK_Notification_HasRecipient",
                "[CustomerId] IS NOT NULL OR [UserId] IS NOT NULL"
            );

            // Soft delete filter
            builder.HasQueryFilter(n => !n.IsDeleted);
        }
    }
}