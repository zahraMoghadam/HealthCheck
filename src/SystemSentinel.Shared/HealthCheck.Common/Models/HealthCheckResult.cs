using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemSentinel.Common.Models
{
    public class HealthCheckResult
    {
        public string? SystemName { get; set; }
        public string? ModuleName { get; set; }
        public string? Status { get; set; } 
        public string? Details { get; set; }
        public DateTime CheckedAt { get; set; }
    }
}
