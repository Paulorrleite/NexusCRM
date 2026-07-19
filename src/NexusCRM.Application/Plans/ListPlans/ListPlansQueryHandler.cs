using MediatR;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Plans.ListPlans;

public sealed record ListPlansQuery(
    string? Name,
    string? Description,
    BillingPeriod? BillingPeriod,
    bool? IsActive) : IRequest<IReadOnlyCollection<PlanListItem>>;

public sealed class ListPlansQueryHandler(IPlanQueries planQueries)
    : IRequestHandler<ListPlansQuery, IReadOnlyCollection<PlanListItem>>
{
    public async Task<IReadOnlyCollection<PlanListItem>> Handle(
        ListPlansQuery query,
        CancellationToken cancellationToken)
    {
        return await planQueries.ListAsync(
            query.Name,
            query.Description,
            query.BillingPeriod,
            query.IsActive,
            cancellationToken).ConfigureAwait(false);
    }
}
