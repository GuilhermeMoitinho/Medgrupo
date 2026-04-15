using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Medgrupo.WebApi.Configurations;

public static class MvcConfig
{
    public static IServiceCollection AddAppMvc(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        services.Configure<ApiBehaviorOptions>(opt =>
        {
            opt.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(kv => kv.Value?.Errors.Count > 0)
                    .SelectMany(kv => kv.Value!.Errors.Select(e => new
                    {
                        Key = string.IsNullOrWhiteSpace(kv.Key) ? "body" : kv.Key,
                        Message = string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? FriendlyMessage(e.Exception)
                            : e.ErrorMessage
                    }))
                    .ToArray();

                return new BadRequestObjectResult(new
                {
                    status = StatusCodes.Status400BadRequest,
                    errors
                });
            };
        });

        return services;
    }

    private static string FriendlyMessage(Exception? ex) => ex switch
    {
        JsonException je when je.Path is not null =>
            $"Valor inválido para o campo '{je.Path.TrimStart('$', '.')}'.",
        JsonException => "JSON inválido no corpo da requisição.",
        _ => "Valor inválido."
    };
}
