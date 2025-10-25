using System;
using System.Linq;
using VietCommerce.Core.Common;
using VietCommerce.Core.Helpers;
namespace VietCommerce.Core.Common.Utils
{
    public static class TenantContextHelper
    {
        /// <summary>
        /// Ki?m tra quy?n truy c?p tenant
        /// </summary>
        public static void ValidateTenantAccess(Guid requestedTenantId)
        {
            var currentTenantId = TenantHelper.CurrentTenantId;
            if (currentTenantId == null)
                throw new InvalidOperationException("No tenant context available");
            if (currentTenantId.Value != requestedTenantId)
                throw new UnauthorizedAccessException($"Access denied to tenant {requestedTenantId}");
        }
        /// <summary>
        /// Ki?m tra tenant c?a entity
        /// </summary>
        public static T FilterByTenant<T>(T entity) where T : ITenantEntity
        {
            if (entity == null) return entity;
            var currentTenantId = TenantHelper.CurrentTenantId;
            if (currentTenantId != null && entity.TenantId != currentTenantId.Value)
                throw new UnauthorizedAccessException("Access denied - tenant mismatch");
            return entity;
        }
        /// <summary>
        /// Áp d?ng filter theo tenant cho IQueryable
        /// </summary>
        public static IQueryable<T> ApplyTenantFilter<T>(IQueryable<T> query) where T : class, ITenantEntity
        {
            var tenantId = TenantHelper.CurrentTenantId;
            if (tenantId != null)
            {
                return query.Where(x => x.TenantId == tenantId.Value);
            }
            return query;
        }
    }
}
