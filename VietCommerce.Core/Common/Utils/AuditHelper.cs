using System;
using System.Text.Json;
using VietCommerce.Core.Entities.Audit;
using VietCommerce.Core.Entities.Users;
namespace VietCommerce.Core.Common.Utils
{
    public static class AuditHelper
    {
        // Phuong th?c t?o AuditLog phù h?p v?i các thu?c tính hi?n t?i
        public static AuditLog CreateAuditLog(
            string action,
            string? targetType,
            Guid? targetId,
            object? meta,
            Guid userId)
        {
            return new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Action = action,
                TargetType = targetType,
                TargetId = targetId,
                Meta = meta != null ? JsonSerializer.Serialize(meta) : null,
                CreatedAt = DateTime.UtcNow
            };
        }
    public static class Actions
        {
            public const string CREATE = "CREATE";
            public const string UPDATE = "UPDATE";
            public const string DELETE = "DELETE";
            public const string LOGIN = "LOGIN";
            public const string LOGOUT = "LOGOUT";
            public const string PASSWORD_CHANGE = "PASSWORD_CHANGE";
            public const string PRICE_UPDATE = "PRICE_UPDATE";
            public const string INVENTORY_ADJUSTMENT = "INVENTORY_ADJUSTMENT";
            public const string ORDER_STATUS_CHANGE = "ORDER_STATUS_CHANGE";
        }
    }
}
