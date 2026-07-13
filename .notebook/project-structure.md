# Project structure

## Current layout

The repository uses a `src` layout with separate projects:

- `src/NexusCRM.Domain`: domain entities, value objects, enums, and domain
  exceptions.
- `src/NexusCRM.Application`: CQRS commands, queries, handlers, and application
  contracts.
- `src/NexusCRM.Infrastructure`: persistence, identity, integrations, and
  implementations of application contracts.
- `src/NexusCRM.API`: ASP.NET Core Web API composition root and controllers.

Domain code for the first module lives under
`src/NexusCRM.Domain/Domain/Organizations`.

Inside a domain module, entities live in an `Entities` folder and enums live in
an `Enums` folder.

Domain tests live in `tests/NexusCRM.Domain.Tests`.

## Dependency direction

Project references follow Clean Architecture:

- `NexusCRM.Domain` has no project references.
- `NexusCRM.Application` references `NexusCRM.Domain`.
- `NexusCRM.Infrastructure` references `NexusCRM.Application` and
  `NexusCRM.Domain`.
- `NexusCRM.API` references `NexusCRM.Application` and
  `NexusCRM.Infrastructure`.
- `NexusCRM.Domain.Tests` references `NexusCRM.Domain`.

## CQRS convention

CQRS messaging contracts live in
`src/NexusCRM.Application/Abstractions/Messaging`.

Commands and queries should use separate request and handler types. Handlers
should depend on direct contracts instead of calling other handlers.
