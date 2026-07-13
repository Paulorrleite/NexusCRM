# Organizations and subscriptions module

## Purpose

The Organizations and Subscriptions module registers the companies that use
NexusCRM. It owns the SaaS tenant boundary and the rules that decide what each
organization can access.

This is the first module because every CRM record will later belong to an
organization.

## Responsibilities

This module is responsible for:

- Registering the subscribing company.
- Managing plans and subscriptions.
- Managing organization users.
- Managing user invitations.
- Managing profiles, roles, and permissions.
- Enforcing limits per plan.
- Managing organization settings.
- Providing the tenant boundary used for data isolation.

## Out of scope

This module does not own CRM customer data such as companies, contacts, leads,
opportunities, tasks, notes, or activity history.

It can define whether an organization has access to a feature, but each CRM
module owns its own business records and behavior.

## Main entities

### Organization

Represents the tenant.

Initial fields:

- Id.
- Name.
- Slug.
- Status.
- CreatedAt.
- UpdatedAt.

Possible statuses:

- Active.
- Suspended.
- Cancelled.

### Plan

Represents the commercial plan available to organizations.

Initial fields:

- Id.
- Name.
- Description.
- Price.
- BillingPeriod.
- IsActive.

Plan limits can include:

- Maximum users.
- Maximum contacts.
- Maximum companies.
- Maximum opportunities.
- Enabled features.

### Subscription

Represents the current plan relationship for an organization.

Initial fields:

- Id.
- OrganizationId.
- PlanId.
- Status.
- StartedAt.
- CurrentPeriodStart.
- CurrentPeriodEnd.
- CancelledAt.

Possible statuses:

- Trialing.
- Active.
- PastDue.
- Cancelled.

### User

Represents a person who can access NexusCRM.

Initial fields:

- Id.
- Email.
- Name.
- PasswordHash.
- Status.
- CreatedAt.
- UpdatedAt.

A user can belong to one or more organizations through organization membership.

### Role

Represents a set of permissions inside an organization.

Suggested roles:

- Owner.
- Administrator.
- Sales Manager.
- Sales Representative.
- Viewer.

Roles are organization-scoped. A user can have different roles in different
organizations.

### UserInvitation

Represents an invitation for a person to join an organization.

Initial fields:

- Id.
- OrganizationId.
- Email.
- RoleId.
- InvitedByUserId.
- TokenHash.
- Status.
- ExpiresAt.
- AcceptedAt.
- CreatedAt.

Possible statuses:

- Pending.
- Accepted.
- Expired.
- Cancelled.

## Relationships

```text
Organization
├── Subscription
│   └── Plan
├── Organization users
│   ├── User
│   └── Role
└── User invitations
    └── Role
```

`OrganizationUser` is the membership between `Organization`, `User`, and
`Role`. It answers which users belong to which organization and what access they
have inside that organization.

## Permissions

Permissions describe allowed actions. Roles group permissions.

Examples:

- `users.manage`
- `organization.settings.update`
- `subscription.read`
- `subscription.manage`
- `contacts.read`
- `contacts.create`
- `companies.read`
- `opportunities.update`

Permission checks must happen in backend application behavior. UI checks are
only usability improvements.

## Commands

Initial command candidates:

- `RegisterOrganizationCommand`: creates the first organization and assigns the
  first user as owner.
- `InviteUserCommand`: invites a user to join an organization with a role.
- `AcceptUserInvitationCommand`: accepts an invitation and creates or links the
  user account.
- `ChangeUserRoleCommand`: changes a user's role inside an organization.
- `UpdateOrganizationSettingsCommand`: updates organization configuration.
- `ChangeSubscriptionPlanCommand`: changes the organization's plan.
- `CancelSubscriptionCommand`: cancels the subscription.

Commands must validate organization membership, permissions, subscription
status, and plan limits before changing state.

## Queries

Initial query candidates:

- `GetCurrentOrganizationQuery`.
- `ListUserOrganizationsQuery`.
- `GetOrganizationSettingsQuery`.
- `ListOrganizationUsersQuery`.
- `ListPendingInvitationsQuery`.
- `GetSubscriptionQuery`.
- `ListPlansQuery`.

Queries must be read-only and must filter data by the current organization when
returning organization-scoped information.

## Plan limits

Plan limits should be explicit and enforced by backend use cases.

Examples:

- A Free plan may allow only a small number of users.
- A disabled feature must not be available through the API.
- CRM modules must check relevant limits before creating records.

Early implementation can start with simple plan data and limits. Do not build a
complete billing integration until a real use case needs it.

## Tenant isolation rules

- Every organization-scoped record must include `OrganizationId`.
- A user must act inside a selected organization.
- A user can access an organization only through active membership.
- Commands must reject actions outside the current organization.
- Queries must never return records from another organization.
- Invitations must be scoped to one organization.
- Roles and permissions must be evaluated inside the organization context.

## Suggested first vertical slices

Build this module in small slices:

1. Register an organization with the first owner user.
2. List organizations available to the current user.
3. View the current organization profile.
4. Invite a user to an organization.
5. Accept an invitation.
6. List organization users.
7. Change a user's role.
8. Add plan and subscription data.
9. Enforce the first plan limit.

The first implementation should not include payment processing. A local plan and
subscription model is enough to learn the domain and protect future CRM modules.
