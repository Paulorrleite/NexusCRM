using NexusCRM.Application.Organizations;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Abstractions.Persistence;

public interface IOrganizationQueries
{
    Task<IReadOnlyCollection<OrganizationListItem>> ListAsync(
        string? name,
        string? slug,
        OrganizationStatus? status,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<OrganizationListItem>> SearchAsync(
        string searchTerm,
        OrganizationStatus? status,
        CancellationToken cancellationToken);
}
