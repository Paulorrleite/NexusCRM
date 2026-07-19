using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Application.Plans.RegisterPlan;
using NexusCRM.Domain.Organizations;
using NexusCRM.IntegrationTests.Infrastructure;

namespace NexusCRM.IntegrationTests.Plans;

[Collection(IntegrationTestCollection.Name)]
public sealed class RegisterPlanIntegrationTests(IntegrationTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task RegisterPlan_persists_plan()
    {
        using var scope = Fixture.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var result = await sender.Send(
            PlanCommands.Register("Professional", "Professional plan"),
            CancellationToken.None);

        var planQueries = scope.ServiceProvider.GetRequiredService<IPlanQueries>();
        var plans = await planQueries.ListAsync(
            "professional",
            null,
            BillingPeriod.Monthly,
            true,
            CancellationToken.None);

        var plan = Assert.Single(plans);
        Assert.Equal(result.PlanId, plan.Id);
        Assert.Equal("Professional", plan.Name);
        Assert.Equal("Professional plan", plan.Description);
        Assert.Equal(49, plan.Price);
        Assert.Equal(BillingPeriod.Monthly, plan.BillingPeriod);
        Assert.True(plan.IsActive);
    }
}
