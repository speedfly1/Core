using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Core.Configuration;
using Core.Consul;
using Core.ServiceRegistry;
using Core.Health;
using Core.Bootstrap;

namespace Core.Extensions;

/// <summary>
/// Extension methods for service collection configuration
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds microservice core services with Consul integration
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddMicroserviceCore(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure options
        services.Configure<ConsulOptions>(configuration.GetSection("Consul"));
        services.Configure<ServiceRegistrationOptions>(configuration.GetSection("ServiceRegistration"));

        // Register core services
        services.AddSingleton<IConsulClientWrapper, ConsulClientWrapper>();
        services.AddSingleton<IServiceRegistry, ServiceRegistry>();
        services.AddSingleton<IHealthChecker, HealthChecker>();
        services.AddSingleton<IMicroserviceBootstrap, MicroserviceBootstrapService>();

        // Register hosted service for automatic bootstrap
        services.AddHostedService<MicroserviceBootstrap>();

        return services;
    }

    /// <summary>
    /// Adds microservice core services with custom Consul options
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="consulOptions">Consul configuration options</param>
    /// <param name="registrationOptions">Service registration options</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddMicroserviceCore(
        this IServiceCollection services,
        ConsulOptions consulOptions,
        ServiceRegistrationOptions? registrationOptions = null)
    {
        // Configure options
        services.Configure<ConsulOptions>(options =>
        {
            options.Address = consulOptions.Address;
            options.ServiceName = consulOptions.ServiceName;
            options.ServiceId = consulOptions.ServiceId;
            options.Tags = consulOptions.Tags;
            options.Port = consulOptions.Port;
            options.ServiceAddress = consulOptions.ServiceAddress;
            options.HealthCheckPath = consulOptions.HealthCheckPath;
            options.HealthCheckIntervalSeconds = consulOptions.HealthCheckIntervalSeconds;
            options.HealthCheckTimeoutSeconds = consulOptions.HealthCheckTimeoutSeconds;
            options.DeregisterCriticalServiceAfterSeconds = consulOptions.DeregisterCriticalServiceAfterSeconds;
            options.Enabled = consulOptions.Enabled;
            options.Datacenter = consulOptions.Datacenter;
            options.Token = consulOptions.Token;
        });

        services.Configure<ServiceRegistrationOptions>(options =>
        {
            if (registrationOptions != null)
            {
                options.AutoRegister = registrationOptions.AutoRegister;
                options.AutoDeregister = registrationOptions.AutoDeregister;
                options.RegistrationDelaySeconds = registrationOptions.RegistrationDelaySeconds;
                options.MaxRetryAttempts = registrationOptions.MaxRetryAttempts;
                options.RetryDelaySeconds = registrationOptions.RetryDelaySeconds;
            }
        });

        // Register core services
        services.AddSingleton<IConsulClientWrapper, ConsulClientWrapper>();
        services.AddSingleton<IServiceRegistry, ServiceRegistry>();
        services.AddSingleton<IHealthChecker, HealthChecker>();
        services.AddSingleton<IMicroserviceBootstrap, MicroserviceBootstrapService>();

        // Register hosted service for automatic bootstrap
        services.AddHostedService<MicroserviceBootstrap>();

        return services;
    }

    /// <summary>
    /// Adds microservice core services with minimal configuration
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="serviceName">Name of the service</param>
    /// <param name="port">Port the service runs on</param>
    /// <param name="consulAddress">Consul server address</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddMicroserviceCore(
        this IServiceCollection services,
        string serviceName,
        int port,
        string consulAddress = "http://localhost:8500")
    {
        var consulOptions = new ConsulOptions
        {
            ServiceName = serviceName,
            Port = port,
            Address = consulAddress
        };

        return services.AddMicroserviceCore(consulOptions);
    }
}

