

using SystemSentinel.BaseHealthCheck.Module;


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

        public string ModuleName => "Default HealthCheck";

        public string ProjectName => "Api HealthCheck";

        public async Task<HealthCheckStatusResult> CheckHealthAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiUrl);
                return new HealthCheckStatusResult
                {
                    IsHealthy = response.IsSuccessStatusCode,
                    StatusCode = response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new HealthCheckStatusResult
                {
                    IsHealthy = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}