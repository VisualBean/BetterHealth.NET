namespace HealthCheckDashboard.NET.Example;

using HealthCheckDashboard.NET;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
            
        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddHealthChecks()
            .AddCheck<SimpleSuccessHealthCheck>("Simple Success", tags: new[] {"Simple", "Success"})
            .AddCheck<SimpleDegradedHealthCheck>("Simple Degraded", tags: new[] {"Simple", "Degraded"})
            .AddCheck<SimpleFailedHealthCheck>("Simple Failed", tags: new[] {"Simple", "Failed"})
            .AddCheck<SimpleExceptionHealthCheck>("Simple Exception", tags: new[] {"Simple", "Exception"});
           
        var app = builder.Build();

        app.MapHealthCheckDashboard(configure: config =>
        {
        });

        app.MapHealthChecks("/health");
        app.UseAuthorization();
            
        app.MapControllers();

        app.Run();
    }
}