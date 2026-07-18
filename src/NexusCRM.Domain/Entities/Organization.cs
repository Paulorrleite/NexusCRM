using NexusCRM.Domain;

namespace NexusCRM.Domain.Organizations;

public sealed class Organization
{
    private Organization(
        Guid id,
        string name,
        string slug,
        OrganizationStatus status,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        Name = name;
        Slug = slug;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; }

    public string Name { get; private set; }

    public string Slug { get; }

    public OrganizationStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public bool IsActive => Status == OrganizationStatus.Active;

    public static Organization Register(
        string name,
        string slug,
        DateTimeOffset createdAt)
    {
        return new Organization(
            Guid.NewGuid(),
            RequiredText(name, nameof(name)),
            RequiredText(slug, nameof(slug)),
            OrganizationStatus.Active,
            createdAt,
            createdAt);
    }

    public void Rename(string name, DateTimeOffset updatedAt)
    {
        EnsureNotCancelled();

        Name = RequiredText(name, nameof(name));
        UpdatedAt = updatedAt;
    }

    public void Suspend(DateTimeOffset updatedAt)
    {
        EnsureNotCancelled();

        Status = OrganizationStatus.Suspended;
        UpdatedAt = updatedAt;
    }

    public void Activate(DateTimeOffset updatedAt)
    {
        EnsureNotCancelled();

        Status = OrganizationStatus.Active;
        UpdatedAt = updatedAt;
    }

    public void Cancel(DateTimeOffset updatedAt)
    {
        Status = OrganizationStatus.Cancelled;
        UpdatedAt = updatedAt;
    }

    private void EnsureNotCancelled()
    {
        if (Status == OrganizationStatus.Cancelled)
        {
            throw new DomainException("A cancelled organization cannot be changed.");
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
}
