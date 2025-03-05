using SystemSentinel.Core.Module;

namespace SystemSentinel.Default.HealthCheckModule.HealthCheckServices
{
    public class HttpApiHealthCheckModule : IHealthCheckModule
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;

        public HttpApiHealthCheckModule(HttpClient httpClient, string apiUrl)
        {
            _httpClient = httpClient;
            _apiUrl = apiUrl;
        }

        public async Task<HealthCheckResult> CheckHealthAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiUrl);
                return new HealthCheckResult
                {
                    IsHealthy = response.IsSuccessStatusCode,
                    StatusCode = response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new HealthCheckResult
                {
                    IsHealthy = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}