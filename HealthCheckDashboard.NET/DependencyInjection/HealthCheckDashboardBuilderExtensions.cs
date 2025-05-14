namespace HealthCheckDashboard.NET;

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;

public static class HealthCheckDashboardBuilderExtensions
{
    /// <summary>
    /// Adds a health check dashboard to the IEndpointRouteBuilder with the specified template.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> the dashboard shoUld be added to.</param>
    /// <param name="pattern">The url-pattern for the dashboard.</param>
    /// <param name="configure">Optional dashboard configuration.</param>
    /// <returns></returns>
    public static IEndpointConventionBuilder MapHealthCheckDashboard(
        this WebApplication app,
        [StringSyntax("Route")] string pattern = "/health-dashboard",
        Action<HealthCheckDashboardOptions>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);
        if (!pattern.StartsWith('/'))
        {
            pattern = "/" + pattern;
        }

        var options = new HealthCheckDashboardOptions();
        configure?.Invoke(options);

        var assembly = typeof(HealthCheckDashboardBuilderExtensions).Assembly;
        var embeddedProvider = new ManifestEmbeddedFileProvider(assembly, "health-dashboard");
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = embeddedProvider,
            RequestPath = pattern,
        });

        var routeGroup = app.MapGroup(pattern);

        // Serve dashboard html files
        routeGroup.MapGet(string.Empty, async (ctx) =>
        {
            ctx.Response.Redirect($"{pattern}/index.html");
        });

        // Serve dashboard endpoint
        routeGroup.MapGet("data", async (HealthCheckService healthCheckService) =>
        {
            var report = await healthCheckService.CheckHealthAsync(options.Predicate);
            return Results.Json(new
            {
                Status = report.Status.ToString(),
                Checks = report.Entries.Select(entry => new
                {
                    Name = entry.Key,

                    Status = entry.Value.Status.ToString(),
                    Description = entry.Value.Description,
                    Duration = entry.Value.Duration,
                    entry.Value.Tags,
                    entry.Value.Data,
                }),
            });
        });

        return routeGroup;
    }
}
