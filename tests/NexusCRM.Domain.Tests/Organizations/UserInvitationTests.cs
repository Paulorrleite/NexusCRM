using NexusCRM.Domain;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.Domain.Tests.Organizations;

public sealed class UserInvitationTests
{
    [Fact]
    public void Create_creates_pending_invitation()
    {
        var createdAt = DateTimeOffset.UtcNow;
        var expiresAt = createdAt.AddDays(7);
        var organizationId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var invitedByUserId = Guid.NewGuid();

        var invitation = UserInvitation.Create(
            organizationId,
            " New.User@Example.com ",
            roleId,
            invitedByUserId,
            "token-hash",
            expiresAt,
            createdAt);

        Assert.NotEqual(Guid.Empty, invitation.Id);
        Assert.Equal(organizationId, invitation.OrganizationId);
        Assert.Equal("new.user@example.com", invitation.Email);
        Assert.Equal(roleId, invitation.RoleId);
        Assert.Equal(invitedByUserId, invitation.InvitedByUserId);
        Assert.Equal(UserInvitationStatus.Pending, invitation.Status);
        Assert.Equal(expiresAt, invitation.ExpiresAt);
        Assert.Null(invitation.AcceptedAt);
    }

    [Fact]
    public void Accept_marks_invitation_as_accepted()
    {
        var createdAt = DateTimeOffset.UtcNow;
        var invitation = CreateInvitation(createdAt);
        var acceptedAt = createdAt.AddDays(1);

        invitation.Accept(acceptedAt);

        Assert.Equal(UserInvitationStatus.Accepted, invitation.Status);
        Assert.Equal(acceptedAt, invitation.AcceptedAt);
    }

    [Fact]
    public void Accept_rejects_expired_invitation()
    {
        var createdAt = DateTimeOffset.UtcNow;
        var invitation = CreateInvitation(createdAt);

        Assert.Throws<DomainException>(() => invitation.Accept(createdAt.AddDays(8)));
        Assert.Equal(UserInvitationStatus.Expired, invitation.Status);
    }

    [Fact]
    public void Cancel_prevents_accepting_invitation()
    {
        var invitation = CreateInvitation(DateTimeOffset.UtcNow);

        invitation.Cancel();

        Assert.Throws<DomainException>(() => invitation.Accept(DateTimeOffset.UtcNow));
    }

    private static UserInvitation CreateInvitation(DateTimeOffset createdAt)
    {
        return UserInvitation.Create(
            Guid.NewGuid(),
            "user@example.com",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "token-hash",
            createdAt.AddDays(7),
            createdAt);
    }
}
