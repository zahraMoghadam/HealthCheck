using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using HC=Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemSentinel.BaseHealthCheck.Module.Extentions;
using SystemSentinel.BaseHealthCheck.Module.Model;
using SystemSentinel.Common.Utilities;

namespace SystemSentinel.BaseHealthCheck.Module.Middleware
{
    public static partial class HealthCheckBuilderExtensions
    {
       
        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckResult> check)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return builder.AddCheck(name, HealthCheck.FromCheck(check), builder.DefaultCacheDuration);
        }

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<CancellationToken, IHealthCheckResult> check)
        {
             Guard.ArgumentNotNull(nameof(builder), builder);

            return builder.AddCheck(name, HealthCheck.FromCheck(check), builder.DefaultCacheDuration);
        }

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<IHealthCheckResult> check, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);
            return builder.AddCheck(name, HealthCheck.FromCheck(check), cacheDuration);
        }
        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<CancellationToken, IHealthCheckResult> check, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);
            return builder.AddCheck(name, HealthCheck.FromCheck(check), cacheDuration);
        }

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<Task<IHealthCheckResult>> check)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return builder.AddCheck(name, HealthCheck.FromTaskCheck(check), builder.DefaultCacheDuration);
        }

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<CancellationToken, Task<IHealthCheckResult>> check)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return builder.AddCheck(name, HealthCheck.FromTaskCheck(check), builder.DefaultCacheDuration);
        }

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<Task<IHealthCheckResult>> check, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);
            return builder.AddCheck(name, HealthCheck.FromTaskCheck(check), cacheDuration);
        }
        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<CancellationToken, Task<IHealthCheckResult>> check, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return builder.AddCheck(name, HealthCheck.FromTaskCheck(check), cacheDuration);
        }

        public static HealthCheckBuilder AddValueTaskCheck(this HealthCheckBuilder builder, string name, Func<ValueTask<IHealthCheckResult>> check)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return builder.AddCheck(name, HealthCheck.FromValueTaskCheck(check), builder.DefaultCacheDuration);
        }
        public static HealthCheckBuilder AddValueTaskCheck(this HealthCheckBuilder builder, string name, Func<CancellationToken, ValueTask<IHealthCheckResult>> check)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return builder.AddCheck(name, HealthCheck.FromValueTaskCheck(check), builder.DefaultCacheDuration);
        }
        public static HealthCheckBuilder AddValueTaskCheck(this HealthCheckBuilder builder, string name, Func<ValueTask<IHealthCheckResult>> check, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return builder.AddCheck(name, HealthCheck.FromValueTaskCheck(check), cacheDuration);
        }
        public static HealthCheckBuilder AddValueTaskCheck(this HealthCheckBuilder builder, string name, Func<CancellationToken, ValueTask<IHealthCheckResult>> check, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return builder.AddCheck(name, HealthCheck.FromValueTaskCheck(check), cacheDuration);
        }

        // IHealthCheck versions of AddCheck

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string checkName, IHealthCheck check)
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return builder.AddCheck(checkName, check, builder.DefaultCacheDuration);
        }

        // Type versions of AddCheck

        public static HealthCheckBuilder AddCheck<TCheck>(this HealthCheckBuilder builder, string name) where TCheck : class, IHealthCheck
        {
            Guard.ArgumentNotNull(nameof(builder), builder);

            return builder.AddCheck<TCheck>(name, builder.DefaultCacheDuration);
        }
    }
}
