using System.Text.Json.Serialization;

namespace Medgrupo.WebApi.Configurations;

public static class MvcConfig
{
    public static IServiceCollection AddAppMvc(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                opt.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });
        return services;
    }
}
