using NexusCRM.Application.Organizations.InactivateOrganization;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Tests.Organizations;

public sealed class InactivateOrganizationCommandHandlerTests
{
    [Fact]
    public async Task Handle_cancels_organization_and_saves_changes()
    {
        var organization = Organization.Register(
            "Reus Tecnologia",
            DateTimeOffset.UtcNow);
        var organizationRepository = new FakeOrganizationRepository();
        organizationRepository.Organizations.Add(organization);
        var unitOfWork = new FakeUnitOfWork();
        var handler = new InactivateOrganizationCommandHandler(organizationRepository, unitOfWork);

        var result = await handler.Handle(
            new InactivateOrganizationCommand(organization.Id),
            CancellationToken.None);

        Assert.True(result.Inactivated);
        Assert.Equal(OrganizationStatus.Cancelled, organization.Status);
        Assert.Equal(organization.Id, organizationRepository.UpdatedOrganizationId);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }
}
