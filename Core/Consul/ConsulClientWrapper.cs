using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Core.Configuration;

namespace Core.Consul;

/// <summary>
/// Implementation of Consul client wrapper
/// </summary>
public class ConsulClientWrapper : IConsulClientWrapper, IDisposable
{
    private readonly IConsulClient _consulClient;
    private readonly ILogger<ConsulClientWrapper> _logger;
    private readonly ConsulOptions _options;
    private bool _disposed;

    public ConsulClientWrapper(IOptions<ConsulOptions> options, ILogger<ConsulClientWrapper> logger)
    {
        _options = options.Value;
        _logger = logger;

        var consulConfig = new ConsulClientConfiguration
        {
            Address = new Uri(_options.Address)
        };

        if (!string.IsNullOrEmpty(_options.Datacenter))
        {
            consulConfig.Datacenter = _options.Datacenter;
        }

        if (!string.IsNullOrEmpty(_options.Token))
        {
            consulConfig.Token = _options.Token;
        }

        _consulClient = new ConsulClient(consulConfig);
    }

    public async Task RegisterServiceAsync(AgentServiceRegistration serviceRegistration, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Registering service {ServiceId} with Consul", serviceRegistration.ID);
            
            var result = await _consulClient.Agent.ServiceRegister(serviceRegistration, cancellationToken);
            
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation("Successfully registered service {ServiceId}", serviceRegistration.ID);
            }
            else
            {
                _logger.LogWarning("Failed to register service {ServiceId}. Status: {StatusCode}", 
                    serviceRegistration.ID, result.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering service {ServiceId}", serviceRegistration.ID);
            throw;
        }
    }

    public async Task DeregisterServiceAsync(string serviceId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deregistering service {ServiceId} from Consul", serviceId);
            
            var result = await _consulClient.Agent.ServiceDeregister(serviceId, cancellationToken);
            
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogInformation("Successfully deregistered service {ServiceId}", serviceId);
            }
            else
            {
                _logger.LogWarning("Failed to deregister service {ServiceId}. Status: {StatusCode}", 
                    serviceId, result.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deregistering service {ServiceId}", serviceId);
            throw;
        }
    }

    public async Task<ServiceEntry[]> GetHealthyServicesAsync(string serviceName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting healthy services for {ServiceName}", serviceName);
            
            var result = await _consulClient.Health.Service(serviceName, tag: "", passingOnly: true, cancellationToken);
            
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogDebug("Found {Count} healthy services for {ServiceName}", 
                    result.Response.Length, serviceName);
                return result.Response;
            }
            else
            {
                _logger.LogWarning("Failed to get healthy services for {ServiceName}. Status: {StatusCode}", 
                    serviceName, result.StatusCode);
                return Array.Empty<ServiceEntry>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting healthy services for {ServiceName}", serviceName);
            return Array.Empty<ServiceEntry>();
        }
    }

    public async Task<ServiceEntry[]> GetAllServicesAsync(string serviceName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all services for {ServiceName}", serviceName);
            
            var result = await _consulClient.Health.Service(serviceName, tag: "", passingOnly: false, cancellationToken);
            
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogDebug("Found {Count} services for {ServiceName}", 
                    result.Response.Length, serviceName);
                return result.Response;
            }
            else
            {
                _logger.LogWarning("Failed to get services for {ServiceName}. Status: {StatusCode}", 
                    serviceName, result.StatusCode);
                return Array.Empty<ServiceEntry>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting services for {ServiceName}", serviceName);
            return Array.Empty<ServiceEntry>();
        }
    }

    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _consulClient.Status.Leader(cancellationToken);
            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking Consul health");
            return false;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _consulClient?.Dispose();
            _disposed = true;
        }
    }
}

