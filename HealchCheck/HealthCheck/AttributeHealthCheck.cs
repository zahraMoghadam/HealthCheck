using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.Extensions.HealthChecks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace HealthCheck.HealthCheck
{


    //    public class AttributeHealthCheck : IHealthCheck
    //    {
    //        private readonly IServiceProvider _serviceProvider;

    //        public AttributeHealthCheck(IServiceProvider serviceProvider)
    //        {
    //            _serviceProvider = serviceProvider;
    //        }

    //        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    //        {
    //            var assemblies = AppDomain.CurrentDomain.GetAssemblies();





    //            var controllerTypes = typeof(Startup).Assembly.GetTypes().Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract).ToList();
    //            foreach (var type in controllerTypes)
    //            { var controller = type.Name; if (controller == null) 
    //                { continue; }
    //                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where(m => m.GetCustomAttributes(typeof(HealthCheckAttribute), false).Any());
    //                foreach (var method in methods) 
    //                { var result = method.Invoke(type.Name, null) as IActionResult;
    //                    if (result is not OkObjectResult) { return HealthCheckResult.Unhealthy($"{type.Name}. failed."); } } }
    //            return HealthCheckResult.Healthy("All actions are healthy.");
    //        }


    //    }
    //}
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class HealthCheckAttribute : Attribute, IActionFilter
    {
        private readonly IEnumerable<IHealthCheckService> _services;
        private readonly ILogger<HealthCheckAttribute> _logger;

        public HealthCheckAttribute(IEnumerable<IHealthCheckService> services, ILogger<HealthCheckAttribute> logger)
        {
            _services = services;
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            throw new NotImplementedException();
        }

        public   void OnActionExecuting(ActionExecutedContext context)
        {
            var healthCheckResults =  Task.WhenAll(_services.Select(s => s.CheckHealthAsync())).Result;
            var unhealthyServices = healthCheckResults.Where(r => r.CheckStatus != CheckStatus.Healthy).Select(r => JObject.Parse(r.ToString())).ToList();

            if (unhealthyServices.Any())
            {
                var responseContent = JsonSerializer.Serialize(new { UnhealthyServices = unhealthyServices });
                _logger.LogError("One or more services are unhealthy.");
                context.Result = new ObjectResult(responseContent) { StatusCode = (int)HttpStatusCode.ServiceUnavailable };
            }
            else
            {
                _logger.LogInformation("All services are healthy.");
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            throw new NotImplementedException();
        }
    }

}

//public void OnActionExecuting(ActionExecutingContext context)
//{
//    var serviceProvider = context.HttpContext.RequestServices;
//    var healthCheck = serviceProvider.GetService<IHealthCheck>();

//    var result = healthCheck.CheckHealthAsync(new HealthCheckContext()).Result;
//    if (!result.Status.Equals(HealthStatus.Healthy))
//    {
//        context.Result = new ContentResult
//        {
//            StatusCode = (int)HttpStatusCode.ServiceUnavailable,
//            Content = "سلامت سیستم تأیید نشد"
//        };
//    }
//}

