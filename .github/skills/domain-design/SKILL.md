---
name: domain-design
description: Iteratively challenge domain requirements and design module aggregates, behavior, events, and API contracts.
---

# Domain Design

Design domain model through repeated discovery and challenge rounds. Do not
produce a one-pass list of entities. The result is a reviewed domain contract
that gives Drax Planner enough information to create separate backend and
frontend plans.

## Required workflow

Run every phase in order. After each challenge round, show changed decisions,
open questions, and assumptions. Continue until user explicitly confirms no blocking question,
contradictory invariant, unclear ownership, or untestable behavior remains.

### 1. Establish scope

- Identify module, business outcome, actors, existing capabilities, and
  in-scope/out-of-scope behavior.
- Separate one aggregate enhancement from a new aggregate or a new module.
- Record external systems, cross-module dependencies, authorization needs, and
  consistency expectations.
- State assumptions explicitly. Never silently invent business rules.

### 2. Model first pass

- Define bounded-context ownership and aggregate roots.
- For each aggregate, define entities, value objects, strongly typed IDs,
  relationships, lifecycle/status, invariants, and transaction boundary.
- Assign every piece of data to one owning aggregate. Reject shared mutable
  state and direct module references.
- Define commands, queries, domain behavior, validation rules, constants,
  errors, permissions, and domain/integration events.
- For every event, define producer, payload, timing, consumers, retry/
  idempotency expectations, and failure handling.

### 3. Challenge round

Challenge the model with concrete scenarios, including:

- create, update, delete/archive, and invalid transitions;
- duplicate requests, retries, concurrent updates, and stale versions;
- missing related data, authorization changes, and partial external failure;
- event replay, consumer failure, ordering, and eventual consistency;
- list/detail/filter/sort/paging needs and empty or large result sets.

For each scenario, identify violated invariants, missing behavior, wrong
aggregate ownership, unsuitable event boundaries, or API ambiguity. Revise the
model and repeat this round until resolved or explicitly marked as a human
decision.

### 4. API contract round

Define planner-ready contracts for every user-facing capability. Keep API
contracts separate from domain entities. For each endpoint specify:

- endpoint kind (`Command` or `Query`), HTTP method, route, and authorization;
- request contract, field rules, required/optional semantics, and examples;
- response contract, status codes, error shape, paging/filter/sort metadata;
- idempotency/concurrency requirements and emitted events;
- frontend mapping needs: view model, form/list/detail state, and user-visible
  validation or authorization behavior.

Use project route conventions such as
`POST /api/{module}/{aggregate}/{feature}` and
`GET /api/{module}/{aggregate}/{id}`. Do not expose persistence or domain
types directly.

### 5. Final review gate

Before finalizing, verify:

- every requirement maps to behavior and at least one API capability or is
  explicitly out of scope;
- every aggregate has clear ownership and transaction boundary;
- every command has validation, authorization, and error outcomes;
- every event has producer, contract, handling, idempotency, and failure plan;
- every endpoint has complete request/response/error contracts;
- backend and frontend plans can be generated without inventing names or rules.

If any check fails, return to the relevant phase instead of finalizing.

## Required output

```markdown
# Domain Design: {Module} - {Capability}

## 1. Scope and decisions
- Business outcome:
- In scope:
- Out of scope:
- Actors and permissions:
- Assumptions:
- Open decisions:

## 2. Aggregates and boundaries
### 2.1 {AggregateName}
- Responsibility and owner:
- Entities and value objects:
- Identity and lifecycle:
- Invariants:
- Commands and behavior:
- Queries/read needs:
- Relationships and consistency boundary:

## 3. Rules and errors
### 3.1 Constants
### 3.2 Validation rules
### 3.3 Domain errors and API error mapping

## 4. Events
### 4.1 {EventName}
- Type: domain / integration
- Producer and trigger:
- Payload:
- Consumers:
- Ordering, retry, idempotency, and failure handling:

## 5. API contracts
### 5.1 {FeatureName}
- Aggregate:
- Kind: Command / Query
- Method and route:
- Authorization:
- Request: `{RequestContractName}` with fields, rules, and example
- Response: `{ResponseContractName}` with fields and example
- Status and error responses:
- Paging/filter/sort or concurrency behavior:
- Emitted events:
- Frontend mapping and UX states:
```

Do not write implementation code, detailed implementation plans, migrations, or
worktree branches in this skill. Produce domain decisions and API contracts
precise enough for planning workflows to continue without redesigning the
domain.