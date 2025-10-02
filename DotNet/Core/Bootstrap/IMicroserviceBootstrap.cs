namespace Core.Bootstrap;

/// <summary>
/// Interface for microservice bootstrap operations
/// </summary>
public interface IMicroserviceBootstrap
{
    /// <summary>
    /// Manually registers the service with Consul
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the registration operation</returns>
    Task RegisterServiceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Manually deregisters the service from Consul
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the deregistration operation</returns>
    Task DeregisterServiceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current health status of the service
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Health status of the service</returns>
    Task<Health.HealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Discovers healthy instances of a service
    /// </summary>
    /// <param name="serviceName">Name of the service to discover</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of healthy service instances</returns>
    Task<Consul.ServiceEntry[]> DiscoverServiceAsync(string serviceName, CancellationToken cancellationToken = default);
}

