using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.HealthChecks;
using System.Threading.Tasks;

namespace SystemSentinel.HealthCheck
{
    public class CustomHealthCheckMiddleware
    {
        
        private readonly IHealthCheckService _healthCheckService;

        // سازنده Middleware
        public CustomHealthCheckMiddleware( IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        // متد InvokeAsync برای پردازش درخواست‌ها
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path == "/health")
            {
                // اجرای Health Check و دریافت گزارش
                var healthReport = await _healthCheckService.CheckHealthAsync();

                // تبدیل گزارش به JSON
                var result = System.Text.Json.JsonSerializer.Serialize(healthReport);

                // تنظیم نوع محتوا و ارسال پاسخ
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }
            else
            {
                // اگر مسیر درخواست "/health" نبود، به Middleware بعدی منتقل شو
                await next(context);
            }
        }
    }
}
