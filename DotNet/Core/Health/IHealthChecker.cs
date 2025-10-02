namespace Core.Health;

/// <summary>
/// Interface for health checking functionality
/// </summary>
public interface IHealthChecker
{
    /// <summary>
    /// Checks if the service is healthy
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if healthy, false otherwise</returns>
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed health status information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Health status details</returns>
    Task<HealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents the health status of a service
/// </summary>
public class HealthStatus
{
    /// <summary>
    /// Overall health status
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// Health status message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp of the health check
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Additional health check details
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = new();

    /// <summary>
    /// Service uptime
    /// </summary>
    public TimeSpan Uptime { get; set; }

    /// <summary>
    /// Service version
    /// </summary>
    public string? Version { get; set; }
}

