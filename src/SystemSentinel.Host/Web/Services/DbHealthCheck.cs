using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SystemSentinel.Services
{
    public class DbHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                //using (AppContext )
                //{
                //    if (pgSqlConnection.State !=
                //        System.Data.ConnectionState.Open)
                //        pgSqlConnection.Open();

                //    if (pgSqlConnection.State == System.Data.ConnectionState.Open)
                //    {
                //        pgSqlConnection.Close();
                //        return Task.FromResult(
                //        HealthCheckResult.Healthy("The database is up and running."));
                //    }
                //}

                return Task.FromResult(
                      new HealthCheckResult(
                      context.Registration.FailureStatus, "The database is down."));
            }
            catch (Exception)
            {
                return Task.FromResult(
                    new HealthCheckResult(
                        context.Registration.FailureStatus, "The database is down."));
            }
        }
    }
}