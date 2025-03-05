using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemSentinel.BaseHealthCheck.Module;

namespace SystemSentinel.Default.HealthCheckModule.HealthCheckServices
{
    public class LinkHealthCheckModule : IHealthCheckModule
    {
        public string ModuleName => "Link Health";

        public string ProjectName => "Default";

        public async Task<HealthCheckStatusResult> CheckHealthAsync()
        {
            var linksToCheck = new List<string> { "http://example.com", "http://another-link.com" };
            var unhealthyLinks = new List<string>();

            foreach (var link in linksToCheck)
            {
                try
                {
                    var response = await new HttpClient().GetAsync(link);
                    if (!response.IsSuccessStatusCode)
                    {
                        unhealthyLinks.Add(link);
                    }
                }
                catch
                {
                    unhealthyLinks.Add(link);
                }
            }

            return new HealthCheckStatusResult
            {
                IsHealthy = !unhealthyLinks.Any(),
                StatusMessage = unhealthyLinks.Count == 0 ? "All links are healthy" : $"Unhealthy links: {string.Join(", ", unhealthyLinks)}"
            };
        }
    }
}
