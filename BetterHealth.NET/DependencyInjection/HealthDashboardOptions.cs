namespace BetterHealth.NET;

using System.Diagnostics.CodeAnalysis;

public class HealthDashboardOptions
{
    [StringSyntax("Route")]
    public string Pattern { get; set; } = "/health-dashboard";

    public bool WithLogInterception { get; set; }
}
