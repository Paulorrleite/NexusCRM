using MediatR;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Organizations.SearchOrganizations;

public sealed record SearchOrganizationsQuery(
    string SearchTerm,
    OrganizationStatus? Status) : IRequest<IReadOnlyCollection<OrganizationListItem>>;

public sealed class SearchOrganizationsQueryHandler(IOrganizationQueries organizationQueries)
    : IRequestHandler<SearchOrganizationsQuery, IReadOnlyCollection<OrganizationListItem>>
{
    public async Task<IReadOnlyCollection<OrganizationListItem>> Handle(
        SearchOrganizationsQuery query,
        CancellationToken cancellationToken)
    {
        return await organizationQueries.SearchAsync(
            query.SearchTerm,
            query.Status,
            cancellationToken).ConfigureAwait(false);
    }
}
