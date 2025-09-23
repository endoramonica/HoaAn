using System;
using System.Threading;

namespace VietCommerce.Core.Helpers
{
    public static class TenantHelper
    {
        // ThreadLocal lưu Guid? thay vì int?
        private static readonly ThreadLocal<Guid?> _currentTenantId = new();

        public static Guid? CurrentTenantId
        {
            get => _currentTenantId.Value;
            set => _currentTenantId.Value = value;
        }

        /// <summary>
        /// Set tenant hiện tại
        /// </summary>
        public static void SetTenant(Guid tenantId)
        {
            CurrentTenantId = tenantId;
        }

        /// <summary>
        /// Xóa tenant hiện tại
        /// </summary>
        public static void ClearTenant()
        {
            CurrentTenantId = null;
        }

        /// <summary>
        /// Kiểm tra tenant có tồn tại hay không
        /// </summary>
        public static bool HasTenant()
        {
            return CurrentTenantId.HasValue;
        }

        /// <summary>
        /// Lấy tenant hiện tại hoặc ném lỗi nếu không tồn tại
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
        /// Lấy connection string cho tenant (nếu cần customize theo tenant)
        /// </summary>
        public static string GetTenantConnectionString(string baseConnectionString, Guid tenantId)
        {
            // Ví dụ: có thể append tenantId vào DB name hoặc schema
            return baseConnectionString;
        }
    }
}
