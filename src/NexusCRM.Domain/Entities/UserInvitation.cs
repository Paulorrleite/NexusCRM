using NexusCRM.Domain;

namespace NexusCRM.Domain.Organizations;

public sealed class UserInvitation
{
    private UserInvitation(
        Guid id,
        Guid organizationId,
        string email,
        Guid roleId,
        Guid invitedByUserId,
        string tokenHash,
        UserInvitationStatus status,
        DateTimeOffset expiresAt,
        DateTimeOffset? acceptedAt,
        DateTimeOffset createdAt)
    {
        Id = id;
        OrganizationId = organizationId;
        Email = email;
        RoleId = roleId;
        InvitedByUserId = invitedByUserId;
        TokenHash = tokenHash;
        Status = status;
        ExpiresAt = expiresAt;
        AcceptedAt = acceptedAt;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }

    public Guid OrganizationId { get; }

    public string Email { get; }

    public Guid RoleId { get; private set; }

    public Guid InvitedByUserId { get; }

    public string TokenHash { get; }

    public UserInvitationStatus Status { get; private set; }

    public DateTimeOffset ExpiresAt { get; }

    public DateTimeOffset? AcceptedAt { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public bool IsPending => Status == UserInvitationStatus.Pending;

    public static UserInvitation Create(
        Guid organizationId,
        string email,
        Guid roleId,
        Guid invitedByUserId,
        string tokenHash,
        DateTimeOffset expiresAt,
        DateTimeOffset createdAt)
    {
        EnsureValidId(organizationId, nameof(organizationId));
        EnsureValidId(roleId, nameof(roleId));
        EnsureValidId(invitedByUserId, nameof(invitedByUserId));

        if (expiresAt <= createdAt)
        {
            throw new DomainException("Invitation expiration must be after the creation date.");
        }

        return new UserInvitation(
            Guid.NewGuid(),
            organizationId,
            NormalizeEmail(email),
            roleId,
            invitedByUserId,
            RequiredText(tokenHash, nameof(tokenHash)),
            UserInvitationStatus.Pending,
            expiresAt,
            acceptedAt: null,
            createdAt);
    }

    public void ChangeRole(Guid roleId)
    {
        EnsurePending();
        EnsureValidId(roleId, nameof(roleId));

        RoleId = roleId;
    }

    public void Accept(DateTimeOffset acceptedAt)
    {
        EnsurePending();

        if (acceptedAt > ExpiresAt)
        {
            Status = UserInvitationStatus.Expired;
            throw new DomainException("An expired invitation cannot be accepted.");
        }

        Status = UserInvitationStatus.Accepted;
        AcceptedAt = acceptedAt;
    }

    public void Cancel()
    {
        EnsurePending();

        Status = UserInvitationStatus.Cancelled;
    }

    public void Expire(DateTimeOffset expiredAt)
    {
        EnsurePending();

        if (expiredAt < ExpiresAt)
        {
            throw new DomainException("Invitation has not expired yet.");
        }

        Status = UserInvitationStatus.Expired;
    }

    private void EnsurePending()
    {
        if (Status != UserInvitationStatus.Pending)
        {
            throw new DomainException("Only a pending invitation can be changed.");
        }
    }

    private static void EnsureValidId(Guid id, string name)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException($"{name} is required.");
        }
    }

    private static string NormalizeEmail(string email)
    {
        var normalized = RequiredText(email, nameof(email)).ToLowerInvariant();

        if (!normalized.Contains('@', StringComparison.Ordinal))
        {
            throw new DomainException("Email must be valid.");
        }

        return normalized;
    }

    private static string RequiredText(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{name} is required.");
        }

        return value.Trim();
    }
}
