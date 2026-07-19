using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Application.Plans;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Tests.Plans;

internal sealed class FakePlanRepository : IPlanRepository
{
    public List<Plan> Plans { get; } = [];

    public HashSet<Guid> ExistingPlanIds { get; } = [];

    public Guid? UpdatedPlanId { get; private set; }

    public Guid? DeletedPlanId { get; private set; }

    public Task AddAsync(Plan plan, CancellationToken cancellationToken)
    {
        Plans.Add(plan);
        ExistingPlanIds.Add(plan.Id);

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid planId, CancellationToken cancellationToken)
    {
        return Task.FromResult(ExistingPlanIds.Contains(planId));
    }

    public Task UpdateAsync(
        Guid planId,
        string name,
        string description,
        decimal price,
        BillingPeriod billingPeriod,
        PlanLimits limits,
        IReadOnlyCollection<string> enabledFeatures,
        bool isActive,
        CancellationToken cancellationToken)
    {
        UpdatedPlanId = planId;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid planId, CancellationToken cancellationToken)
    {
        DeletedPlanId = planId;

        return Task.CompletedTask;
    }
}

internal sealed class FakePlanQueries : IPlanQueries
{
    public List<PlanUsageOrganization> OrganizationsUsingPlan { get; } = [];

    public List<PlanListItem> Plans { get; } = [];

    public string? LastName { get; private set; }

    public string? LastDescription { get; private set; }

    public BillingPeriod? LastBillingPeriod { get; private set; }

    public bool? LastIsActive { get; private set; }

    public Task<IReadOnlyCollection<PlanUsageOrganization>> GetOrganizationsUsingPlanAsync(
        Guid planId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<PlanUsageOrganization>>(OrganizationsUsingPlan);
    }

    public Task<IReadOnlyCollection<PlanListItem>> ListAsync(
        string? name,
        string? description,
        BillingPeriod? billingPeriod,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        LastName = name;
        LastDescription = description;
        LastBillingPeriod = billingPeriod;
        LastIsActive = isActive;

        return Task.FromResult<IReadOnlyCollection<PlanListItem>>(Plans);
    }
}

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCallCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCallCount++;

        return Task.FromResult(1);
    }
}
