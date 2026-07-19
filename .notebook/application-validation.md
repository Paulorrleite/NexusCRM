# Application validation
> FluentValidation for CQRS request validation

Entry: `src/NexusCRM.Application/DependencyInjection.cs:AddApplication()`

Flow: API/controller -> MediatR `ISender.Send()` -> `ValidationBehavior<TRequest, TResponse>` -> request handler

Registration:
- Validators scanned from `src/NexusCRM.Application`
- Open MediatR behavior registered in `AddApplication()`

Pattern:
- Command/query validators live beside their request type
- Validators check request shape/input constraints
- Domain invariants remain in domain objects
- API maps `FluentValidation.ValidationException` to validation problem responses in `src/NexusCRM.API/Program.cs`

Plan validators:
- `RegisterPlanCommandValidator`
- `EditPlanCommandValidator`
- `DeletePlanCommandValidator`
- `ListPlansQueryValidator`

Updated: 2026-07-19
