using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Abstractions.Persistence;

public interface IOrganizationRepository
{
    Task AddAsync(Organization organization, CancellationToken cancellationToken);

    Task<Organization?> GetByIdAsync(Guid organizationId, CancellationToken cancellationToken);

    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken);

    Task UpdateAsync(Organization organization, CancellationToken cancellationToken);
}
