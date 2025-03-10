using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SystemSentinel.BaseHealthCheck.Module.Attributes;

namespace SystemSentinel.Default.HealthCheckModule.APi
{
    [HealthCheckDynamic("WeatherForecastController")]
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultController
   : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<DefaultController> _logger;

        public DefaultController(ILogger<DefaultController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ServiceFilter(typeof(HealthCheckDynamicAttribute))]
        public string Get()
        {
            //var rng = new Random();
            //var res= Enumerable.Range(1, 5).Select(index => new 
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //})
            //.ToArray();
            return "-1";
        }

        [HealthCheckDynamic("Action2")]
        [HttpGet("action2")]
        //[ServiceFilter(typeof(HealthCheckDynamicAttribute))]
        public IActionResult Action2()
        { return Ok(); }
    }
}
