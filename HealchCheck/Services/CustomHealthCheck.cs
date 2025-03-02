using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealchCheck.Services
{
    public class CustomHealthCheck : IHealthCheck
    {
       
            private readonly ILogger<CustomHealthCheck> _logger;

            public CustomHealthCheck(ILogger<CustomHealthCheck> logger)
            {
                _logger = logger;
            }
            public Task<HealthCheckResult> CheckHealthAsync(
               HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                try
                {
                    // منطق چک سلامت
                    _logger.LogInformation("Health check passed.");
                    return Task.FromResult(HealthCheckResult.Healthy("Custom health check passed"));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Health check failed.");
                    return Task.FromResult(HealthCheckResult.Unhealthy("Health check failed"));
                }
            }
        }
    }
