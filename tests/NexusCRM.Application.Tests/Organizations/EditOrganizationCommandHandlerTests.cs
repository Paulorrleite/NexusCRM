using NexusCRM.Application.Organizations.EditOrganization;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Tests.Organizations;

public sealed class EditOrganizationCommandHandlerTests
{
    [Fact]
    public async Task Handle_renames_and_suspends_organization()
    {
        var organization = Organization.Register(
            "Reus Tecnologia",
            DateTimeOffset.UtcNow);
        var organizationRepository = new FakeOrganizationRepository();
        organizationRepository.Organizations.Add(organization);
        var unitOfWork = new FakeUnitOfWork();
        var handler = new EditOrganizationCommandHandler(organizationRepository, unitOfWork);

        var result = await handler.Handle(
            new EditOrganizationCommand(
                organization.Id,
                "Reus Labs",
                OrganizationStatus.Suspended),
            CancellationToken.None);

        Assert.True(result.Updated);
        Assert.Equal("Reus Labs", organization.Name);
        Assert.Equal(OrganizationStatus.Suspended, organization.Status);
        Assert.Equal(organization.Id, organizationRepository.UpdatedOrganizationId);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }
}
