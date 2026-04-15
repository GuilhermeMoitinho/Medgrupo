using Medgrupo.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Medgrupo.WebApi.Configurations;

public static class DatabaseConfig
{
    public static IServiceCollection AddAppDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(5)));
        return services;
    }

    public static async Task EnsureDatabaseCreatedAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var retries = 10;
        while (retries-- > 0)
        {
            try
            {
                await context.Database.EnsureCreatedAsync();
                return;
            }
            catch
            {
                if (retries == 0) throw;
                await Task.Delay(3000);
            }
        }
    }
}
