using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemSentinel.BaseHealthCheck.Module;

namespace SystemSentinel.Default.HealthCheckModule.HealthCheckServices
{
    public class DatabaseHealthCheckModule : IHealthCheckModule
    {
        public string ModuleName => "Database Health";

        public string ProjectName => "Default";


        public async Task<HealthCheckStatusResult> CheckHealthAsync()
        {
            try
            {
               
              
                return new HealthCheckStatusResult
                {
                    IsHealthy = true,
                    StatusMessage = "Database is online"
                };
            }
            catch (Exception ex)
            {
                return new HealthCheckStatusResult
                {
                    IsHealthy = false,
                    StatusMessage = $"Error: {ex.Message}"
                };
            }
        }

        Task<HealthCheckStatusResult> IHealthCheckModule.CheckHealthAsync()
        {
            throw new NotImplementedException();
        }
    }
}
