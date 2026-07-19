using NexusCRM.Application.Organizations.RegisterOrganization;

namespace NexusCRM.Application.Tests.Organizations;

public sealed class RegisterOrganizationCommandHandlerTests
{
    [Fact]
    public async Task Handle_registers_organization_and_saves_changes()
    {
        var organizationRepository = new FakeOrganizationRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new RegisterOrganizationCommandHandler(organizationRepository, unitOfWork);

        var result = await handler.Handle(
            new RegisterOrganizationCommand("Reus Tecnologia"),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.OrganizationId);
        Assert.Equal("reus-tecnologia", result.Slug);
        Assert.Single(organizationRepository.Organizations);
        Assert.Equal(result.OrganizationId, organizationRepository.Organizations[0].Id);
        Assert.Equal("reus-tecnologia", organizationRepository.Organizations[0].Slug);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_uses_unique_slug_suffix_when_base_slug_exists()
    {
        var organizationRepository = new FakeOrganizationRepository();
        organizationRepository.ExistingSlugs.Add("reus-tecnologia");
        var unitOfWork = new FakeUnitOfWork();
        var handler = new RegisterOrganizationCommandHandler(organizationRepository, unitOfWork);

        var result = await handler.Handle(
            new RegisterOrganizationCommand("Reus Tecnologia"),
            CancellationToken.None);

        Assert.Equal("reus-tecnologia-2", result.Slug);
        Assert.Equal("reus-tecnologia-2", organizationRepository.Organizations[0].Slug);
    }
}
