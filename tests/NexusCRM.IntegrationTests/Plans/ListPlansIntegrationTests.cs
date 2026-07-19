using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NexusCRM.Application.Plans.ListPlans;
using NexusCRM.Domain.Organizations;
using NexusCRM.IntegrationTests.Infrastructure;

namespace NexusCRM.IntegrationTests.Plans;

[Collection(IntegrationTestCollection.Name)]
public sealed class ListPlansIntegrationTests(IntegrationTestFixture fixture)
    : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task ListPlans_filters_by_name_description_billing_period_and_active_status()
    {
        using var scope = Fixture.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        await sender.Send(
            PlanCommands.Register("Professional", "Monthly sales plan"),
            CancellationToken.None);
        await sender.Send(
            PlanCommands.Register("Enterprise", "Yearly sales plan", BillingPeriod.Yearly),
            CancellationToken.None);

        var plans = await sender.Send(
            new ListPlansQuery("pro", "monthly", BillingPeriod.Monthly, true),
            CancellationToken.None);

        var plan = Assert.Single(plans);
        Assert.Equal("Professional", plan.Name);
    }
}
