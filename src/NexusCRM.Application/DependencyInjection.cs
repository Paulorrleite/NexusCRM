using FluentValidation;
using NexusCRM.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace NexusCRM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        return services;
    }
}
