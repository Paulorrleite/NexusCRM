using NexusCRM.Domain.Organizations;

namespace NexusCRM.Application.Organizations;

public sealed record OrganizationListItem(
    Guid Id,
    string Name,
    string Slug,
    OrganizationStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
