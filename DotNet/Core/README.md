# Microservice.Core

A comprehensive .NET library for bootstrapping microservices with Consul service registration, health checks, and service discovery.

## Features

- **Consul Integration**: Automatic service registration and deregistration with Consul
- **Health Checks**: Built-in health checking with customizable endpoints
- **Service Discovery**: Easy discovery of healthy service instances
- **Configuration**: Flexible configuration through appsettings.json or code
- **Dependency Injection**: Seamless integration with .NET DI container
- **ASP.NET Core Support**: Includes health check controllers for web applications

## Installation

```bash
dotnet add package Microservice.Core
```

## Quick Start

### 1. Basic Setup

```csharp
using Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add microservice core services
builder.Services.AddMicroserviceCore(builder.Configuration);

var app = builder.Build();

// Add health check endpoints
app.MapControllers();

app.Run();
```

### 2. Configuration

Add to your `appsettings.json`:

```json
{
  "Consul": {
    "Address": "http://localhost:8500",
    "ServiceName": "my-microservice",
    "Port": 5000,
    "ServiceAddress": "localhost",
    "HealthCheckPath": "/health",
    "Enabled": true
  },
  "ServiceRegistration": {
    "AutoRegister": true,
    "AutoDeregister": true
  }
}
```

### 3. Programmatic Configuration

```csharp
var consulOptions = new ConsulOptions
{
    ServiceName = "my-service",
    Port = 5000,
    Address = "http://localhost:8500",
    HealthCheckPath = "/health"
};

builder.Services.AddMicroserviceCore(consulOptions);
```

### 4. Minimal Configuration

```csharp
builder.Services.AddMicroserviceCore("my-service", 5000);
```

## Configuration Options

### ConsulOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| Address | string | "http://localhost:8500" | Consul server address |
| ServiceName | string | Required | Service name to register |
| ServiceId | string | Auto-generated | Unique service identifier |
| Tags | string[] | [] | Service tags for categorization |
| Port | int | Required | Service port |
| ServiceAddress | string | "localhost" | Service IP/hostname |
| HealthCheckPath | string | "/health" | Health check endpoint |
| HealthCheckIntervalSeconds | int | 10 | Health check interval |
| HealthCheckTimeoutSeconds | int | 5 | Health check timeout |
| DeregisterCriticalServiceAfterSeconds | int | 30 | Critical service timeout |
| Enabled | bool | true | Enable/disable registration |
| Datacenter | string | null | Consul datacenter |
| Token | string | null | Consul authentication token |

### ServiceRegistrationOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| AutoRegister | bool | true | Auto-register on startup |
| AutoDeregister | bool | true | Auto-deregister on shutdown |
| RegistrationDelaySeconds | int | 0 | Delay before registration |
| MaxRetryAttempts | int | 3 | Max registration retries |
| RetryDelaySeconds | int | 5 | Delay between retries |

## Usage Examples

### Service Discovery

```csharp
public class MyService
{
    private readonly IMicroserviceBootstrap _bootstrap;

    public MyService(IMicroserviceBootstrap bootstrap)
    {
        _bootstrap = bootstrap;
    }

    public async Task<ServiceEntry[]> GetServiceInstances(string serviceName)
    {
        return await _bootstrap.DiscoverServiceAsync(serviceName);
    }
}
```

### Manual Service Registration

```csharp
public class MyService
{
    private readonly IMicroserviceBootstrap _bootstrap;

    public MyService(IMicroserviceBootstrap bootstrap)
    {
        _bootstrap = bootstrap;
    }

    public async Task RegisterManually()
    {
        await _bootstrap.RegisterServiceAsync();
    }

    public async Task DeregisterManually()
    {
        await _bootstrap.DeregisterServiceAsync();
    }
}
```

### Health Checks

```csharp
public class MyService
{
    private readonly IHealthChecker _healthChecker;

    public MyService(IHealthChecker healthChecker)
    {
        _healthChecker = healthChecker;
    }

    public async Task<bool> IsHealthy()
    {
        return await _healthChecker.IsHealthyAsync();
    }

    public async Task<HealthStatus> GetDetailedHealth()
    {
        return await _healthChecker.GetHealthStatusAsync();
    }
}
```

## Health Check Endpoints

When using ASP.NET Core, the library provides the following endpoints:

- `GET /health` - Simple health check
- `GET /health/detailed` - Detailed health information
- `GET /health/ready` - Readiness check
- `GET /health/live` - Liveness check

## Advanced Usage

### Custom Health Checker

```csharp
public class CustomHealthChecker : IHealthChecker
{
    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        // Your custom health logic
        return true;
    }

    public async Task<HealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default)
    {
        return new HealthStatus
        {
            IsHealthy = true,
            Message = "Custom health check passed",
            Details = new Dictionary<string, object>
            {
                ["custom_metric"] = 42
            }
        };
    }
}

// Register in DI
builder.Services.AddSingleton<IHealthChecker, CustomHealthChecker>();
```

### Service Registry Direct Usage

```csharp
public class MyService
{
    private readonly IServiceRegistry _serviceRegistry;

    public MyService(IServiceRegistry serviceRegistry)
    {
        _serviceRegistry = serviceRegistry;
    }

    public async Task RegisterWithCustomOptions()
    {
        // Service will be registered with configured options
        await _serviceRegistry.RegisterAsync();
    }
}
```

## Consul Setup

### Using Docker

```bash
docker run -d --name consul -p 8500:8500 consul:latest
```

### Using Consul Binary

1. Download Consul from https://www.consul.io/downloads
2. Start Consul in development mode:
   ```bash
   consul agent -dev
   ```

## Troubleshooting

### Service Not Registering

1. Check Consul is running and accessible
2. Verify configuration in appsettings.json
3. Check logs for registration errors
4. Ensure service port is correct

### Health Checks Failing

1. Verify health check endpoint is accessible
2. Check health check timeout settings
3. Review service logs for errors

### Service Discovery Issues

1. Ensure services are registered and healthy in Consul
2. Check service names match exactly
3. Verify Consul connectivity

## License

This project is licensed under the MIT License.
