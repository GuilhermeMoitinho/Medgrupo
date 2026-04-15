using System.Reflection;
using Microsoft.OpenApi.Models;

namespace Medgrupo.WebApi.Configurations;

public static class SwaggerConfig
{
    public static IServiceCollection AddAppSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Medgrupo Contacts API",
                Version = "v1",
                Description = "API REST para gerenciamento de contatos - Avaliação técnica Medgrupo.",
                Contact = new OpenApiContact { Name = "Medgrupo" }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
        });
        return services;
    }

    public static IApplicationBuilder UseAppSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medgrupo Contacts API v1");
            c.RoutePrefix = "swagger";
        });
        return app;
    }
}
