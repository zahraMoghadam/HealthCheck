using System.Net;

namespace SystemSentinel.BaseHealthCheck.Module
{
    public class HealthCheckStatusResult
    {
        private bool isHealthy;

        public string? Url { get; set; }
        public bool IsHealthy { get => isHealthy; set => isHealthy = value; }
        public string? StatusMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public DateTime? ResponseTime { get; set; }
        public string? ErrorMessage { get; set; }
    }
}