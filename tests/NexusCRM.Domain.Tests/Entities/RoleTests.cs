using NexusCRM.Domain.Organizations;

namespace NexusCRM.Domain.Tests.Entities;

public sealed class RoleTests
{
    [Fact]
    public void HasPermission_uses_case_insensitive_permissions()
    {
        var role = Role.Create(Guid.NewGuid(), "Administrator", [" users.manage "]);

        Assert.Null(role.SystemRole);
        Assert.False(role.IsSystemRole);
        Assert.True(role.HasPermission("USERS.MANAGE"));
    }

    [Fact]
    public void CreateSystemRole_creates_role_for_known_system_role()
    {
        var organizationId = Guid.NewGuid();

        var role = Role.CreateSystemRole(
            organizationId,
            SystemRole.SalesManager,
            ["opportunities.read", "opportunities.update"]);

        Assert.NotEqual(Guid.Empty, role.Id);
        Assert.Equal(organizationId, role.OrganizationId);
        Assert.Equal("Sales Manager", role.Name);
        Assert.Equal(SystemRole.SalesManager, role.SystemRole);
        Assert.True(role.IsSystemRole);
        Assert.True(role.HasPermission("opportunities.read"));
        Assert.True(role.HasPermission("opportunities.update"));
    }

    [Fact]
    public void GrantAndRevokePermission_changes_available_permissions()
    {
        var role = Role.Create(Guid.NewGuid(), "Viewer", []);

        role.GrantPermission("contacts.read");
        Assert.True(role.HasPermission("contacts.read"));

        role.RevokePermission("contacts.read");
        Assert.False(role.HasPermission("contacts.read"));
    }
}
