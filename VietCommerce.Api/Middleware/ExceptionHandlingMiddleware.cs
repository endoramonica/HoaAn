using System.Net;
using System.Text.Json;
using VietCommerce.Core.Common.Exceptions;
using VietCommerce.Core.Models;

namespace VietCommerce.Api.Middleware;

/// <summary>
/// Global exception handling middleware that catches all unhandled exceptions
/// and returns consistent error responses
/// Requirements: 8.1, 8.2, 8.3, 8.4, 8.5
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError($"❌ Unhandled exception: {ex.GetType().Name} - {ex.Message}");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        ErrorResponse errorResponse;

        switch (exception)
        {
            // Handle business exceptions with custom error codes
            case BusinessException businessEx:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse = ErrorResponse.Conflict(
                    message: businessEx.Message,
                    errorCode: businessEx.Code);
                break;

            // Handle tenant exceptions (403 - access denied)
            case TenantException tenantEx:
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                errorResponse = ErrorResponse.Forbidden(
                    message: tenantEx.Message,
                    errorCode: "TENANT_ERROR");
                break;

            // Handle inventory exceptions (409 - business rule violation)
            case InventoryException inventoryEx:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse = ErrorResponse.Conflict(
                    message: inventoryEx.Message,
                    errorCode: "INVENTORY_ERROR");
                break;

            // Handle validation exceptions (400)
            case ArgumentException argEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = ErrorResponse.ValidationError(
                    message: argEx.Message,
                    errorCode: "INVALID_ARGUMENT");
                break;

            // Handle key not found exceptions (404)
            case KeyNotFoundException keyEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse = ErrorResponse.NotFound(
                    message: keyEx.Message,
                    errorCode: "RESOURCE_NOT_FOUND");
                break;

            // Handle invalid operation exceptions (409 - business rule violation)
            case InvalidOperationException invOpEx:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse = ErrorResponse.Conflict(
                    message: invOpEx.Message,
                    errorCode: "BUSINESS_RULE_VIOLATION");
                break;

            // Handle unauthorized access (401)
            case UnauthorizedAccessException unAuthEx:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse = ErrorResponse.Unauthorized(
                    message: unAuthEx.Message,
                    errorCode: "UNAUTHORIZED_ACCESS");
                break;

            // Handle all other exceptions as 500 server errors
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse = ErrorResponse.ServerError(
                    message: "An unexpected error occurred. Please try again later.",
                    errorCode: "INTERNAL_SERVER_ERROR");
                break;
        }

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(errorResponse, options);

        return context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Extension method to register the exception handling middleware
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
