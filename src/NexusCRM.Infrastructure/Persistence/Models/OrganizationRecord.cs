using NexusCRM.Domain.Organizations;

namespace NexusCRM.Infrastructure.Persistence.Models;

internal sealed class OrganizationRecord
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public OrganizationStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public static OrganizationRecord FromDomain(Organization organization)
    {
        return new OrganizationRecord
        {
            Id = organization.Id,
            Name = organization.Name,
            Slug = organization.Slug,
            Status = organization.Status,
            CreatedAt = organization.CreatedAt,
            UpdatedAt = organization.UpdatedAt
        };
    }

    public Organization ToDomain()
    {
        return Organization.Load(
            Id,
            Name,
            Slug,
            Status,
            CreatedAt,
            UpdatedAt);
    }

    public void UpdateFromDomain(Organization organization)
    {
        Name = organization.Name;
        Status = organization.Status;
        UpdatedAt = organization.UpdatedAt;
    }
}
