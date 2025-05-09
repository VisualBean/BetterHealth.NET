using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BetterHealth.NET;

public class HealthDashboardOptions
{
    /// <summary>
    /// Gets or sets a predicate that is used to filter the set of health checks executed.
    /// </summary>
    public Func<HealthCheckRegistration, bool>? Predicate { get; set; }
}
