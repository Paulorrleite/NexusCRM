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
}
