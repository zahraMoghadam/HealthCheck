using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SystemSentinel.BaseHealthCheck.Module.Model;
using SystemSentinel.Common.Utilities;
using HC=Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthCheckResult = SystemSentinel.BaseHealthCheck.Module.Model.HealthCheckResult;

namespace SystemSentinel.BaseHealthCheck.Module.Extentions
{
    public abstract class CachedHealthCheck
    {
        private static readonly TypeInfo HealthCheckTypeInfo = typeof(IHealthCheck).GetTypeInfo();

        private volatile int _writerCount;

        public CachedHealthCheck(string name, TimeSpan cacheDuration)
        {
            Guard.ArgumentNotNullOrEmpty(nameof(name), name);
            Guard.ArgumentValid(cacheDuration.TotalMilliseconds >= 0, nameof(cacheDuration), "Cache duration must be zero (disabled) or greater than zero.");

            Name = name;
            CacheDuration = cacheDuration;
        }

        public IHealthCheckResult CachedResult { get; internal set; }

        public TimeSpan CacheDuration { get; }

        public DateTimeOffset CacheExpiration { get; internal set; }

        public string Name { get; }

        protected virtual DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

        protected abstract IHealthCheck Resolve(IServiceProvider serviceProvider);

        public async ValueTask<IHealthCheckResult> RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default(CancellationToken))
        {
            while (CacheExpiration <= UtcNow)
            {
                if (Interlocked.Exchange(ref _writerCount, 1) != 0)
                {
                    await Task.Delay(5, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                try
                {
                    var check = Resolve(serviceProvider);
                    CachedResult = await check.CheckAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    CachedResult = HealthCheckResult.Unhealthy("The health check operation timed out");
                }
                catch (Exception ex)
                {
                    CachedResult = HealthCheckResult.Unhealthy($"Exception during check: {ex.GetType().FullName}");
                }

                CacheExpiration = UtcNow + CacheDuration;
                _writerCount = 0;
                break;
            }

            return CachedResult;
        }

        public static CachedHealthCheck FromHealthCheck(string name, TimeSpan cacheDuration, IHealthCheck healthCheck)
        {
            Guard.ArgumentNotNull(nameof(healthCheck), healthCheck);

            return new TypeOrHealthCheck_HealthCheck(name, cacheDuration, healthCheck);
        }

        public static CachedHealthCheck FromType(string name, TimeSpan cacheDuration, Type healthCheckType)
        {
            Guard.ArgumentNotNull(nameof(healthCheckType), healthCheckType);
            Guard.ArgumentValid(HealthCheckTypeInfo.IsAssignableFrom(healthCheckType.GetTypeInfo()), nameof(healthCheckType), $"Health check must implement '{typeof(IHealthCheck).FullName}'.");

            return new TypeOrHealthCheck_Type(name, cacheDuration, healthCheckType);
        }

        class TypeOrHealthCheck_HealthCheck : CachedHealthCheck
        {
            private readonly IHealthCheck _healthCheck;

            public TypeOrHealthCheck_HealthCheck(string name, TimeSpan cacheDuration, IHealthCheck healthCheck) : base(name, cacheDuration)
                => _healthCheck = healthCheck;

            protected override IHealthCheck Resolve(IServiceProvider serviceProvider) => _healthCheck;
        }

        class TypeOrHealthCheck_Type : CachedHealthCheck
        {
            private readonly Type _healthCheckType;

            public TypeOrHealthCheck_Type(string name, TimeSpan cacheDuration, Type healthCheckType) : base(name, cacheDuration)
                => _healthCheckType = healthCheckType;

            protected override IHealthCheck Resolve(IServiceProvider serviceProvider)
                => (IHealthCheck)serviceProvider.GetRequiredService(_healthCheckType);
        }
    }
}

