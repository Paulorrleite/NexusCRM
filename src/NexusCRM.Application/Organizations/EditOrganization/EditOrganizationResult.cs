namespace NexusCRM.Application.Organizations.EditOrganization;

public sealed record EditOrganizationResult(bool Updated)
{
    public static EditOrganizationResult Success { get; } = new(Updated: true);
}
