namespace NexusCRM.Application.Plans;

public sealed record PlanUsageOrganization(
    Guid OrganizationId,
    string OrganizationName,
    string OrganizationSlug);
