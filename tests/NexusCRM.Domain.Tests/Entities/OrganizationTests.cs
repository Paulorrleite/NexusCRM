using NexusCRM.Domain.Organizations;

namespace NexusCRM.Domain.Tests.Entities;

public sealed class OrganizationTests
{
    [Fact]
    public void Register_creates_active_organization()
    {
        var createdAt = DateTimeOffset.UtcNow;

        var organization = Organization.Register(" Reus Tecnologia ", createdAt);

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
        var organization = Organization.Register("Reus Tecnologia", DateTimeOffset.UtcNow);
        organization.Cancel(DateTimeOffset.UtcNow);

        Assert.Throws<DomainException>(() => organization.Rename("New name", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Suspend_changes_status_to_suspended()
    {
        var organization = Organization.Register("Reus Tecnologia", DateTimeOffset.UtcNow);
        var updatedAt = DateTimeOffset.UtcNow.AddMinutes(1);

        organization.Suspend(updatedAt);

        Assert.Equal(OrganizationStatus.Suspended, organization.Status);
        Assert.False(organization.IsActive);
        Assert.Equal(updatedAt, organization.UpdatedAt);
    }

    [Fact]
    public void Suspend_rejects_cancelled_organization()
    {
        var organization = Organization.Register("Reus Tecnologia", DateTimeOffset.UtcNow);
        organization.Cancel(DateTimeOffset.UtcNow);

        Assert.Throws<DomainException>(() => organization.Suspend(DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Activate_changes_status_to_active()
    {
        var organization = Organization.Register("Reus Tecnologia", DateTimeOffset.UtcNow);
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
        var organization = Organization.Register("Reus Tecnologia", DateTimeOffset.UtcNow);
        organization.Cancel(DateTimeOffset.UtcNow);

        Assert.Throws<DomainException>(() => organization.Activate(DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData("Reus Tecnologia", "reus-tecnologia")]
    [InlineData("  Reus   Tecnologia  ", "reus-tecnologia")]
    [InlineData("Reus Tecnologia CRM", "reus-tecnologia-crm")]
    [InlineData("Sao Paulo CRM", "sao-paulo-crm")]
    [InlineData("Nexus_CRM!", "nexus-crm")]
    public void GenerateSlug_creates_url_friendly_slug(string name, string expectedSlug)
    {
        var slug = Organization.GenerateSlug(name);

        Assert.Equal(expectedSlug, slug);
    }

    [Fact]
    public void Register_appends_slug_suffix_when_provided()
    {
        var organization = Organization.Register(
            "Reus Tecnologia",
            DateTimeOffset.UtcNow,
            slugSuffix: 1);

        Assert.Equal("reus-tecnologia-2", organization.Slug);
    }

    [Fact]
    public void GenerateSlug_rejects_name_without_letters_or_numbers()
    {
        Assert.Throws<DomainException>(() => Organization.GenerateSlug("---"));
    }
}
