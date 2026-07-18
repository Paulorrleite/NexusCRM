using NexusCRM.Domain.Organizations;

namespace NexusCRM.Domain.Tests.Entities;

public sealed class OrganizationTests
{
    [Fact]
    public void Register_creates_active_organization()
    {
        var createdAt = DateTimeOffset.UtcNow;

        var organization = Organization.Register(" Reus Tecnologia ", " reus-tecnologia ", createdAt);

        Assert.NotEqual(Guid.Empty, organization.Id);
        Assert.Equal("Reus Tecnologia", organization.Name);
        Assert.Equal("reus-tecnologia", organization.Slug);
        Assert.Equal(OrganizationStatus.Active, organization.Status);
        Assert.Equal(createdAt, organization.CreatedAt);
        Assert.Equal(createdAt, organization.UpdatedAt);
    }

    [Fact]
    public void Rename_rejects_cancelled_organization()
    {
        var organization = Organization.Register("Reus Tecnologia", "reus-tecnologia", DateTimeOffset.UtcNow);
        organization.Cancel(DateTimeOffset.UtcNow);

        Assert.Throws<DomainException>(() => organization.Rename("New name", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Suspend_changes_status_to_suspended()
    {
        var organization = Organization.Register("Reus Tecnologia", "reus-tecnologia", DateTimeOffset.UtcNow);
        var updatedAt = DateTimeOffset.UtcNow.AddMinutes(1);

        organization.Suspend(updatedAt);

        Assert.Equal(OrganizationStatus.Suspended, organization.Status);
        Assert.False(organization.IsActive);
        Assert.Equal(updatedAt, organization.UpdatedAt);
    }

    [Fact]
    public void Suspend_rejects_cancelled_organization()
    {
        var organization = Organization.Register("Reus Tecnologia", "reus-tecnologia", DateTimeOffset.UtcNow);
        organization.Cancel(DateTimeOffset.UtcNow);

        Assert.Throws<DomainException>(() => organization.Suspend(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Activate_changes_status_to_active()
    {
        var organization = Organization.Register("Reus Tecnologia", "reus-tecnologia", DateTimeOffset.UtcNow);
        organization.Suspend(DateTimeOffset.UtcNow.AddMinutes(1));
        var updatedAt = DateTimeOffset.UtcNow.AddMinutes(2);

        organization.Activate(updatedAt);

        Assert.Equal(OrganizationStatus.Active, organization.Status);
        Assert.True(organization.IsActive);
        Assert.Equal(updatedAt, organization.UpdatedAt);
    }

    [Fact]
    public void Activate_rejects_cancelled_organization()
    {
        var organization = Organization.Register("Reus Tecnologia", "reus-tecnologia", DateTimeOffset.UtcNow);
        organization.Cancel(DateTimeOffset.UtcNow);

        Assert.Throws<DomainException>(() => organization.Activate(DateTimeOffset.UtcNow));
    }
}
