using NexusCRM.Application.Organizations.EditOrganization;
using NexusCRM.Application.Organizations.RegisterOrganization;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.IntegrationTests.Organizations;

internal static class OrganizationCommands
{
    public static RegisterOrganizationCommand Register(string name)
    {
        return new RegisterOrganizationCommand(name);
    }

    public static EditOrganizationCommand Edit(
        Guid organizationId,
        string name,
        OrganizationStatus status = OrganizationStatus.Active)
    {
        return new EditOrganizationCommand(organizationId, name, status);
    }
}
