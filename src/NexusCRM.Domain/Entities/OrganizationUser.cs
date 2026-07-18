using NexusCRM.Domain;

namespace NexusCRM.Domain.Organizations;

public sealed class OrganizationUser
{
    private OrganizationUser(
        Guid id,
        Guid organizationId,
        Guid userId,
        Guid roleId,
        OrganizationUserStatus status,
        DateTimeOffset joinedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        UserId = userId;
        RoleId = roleId;
        Status = status;
        JoinedAt = joinedAt;
    }

    public Guid Id { get; }

    public Guid OrganizationId { get; }

    public Guid UserId { get; }

    public Guid RoleId { get; private set; }

    public OrganizationUserStatus Status { get; private set; }

    public DateTimeOffset JoinedAt { get; }

    public bool IsActive => Status == OrganizationUserStatus.Active;

    public static OrganizationUser AddOwner(
        Guid organizationId,
        Guid userId,
        Guid roleId,
        DateTimeOffset joinedAt)
    {
        return AddMember(organizationId, userId, roleId, joinedAt);
    }

    public static OrganizationUser AddMember(
        Guid organizationId,
        Guid userId,
        Guid roleId,
        DateTimeOffset joinedAt)
    {
        EnsureValidId(organizationId, nameof(organizationId));
        EnsureValidId(userId, nameof(userId));
        EnsureValidId(roleId, nameof(roleId));

        return new OrganizationUser(
            Guid.NewGuid(),
            organizationId,
            userId,
            roleId,
            OrganizationUserStatus.Active,
            joinedAt);
    }

    public void ChangeRole(Guid roleId)
    {
        EnsureActive();
        EnsureValidId(roleId, nameof(roleId));

        RoleId = roleId;
    }

    public void Deactivate()
    {
        EnsureActive();

        Status = OrganizationUserStatus.Inactive;
    }

    public void Activate()
    {
        if (Status == OrganizationUserStatus.Active)
        {
            return;
        }

        Status = OrganizationUserStatus.Active;
    }

    private void EnsureActive()
    {
        if (Status != OrganizationUserStatus.Active)
        {
            throw new DomainException("The organization user is not active.");
        }
    }

    private static void EnsureValidId(Guid id, string name)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException($"{name} is required.");
        }
    }
}
