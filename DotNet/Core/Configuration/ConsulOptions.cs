namespace Core.Configuration;

/// <summary>
/// Configuration options for Consul service registration
/// </summary>
public class ConsulOptions
{
    /// <summary>
    /// Consul server address (e.g., "http://localhost:8500")
    /// </summary>
    public string Address { get; set; } = "http://localhost:8500";

    /// <summary>
    /// Service name to register in Consul
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Service ID (unique identifier for this service instance)
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// Service tags for categorization and filtering
    /// </summary>
    public string[] Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Service port
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Service address (IP or hostname)
    /// </summary>
    public string ServiceAddress { get; set; } = "localhost";

    /// <summary>
    /// Health check endpoint path
    /// </summary>
    public string HealthCheckPath { get; set; } = "/health";

    /// <summary>
    /// Health check interval in seconds
    /// </summary>
    public int HealthCheckIntervalSeconds { get; set; } = 10;

    /// <summary>
    /// Health check timeout in seconds
    /// </summary>
    public int HealthCheckTimeoutSeconds { get; set; } = 5;

    /// <summary>
    /// Service deregistration critical timeout in seconds
    /// </summary>
    public int DeregisterCriticalServiceAfterSeconds { get; set; } = 30;

    /// <summary>
    /// Whether to enable service registration
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Consul datacenter
    /// </summary>
    public string? Datacenter { get; set; }

    /// <summary>
    /// Consul token for authentication
    /// </summary>
    public string? Token { get; set; }
}

