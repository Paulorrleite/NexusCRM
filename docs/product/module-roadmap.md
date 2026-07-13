# NexusCRM Module Roadmap

## Purpose

This document records the planned product parts for NexusCRM. It is a roadmap, not a commitment to create all folders, projects, tables, or services immediately.

Each part should be implemented only when we build a real use case for it.

## 1. Authentication

Authentication lets a user access the system securely.

User fields:

- Email.
- Password hash.
- Name.
- Access through organization roles and permissions.

Password policy:

- Minimum 8 characters.
- At least 1 lowercase letter.
- At least 1 uppercase letter.
- At least 1 number.
- At least 1 special character.

Main operations:

- Create an account.
- Log in.
- Log out.
- Renew session.
- Reset password.
- View profile.

Likely technologies:

- ASP.NET Core Identity.
- JWT.
- Refresh tokens.
- Password hashing.
- Password policy.
- Lockout after invalid attempts.

Passwords must be hashed. Additional encryption is not required for the password itself.

## 2. Organizations

An organization is the tenant boundary.

Example:

```text
Name: Reus Tecnologia
Slug: reus-tecnologia
Plan: Free
Status: Active
```

Initial entities:

- Organization.
- OrganizationUser.
- User.
- Role.
- Permission.

A user can belong to one or more organizations. Every organization-scoped feature must use the current organization context.

## 3. Users and permissions

Suggested roles:

- Owner.
- Administrator.
- Sales Manager.
- Sales Representative.
- Viewer.

Example permissions:

- `contacts.read`
- `contacts.create`
- `contacts.update`
- `contacts.delete`
- `companies.read`
- `companies.create`
- `companies.update`
- `opportunities.read`
- `opportunities.create`
- `opportunities.update`
- `users.manage`

Permissions should be checked in backend application behavior. The UI may hide unavailable actions, but it is not the security boundary.

## 4. Companies

Companies represent corporate clients.

Fields:

- Id.
- OrganizationId.
- Name.
- TradeName.
- TaxNumber.
- Website.
- Industry.
- EmployeeCount.
- Status.
- CreatedAt.
- UpdatedAt.

Main operations:

- Register company.
- Edit company.
- Deactivate company.
- Search companies.
- Filter companies.
- List related contacts.
- View related opportunities.

Companies are a good first CRM module because contacts, leads, and opportunities can later connect to them.

## 5. Contacts

Contacts represent people related to a company.

Fields:

- Id.
- OrganizationId.
- CompanyId.
- FirstName.
- LastName.
- Email.
- JobTitle.
- Notes.
- Status.
- CreatedAt.
- UpdatedAt.

Relationship example:

```text
Company
├── Contact 1
├── Contact 2
└── Contact 3
```

A contact can have:

- Multiple phone numbers.
- Multiple addresses.
- Tags.
- Observations.
- Photo.

The first implementation should keep contact behavior small. Add phone numbers, addresses, tags, and photo support when those use cases are needed.

## 6. Leads

A lead represents someone who has not yet become a real customer or opportunity.

Fields:

- Id.
- OrganizationId.
- Name.
- Email.
- Phone.
- Source.
- Status.
- AssignedUserId.
- EstimatedValue.
- CreatedAt.

Statuses:

- New.
- Contacted.
- Qualified.
- Disqualified.
- Converted.

Sources:

- Website.
- Referral.
- LinkedIn.
- Advertisement.
- Manual.
- Other.

Important domain operation:

- `ConvertLeadCommand`.

Lead conversion can:

- Validate that the lead is qualified.
- Create a company when necessary.
- Create a contact.
- Create an opportunity.
- Update the lead to converted.
- Register an activity history event.

This operation should not be modeled as generic update logic. It is a business workflow with explicit rules and should have its own Command.

## 7. Opportunities

Opportunities represent commercial negotiations.

Fields:

- Id.
- OrganizationId.
- CompanyId.
- ContactId.
- Title.
- Stage.
- EstimatedValue.
- Probability.
- ExpectedCloseDate.
- AssignedUserId.
- CreatedAt.
- UpdatedAt.

Stages:

- Prospecting.
- Qualification.
- Proposal.
- Negotiation.
- Won.
- Lost.

Opportunities can be displayed on a Kanban board by stage. Moving a card must be treated as a domain operation, not as a generic update.

Recommended command:

- `ChangeOpportunityStageCommand`.

Possible rules:

- A closed opportunity cannot return to an open stage without permission.
- An opportunity marked as lost needs a reason.
- An opportunity marked as won needs a closing date.
- Probability may change according to the stage.

## 8. Tasks

Tasks represent work assigned to users.

Tasks can be related to:

- Company.
- Contact.
- Lead.
- Opportunity.

Types:

- Call.
- Email.
- Meeting.
- FollowUp.
- Other.

Statuses:

- Pending.
- Completed.
- Cancelled.
- Overdue.

Fields:

- Title.
- Description.
- DueDate.
- AssignedUserId.
- RelatedEntityType.
- RelatedEntityId.

Useful dashboard views:

- Overdue tasks.
- Tasks for today.
- Next tasks.

## 9. Activity history

Activity history records important business activity across the CRM.

Examples:

- Contact created.
- Opportunity moved.
- Task completed.
- Lead converted.
- Note added.
- User assigned.
- Opportunity won or lost.

Timeline examples:

```text
10:42 - Paulo moved the opportunity from Proposal to Negotiation
09:10 - Ana added a note to the contact
Yesterday - Paulo converted the lead João Silva
```

Possible implementation approaches:

- Domain events.
- EF Core interceptor.
- Audit service.
- Activity table.

For this project, domain events are a good portfolio-friendly choice if we keep them simple. They make business events explicit without turning the first version into an event-sourced system.
