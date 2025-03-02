using HealchCheck.Data;
using HealthCheck.Filters;
using HealthCheck.HealthCheck;
using HealthCheck.HealthCheck.HealthCheck;
using HealthChecks.UI.Client;
using HealthChecks.UI.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

namespace HealthCheck
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
           .AddEntityFrameworkStores<ApplicationDbContext>();
           services.AddDbContext<ApplicationDbContext>(options =>
           options.UseSqlServer(connectionString));
            services.AddControllersWithViews();
            services.AddControllers(options =>
            {
                // ثبت فیلتر HealthCheckAttribute برای استفاده در کنترلرها
                //options.Filters.Add<HealthCheckAttribute>();
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HealthCheck", Version = "v1" });
            });

            var evaluationTimeInSeconds = 60;
            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Period = TimeSpan.FromSeconds(evaluationTimeInSeconds);
            });
            services.AddSingleton<IHealthCheckPublisher, CustomHealthCheckPublisher>();
            services.AddSingleton<IHealthCheck, HealthCheckHandler>();

            //////////////////////////////////////////////////////////////// Add Healch Check
            services
                .AddHealthChecksUI(options =>
                {
                    options.SetHeaderText("SRM Health Check");
                    options.AddHealthCheckEndpoint("بررسی سلامت API ها ", "/healthcheck/api");
                    options.AddHealthCheckEndpoint("بررسی سلامت سرویس ها", "/healthcheck/custom");
                    options.AddHealthCheckEndpoint("بررسی سلامت حافظه", "/healthcheck/memory");
                    options.AddHealthCheckEndpoint("بررسی سلامت دیتابیس SQL Server", "/healthcheck/database");
                    options.SetEvaluationTimeInSeconds(evaluationTimeInSeconds); //time in seconds between check    
                    options.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
                    options.SetApiMaxActiveRequests(1);
                    //options.AddCustomStylesheet("dotnet.css");

                    options.AddWebhookNotification("webhook1", uri: "https://healthchecks2.requestcatcher.com/",
                        payload: "{ message: \"Webhook report for [[LIVENESS]]: [[FAILURE]] - Description: [[DESCRIPTIONS]]\"}",
                            restorePayload: "{ message: \"[[LIVENESS]] is back to life\"}",
                            shouldNotifyFunc: (livenessName, report) => DateTime.UtcNow.Hour >= 8 && DateTime.UtcNow.Hour <= 23,
                            customMessageFunc: (livenessName, report) =>
                            {
                                var failing = report.Entries.Where(e => e.Value.Status == UIHealthStatus.Unhealthy);
                                return $"{failing.Count()} healthchecks are failing";
                            }, customDescriptionFunc: (livenessName, report) =>
                            {
                                var failing = report.Entries.Where(e => e.Value.Status == UIHealthStatus.Unhealthy);
                                return $"{string.Join(" - ", failing.Select(f => f.Key))} healthchecks are failing";
                            });

                    //Webhook endpoint with default failure and description messages

                    options.AddWebhookNotification(
                        name: "webhook1",
                        uri: "https://healthchecks.requestcatcher.com/",
                        payload: "{ message: \"Webhook report for [[LIVENESS]]: [[FAILURE]] - Description: [[DESCRIPTIONS]]\"}",
                        restorePayload: "{ message: \"[[LIVENESS]] is back to life\"}"); ; //api requests concurrency
                                                                                           //options.AddHealthCheckEndpoint("feedback api", "/api/health"); //map health check api
                                                                                           //options.AddCustomStylesheet("./HealthCheck/Custom.css");

                })
                   .AddInMemoryStorage();

            services
                .AddHealthChecks()
                .AddCheck<MemoryHealthCheck>("بررسی سلامت حافظه", tags: new[] { "memory" })
                .AddCheck<DbHealthCheck>("بررسی سلامت دیتابیس", tags: new[] { "database" })
                .AddCheck<CustomHealthCheck>("سرویس Custom", tags: new[] { "custom" })
                //.AddCheck<AttributeHealthCheck>("dynamic_attribute_health_check", tags: new[] { "dynamic actions" })
                .AddCheck("Example", () =>
                 HealthCheckResult.Healthy("Example is OK!"), tags: new[] { "api" })
               .AddCheck("Sample", () => HealthCheckResult.Healthy("A healthy result."), tags: new[] { "api" })
               .AddUrlGroup(new Uri("http://zahrapourmoghadam.ir/"), name: "Zahra Moghadam Website")
             .AddSqlServer(Configuration["ConnectionStrings:DefaultConnection"]);
            //.AddApplicationInsightsPublisher()
            // .AddCloudWatchPublisher()
            // .AddDatadogPublisher("myservice.healthchecks"); 
            //services.AddSingleton<CustomHealthCheckMiddleware>();

            services.AddSingleton<IHealthCheck, CustomHealthCheck>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins("https://example.com", "https://anotherexample.com")  // فقط این دامنه مجاز است
                           .AllowAnyMethod()                 // هر متدی مجاز است
                           .AllowAnyHeader();                // هر هدر مجاز است
                });
                services.AddRazorPages();
                //options.AddPolicy("AllowAll", builder =>
                //{
                //    builder.AllowAnyOrigin()  // همه دامنه‌ها مجاز هستند
                //           .AllowAnyMethod()  // هر متدی مجاز است
                //           .AllowAnyHeader(); // هر هدر مجاز است
                //});
                //options.AddPolicy("AllowCredentials", builder =>
                //{
                //    builder.WithOrigins("https://example.com")  // فقط این دامنه مجاز است
                //           .AllowAnyMethod()
                //           .AllowAnyHeader()
                //           .AllowCredentials();  // اجازه دادن به ارسال اطلاعات هویتی (کوکی‌ها، هدرهای احراز هویت)
                //});

                //options.AddPolicy("AllowSpecificMethods", builder =>
                //{
                //    builder.WithOrigins("https://example.com")
                //           .WithMethods("GET", "POST")  // فقط GET و POST مجاز هستند
                //           .AllowAnyHeader();
                //});

                //options.AddPolicy("AllowSpecificHeaders", builder =>
                //{
                //    builder.WithOrigins("https://example.com")
                //           .WithHeaders("Content-Type", "Authorization")  // فقط این هدرها مجاز هستند
                //           .AllowAnyMethod();
                //});

                //options.AddPolicy("AllowWithPreflightCache", builder =>
                //{
                //    builder.WithOrigins("https://example.com")
                //           .AllowAnyMethod()
                //           .AllowAnyHeader()
                //           .SetPreflightMaxAge(TimeSpan.FromMinutes(10));  // مدت زمان کش شدن پیش‌بررسی ۱۰ دقیقه
                //});

            });


            /////////////////////////////////////////////////////////////////////// Add Healch Check
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HealthCheck v1"));
                app.UseMigrationsEndPoint();
            }

            app.UseHttpsRedirection();


            app.UseRouting();
            app.UseMetricServer();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            app.UseCors("AllowSpecificOrigin");  // استفاده از پالیسی CORS
                                                 //app.UseCors("AllowAll");
            app.UseHealthChecks("/healthcheck/custom", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("custom"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/healthcheck/memory", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("memory"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse

            });
            app.UseHealthChecks("/healthcheck/database", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("database"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecks("/healthcheck/api", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("api"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/healthcheck/dynamic actions", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("dynamic actions"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksPrometheusExporter("/metrics",
                options => options.ResultStatusCodes[HealthStatus.Unhealthy] = (int)HttpStatusCode.OK);

            app.UseEndpoints(endpoints =>
            {
               
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                 name: "default",
                 pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapMetrics();

                endpoints.MapHealthChecks("/healthcheck",
               new HealthCheckOptions
               {
                   Predicate = _ => true,
                   AllowCachingResponses = false,
                   ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                   ResultStatusCodes =
               {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                 }
               });
                endpoints.MapHealthChecksUI(options => { options.UIPath = "/dashboard"; options.AsideMenuOpened = true; });
                endpoints.MapRazorPages();
            });
           
        }
    }
}
