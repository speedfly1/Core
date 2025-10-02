using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Core.Configuration;
using Core.Consul;

namespace Core.ServiceRegistry;

/// <summary>
/// Implementation of service registry for Consul
/// </summary>
public class ServiceRegistry : IServiceRegistry
{
    private readonly IConsulClientWrapper _consulClient;
    private readonly ILogger<ServiceRegistry> _logger;
    private readonly ConsulOptions _consulOptions;
    private readonly ServiceRegistrationOptions _registrationOptions;
    private readonly string _serviceId;
    private readonly string _serviceName;

    public ServiceRegistry(
        IConsulClientWrapper consulClient,
        IOptions<ConsulOptions> consulOptions,
        IOptions<ServiceRegistrationOptions> registrationOptions,
        ILogger<ServiceRegistry> logger)
    {
        _consulClient = consulClient;
        _consulOptions = consulOptions.Value;
        _registrationOptions = registrationOptions.Value;
        _logger = logger;

        _serviceName = _consulOptions.ServiceName;
        _serviceId = string.IsNullOrEmpty(_consulOptions.ServiceId) 
            ? $"{_serviceName}-{Environment.MachineName}-{Environment.ProcessId}"
            : _consulOptions.ServiceId;
    }

    public string ServiceId => _serviceId;
    public string ServiceName => _serviceName;

    public async Task RegisterAsync(CancellationToken cancellationToken = default)
    {
        if (!_consulOptions.Enabled)
        {
            _logger.LogInformation("Service registration is disabled");
            return;
        }

        if (string.IsNullOrEmpty(_serviceName))
        {
            throw new InvalidOperationException("Service name is required for registration");
        }

        var registration = CreateServiceRegistration();
        
        var attempts = 0;
        var maxAttempts = _registrationOptions.MaxRetryAttempts;

        while (attempts < maxAttempts)
        {
            try
            {
                if (_registrationOptions.RegistrationDelaySeconds > 0 && attempts == 0)
                {
                    _logger.LogInformation("Waiting {Delay} seconds before registration", 
                        _registrationOptions.RegistrationDelaySeconds);
                    await Task.Delay(TimeSpan.FromSeconds(_registrationOptions.RegistrationDelaySeconds), cancellationToken);
                }

                await _consulClient.RegisterServiceAsync(registration, cancellationToken);
                _logger.LogInformation("Service {ServiceId} registered successfully", _serviceId);
                return;
            }
            catch (Exception ex)
            {
                attempts++;
                _logger.LogWarning(ex, "Failed to register service {ServiceId} (attempt {Attempt}/{MaxAttempts})", 
                    _serviceId, attempts, maxAttempts);

                if (attempts >= maxAttempts)
                {
                    _logger.LogError(ex, "Failed to register service {ServiceId} after {MaxAttempts} attempts", 
                        _serviceId, maxAttempts);
                    throw;
                }

                if (_registrationOptions.RetryDelaySeconds > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(_registrationOptions.RetryDelaySeconds), cancellationToken);
                }
            }
        }
    }

    public async Task DeregisterAsync(CancellationToken cancellationToken = default)
    {
        if (!_consulOptions.Enabled)
        {
            _logger.LogInformation("Service deregistration is disabled");
            return;
        }

        try
        {
            await _consulClient.DeregisterServiceAsync(_serviceId, cancellationToken);
            _logger.LogInformation("Service {ServiceId} deregistered successfully", _serviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deregister service {ServiceId}", _serviceId);
            throw;
        }
    }

    public async Task<ServiceEntry[]> DiscoverAsync(string serviceName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Discovering healthy instances of service {ServiceName}", serviceName);
            var services = await _consulClient.GetHealthyServicesAsync(serviceName, cancellationToken);
            _logger.LogDebug("Found {Count} healthy instances of service {ServiceName}", services.Length, serviceName);
            return services;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to discover services for {ServiceName}", serviceName);
            return Array.Empty<ServiceEntry>();
        }
    }

    private AgentServiceRegistration CreateServiceRegistration()
    {
        var healthCheckUrl = $"http://{_consulOptions.ServiceAddress}:{_consulOptions.Port}{_consulOptions.HealthCheckPath}";

        var registration = new AgentServiceRegistration
        {
            ID = _serviceId,
            Name = _serviceName,
            Address = _consulOptions.ServiceAddress,
            Port = _consulOptions.Port,
            Tags = _consulOptions.Tags,
            Check = new AgentServiceCheck
            {
                HTTP = healthCheckUrl,
                Interval = TimeSpan.FromSeconds(_consulOptions.HealthCheckIntervalSeconds),
                Timeout = TimeSpan.FromSeconds(_consulOptions.HealthCheckTimeoutSeconds),
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(_consulOptions.DeregisterCriticalServiceAfterSeconds)
            }
        };

        _logger.LogDebug("Created service registration: {ServiceId} at {Address}:{Port}", 
            _serviceId, _consulOptions.ServiceAddress, _consulOptions.Port);

        return registration;
    }
}

