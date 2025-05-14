# HealthCheckDashboard.NET
A health check dashboard built on top of `IHealthCheck`.  
It works by fetching the health status from each healthcheck through the healthservice (internal to the healthcheck library),  
and exposing this as an endpoint.

![image](https://github.com/user-attachments/assets/db688134-e2d8-495e-a208-a906ca2c7c5a)
 
## Example
```csharp

// Add your healthchecks as usual. 
builder.Services.AddHealthChecks()
    .AddCheck<SimpleSuccessHealthCheck>("Simple Success", tags: new[] {"Simple", "Success"})
    .AddCheck<SimpleDegradedHealthCheck>("Simple Degraded", tags: new[] {"Simple", "Degraded"})
    .AddCheck<SimpleFailedHealthCheck>("Simple Failed", tags: new[] {"Simple", "Failed"})
    .AddCheck<SimpleExceptionHealthCheck>("Simple Exception", tags: new[] {"Simple", "Exception"});

var app = builder.Build();
app.MapHealthChecks("/health");

// Use health dashboard and configure it as needed.
app.MapHealthCheckDashboard("/health-dashboard");

/* Normal auth, controller etc. setup.
...
*/

app.Run();
```

And that is it.

The dashboard follows mostly the same conventions that the health checks do.

## Require authorization
Call `RequireAuthorization` to run Authorization Middleware on the health dashboard endpoint. A `RequireAuthorization` overload accepts one or more authorization policies. 
If a policy isn't provided, the default authorization policy is used:

```csharp
  app.MapHealthCheckDashboard("/health-dashboard")
      .RequireAuthorization();
```
## Filtering health checks
By default, the Health dashboard shows all registered health checks.
To show a subset of health checks, provide a function that returns a boolean to the `Predicate` option.

The following example filters the health checks so that only those tagged with sample run:
```csharp
app.MapHealthCheckDashboard("/health-dashboard", new HealthDashboardOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("sample")
});
```
