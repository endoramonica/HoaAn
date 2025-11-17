using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VietCommerce.Application.Services.Services.Interfaces;
using VietCommerce.Core.Models;

namespace VietCommerce.Application.Services.Services;

/// <summary>
/// Background service that subscribes to Redis Pub/Sub channel
/// and invalidates permission caches across all API instances
/// </summary>
public class PermissionCacheInvalidationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PermissionCacheInvalidationService> _logger;

    public PermissionCacheInvalidationService(
        IServiceProvider serviceProvider,
        ILogger<PermissionCacheInvalidationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Permission Cache Invalidation Service started");

        try
        {
            // Create scope to resolve scoped services
            using var scope = _serviceProvider.CreateScope();
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

            // Subscribe to invalidation channel
            await cacheService.SubscribeAsync<CacheInvalidationMessage>(
                "permissions:invalidate",
                message => HandleInvalidation(message)
            );

            _logger.LogInformation("✅ Subscribed to Redis Pub/Sub channel: permissions:invalidate");

            // Keep the service running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Permission Cache Invalidation Service is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in Permission Cache Invalidation Service");
            throw;
        }
    }

    private void HandleInvalidation(CacheInvalidationMessage message)
    {
        try
        {
            _logger.LogInformation("📨 Received cache invalidation: UserId={UserId}, Type={Type}, Timestamp={Timestamp}",
                message.UserId, message.Type, message.Timestamp);

            // Create new scope for each message (avoid scope issues)
            using var scope = _serviceProvider.CreateScope();
            var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

            // Clear cache for the user
            permissionService.ClearUserPermissionsCache(message.UserId);

            _logger.LogInformation("✅ Cache cleared for user {UserId}", message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error handling cache invalidation for user {UserId}", message.UserId);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🛑 Permission Cache Invalidation Service stopped");
        return base.StopAsync(cancellationToken);
    }
}