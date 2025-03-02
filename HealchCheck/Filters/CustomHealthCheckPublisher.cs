using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using Prometheus;

namespace HealthCheck.Filters
{
    public class CustomHealthCheckPublisher : IHealthCheckPublisher
    {
        private readonly ILogger<CustomHealthCheckPublisher> _logger;
        protected readonly CollectorRegistry Registry;
        public CustomHealthCheckPublisher(ILogger<CustomHealthCheckPublisher> logger)
        {
            _logger = logger;
            Registry = Metrics.DefaultRegistry;
            var factory = Metrics.WithCustomRegistry(Registry);
        }

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            // ثبت وضعیت Health Check‌ها در لاگ‌ها
            _logger.LogInformation("Health Check Status: {Status}", report.Status);
           
           
            var nonhealthyChecks = report.Entries
            .Where(x => x.Value.Status != HealthStatus.Healthy)
            .Select(x => new { x.Key, x.Value.Status, x.Value.Description })
            .ToList();

            if (nonhealthyChecks.Any())
            {
                // ارسال گزارش سلامت به سرویس‌های مانیتورینگ
                foreach (var entry in nonhealthyChecks)
                {
                    Console.WriteLine($"Check: {entry.Key}, Status: {entry.Status}");
                    _logger.LogInformation("Health Check '{CheckName}': {Status} - {Description}",
                    entry.Key,
                    entry.Status,
                    entry.Description);
                }
            }


           

            foreach (var entry in report.Entries)
            {
               
            }

            return Task.CompletedTask;
        }
    }
}
