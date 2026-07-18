using NexusCRM.Domain.Organizations;

namespace NexusCRM.Domain.Tests.Entities;

public sealed class OrganizationUserTests
{
    [Fact]
    public void AddMember_creates_active_membership()
    {
        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var joinedAt = DateTimeOffset.UtcNow;

        var member = OrganizationUser.AddMember(organizationId, userId, roleId, joinedAt);

        Assert.NotEqual(Guid.Empty, member.Id);
        Assert.Equal(organizationId, member.OrganizationId);
        Assert.Equal(userId, member.UserId);
        Assert.Equal(roleId, member.RoleId);
        Assert.Equal(OrganizationUserStatus.Active, member.Status);
        Assert.Equal(joinedAt, member.JoinedAt);
    }

    [Fact]
    public void ChangeRole_rejects_inactive_membership()
    {
        var member = OrganizationUser.AddMember(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        member.Deactivate();

        Assert.Throws<DomainException>(() => member.ChangeRole(Guid.NewGuid()));
    }
}
