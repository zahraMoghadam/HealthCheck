using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemSentinel.BaseHealthCheck.Module;

namespace SystemSentinel.Default.HealthCheckModule.HealthCheckServices
{
    public class MemoryHealthCheckModule : IHealthCheckModule
    {
        public string ModuleName => "Memory Health";

        public string ProjectName =>"Default";

        public async Task<HealthCheckStatusResult> CheckHealthAsync()
        {
            var totalMemory = GC.GetTotalMemory(false);
            var threshold = 500000000; // Set your own memory threshold

            return new HealthCheckStatusResult
            {
                IsHealthy = totalMemory < threshold,
                StatusMessage = $"Total Memory: {totalMemory / 1024 / 1024} MB"
            };
        }
    }
}
