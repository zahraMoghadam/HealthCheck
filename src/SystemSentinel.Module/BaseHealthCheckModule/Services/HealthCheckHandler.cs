using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SystemSentinel.BaseHealthCheck.Module.Attributes;
using SystemSentinel.BaseHealthCheck.Module.Contracts;

namespace SystemSentinel.BaseHealthCheck.Module.Services
{
    public class HealthCheckHandler: IHealthCheckHandler
    {
        private readonly IServiceProvider _serviceProvider;

        public HealthCheckHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }

        public List<ControllerBase?> GetAllControllers()
        {

            return GetSubClasses<ControllerBase>()
             .Select(item =>
             {
                 if (item is null)
                 {
                     throw new ArgumentNullException(nameof(item));
                 }

                 return Activator.CreateInstance(item) as ControllerBase;
             })
             .Where(controller => controller != null)
             .ToList();
        }
        //گرفتن لیست کنترلر هایی که Attribute برای آن ها تعریف شده است.
        public async Task<List<HealthCheckDynamicAttribute>> GetControllersCustomAttributes()
        {

            return await Task.Run(() => GetSubClasses<ControllerBase>()
                 .SelectMany(controller => controller.GetCustomAttributes<HealthCheckDynamicAttribute>())
                 .ToList());
        }

        public async Task<List<HealthCheckDynamicAttribute>> GetControllersActions()
        {
            return await Task.Run(() => GetSubClasses<ControllerBase>()
              .SelectMany(controller => controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                  .SelectMany(method => method.GetCustomAttributes<HealthCheckDynamicAttribute>())
              ).ToList());
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // لیستی برای ذخیره نتایج سلامت هر کنترلر
            var healthCheckResults = new List<object>(); // شیء برای ذخیره نتایج
            var duration = DateTime.UtcNow; // زمان شروع بررسی
            var ControllersWithAttribute = await GetControllersCustomAttributes();
            var ControllersActions = await GetControllersActions();
            var controllerHealthCheckResults = new List<HealthCheckResult>();
            var details = new List<string>();

            foreach (var customAttribute in ControllersWithAttribute)
            {

                if (customAttribute != null)
                {
                    // Return healthy or unhealthy status based on custom logic
                    var result = await Task.FromResult(HealthCheckResult.Healthy($"Health check passed for controller: {customAttribute.Name}"));
                    controllerHealthCheckResults.Add(result);

                    // ایجاد یک شیء برای هر کنترلر و وضعیت آن
                    var controllerResult = new
                    {
                        controller = customAttribute.Name,
                        status = result.Status.ToString(),
                        description = $"Controller {customAttribute.Name}: {result.Status}",
                        duration = DateTime.UtcNow - duration, // محاسبه مدت زمان
                        tags = new List<string>() // می‌توانید برچسب‌ها را در اینجا اضافه کنید
                    };

                    healthCheckResults.Add(controllerResult);

                }
                Console.Write(customAttribute);
            }


            foreach (var action in ControllersActions)
            {

                if (action != null)
                {
                    // Return healthy or unhealthy status based on custom logic
                    var result = await Task.FromResult(HealthCheckResult.Healthy($"Health check passed for action: {action.Name}"));
                    controllerHealthCheckResults.Add(result);
                    // ایجاد یک شیء برای هر کنترلر و وضعیت آن
                    var controllerResult = new
                    {
                        controller = action.Name,
                        status = result.Status.ToString(),
                        description = $"Controller {action.Name}: {result.Status}",
                        duration = DateTime.UtcNow - duration, // محاسبه مدت زمان
                        tags = new List<string>() // می‌توانید برچسب‌ها را در اینجا اضافه کنید
                    };

                    healthCheckResults.Add(controllerResult);
                }
                Console.Write(action);
            }
            // ساختار نهایی که شامل تمامی نتایج سلامت است
            var finalResult = new
            {
                status = healthCheckResults.All(r => ((dynamic)r).status == "Healthy") ? "Healthy" : "Unhealthy",
                description = string.Join("\n", healthCheckResults.Select(r => ((dynamic)r).description)),
                data = healthCheckResults, // لیست نتایج سلامت برای هر کنترلر
                duration = DateTime.UtcNow - duration, // مدت زمان کل بررسی‌ها
                tags = new List<string>() // برچسب‌های اضافی در صورت نیاز
            };
            // ساختار نهایی که شامل تمامی نتایج سلامت است
            var finalUnhealthyResult = new
            {
                status = healthCheckResults.All(r => ((dynamic)r).status == "Unhealthy") ? "Unhealthy" : "Healthy",
                description = string.Join("\n", healthCheckResults.Select(r => ((dynamic)r).description)),
                data = healthCheckResults, // لیست نتایج سلامت برای هر کنترلر
                duration = DateTime.UtcNow - duration, // مدت زمان کل بررسی‌ها
                tags = new List<string>() // برچسب‌های اضافی در صورت نیاز
            };

            // برای تست، چاپ داده‌ها در کنسول
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(finalResult, Newtonsoft.Json.Formatting.Indented));

            // بازگشت نتیجه کلی
            return finalResult.status == "Healthy"
                ? HealthCheckResult.Healthy(finalResult.ToString())
                : HealthCheckResult.Unhealthy(finalUnhealthyResult.ToString());

        }
    }
}
    

