using NexusCRM.Domain.Organizations;
using NexusCRM.Application.Plans;

namespace NexusCRM.Application.Abstractions.Persistence;

public interface IPlanQueries
{
    Task<IReadOnlyCollection<PlanUsageOrganization>> GetOrganizationsUsingPlanAsync(
        Guid planId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PlanListItem>> ListAsync(
        string? name,
        string? description,
        BillingPeriod? billingPeriod,
        bool? isActive,
        CancellationToken cancellationToken);
}
