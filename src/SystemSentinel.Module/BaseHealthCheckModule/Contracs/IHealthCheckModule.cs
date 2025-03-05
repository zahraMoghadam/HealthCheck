using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemSentinel.BaseHealthCheck.Module.Contracs
{
    public interface IHealthCheckModule
    {
        Task<HealthCheckStatusResult> CheckHealthAsync();
        string ModuleName { get; }
        string ProjectName { get; }
    }
}
