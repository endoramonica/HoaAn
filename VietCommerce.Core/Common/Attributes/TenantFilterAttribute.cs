using System;

namespace VietCommerce.Core.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class TenantFilterAttribute : Attribute
    {
        public bool IsRequired { get; }

        public TenantFilterAttribute(bool isRequired = true)
        {
            IsRequired = isRequired;
        }
    }
}
