# NexusCRM SaaS CRM Scope

## Purpose

NexusCRM is a SaaS CRM for small businesses. It helps each customer organization manage its own users and customer-facing work without sharing data with other organizations.

The system will be built incrementally, one business capability at a time. Each module should be understandable on its own before the next module is added.

## Tenant model

An organization is the tenant boundary.

Each organization owns its users and CRM data:

```text
Organization: Alpha Technology
├── Users
├── Contacts
├── Companies
├── Leads
├── Opportunities
├── Tasks
├── Notes
└── Interaction History

Organization: Beta Accounting
├── Users
├── Contacts
├── Companies
├── Leads
├── Opportunities
├── Tasks
├── Notes
└── Interaction History
```

Data from one organization must not be visible to another organization. Tenant isolation is a core security and domain rule, not only a UI filter.

## Product parts

The system is divided into nine parts:

1. Authentication.
2. Organizations.
3. Users and permissions.
4. Companies.
5. Contacts.
6. Leads.
7. Opportunities.
8. Tasks.
9. Activity history.

See [module roadmap](module-roadmap.md) for the planned responsibilities, entities, and important business operations for each part.

## Development approach

Build the system as small vertical slices:

1. Choose one capability.
2. Define the user-facing behavior.
3. Classify operations as Commands or Queries.
4. Add only the domain, application, API, persistence, and tests needed for that slice.
5. Review the code before moving to the next capability.

Avoid creating empty architecture layers or placeholder modules. Architecture should grow from real use cases.

## Suggested first slices

The first slices should establish tenant foundations before broad CRM behavior:

1. Authentication basics: create account, log in, renew session, view profile.
2. Organization creation: the first user creates an organization and becomes its owner.
3. Users and permissions: assign organization roles and enforce permissions.
4. Companies: first organization-scoped CRM records.

After these foundations, contacts, leads, opportunities, tasks, and activity history can be added as focused vertical slices.
