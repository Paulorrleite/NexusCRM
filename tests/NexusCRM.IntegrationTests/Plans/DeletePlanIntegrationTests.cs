using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NexusCRM.Application.Plans.DeletePlan;
using NexusCRM.Application.Plans.ListPlans;
using NexusCRM.Domain.Organizations;
using NexusCRM.IntegrationTests.Infrastructure;

namespace NexusCRM.IntegrationTests.Plans;

[Collection(IntegrationTestCollection.Name)]
public sealed class DeletePlanIntegrationTests(IntegrationTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task DeletePlan_deletes_unused_plan()
    {
        using var scope = Fixture.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var registered = await sender.Send(
            PlanCommands.Register("Professional", "Professional plan"),
            CancellationToken.None);

        var result = await sender.Send(
            new DeletePlanCommand(registered.PlanId),
            CancellationToken.None);

        var plans = await sender.Send(
            new ListPlansQuery("professional", null, null, null),
            CancellationToken.None);

        Assert.True(result.Deleted);
        Assert.Empty(result.OrganizationsUsingPlan);
        Assert.Empty(plans);
    }

    [Fact]
    public async Task DeletePlan_returns_organizations_and_does_not_delete_used_plan()
    {
        using var scope = Fixture.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var registered = await sender.Send(
            PlanCommands.Register("Professional", "Professional plan"),
            CancellationToken.None);
        await PlanUsageSeeder.SeedOrganizationUsingPlanAsync(scope, registered.PlanId);

        var result = await sender.Send(
            new DeletePlanCommand(registered.PlanId),
            CancellationToken.None);

        var plans = await sender.Send(
            new ListPlansQuery("professional", null, BillingPeriod.Monthly, true),
            CancellationToken.None);

        Assert.False(result.Deleted);
        Assert.Equal("Reus Tecnologia", Assert.Single(result.OrganizationsUsingPlan).OrganizationName);
        Assert.Equal("Professional", Assert.Single(plans).Name);
    }
}
