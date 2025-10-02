using Consul;

namespace Core.Consul;

/// <summary>
/// Wrapper interface for Consul client operations
/// </summary>
public interface IConsulClientWrapper
{
    /// <summary>
    /// Registers a service with Consul
    /// </summary>
    /// <param name="serviceRegistration">Service registration details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the registration operation</returns>
    Task RegisterServiceAsync(AgentServiceRegistration serviceRegistration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deregisters a service from Consul
    /// </summary>
    /// <param name="serviceId">Service ID to deregister</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the deregistration operation</returns>
    Task DeregisterServiceAsync(string serviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets healthy service instances
    /// </summary>
    /// <param name="serviceName">Name of the service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of healthy service instances</returns>
    Task<ServiceEntry[]> GetHealthyServicesAsync(string serviceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all service instances
    /// </summary>
    /// <param name="serviceName">Name of the service</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all service instances</returns>
    Task<ServiceEntry[]> GetAllServicesAsync(string serviceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the Consul client is healthy
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if healthy, false otherwise</returns>
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);
}

