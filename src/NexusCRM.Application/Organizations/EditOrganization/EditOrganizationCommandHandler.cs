using MediatR;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Domain;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Organizations.EditOrganization;

public sealed record EditOrganizationCommand(
    Guid OrganizationId,
    string Name,
    OrganizationStatus Status) : IRequest<EditOrganizationResult>;

public sealed class EditOrganizationCommandHandler(
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<EditOrganizationCommand, EditOrganizationResult>
{
    public async Task<EditOrganizationResult> Handle(
        EditOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await organizationRepository
            .GetByIdAsync(command.OrganizationId, cancellationToken)
            .ConfigureAwait(false);

        if (organization is null)
        {
            throw new DomainException("Organization was not found.");
        }

        var updatedAt = DateTimeOffset.UtcNow;
        organization.Rename(command.Name, updatedAt);

        if (command.Status == OrganizationStatus.Active)
        {
            organization.Activate(updatedAt);
        }
        else if (command.Status == OrganizationStatus.Suspended)
        {
            organization.Suspend(updatedAt);
        }

        await organizationRepository.UpdateAsync(organization, cancellationToken).ConfigureAwait(false);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return EditOrganizationResult.Success;
    }
}
