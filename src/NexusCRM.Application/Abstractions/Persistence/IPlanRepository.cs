using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Abstractions.Persistence;

public interface IPlanRepository
{
    Task AddAsync(Plan plan, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid planId, CancellationToken cancellationToken);

    Task UpdateAsync(
        Guid planId,
        string name,
        string description,
        decimal price,
        BillingPeriod billingPeriod,
        PlanLimits limits,
        IReadOnlyCollection<string> enabledFeatures,
        bool isActive,
        CancellationToken cancellationToken);

    Task DeleteAsync(Guid planId, CancellationToken cancellationToken);
}
