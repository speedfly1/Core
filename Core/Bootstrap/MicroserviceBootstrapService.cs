using Microsoft.Extensions.Logging;
using Core.ServiceRegistry;
using Core.Health;
using Consul;

namespace Core.Bootstrap;

/// <summary>
/// Service implementation of microservice bootstrap operations
/// </summary>
public class MicroserviceBootstrapService : IMicroserviceBootstrap
{
    private readonly IServiceRegistry _serviceRegistry;
    private readonly IHealthChecker _healthChecker;
    private readonly ILogger<MicroserviceBootstrapService> _logger;

    public MicroserviceBootstrapService(
        IServiceRegistry serviceRegistry,
        IHealthChecker healthChecker,
        ILogger<MicroserviceBootstrapService> logger)
    {
        _serviceRegistry = serviceRegistry;
        _healthChecker = healthChecker;
        _logger = logger;
    }

    public async Task RegisterServiceAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Manually registering service {ServiceId}", _serviceRegistry.ServiceId);
        await _serviceRegistry.RegisterAsync(cancellationToken);
    }

    public async Task DeregisterServiceAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Manually deregistering service {ServiceId}", _serviceRegistry.ServiceId);
        await _serviceRegistry.DeregisterAsync(cancellationToken);
    }

    public async Task<HealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting health status for service {ServiceId}", _serviceRegistry.ServiceId);
        return await _healthChecker.GetHealthStatusAsync(cancellationToken);
    }

    public async Task<ServiceEntry[]> DiscoverServiceAsync(string serviceName, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Discovering service {ServiceName}", serviceName);
        return await _serviceRegistry.DiscoverAsync(serviceName, cancellationToken);
    }
}

