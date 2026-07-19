using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Infrastructure.Persistence;
using NexusCRM.Infrastructure.Persistence.Repositories;

namespace NexusCRM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NexusCRM");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'NexusCRM' is required.");
        }

        services.AddDbContext<NexusCrmDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<IPlanQueries, PlanQueries>();
        services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<NexusCrmDbContext>());

        return services;
    }
}
