# AGENTS.md

## Goal

Produce clean, maintainable, production-ready code.

Never sacrifice readability for cleverness.

This repository currently starts as a small `.NET 10` ASP.NET Core Web API with controller-based routing and OpenAPI enabled for development. Treat that as current context only. Do not assume it is the intended permanent architecture, and do not create empty layers, projects, folders, or abstractions until an actual application feature needs them.

---

## Core Rules

* Think before editing.
* Inspect the repository before assuming a layer, project, folder, skill, convention, test framework, or documentation file exists.
* Prefer the smallest correct change.
* Preserve existing architecture unless explicitly asked to refactor.
* Do not introduce unnecessary abstractions or speculative infrastructure.
* Avoid breaking public APIs.
* If requirements are ambiguous, ask instead of guessing.
* Reuse existing project conventions when they are consistent with this document.
* Touch only files required by the task.
* Do not clean up unrelated code unless explicitly asked.

---

## Architecture

Actual application features MUST follow Clean Architecture, CQRS, SOLID, DRY, KISS, YAGNI, modular design, and Clean Code principles.

Apply these standards when implementing real behavior. Do not scaffold empty Domain, Application, Infrastructure, WebUi, test, or feature folders only to satisfy an abstract architecture.

Rules:

* Keep UI, API, application logic, domain logic, and data access separated.
* Dependencies must point inward according to Clean Architecture boundaries.
* Domain must not depend on Application, Infrastructure, API, or WebUi.
* Application must not depend directly on Infrastructure implementations.
* Infrastructure must implement contracts defined by inner layers when appropriate.
* API and WebUi must not contain domain business rules.
* Prefer composition over inheritance.
* Keep module boundaries explicit and cohesive.
* Avoid cross-module persistence shortcuts and hidden shared mutable state.
* Expose narrow contracts between modules; keep implementation details internal.
* Keep transaction boundaries explicit and aligned with business operations.

When introducing a new architectural layer, project, module, or bounded context:

1. Confirm the feature needs it.
2. Inspect existing patterns first.
3. Keep the public surface minimal.
4. Add only the files required for the use case and tests.

---

## CQRS

Application use cases MUST follow CQRS.

Before creating or modifying an application use case, classify the operation by intent:

* Command: changes application state.
* Query: retrieves data without changing application state.

Commands and Queries MUST use separate request and handler types. Handlers MUST NOT call other handlers. Prefer direct dependencies inside handlers instead of chaining Commands or Queries.

### Commands

Commands represent intent to change application state.

Examples: create, register, update, edit, activate, deactivate, cancel, approve, reject, process, import, transfer, close, reopen, remove, delete when the domain allows deletion.

Rules:

* Commands may read data required to validate or execute the operation.
* Reads required by a Command belong to the Command workflow.
* Do not create or call a Query Handler solely to retrieve data for a Command.
* Do not call another Command Handler from a Command Handler.
* Keep Commands focused on business intent rather than generic CRUD terminology when meaningful domain language exists.
* Commands may persist changes.
* Commands may return data required to represent the result.
* Do not return large read models from Commands when callers can use a dedicated Query.
* Validate input and business preconditions before persisting changes whenever possible.
* Keep concurrency and transaction behavior explicit when relevant.

### Queries

Queries represent intent to retrieve data without changing application state.

Examples: get, retrieve, list, search, filter, summarize, report, inspect, look up.

Rules:

* Queries MUST be read-only.
* Queries MUST NOT change application state.
* Queries MUST NOT call `SaveChangesAsync`.
* Queries MUST NOT cause observable side effects.
* Do not call a Command Handler from a Query Handler.
* Keep Queries focused on the data required by the caller.
* Prefer read-optimized DTO projections.
* Avoid loading complete entities when only a projection is required.
* Prefer server-side filtering, ordering, projection, and pagination.
* Use no-tracking access patterns when entities do not need to be tracked.
* Avoid N+1 queries.
* Do not expose persistence entities directly as Query results.

### Mixed Operations

If a task appears to combine reads and writes:

* Determine the primary intent.
* Use a Command when the primary intent is changing state.
* Keep reads required to validate or execute a Command inside the Command workflow.
* Keep independently useful read-only use cases as Queries.
* Do not split one transactional business operation into artificial Command and Query chains.

---

## Code Style

### C#

* Target `.NET 10`.
* Keep nullable reference types enabled.
* Prefer async/await for asynchronous I/O operations.
* Use `ConfigureAwait(false)` in library code.
* Propagate `CancellationToken` through asynchronous application and infrastructure operations when supported.
* Do not use async methods when no asynchronous work is performed.
* Prefer LINQ only when readability improves.
* Prefer pattern matching when it makes intent clearer.
* Prefer collection expressions when appropriate.
* Avoid unnecessary allocations and materialization.
* Use meaningful variable names.
* Keep methods small and focused.
* Prefer explicit domain intent over generic CRUD terminology.
* Prefer immutable request and response models when practical.

### TypeScript

* Use strict typing.
* Never use `any` unless explicitly requested.
* Prefer interfaces for contracts.
* Prefer `readonly` whenever practical.
* Keep functions pure whenever practical.
* Avoid duplicated state.
* Prefer explicit domain types over primitive strings when a finite set of values exists.

### Vue

* Prefer Composition API.
* Keep business logic outside templates.
* Avoid duplicated reactive state.
* Prefer `computed` over `watch` when possible.
* Keep components focused on presentation and user interaction.
* Move reusable business or workflow logic into composables or feature services when appropriate.

---

## Frontend and WebUi

There is no WebUi project in the current repository snapshot. If frontend work is introduced later, inspect the repository first and follow existing frontend conventions.

When present:

* Read and follow `docs/design/DESIGN.md` before creating, modifying, reviewing, or refactoring frontend code.
* Keep frontend code in the established WebUi project or the existing frontend location.
* Keep API access in feature `api` modules or shared API utilities.
* Keep pages focused on composition and workflow.
* Keep reusable UI in shared component areas.
* Do not duplicate backend business rules in the UI.
* Treat client-side validation as user feedback only; backend validation remains authoritative.

---

## Skills

Use relevant skills based on the task. Do not invoke skills indiscriminately.

Skills supplement this `AGENTS.md`; they do not override it. If a skill conflicts with this file, follow this file. If a referenced skill, connector, MCP, or documentation file is unavailable, say so briefly and continue with the best available project conventions.

Before using a skill, read its `SKILL.md` and only load referenced materials needed for the current task.

### Skill Selection

* CodeNavi: Use for non-trivial feature work, bug fixing, refactoring, flow investigation, impact analysis, or unfamiliar code. Skip for trivial isolated edits.
* Coding Guidelines: Use when writing, modifying, or reviewing code.
* Docs Writer: Use when creating or editing documentation, README files, guides, or markdown docs.
* Modular Design Principles: Use when designing or reviewing module boundaries, bounded contexts, coupling, ownership, or cross-module contracts.
* Tactical DDD: Use when reviewing, designing, or refactoring domain models, aggregates, value objects, domain services, or domain events.
* Frontend Design: Use when creating frontend UI, pages, components, apps, or polished visual experiences.
* Web Design Guidelines: Use when reviewing UI, UX, accessibility, interaction patterns, or frontend design compliance.
* Playwright: Use for browser automation, end-to-end UI testing, screenshots, responsive checks, and user-flow validation.
* Best Practices: Use for modern web best-practice reviews involving security, compatibility, or code quality.
* Security Best Practices: Use when explicitly asked for security guidance, security reviews, vulnerability reports, or secure-by-default implementation help.
* Skill Architect: Use when designing or creating a new skill.
* Subagent Creator: Use when creating specialized subagents for isolated or parallel work.
* Mermaid Studio: Use for diagrams only when an actual Mermaid Studio skill, app, connector, or tool is available. Otherwise write Mermaid diagrams directly in Markdown when useful.

### Project-Specific Skills

Project-specific skills may exist under `.agents/skills`.

When present:

* Use `.agents/skills/create-command/SKILL.md` for creating, structurally modifying, or reviewing Command use cases.
* Use `.agents/skills/create-query/SKILL.md` for creating, structurally modifying, or reviewing Query use cases.

Do not assume these folders exist. Inspect first.

---

## Development Workflow

For non-trivial code changes:

1. Inspect the relevant code, tests, project files, and docs.
2. Identify the task type: Command, Query, frontend, documentation, refactor, bug fix, test, security, or tooling.
3. Select only the relevant skill or skills.
4. Find similar implementations before introducing a new pattern.
5. Implement the smallest complete change.
6. Add or update tests when behavior changes.
7. Run `dotnet build` after code changes when it can be executed.
8. Run relevant tests after code changes when they can be executed.
9. Report anything that could not be built, tested, or verified.

Use `dotnet test` when a test project exists. If no tests exist yet, state that explicitly and use the best available verification, such as `dotnet build` or targeted manual checks.

---

## Testing

When changing behavior:

* Update existing tests affected by the change.
* Create new tests when behavior is added or regression coverage is appropriate.
* Test observable behavior rather than implementation details.
* Keep tests deterministic.
* Avoid unnecessary mocking.
* Prefer clear Arrange, Act, and Assert structure.
* Cover relevant success and failure paths.
* For Commands, test state changes, validation failures, authorization-relevant behavior, and important business rules.
* For Queries, test filtering, projection, empty results, pagination, ordering, and relevant edge cases.
* Do not weaken, delete, or skip tests only to make a change pass.

If tests cannot be executed, explain why.

---

## Performance

Before writing code, consider:

* unnecessary allocations
* repeated LINQ enumerations
* repeated database queries
* N+1 queries
* unnecessary collection materialization
* O(n²) algorithms
* unnecessary async operations
* loading more data than required
* client-side filtering that should happen in the database

Prefer simple, readable, efficient implementations. Do not sacrifice readability for premature optimization.

For Queries, pay particular attention to projection before materialization, server-side filtering, pagination, no-tracking reads, and avoiding unnecessary navigation loading.

For Commands, pay particular attention to minimizing database round trips, avoiding duplicate existence checks when one operation can safely enforce the invariant, transaction boundaries, and concurrency behavior.

---

## Error Handling

* Fail with meaningful errors.
* Never swallow exceptions.
* Validate input early.
* Do not use exceptions for normal business flow when the project has an established result or validation pattern.
* Preserve useful exception context when rethrowing.
* Do not expose infrastructure details or sensitive information to API consumers.
* Handle expected domain and validation failures using the project's established conventions.

---

## Security

Never:

* hardcode secrets
* expose credentials
* log sensitive data
* disable security checks
* ignore SQL injection risks
* trust client-side validation as a security boundary
* expose internal exception details to external consumers

Always:

* validate untrusted input
* use parameterized database access
* respect authorization boundaries
* preserve existing authentication and authorization behavior unless explicitly asked to change it
* keep secrets in configuration or secret stores appropriate for the environment
* review security impact when touching authentication, authorization, tenant boundaries, imports, file handling, payments, or personally identifiable information

---

## Refactoring

When refactoring:

* Preserve behavior.
* Keep the refactoring scope explicit.
* Avoid unrelated formatting changes.
* Avoid touching unrelated files.
* Do not combine broad architectural refactoring with a small feature or bug fix unless required.
* Run `dotnet build` and relevant tests when they can be executed.
* Report any verification that could not be completed.

---

## Git

Do not create commits unless explicitly asked.

Do not push.

Do not create branches unless explicitly asked.

Never revert user changes unless explicitly requested. If the worktree is dirty, preserve unrelated changes and work around them.

### Commit Guidelines

When creating or suggesting commits, follow Conventional Commits:

```text
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

Common types:

* `feat`: introduces a feature
* `fix`: fixes a bug
* `refactor`: restructures code without changing behavior
* `perf`: improves performance
* `test`: adds or updates tests
* `docs`: changes documentation
* `style`: changes formatting without behavior changes
* `build`: changes build system or dependencies
* `ci`: changes CI/CD configuration
* `chore`: maintenance without behavior changes

Rules:

* Use lowercase commit types.
* Keep the description concise and imperative.
* Use a scope when it clarifies the affected module or feature.
* Avoid vague descriptions such as `fix stuff`, `update code`, or `changes`.
* Inspect `git status` and the staged diff before proposing or creating a commit.
* Verify staged changes belong to the same logical change.
* Recommend splitting unrelated staged changes.
* Never push unless explicitly asked.
* For breaking changes, use `!` after the type or scope, or include a `BREAKING CHANGE:` footer.

Examples:

```text
feat(cashier): add payment form filtering
fix(payment): include missing payment methods in balance
refactor(menu): simplify disabled state synchronization
test(cashier): add payment form validation tests
ci(pipeline): update dotnet test configuration
```

---

## Documentation

When creating or editing documentation:

* Use Docs Writer.
* Inspect the implementation before documenting behavior.
* Keep documentation concise, accurate, and task-oriented.
* Use diagrams when they clarify architecture or flow.
* Use Mermaid Studio only when it is actually available; otherwise use Markdown Mermaid diagrams when appropriate.
* Update related docs and links when a documented behavior or workflow changes.
* Use optional files such as `CONTRIBUTING.md`, `docs/design/DESIGN.md`, or project-specific guides only when present.

---

## Responses

Be concise.

When proposing or completing changes, summarize:

* what changed
* relevant architectural decisions
* tests/builds executed and their results
* anything that could not be built, tested, or verified

State assumptions explicitly. When uncertain, say so. Prefer facts over speculation.

Keep explanations proportional to the complexity of the task.
