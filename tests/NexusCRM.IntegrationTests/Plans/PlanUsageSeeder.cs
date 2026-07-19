using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NexusCRM.Infrastructure.Persistence;

namespace NexusCRM.IntegrationTests.Plans;

internal static class PlanUsageSeeder
{
    public static async Task SeedOrganizationUsingPlanAsync(IServiceScope scope, Guid planId)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<NexusCrmDbContext>();
        var organizationId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        var currentPeriodEnd = now.AddMonths(1);

        await dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
            INSERT INTO organizations (
                id,
                name,
                slug,
                status,
                created_at,
                updated_at
            )
            VALUES (
                {organizationId},
                {"Reus Tecnologia"},
                {"reus-tecnologia"},
                {"Active"},
                {now},
                {now}
            );

            INSERT INTO subscriptions (
                id,
                organization_id,
                plan_id,
                status,
                started_at,
                current_period_start,
                current_period_end,
                cancelled_at
            )
            VALUES (
                {subscriptionId},
                {organizationId},
                {planId},
                {"Active"},
                {now},
                {now},
                {currentPeriodEnd},
                {null}
            );
            """);
    }
}
