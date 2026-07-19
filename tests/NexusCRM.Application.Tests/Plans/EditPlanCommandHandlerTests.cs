using NexusCRM.Application.Plans;
using NexusCRM.Application.Plans.EditPlan;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Tests.Plans;

public sealed class EditPlanCommandHandlerTests
{
    [Fact]
    public async Task Handle_updates_plan_when_not_in_use()
    {
        var planId = Guid.NewGuid();
        var planRepository = new FakePlanRepository();
        planRepository.ExistingPlanIds.Add(planId);
        var planQueries = new FakePlanQueries();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new EditPlanCommandHandler(planRepository, planQueries, unitOfWork);

        var result = await handler.Handle(
            CreateCommand(planId),
            CancellationToken.None);

        Assert.True(result.Updated);
        Assert.Empty(result.OrganizationsUsingPlan);
        Assert.Equal(planId, planRepository.UpdatedPlanId);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_returns_organizations_and_does_not_update_when_plan_is_in_use()
    {
        var planId = Guid.NewGuid();
        var organization = new PlanUsageOrganization(Guid.NewGuid(), "Reus Tecnologia", "reus-tecnologia");
        var planRepository = new FakePlanRepository();
        planRepository.ExistingPlanIds.Add(planId);
        var planQueries = new FakePlanQueries();
        planQueries.OrganizationsUsingPlan.Add(organization);
        var unitOfWork = new FakeUnitOfWork();
        var handler = new EditPlanCommandHandler(planRepository, planQueries, unitOfWork);

        var result = await handler.Handle(
            CreateCommand(planId),
            CancellationToken.None);

        Assert.False(result.Updated);
        Assert.Equal([organization], result.OrganizationsUsingPlan);
        Assert.Null(planRepository.UpdatedPlanId);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    private static EditPlanCommand CreateCommand(Guid planId)
    {
        return new EditPlanCommand(
            planId,
            "Professional",
            "Professional plan",
            49,
            BillingPeriod.Monthly,
            10,
            1_000,
            100,
            500,
            ["Contacts.Read"],
            IsActive: true);
    }
}
