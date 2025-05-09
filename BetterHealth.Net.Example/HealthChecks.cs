using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BetterHealth.Net.Example
{
    public class SimpleSuccessHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy("Ok ✅"));
        }
    }

    public class SimpleDegradedHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Degraded("Not so good"));
        }
    }

    public class SimpleFailedHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Not too great ❌"));
        }
    }

    public class SimpleExceptionHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            throw new Exception("EXCEPTION THROWN!");
        }
    }
}
