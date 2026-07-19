using NexusCRM.Domain;

namespace NexusCRM.Domain.Organizations;

public sealed class Role : Entity<Guid>
{
    private readonly HashSet<string> _permissions;

    private Role(
        Guid id,
        Guid organizationId,
        string name,
        SystemRole? systemRole,
        IEnumerable<string> permissions)
        : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        SystemRole = systemRole;
        _permissions = permissions
            .Select(NormalizePermission)
            .Where(permission => permission.Length > 0)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public Guid OrganizationId { get; }

    public string Name { get; private set; }

    public SystemRole? SystemRole { get; }

    public bool IsSystemRole => SystemRole.HasValue;

    public IReadOnlyCollection<string> Permissions => _permissions;

    public static Role Create(
        Guid organizationId,
        string name,
        IEnumerable<string> permissions)
    {
        EnsureValidId(organizationId, nameof(organizationId));

        return new Role(
            Guid.NewGuid(),
            organizationId,
            RequiredText(name, nameof(name)),
            systemRole: null,
            permissions);
    }

    public static Role CreateSystemRole(
        Guid organizationId,
        SystemRole systemRole,
        IEnumerable<string> permissions)
    {
        EnsureValidId(organizationId, nameof(organizationId));

        return new Role(
            Guid.NewGuid(),
            organizationId,
            GetSystemRoleName(systemRole),
            systemRole,
            permissions);
    }

    public bool HasPermission(string permission)
    {
        return _permissions.Contains(NormalizePermission(permission));
    }

    public void GrantPermission(string permission)
    {
        _permissions.Add(RequiredText(permission, nameof(permission)).ToLowerInvariant());
    }

    public void RevokePermission(string permission)
    {
        _permissions.Remove(NormalizePermission(permission));
    }

    private static void EnsureValidId(Guid id, string name)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException($"{name} is required.");
        }
    }

    private static string RequiredText(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{name} is required.");
        }

        return value.Trim();
    }

    private static string NormalizePermission(string permission)
    {
        return permission.Trim().ToLowerInvariant();
    }

    private static string GetSystemRoleName(SystemRole systemRole)
    {
        return systemRole switch
        {
            Organizations.SystemRole.Owner => "Owner",
            Organizations.SystemRole.Administrator => "Administrator",
            Organizations.SystemRole.SalesManager => "Sales Manager",
            Organizations.SystemRole.SalesRepresentative => "Sales Representative",
            Organizations.SystemRole.Viewer => "Viewer",
            _ => throw new DomainException("System role is invalid.")
        };
    }
}
