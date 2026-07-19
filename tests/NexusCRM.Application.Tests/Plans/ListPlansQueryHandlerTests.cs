using NexusCRM.Application.Plans;
using NexusCRM.Application.Plans.ListPlans;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Tests.Plans;

public sealed class ListPlansQueryHandlerTests
{
    [Fact]
    public async Task Handle_returns_plans_from_query_contract()
    {
        var plan = new PlanListItem(
            Guid.NewGuid(),
            "Professional",
            "Professional plan",
            49,
            BillingPeriod.Monthly,
            IsActive: true);
        var planQueries = new FakePlanQueries();
        planQueries.Plans.Add(plan);
        var handler = new ListPlansQueryHandler(planQueries);

        var result = await handler.Handle(
            new ListPlansQuery("pro", null, BillingPeriod.Monthly, true),
            CancellationToken.None);

        Assert.Equal([plan], result);
        Assert.Equal("pro", planQueries.LastName);
        Assert.Null(planQueries.LastDescription);
        Assert.Equal(BillingPeriod.Monthly, planQueries.LastBillingPeriod);
        Assert.True(planQueries.LastIsActive);
    }
}
