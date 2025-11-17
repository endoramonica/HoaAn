using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text.Json;
using VietCommerce.Application.Attributes;
using VietCommerce.Application.Services.Services.Interfaces;

namespace VietCommerce.Api.Middleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PermissionMiddleware> _logger;

        public PermissionMiddleware(RequestDelegate next, ILogger<PermissionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IPermissionService permissionService)
        {
            var user = context.User;

            // Nếu chưa login → bỏ qua middleware
            if (user?.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            // Lấy UserId từ JWT
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                await _next(context);
                return;
            }

            // Attach user context để downstream Service sử dụng
            context.Items["UserId"] = userId;
            context.Items["Roles"] = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            context.Items["Permissions"] = user.FindAll("permission").Select(c => c.Value).ToList();

            // Lấy endpoint metadata
            var endpoint = context.GetEndpoint();

            // ================
            // BƯỚC 1: Kiểm tra RequirePermissionAttribute
            // ================
            var permissionAttr = endpoint?.Metadata?.GetMetadata<RequirePermissionAttribute>();
            if (permissionAttr != null)
            {
                var fullPolicy = permissionAttr.Policy;

                // Policy dạng "perm:view_user"
                var requiredPermission = fullPolicy.StartsWith("perm:")
                    ? fullPolicy["perm:".Length..]
                    : fullPolicy;

                var hasPermission = await permissionService.CheckUserPermissionAsync(userId, requiredPermission);
                if (!hasPermission)
                {
                    _logger.LogWarning(
                        "User {UserId} lacks permission: {Permission} at {Path}",
                        userId, requiredPermission, context.Request.Path
                    );

                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        success = false,
                        message = $"Forbidden: Missing permission '{requiredPermission}'."
                    }));

                    return;
                }
            }

            // ================
            // BƯỚC 2: Kiểm tra AuthorizeAttribute cũ (nếu dùng Policy mà KHÔNG bắt đầu bằng 'perm:')
            // ================
            var policyAttr = endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>();
            if (policyAttr != null &&
                !string.IsNullOrWhiteSpace(policyAttr.Policy) &&
                !policyAttr.Policy.StartsWith("perm:"))
            {
                var policyName = policyAttr.Policy;

                var hasPolicy = await permissionService.CheckUserPermissionAsync(userId, policyName);
                if (!hasPolicy)
                {
                    _logger.LogWarning(
                        "User {UserId} did not meet policy {Policy} at {Path}",
                        userId, policyName, context.Request.Path
                    );

                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        success = false,
                        message = $"Forbidden: Missing policy '{policyName}'."
                    }));

                    return;
                }
            }

            // Qua được hết → cho vào pipeline tiếp theo
            await _next(context);
        }
    }

    // Extension để add vào middleware pipeline
    public static class PermissionMiddlewareExtensions
    {
        public static IApplicationBuilder UsePermissionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<PermissionMiddleware>();
        }
    }
}
