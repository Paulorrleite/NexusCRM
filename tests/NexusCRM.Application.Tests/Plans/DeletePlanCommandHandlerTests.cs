using NexusCRM.Application.Plans;
using NexusCRM.Application.Plans.DeletePlan;

namespace NexusCRM.Application.Tests.Plans;

public sealed class DeletePlanCommandHandlerTests
{
    [Fact]
    public async Task Handle_deletes_plan_when_not_in_use()
    {
        var planId = Guid.NewGuid();
        var planRepository = new FakePlanRepository();
        planRepository.ExistingPlanIds.Add(planId);
        var planQueries = new FakePlanQueries();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new DeletePlanCommandHandler(planRepository, planQueries, unitOfWork);

        var result = await handler.Handle(
            new DeletePlanCommand(planId),
            CancellationToken.None);

        Assert.True(result.Deleted);
        Assert.Empty(result.OrganizationsUsingPlan);
        Assert.Equal(planId, planRepository.DeletedPlanId);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_returns_organizations_and_does_not_delete_when_plan_is_in_use()
    {
        var planId = Guid.NewGuid();
        var organization = new PlanUsageOrganization(Guid.NewGuid(), "Reus Tecnologia", "reus-tecnologia");
        var planRepository = new FakePlanRepository();
        planRepository.ExistingPlanIds.Add(planId);
        var planQueries = new FakePlanQueries();
        planQueries.OrganizationsUsingPlan.Add(organization);
        var unitOfWork = new FakeUnitOfWork();
        var handler = new DeletePlanCommandHandler(planRepository, planQueries, unitOfWork);

        var result = await handler.Handle(
            new DeletePlanCommand(planId),
            CancellationToken.None);

        Assert.False(result.Deleted);
        Assert.Equal([organization], result.OrganizationsUsingPlan);
        Assert.Null(planRepository.DeletedPlanId);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }
}
