using System;

namespace VietCommerce.Core.Common.Exceptions
{
    public class TenantException : Exception
    {
        public TenantException(string message) : base(message) { }
        
        public TenantException(string message, Exception innerException) 
            : base(message, innerException) { }
        
        public static TenantException NotFound(int tenantId)
        {
            return new TenantException($"Tenant with ID {tenantId} not found");
        }
        
        public static TenantException NoContext()
        {
            return new TenantException("No tenant context available");
        }
        
        public static TenantException AccessDenied(int tenantId)
        {
            return new TenantException($"Access denied to tenant {tenantId}");
        }
    }
}
