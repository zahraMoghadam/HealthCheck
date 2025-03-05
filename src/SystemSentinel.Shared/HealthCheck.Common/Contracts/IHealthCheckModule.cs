using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemSentinel.Common.Models;

namespace SystemSentinel.Common.Contracts
{
    public class IHealthCheckModule
    {
       private string? ProjectName { get; }
        string?  ModuleName { get; }
        Task<HealthCheckResult>? CheckHealthAsync { get; }
    }
}
