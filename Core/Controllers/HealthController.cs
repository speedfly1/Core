using Microsoft.AspNetCore.Mvc;
using Core.Health;

namespace Core.Controllers;

/// <summary>
/// Health check controller for microservice health endpoints
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly IHealthChecker _healthChecker;
    private readonly ILogger<HealthController> _logger;

    public HealthController(IHealthChecker healthChecker, ILogger<HealthController> logger)
    {
        _healthChecker = healthChecker;
        _logger = logger;
    }

    /// <summary>
    /// Simple health check endpoint
    /// </summary>
    /// <returns>200 OK if healthy, 503 Service Unavailable if unhealthy</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var isHealthy = await _healthChecker.IsHealthyAsync();
            
            if (isHealthy)
            {
                return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
            }
            else
            {
                return StatusCode(503, new { status = "unhealthy", timestamp = DateTime.UtcNow });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during health check");
            return StatusCode(503, new { status = "error", message = ex.Message, timestamp = DateTime.UtcNow });
        }
    }

    /// <summary>
    /// Detailed health check endpoint
    /// </summary>
    /// <returns>Detailed health status information</returns>
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailed()
    {
        try
        {
            var healthStatus = await _healthChecker.GetHealthStatusAsync();
            
            if (healthStatus.IsHealthy)
            {
                return Ok(healthStatus);
            }
            else
            {
                return StatusCode(503, healthStatus);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during detailed health check");
            return StatusCode(503, new HealthStatus
            {
                IsHealthy = false,
                Message = $"Health check error: {ex.Message}",
                Timestamp = DateTime.UtcNow,
                Details = new Dictionary<string, object> { ["error"] = ex.ToString() }
            });
        }
    }

    /// <summary>
    /// Readiness check endpoint
    /// </summary>
    /// <returns>200 OK if ready to serve requests</returns>
    [HttpGet("ready")]
    public IActionResult GetReady()
    {
        // Simple readiness check - service is ready if it can respond
        return Ok(new { status = "ready", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Liveness check endpoint
    /// </summary>
    /// <returns>200 OK if service is alive</returns>
    [HttpGet("live")]
    public IActionResult GetLive()
    {
        // Simple liveness check - service is alive if it can respond
        return Ok(new { status = "alive", timestamp = DateTime.UtcNow });
    }
}

