using System.Globalization;
using System.Text;
using NexusCRM.Domain;

namespace NexusCRM.Domain.Organizations;

public sealed class Organization : Entity<Guid>
{
    private Organization(
        Guid id,
        string name,
        string slug,
        OrganizationStatus status,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
        : base(id)
    {
        Name = name;
        Slug = slug;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public string Name { get; private set; }

    public string Slug { get; }

    public OrganizationStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public bool IsActive => Status == OrganizationStatus.Active;

    public static Organization Register(
        string name,
        DateTimeOffset createdAt,
        int slugSuffix = 0)
    {
        var normalizedName = RequiredText(name, nameof(name));

        return new Organization(
            Guid.NewGuid(),
            normalizedName,
            GenerateSlug(normalizedName, slugSuffix),
            OrganizationStatus.Active,
            createdAt,
            createdAt);
    }

    public static string GenerateSlug(string name, int suffix = 0)
    {
        if (suffix < 0)
        {
            throw new DomainException("Slug suffix cannot be negative.");
        }

        var normalizedName = RequiredText(name, nameof(name));
        var slug = CreateSlugBase(normalizedName);

        if (slug.Length == 0)
        {
            throw new DomainException("Organization name must contain letters or numbers.");
        }

        return suffix == 0 ? slug : $"{slug}-{suffix + 1}";
    }

    public static Organization Load(
        Guid id,
        string name,
        string slug,
        OrganizationStatus status,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Organization id is required.");
        }

        return new Organization(
            id,
            RequiredText(name, nameof(name)),
            RequiredText(slug, nameof(slug)),
            status,
            createdAt,
            updatedAt);
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

    private static string CreateSlugBase(string value)
    {
        var normalizedValue = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalizedValue.Length);
        var previousWasSeparator = false;

        foreach (var character in normalizedValue)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsAsciiLetterOrDigit(character))
            {
                builder.Append(char.ToLowerInvariant(character));
                previousWasSeparator = false;
                continue;
            }

            if (builder.Length > 0 && !previousWasSeparator)
            {
                builder.Append('-');
                previousWasSeparator = true;
            }
        }

        if (builder.Length > 0 && builder[^1] == '-')
        {
            builder.Length--;
        }

        return builder.ToString();
    }
}
