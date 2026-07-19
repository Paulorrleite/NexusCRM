using MediatR;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Domain;

namespace NexusCRM.Application.Organizations.InactivateOrganization;

public sealed record InactivateOrganizationCommand(Guid OrganizationId) : IRequest<InactivateOrganizationResult>;

public sealed class InactivateOrganizationCommandHandler(
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<InactivateOrganizationCommand, InactivateOrganizationResult>
{
    public async Task<InactivateOrganizationResult> Handle(
        InactivateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await organizationRepository
            .GetByIdAsync(command.OrganizationId, cancellationToken)
            .ConfigureAwait(false);

        if (organization is null)
        {
            throw new DomainException("Organization was not found.");
        }

        organization.Cancel(DateTimeOffset.UtcNow);

        await organizationRepository.UpdateAsync(organization, cancellationToken).ConfigureAwait(false);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return InactivateOrganizationResult.Success;
    }
}
