using Consul;

namespace Core.ServiceRegistry;

/// <summary>
/// Interface for service registry operations
/// </summary>
public interface IServiceRegistry
{
    /// <summary>
    /// Registers the current service with Consul
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the registration operation</returns>
    Task RegisterAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deregisters the current service from Consul
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the deregistration operation</returns>
    Task DeregisterAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Discovers healthy instances of a service
    /// </summary>
    /// <param name="serviceName">Name of the service to discover</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of healthy service instances</returns>
    Task<ServiceEntry[]> DiscoverAsync(string serviceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the service ID of the current service
    /// </summary>
    string ServiceId { get; }

    /// <summary>
    /// Gets the service name of the current service
    /// </summary>
    string ServiceName { get; }
}

