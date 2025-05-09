using System.Diagnostics.CodeAnalysis;

namespace BetterHealth.NET;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;

public static class HealthDashboardBuilderExtensions
{
    public static IEndpointConventionBuilder MapHealthDashboard(this WebApplication app,
    [StringSyntax("Route")] string pattern = "/health-dashboard",
    Action<HealthDashboardOptions>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);
        if (!pattern.StartsWith('/'))
        {
            pattern = "/" + pattern;
        }

        var options = new HealthDashboardOptions();
        configure?.Invoke(options);

        var assembly = typeof(HealthDashboardBuilderExtensions).Assembly;
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
