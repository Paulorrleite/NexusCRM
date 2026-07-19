using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NexusCRM.Application.Plans.ListPlans;
using NexusCRM.Domain.Organizations;
using NexusCRM.IntegrationTests.Infrastructure;

namespace NexusCRM.IntegrationTests.Plans;

[Collection(IntegrationTestCollection.Name)]
public sealed class EditPlanIntegrationTests(IntegrationTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task EditPlan_updates_unused_plan()
    {
        using var scope = Fixture.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var registered = await sender.Send(
            PlanCommands.Register("Professional", "Professional plan"),
            CancellationToken.None);

        var result = await sender.Send(
            PlanCommands.Edit(registered.PlanId, "Enterprise", "Enterprise plan"),
            CancellationToken.None);

        var plans = await sender.Send(
            new ListPlansQuery("enterprise", "enterprise", BillingPeriod.Yearly, true),
            CancellationToken.None);

        Assert.True(result.Updated);
        Assert.Empty(result.OrganizationsUsingPlan);
        Assert.Equal("Enterprise", Assert.Single(plans).Name);
    }

    [Fact]
    public async Task EditPlan_returns_organizations_and_does_not_update_used_plan()
    {
        using var scope = Fixture.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var registered = await sender.Send(
            PlanCommands.Register("Professional", "Professional plan"),
            CancellationToken.None);
        await PlanUsageSeeder.SeedOrganizationUsingPlanAsync(scope, registered.PlanId);

        var result = await sender.Send(
            PlanCommands.Edit(registered.PlanId, "Enterprise", "Enterprise plan"),
            CancellationToken.None);

        var plans = await sender.Send(
            new ListPlansQuery("professional", null, BillingPeriod.Monthly, true),
            CancellationToken.None);

        Assert.False(result.Updated);
        Assert.Equal("Reus Tecnologia", Assert.Single(result.OrganizationsUsingPlan).OrganizationName);
        Assert.Equal("Professional", Assert.Single(plans).Name);
    }
}
