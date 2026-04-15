using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Medgrupo.WebApi.Configurations;

public static class HealthChecksConfig
{
    public static IServiceCollection AddAppHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        services.AddHealthChecks()
            .AddSqlServer(connectionString, name: "sqlserver", tags: new[] { "db", "sql" });

        services.AddHealthChecksUI(opt =>
        {
            opt.SetEvaluationTimeInSeconds(30);
            opt.AddHealthCheckEndpoint("Medgrupo API", "/health");
        }).AddInMemoryStorage();

        return services;
    }

    public static IEndpointRouteBuilder MapAppHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        endpoints.MapHealthChecksUI(opt => opt.UIPath = "/health-ui");
        return endpoints;
    }
}
