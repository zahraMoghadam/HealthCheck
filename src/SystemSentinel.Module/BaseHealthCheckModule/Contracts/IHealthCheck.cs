using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemSentinel.BaseHealthCheck.Module.Model;
using HC = Microsoft.Extensions.Diagnostics.HealthChecks;
using MC=Microsoft.Extensions;

namespace SystemSentinel.BaseHealthCheck.Module.Extentions
{
    public interface IHealthCheck
    {
        ValueTask<IHealthCheckResult> CheckAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
