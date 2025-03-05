using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemSentinel.Common.Contracts
{
    public class IDefinedHealthCheckModule
    {
        private string? ProjectName { get; }
        string? ModuleName { get; }
    }
}
