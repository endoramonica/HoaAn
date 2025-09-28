using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace VietCommerce.Core.Middleware;

public class PermissionMiddleware {
    private readonly RequestDelegate _next;
    public PermissionMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) {
        // TODO: check permission from JWT/Claims
        await _next(context);
    }
}
