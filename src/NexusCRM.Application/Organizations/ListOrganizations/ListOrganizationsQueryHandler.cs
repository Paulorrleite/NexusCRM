using MediatR;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Organizations.ListOrganizations;

public sealed record ListOrganizationsQuery(
    string? Name,
    string? Slug,
    OrganizationStatus? Status) : IRequest<IReadOnlyCollection<OrganizationListItem>>;

public sealed class ListOrganizationsQueryHandler(IOrganizationQueries organizationQueries)
    : IRequestHandler<ListOrganizationsQuery, IReadOnlyCollection<OrganizationListItem>>
{
    public async Task<IReadOnlyCollection<OrganizationListItem>> Handle(
        ListOrganizationsQuery query,
        CancellationToken cancellationToken)
    {
        return await organizationQueries.ListAsync(
            query.Name,
            query.Slug,
            query.Status,
            cancellationToken).ConfigureAwait(false);
    }
}
