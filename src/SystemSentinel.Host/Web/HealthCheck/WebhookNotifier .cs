using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace SystemSentinel.HealthCheck
{
    public class WebhookNotifier : IHealthCheckPublisher
    {
        private readonly HttpClient _httpClient;

        public WebhookNotifier(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

       

        Task IHealthCheckPublisher.PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}