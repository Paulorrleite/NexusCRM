namespace NexusCRM.Application.Organizations.InactivateOrganization;

public sealed record InactivateOrganizationResult(bool Inactivated)
{
    public static InactivateOrganizationResult Success { get; } = new(Inactivated: true);
}
