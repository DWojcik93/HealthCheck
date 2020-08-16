using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheck
{
    public class ICMPHealthCheck : IHealthCheck
    {
        private string Host { get; set; }
        private int Timeout { get; set; }
        private string Message { get; set; }

        public ICMPHealthCheck(string host, int timeout)
        {
            Host = host;
            Timeout = timeout;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync(Host);
                    switch (reply.Status)
                    {
                        case IPStatus.Success:
                            Message = string.Format($"IMCP to {Host} took {Timeout} ms.");
                            return (reply.RoundtripTime > Timeout)
                            ? HealthCheckResult.Degraded(Message)
                            : HealthCheckResult.Healthy(Message);
                        default:
                            Message = string.Format($"IMCP to {Host} failed: {reply.Status}");
                            return HealthCheckResult.Unhealthy(Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Message = string.Format($"IMCP to {Host} failed: {ex.Message}");
                return HealthCheckResult.Unhealthy(Message);
            }
        }
    }
}
