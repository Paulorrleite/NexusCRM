# Tenant isolation architecture notes

## Rule

Every business record must belong to exactly one organization unless it is explicitly global system configuration.

Tenant isolation must be enforced in backend application and persistence logic. The frontend can improve usability, but it must not be the security boundary.

## Core concepts

- Organization: the tenant and top-level data boundary.
- User: a person who belongs to an organization and acts inside that organization.
- Organization-scoped record: data owned by one organization, such as a contact, company, lead, opportunity, task, note, or interaction.

## Design principles

- Commands must validate and write data only inside the current organization.
- Queries must filter by the current organization before returning data.
- API controllers must not contain tenant business rules.
- Domain and application code should model tenant ownership explicitly.
- Persistence queries should avoid loading cross-tenant data and filtering it later in memory.
- Tests for organization-scoped behavior should include cross-tenant denial or exclusion cases.

## Early implementation guidance

Do not add a full multi-project architecture until the first real feature needs it.

When persistence is introduced, prefer an explicit organization identifier on organization-scoped data. The exact implementation can be decided when the first persisted feature is built.

When authentication is introduced, the authenticated user context should provide the current organization identity to application use cases. Until then, early development may use an explicit organization identifier in request models only when needed for learning or bootstrapping.

## Risks to avoid

- Returning data without an organization filter.
- Accepting an organization identifier from the client without checking that the user belongs to it.
- Sharing mutable state across organizations.
- Placing tenant checks only in controllers or UI code.
- Creating generic repositories or services before the first real data access use case exists.
