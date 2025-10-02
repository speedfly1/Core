using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Core.Health;

/// <summary>
/// Default implementation of health checker
/// </summary>
public class HealthChecker : IHealthChecker
{
    private readonly ILogger<HealthChecker> _logger;
    private readonly DateTime _startTime;
    private readonly string? _version;

    public HealthChecker(ILogger<HealthChecker> logger)
    {
        _logger = logger;
        _startTime = DateTime.UtcNow;
        _version = GetVersion();
    }

    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var healthStatus = await GetHealthStatusAsync(cancellationToken);
            return healthStatus.IsHealthy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking health status");
            return false;
        }
    }

    public async Task<HealthStatus> GetHealthStatusAsync(CancellationToken cancellationToken = default)
    {
        var healthStatus = new HealthStatus
        {
            IsHealthy = true,
            Message = "Service is healthy",
            Timestamp = DateTime.UtcNow,
            Uptime = DateTime.UtcNow - _startTime,
            Version = _version
        };

        try
        {
            // Add basic system health checks
            healthStatus.Details["memory_usage_mb"] = GC.GetTotalMemory(false) / 1024 / 1024;
            healthStatus.Details["thread_count"] = Process.GetCurrentProcess().Threads.Count;
            healthStatus.Details["processor_time"] = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;

            // Check if the process is responsive
            await Task.Delay(1, cancellationToken); // Simple responsiveness check

            _logger.LogDebug("Health check completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            healthStatus.IsHealthy = false;
            healthStatus.Message = $"Health check failed: {ex.Message}";
            healthStatus.Details["error"] = ex.ToString();
        }

        return healthStatus;
    }

    private static string? GetVersion()
    {
        try
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return version?.ToString();
        }
        catch
        {
            return null;
        }
    }
}

