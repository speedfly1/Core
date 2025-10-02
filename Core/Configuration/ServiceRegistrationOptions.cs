namespace Core.Configuration;

/// <summary>
/// Options for service registration behavior
/// </summary>
public class ServiceRegistrationOptions
{
    /// <summary>
    /// Whether to automatically register the service on startup
    /// </summary>
    public bool AutoRegister { get; set; } = true;

    /// <summary>
    /// Whether to automatically deregister the service on shutdown
    /// </summary>
    public bool AutoDeregister { get; set; } = true;

    /// <summary>
    /// Delay in seconds before attempting to register the service
    /// </summary>
    public int RegistrationDelaySeconds { get; set; } = 0;

    /// <summary>
    /// Maximum number of registration retry attempts
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Delay between retry attempts in seconds
    /// </summary>
    public int RetryDelaySeconds { get; set; } = 5;
}

