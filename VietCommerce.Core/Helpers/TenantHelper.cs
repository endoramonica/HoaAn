using System;
using System.Threading;
namespace VietCommerce.Core.Helpers
{
    public static class TenantHelper
    {
        // ThreadLocal luu Guid? thay vì int?
        private static readonly ThreadLocal<Guid?> _currentTenantId = new();
        public static Guid? CurrentTenantId
        {
            get => _currentTenantId.Value;
            set => _currentTenantId.Value = value;
        }
        /// <summary>
        /// Set tenant hi?n t?i
        /// </summary>
        public static void SetTenant(Guid tenantId)
        {
            CurrentTenantId = tenantId;
        }
        /// <summary>
        /// Xóa tenant hi?n t?i
        /// </summary>
        public static void ClearTenant()
        {
            CurrentTenantId = null;
        }
        /// <summary>
        /// Ki?m tra tenant có t?n t?i hay không
        /// </summary>
        public static bool HasTenant()
        {
            return CurrentTenantId.HasValue;
        }
        /// <summary>
        /// L?y tenant hi?n t?i ho?c ném l?i n?u không t?n t?i
        /// </summary>
        public static Guid GetTenantIdOrThrow()
        {
            if (!CurrentTenantId.HasValue)
            {
                throw new InvalidOperationException("No tenant context available");
            }
            return CurrentTenantId.Value;
        }
        /// <summary>
        /// L?y connection string cho tenant (n?u c?n customize theo tenant)
        /// </summary>
        public static string GetTenantConnectionString(string baseConnectionString, Guid tenantId)
        {
            // Ví d?: có th? append tenantId vào DB name ho?c schema
            return baseConnectionString;
        }
    }
}
