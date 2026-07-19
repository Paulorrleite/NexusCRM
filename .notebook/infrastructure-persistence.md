# Infrastructure persistence

## Current persistence setup

PostgreSQL persistence is registered from
`src/NexusCRM.Infrastructure/DependencyInjection.cs:AddInfrastructure()`.

The API composition root calls the infrastructure registration from
`src/NexusCRM.API/Program.cs`.

The connection string key is `ConnectionStrings:NexusCRM`. No connection string
or credentials are committed; provide it through app configuration, user secrets,
or environment variables.

## Application contracts

Persistence ports live in
`src/NexusCRM.Application/Abstractions/Persistence`.

- `IPlanRepository` exposes plan persistence needed by application use cases.
- `IUnitOfWork` exposes `SaveChangesAsync()` for explicit transaction boundary
  completion.

## Plan storage

Infrastructure uses an internal EF persistence model,
`src/NexusCRM.Infrastructure/Persistence/Models/PlanRecord.cs`, instead of
mapping EF directly onto the domain `Plan` entity.

This keeps EF materialization concerns out of the domain model. The current
repository supports adding a plan by mapping from `Plan` to `PlanRecord`.

## Integration tests

PostgreSQL integration tests live in `tests/NexusCRM.IntegrationTests`.

They use the `ConnectionStrings:NexusCRMIntegrationTests` user secret or
environment variable. The expected database is dedicated to tests, for example
`nexuscrm_tests`.

`IntegrationTestFixture` creates the database when possible, ensures the schema
exists, disables test parallelization for the integration collection, and
truncates mapped tables before each test.
