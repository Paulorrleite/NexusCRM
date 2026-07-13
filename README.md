# NexusCRM

NexusCRM is a SaaS CRM for small businesses. It is being built incrementally,
one module at a time, so each part of the system stays understandable and
production-ready.

The first module is Organizations and Subscriptions. It defines the tenant
boundary, organization users, roles, invitations, plans, subscriptions, and plan
limits.

## Current status

The repository currently contains:

- Clean Architecture project structure.
- CQRS messaging contracts in the Application project.
- Organizations and Subscriptions domain entities.
- Domain tests for the first module.
- Product and architecture documentation under `docs`.

The API does not expose business endpoints yet.

## Solution structure

```text
src
├── NexusCRM.API
├── NexusCRM.Application
├── NexusCRM.Domain
└── NexusCRM.Infrastructure

tests
└── NexusCRM.Domain.Tests
```

Project responsibilities:

- `NexusCRM.API`: ASP.NET Core Web API composition root and HTTP layer.
- `NexusCRM.Application`: CQRS commands, queries, handlers, and application
  contracts.
- `NexusCRM.Domain`: domain entities, value objects, enums, and domain
  exceptions.
- `NexusCRM.Infrastructure`: persistence, identity, integrations, and
  implementations of application contracts.
- `NexusCRM.Domain.Tests`: domain behavior tests.

Dependency direction:

- Domain depends on no other project.
- Application depends on Domain.
- Infrastructure depends on Application and Domain.
- API depends on Application and Infrastructure.
- Domain tests depend on Domain.

## Requirements

- .NET 10 SDK.

## Build

Run this command from the repository root:

```powershell
dotnet build
```

## Test

Run this command from the repository root:

```powershell
dotnet test
```

## Run the API

Run this command from the repository root:

```powershell
dotnet run --project .\src\NexusCRM.API\NexusCRM.API.csproj
```

In development, OpenAPI is mapped by the API project.

## Documentation

Product documentation:

- [SaaS CRM scope](docs/product/saas-crm-scope.md)
- [Module roadmap](docs/product/module-roadmap.md)
- [Organizations and subscriptions module](docs/product/organizations-and-subscriptions.md)

Architecture documentation:

- [Tenant isolation architecture notes](docs/architecture/tenant-isolation.md)
- [Domain events and activity history](docs/architecture/domain-events-and-activity-history.md)

## Development approach

Build the system through small vertical slices:

1. Define the user-facing behavior.
2. Classify the operation as a Command or Query.
3. Add only the domain, application, API, infrastructure, and tests required for
   that slice.
4. Verify the change with `dotnet build` and relevant tests.

Avoid empty layers or speculative abstractions. Add structure when a real use
case needs it.

