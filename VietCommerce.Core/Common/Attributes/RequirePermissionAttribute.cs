// File: VietCommerce.Core/Common/Attributes/RequirePermissionAttribute.cs
using Microsoft.AspNetCore.Authorization;

namespace VietCommerce.Core.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string permission)
        {
            // Gán Policy dạng "perm:xxx" để Middleware nhận diện
            Policy = $"perm:{permission}";
        }
    }
}