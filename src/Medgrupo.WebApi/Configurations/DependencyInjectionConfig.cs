using FluentValidation;
using Medgrupo.Business.Dtos;
using Medgrupo.Business.Notifications;
using Medgrupo.Business.Notifications.Abstractions;
using Medgrupo.Business.Services;
using Medgrupo.Business.Services.Abstractions;
using Medgrupo.Business.Validators;
using Medgrupo.Data.Repositories;
using Medgrupo.Data.Repositories.Abstractions;

namespace Medgrupo.WebApi.Configurations;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddAppDependencies(this IServiceCollection services)
    {
        services.AddScoped<INotificationContext, NotificationContext>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IValidator<IContactInput>, ContactInputValidator>();
        return services;
    }
}
