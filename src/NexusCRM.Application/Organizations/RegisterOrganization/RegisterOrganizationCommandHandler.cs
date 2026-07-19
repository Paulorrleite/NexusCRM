using MediatR;
using NexusCRM.Application.Abstractions.Persistence;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Organizations.RegisterOrganization;

public sealed record RegisterOrganizationCommand(string Name) : IRequest<RegisterOrganizationResult>;

public sealed class RegisterOrganizationCommandHandler(
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterOrganizationCommand, RegisterOrganizationResult>
{
    public async Task<RegisterOrganizationResult> Handle(
        RegisterOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        var slugSuffix = 0;
        var slug = Organization.GenerateSlug(command.Name, slugSuffix);

        while (await organizationRepository.SlugExistsAsync(slug, cancellationToken).ConfigureAwait(false))
        {
            slugSuffix++;
            slug = Organization.GenerateSlug(command.Name, slugSuffix);
        }

        var organization = Organization.Register(command.Name, DateTimeOffset.UtcNow, slugSuffix);

        await organizationRepository.AddAsync(organization, cancellationToken).ConfigureAwait(false);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new RegisterOrganizationResult(organization.Id, organization.Slug);
    }
}
