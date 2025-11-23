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
    public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
    {
        public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
        {
            // ===== TABLE =====
            builder.ToTable("NotificationTemplates");
            builder.HasKey(t => t.Id);

            // ===== PROPERTIES =====
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.TitleTemplate)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.ContentTemplate)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(t => t.LinkTemplate)
                .HasMaxLength(500);

            builder.Property(t => t.EmailSubjectTemplate)
                .HasMaxLength(200);

            builder.Property(t => t.EmailBodyTemplate)
                .HasColumnType("nvarchar(max)");

            builder.Property(t => t.SmsTemplate)
                .HasMaxLength(500);

            builder.Property(t => t.Priority)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(t => t.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(t => t.EnableInApp)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(t => t.EnableEmail)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(t => t.EnableSms)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(t => t.EnablePush)
                .IsRequired()
                .HasDefaultValue(false);

            // ===== INDEXES =====
            builder.HasIndex(t => t.Code)
                .IsUnique()
                .HasDatabaseName("IX_NotificationTemplates_Code");

            builder.HasIndex(t => new { t.Type, t.IsActive })
                .HasDatabaseName("IX_NotificationTemplates_Type_Active");

            // ===== RELATIONSHIPS =====
            builder.HasMany(t => t.Notifications)
                .WithOne(n => n.Template)
                .HasForeignKey(n => n.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Soft delete filter
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
    }
