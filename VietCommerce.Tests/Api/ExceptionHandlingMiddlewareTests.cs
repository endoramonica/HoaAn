using System.Net;
using System.Text.Json;
using Xunit;
using VietCommerce.Api.Middleware;
using VietCommerce.Core.Common.Exceptions;
using VietCommerce.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace VietCommerce.Tests.Api;

/// <summary>
/// Tests for exception handling middleware
/// Requirements: 8.1, 8.2, 8.3, 8.4, 8.5
/// </summary>
public class ExceptionHandlingMiddlewareTests
{
    private readonly ILogger<ExceptionHandlingMiddleware> _mockLogger;

    public ExceptionHandlingMiddlewareTests()
    {
        _mockLogger = new MockLogger<ExceptionHandlingMiddleware>();
    }

    /// <summary>
    /// Test that BusinessException is handled with 409 Conflict status
    /// </summary>
    [Fact]
    public async Task HandleBusinessException_Returns409Conflict()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new BusinessException("Campaign not in DRAFT status", "CAMPAIGN_NOT_DRAFT");

        var middleware = new ExceptionHandlingMiddleware(
            next: async (ctx) => throw exception,
            logger: _mockLogger
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.Conflict, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = new StreamReader(context.Response.Body).ReadToEnd();
        var response = JsonSerializer.Deserialize<ErrorResponse>(
            responseText,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("Campaign not in DRAFT status", response.Message);
        Assert.Equal("CAMPAIGN_NOT_DRAFT", response.ErrorCode);
        Assert.Equal(409, response.StatusCode);
    }

    /// <summary>
    /// Test that TenantException is handled with 403 Forbidden status
    /// </summary>
    [Fact]
    public async Task HandleTenantException_Returns403Forbidden()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new TenantException("Access denied to tenant");

        var middleware = new ExceptionHandlingMiddleware(
            next: async (ctx) => throw exception,
            logger: _mockLogger
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.Forbidden, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = new StreamReader(context.Response.Body).ReadToEnd();
        var response = JsonSerializer.Deserialize<ErrorResponse>(
            responseText,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("Access denied to tenant", response.Message);
        Assert.Equal("TENANT_ERROR", response.ErrorCode);
        Assert.Equal(403, response.StatusCode);
    }

    /// <summary>
    /// Test that InventoryException is handled with 409 Conflict status
    /// </summary>
    [Fact]
    public async Task HandleInventoryException_Returns409Conflict()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new InventoryException("Insufficient stock for product");

        var middleware = new ExceptionHandlingMiddleware(
            next: async (ctx) => throw exception,
            logger: _mockLogger
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.Conflict, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = new StreamReader(context.Response.Body).ReadToEnd();
        var response = JsonSerializer.Deserialize<ErrorResponse>(
            responseText,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("Insufficient stock for product", response.Message);
        Assert.Equal("INVENTORY_ERROR", response.ErrorCode);
        Assert.Equal(409, response.StatusCode);
    }

    /// <summary>
    /// Test that ArgumentException is handled with 400 Bad Request status
    /// </summary>
    [Fact]
    public async Task HandleArgumentException_Returns400BadRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new ArgumentException("Invalid campaign name");

        var middleware = new ExceptionHandlingMiddleware(
            next: async (ctx) => throw exception,
            logger: _mockLogger
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = new StreamReader(context.Response.Body).ReadToEnd();
        var response = JsonSerializer.Deserialize<ErrorResponse>(
            responseText,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("Invalid campaign name", response.Message);
        Assert.Equal("INVALID_ARGUMENT", response.ErrorCode);
        Assert.Equal(400, response.StatusCode);
    }

    /// <summary>
    /// Test that KeyNotFoundException is handled with 404 Not Found status
    /// </summary>
    [Fact]
    public async Task HandleKeyNotFoundException_Returns404NotFound()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new KeyNotFoundException("Campaign not found");

        var middleware = new ExceptionHandlingMiddleware(
            next: async (ctx) => throw exception,
            logger: _mockLogger
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = new StreamReader(context.Response.Body).ReadToEnd();
        var response = JsonSerializer.Deserialize<ErrorResponse>(
            responseText,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("Campaign not found", response.Message);
        Assert.Equal("RESOURCE_NOT_FOUND", response.ErrorCode);
        Assert.Equal(404, response.StatusCode);
    }

    /// <summary>
    /// Test that InvalidOperationException is handled with 409 Conflict status
    /// </summary>
    [Fact]
    public async Task HandleInvalidOperationException_Returns409Conflict()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new InvalidOperationException("Cannot perform this operation");

        var middleware = new ExceptionHandlingMiddleware(
            next: async (ctx) => throw exception,
            logger: _mockLogger
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.Conflict, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = new StreamReader(context.Response.Body).ReadToEnd();
        var response = JsonSerializer.Deserialize<ErrorResponse>(
            responseText,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("Cannot perform this operation", response.Message);
        Assert.Equal("BUSINESS_RULE_VIOLATION", response.ErrorCode);
        Assert.Equal(409, response.StatusCode);
    }

    /// <summary>
    /// Test that UnauthorizedAccessException is handled with 401 Unauthorized status
    /// </summary>
    [Fact]
    public async Task HandleUnauthorizedAccessException_Returns401Unauthorized()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new UnauthorizedAccessException("User not authorized");

        var middleware = new ExceptionHandlingMiddleware(
            next: async (ctx) => throw exception,
            logger: _mockLogger
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = new StreamReader(context.Response.Body).ReadToEnd();
        var response = JsonSerializer.Deserialize<ErrorResponse>(
            responseText,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("User not authorized", response.Message);
        Assert.Equal("UNAUTHORIZED_ACCESS", response.ErrorCode);
        Assert.Equal(401, response.StatusCode);
    }

    /// <summary>
    /// Test that unknown exceptions are handled with 500 Internal Server Error status
    /// </summary>
    [Fact]
    public async Task HandleUnknownException_Returns500InternalServerError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new Exception("Unexpected error");

        var middleware = new ExceptionHandlingMiddleware(
            next: async (ctx) => throw exception,
            logger: _mockLogger
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = new StreamReader(context.Response.Body).ReadToEnd();
        var response = JsonSerializer.Deserialize<ErrorResponse>(
            responseText,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        Assert.NotNull(response);
        Assert.False(response.Success);
        Assert.Equal("An unexpected error occurred. Please try again later.", response.Message);
        Assert.Equal("INTERNAL_SERVER_ERROR", response.ErrorCode);
        Assert.Equal(500, response.StatusCode);
    }

    /// <summary>
    /// Test that error response always has timestamp
    /// </summary>
    [Fact]
    public async Task ErrorResponse_AlwaysHasTimestamp()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var beforeTime = DateTime.UtcNow;
        var exception = new BusinessException("Test error", "TEST_ERROR");

        var middleware = new ExceptionHandlingMiddleware(
            next: async (ctx) => throw exception,
            logger: _mockLogger
        );

        // Act
        await middleware.InvokeAsync(context);

        var afterTime = DateTime.UtcNow;

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = new StreamReader(context.Response.Body).ReadToEnd();
        var response = JsonSerializer.Deserialize<ErrorResponse>(
            responseText,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        Assert.NotNull(response);
        Assert.True(response.Timestamp >= beforeTime);
        Assert.True(response.Timestamp <= afterTime);
    }

    /// <summary>
    /// Test that middleware passes through successful requests
    /// </summary>
    [Fact]
    public async Task Middleware_PassesThroughSuccessfulRequests()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var middlewareCalled = false;

        var middleware = new ExceptionHandlingMiddleware(
            next: async (ctx) =>
            {
                middlewareCalled = true;
                ctx.Response.StatusCode = 200;
            },
            logger: _mockLogger
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(middlewareCalled);
        Assert.Equal(200, context.Response.StatusCode);
    }
}

/// <summary>
/// Mock logger for testing
/// </summary>
public class MockLogger<T> : ILogger<T>
{
    public IDisposable BeginScope<TState>(TState state) => null!;
    public bool IsEnabled(LogLevel logLevel) => true;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
}
