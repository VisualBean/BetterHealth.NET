namespace HealthCheckDashboard.NET;

using Microsoft.Extensions.Diagnostics.HealthChecks;

public class HealthCheckDashboardOptions
{
    /// <summary>
    /// Gets or sets a predicate that is used to filter the set of health checks executed.
    /// </summary>
    public Func<HealthCheckRegistration, bool>? Predicate { get; set; }
}
