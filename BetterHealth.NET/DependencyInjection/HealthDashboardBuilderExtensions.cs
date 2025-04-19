namespace BetterHealth.NET;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

public static class HealthDashboardBuilderExtensions
{
    public static IEndpointConventionBuilder UseHealthDashboard(this WebApplication app,
    Action<HealthDashboardOptions>? configure = null)
    {
        var options = new HealthDashboardOptions();
        configure?.Invoke(options);

        if (!options.Pattern.StartsWith('/'))
        {
            options.Pattern = "/" + options.Pattern;
        }

        var assembly = typeof(HealthDashboardBuilderExtensions).Assembly;
        var embeddedProvider = new ManifestEmbeddedFileProvider(assembly, "health-dashboard");
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = embeddedProvider,
            RequestPath = options.Pattern,
        });

        var routeGroup = app.MapGroup(options.Pattern);

        // Serve dashboard html files
        routeGroup.MapGet(string.Empty, async (ctx) =>
        {
            ctx.Response.Redirect($"{options.Pattern}/index.html");
        });

        // Serve dashboard endpoint
        routeGroup.MapGet("data", async (HealthCheckService healthCheckService) =>
        {
            var report = await healthCheckService.CheckHealthAsync();
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
