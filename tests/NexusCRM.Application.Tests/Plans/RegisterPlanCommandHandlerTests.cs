using NexusCRM.Application.Plans.RegisterPlan;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Tests.Plans;

public sealed class RegisterPlanCommandHandlerTests
{
    [Fact]
    public async Task Handle_creates_plan_and_saves_changes()
    {
        var planRepository = new FakePlanRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new RegisterPlanCommandHandler(planRepository, unitOfWork);

        var result = await handler.Handle(
            new RegisterPlanCommand(
                "Professional",
                "Professional plan",
                49,
                BillingPeriod.Monthly,
                10,
                1_000,
                100,
                500,
                ["Contacts.Read"]),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.PlanId);
        Assert.Single(planRepository.Plans);
        Assert.Equal(result.PlanId, planRepository.Plans[0].Id);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }
}
