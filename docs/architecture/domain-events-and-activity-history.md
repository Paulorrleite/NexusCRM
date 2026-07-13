# Domain events and activity history

## Purpose

Activity history is a product feature, not only technical audit logging. Users should be able to understand what happened to a customer, lead, opportunity, or task over time.

Domain events are the preferred first approach for important business activity because they keep meaningful changes explicit.

## Domain events

Domain events should represent something that already happened in the business.

Good event names:

- `LeadConverted`.
- `OpportunityStageChanged`.
- `OpportunityWon`.
- `OpportunityLost`.
- `TaskCompleted`.
- `ContactCreated`.
- `NoteAdded`.
- `UserAssigned`.

Avoid events named after technical actions, such as `EntityUpdated` or `DatabaseRowChanged`, when the product needs business language.

## Activity records

An activity record should capture enough information to render a useful timeline.

Useful fields:

- Id.
- OrganizationId.
- ActorUserId.
- ActivityType.
- RelatedEntityType.
- RelatedEntityId.
- Message.
- OccurredAt.
- Metadata.

The activity table should be organization-scoped. Queries for timeline views must filter by organization.

## Implementation guidance

Start simple:

1. A Command completes a meaningful state change.
2. The domain object records a domain event.
3. The application layer persists the state change.
4. A domain event handler creates an activity history record.

This is not event sourcing. The system state still lives in normal persisted entities. Domain events are used to react to important business changes and create timeline entries.

## Examples

`ConvertLeadCommand` may produce `LeadConverted`.

The activity handler can record:

```text
Paulo converted the lead João Silva
```

`ChangeOpportunityStageCommand` may produce `OpportunityStageChanged`.

The activity handler can record:

```text
Paulo moved the opportunity from Proposal to Negotiation
```

## Rules

- Events should be named in past tense.
- Events should include the organization id.
- Events should include the actor user id when the action was performed by a user.
- Events should carry enough data to write activity history without unnecessary extra queries.
- Event handlers must not bypass tenant isolation.
- Do not publish events for every property update by default.
