using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Core.Configuration;
using Core.ServiceRegistry;
using Core.Health;

namespace Core.Bootstrap;

/// <summary>
/// Bootstrap class for microservice with Consul integration
/// </summary>
public class MicroserviceBootstrap : IHostedService
{
    private readonly IServiceRegistry _serviceRegistry;
    private readonly IHealthChecker _healthChecker;
    private readonly ILogger<MicroserviceBootstrap> _logger;
    private readonly ConsulOptions _consulOptions;
    private readonly ServiceRegistrationOptions _registrationOptions;

    public MicroserviceBootstrap(
        IServiceRegistry serviceRegistry,
        IHealthChecker healthChecker,
        IOptions<ConsulOptions> consulOptions,
        IOptions<ServiceRegistrationOptions> registrationOptions,
        ILogger<MicroserviceBootstrap> logger)
    {
        _serviceRegistry = serviceRegistry;
        _healthChecker = healthChecker;
        _consulOptions = consulOptions.Value;
        _registrationOptions = registrationOptions.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting microservice bootstrap for {ServiceName}", _serviceRegistry.ServiceName);

        try
        {
            // Register service with Consul if auto-registration is enabled
            if (_registrationOptions.AutoRegister && _consulOptions.Enabled)
            {
                _logger.LogInformation("Auto-registering service {ServiceId} with Consul", _serviceRegistry.ServiceId);
                await _serviceRegistry.RegisterAsync(cancellationToken);
            }

            // Perform initial health check
            var isHealthy = await _healthChecker.IsHealthyAsync(cancellationToken);
            _logger.LogInformation("Initial health check: {HealthStatus}", isHealthy ? "Healthy" : "Unhealthy");

            _logger.LogInformation("Microservice bootstrap completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start microservice bootstrap");
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping microservice bootstrap for {ServiceName}", _serviceRegistry.ServiceName);

        try
        {
            // Deregister service from Consul if auto-deregistration is enabled
            if (_registrationOptions.AutoDeregister && _consulOptions.Enabled)
            {
                _logger.LogInformation("Auto-deregistering service {ServiceId} from Consul", _serviceRegistry.ServiceId);
                await _serviceRegistry.DeregisterAsync(cancellationToken);
            }

            _logger.LogInformation("Microservice bootstrap stopped successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during microservice bootstrap shutdown");
            // Don't rethrow to avoid masking other shutdown errors
        }
    }
}

