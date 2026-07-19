# Organization CQRS Slice
> Register, edit, inactivate, list, and search organizations

Entry: `src/NexusCRM.API/Features/Organizations/OrganizationController.cs`

Application:
- Commands: `src/NexusCRM.Application/Organizations/RegisterOrganization`,
  `EditOrganization`, `InactivateOrganization`
- Queries: `src/NexusCRM.Application/Organizations/ListOrganizations`,
  `SearchOrganizations`
- DTO: `src/NexusCRM.Application/Organizations/OrganizationListItem.cs`

Persistence:
- Ports: `src/NexusCRM.Application/Abstractions/Persistence/IOrganizationRepository.cs`,
  `IOrganizationQueries.cs`
- EF implementations: `src/NexusCRM.Infrastructure/Persistence/Repositories/OrganizationRepository.cs`,
  `OrganizationQueries.cs`
- EF model maps through `src/NexusCRM.Infrastructure/Persistence/Models/OrganizationRecord.cs`

Behavior:
- Register accepts organization name only; `Organization.Register()` generates
  the base slug in the domain
- Registration checks existing slugs through `IOrganizationRepository` and
  asks the domain for suffixed slug candidates when needed
- Edit uses route id + request body, supports name and Active/Suspended status
- Delete endpoint inactivates by calling `Organization.Cancel()`, resulting in
  `OrganizationStatus.Cancelled`
- List filters by name, slug, status
- Search matches name or slug, with optional status filter

Tests:
- Application tests: `tests/NexusCRM.Application.Tests/Organizations`
- Integration tests: `tests/NexusCRM.IntegrationTests/Organizations`

Updated: 2026-07-19
